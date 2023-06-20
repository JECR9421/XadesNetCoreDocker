using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XadesNetCoreDocker.Models;
using XadesNetCoreDocker.Services;
using XadesNetCoreDocker.Utils;

namespace FactureronlineUtility.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _workPath;
        private readonly string _dirSeparator;
        private readonly DigitalOceanUtil.DigitalOceanMannager _digital;
        

        public SignController(IConfiguration configuration) 
        {
            _configuration = configuration;
            _dirSeparator = "/";
            var tempFilePath = Path.GetTempFileName();
            FileInfo tempInfo = new FileInfo(tempFilePath);
            _workPath = tempInfo.DirectoryName;
            _digital = new DigitalOceanUtil.DigitalOceanMannager(Environment.GetEnvironmentVariable("AWS_ACCESS_KEY"),
                Environment.GetEnvironmentVariable("AWS_SECRET_KEY"),
                 Amazon.RegionEndpoint.USWest2,
                Environment.GetEnvironmentVariable("URL_BUCKET")
                );

        }
        [HttpPost]
        [Route("xadesPost")]
        public JObject PostXades([FromBody]SignRequest request)
        {
            var pathEndFileWork = Utility.getFechaFromClave(request.clave);
            var cloud = (string.IsNullOrWhiteSpace(request.cloudOriginPath)) 
                ? $"{Environment.GetEnvironmentVariable("CLOUD_DEFAULT_ORIGIN")}/" + pathEndFileWork 
                : $"{Environment.GetEnvironmentVariable("BUCKET_BASE")}/{request.cloudOriginPath}";
            var p12File = _workPath + _dirSeparator + request.clave + ".p12";
            try
            {
                var executeSign = new XadesSignService();
                var xml = _workPath + _dirSeparator + request.clave + ".xml";
                if (!string.IsNullOrWhiteSpace(request.xmlToSign))
                {
                    Utility.Base64toFile(request.xmlToSign, xml);
                    var uploadResult = _digital.UploadFile(cloud, xml, request.clave + ".xml");
                    if (!uploadResult)
                        throw new Exception("Fail to upload cloud xml pre sign");
                }
                else if (!System.IO.File.Exists(xml))
                {
                    var downloadResult = _digital.DowloadFile(cloud, xml, request.clave + ".xml");
                    if (!downloadResult.WasDownloaded)
                        throw new Exception("Fail downloading file");
                }

                var result = executeSign.ExecuteSign(request.base64p12, request.p12pass, p12File, xml);
                var xmlFileInfo = _workPath + _dirSeparator + request.clave + "_signed.xml";
                var cloudSaveSigned = (string.IsNullOrWhiteSpace(request.cloudDestinationPath)) ? cloud : $"fedocumentsstorage/{request.cloudDestinationPath}";
                var uploadResult2 = _digital.UploadFile(cloudSaveSigned, result.Path, request.clave + "_signed.xml");
                if (!uploadResult2)
                    throw new Exception("Fail to upload cloud xml signed");
                else
                {
                    System.IO.File.Delete(xml);
                    System.IO.File.Delete(result.Path);
                    System.IO.File.Delete(p12File);
                }
                var response = new XadesRestApiResponse { Error = "0", FileSigned = result.Base64 };
                
                return JObject.FromObject(response);
                
            }
            catch (Exception ex)
            {
                var response = new XadesRestApiResponse { Error = ex.Message.Replace('\\', '/')};
                return JObject.FromObject(response);
            }
        }
    }
}