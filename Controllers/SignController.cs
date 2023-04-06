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
            _dirSeparator = Path.PathSeparator.ToString();
            var tempFilePath = Path.GetTempFileName();
            FileInfo tempInfo = new FileInfo(tempFilePath);
            _workPath = tempInfo.DirectoryName;
            _digital = new DigitalOceanUtil.DigitalOceanMannager(Utility.getAWSConfig(_configuration, "AccessKey"),
                 Utility.getAWSConfig(_configuration, "SecretKey"),
                  Amazon.RegionEndpoint.USWest2,
                 Utility.getAWSConfig(_configuration, "UrlBucket")
                );
        }
        [HttpPost]
        [Route("xadesPost")]
        public JObject PostXades([FromBody]SignRequest request)
        {
            var pathEndFileWork = Utility.getFechaFromClave(request.clave);
            var cloud = "fedocumentsstorage/DSign/Temp-Web2/" + pathEndFileWork;
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

                var result = executeSign.ExecuteSign(request.base64p12, request.p12pass, request.clave+".p12",xml);
                var xmlFileInfo = _workPath + _dirSeparator + request.clave + "_signed.xml";
                var uploadResult2 = _digital.UploadFile(cloud, result.Path, request.clave + "_signed.xml");
                if (!uploadResult2)
                    throw new Exception("Fail to upload cloud xml signed");
                var response = new XadesRestApiResponse { FileSigned = result.Base64 };
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