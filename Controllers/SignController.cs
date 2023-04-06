using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
// using DivisoftUtility;
// using FactureronlineUtility.Models;
// using FEUtility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using XadesNetCoreDocker.Models;

namespace FactureronlineUtility.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignController : ControllerBase
    {
        private readonly string _workPath;
        private readonly string _dirSeparator;
       // private readonly MainUtility _utility;
        private readonly IConfiguration _configuration;
       // private readonly DigitalOceanUtil.DigitalOceanMannager _digital;
        

        public SignController(IConfiguration configuration) 
        {
            //_configuration = configuration;
            //var tempWorkPath = _configuration.GetValue<string>("WorkPath");
            _dirSeparator = Path.PathSeparator.ToString();
            var tempFilePath = Path.GetTempFileName();
            FileInfo tempInfo = new FileInfo(tempFilePath);
            _workPath = tempInfo.DirectoryName;
            //_utility = new MainUtility();
            //_digital = new DigitalOceanUtil.DigitalOceanMannager(_utility.getAWSConfig(_configuration, "AccessKey"),
            //     _utility.getAWSConfig(_configuration, "SecretKey"),
            //      Amazon.RegionEndpoint.USWest2,
            //     _utility.getAWSConfig(_configuration, "UrlBucket")
            //    );
        }
        [HttpPost]
        [Route("xadesPost")]
        public JObject PostXades([FromBody]SignRequest request)
        {
           // var pathEndFileWork = new FeUtil().getFechaFromClave(request.clave);
            //var cloud = "fedocumentsstorage/DSign/Temp/" + pathEndFileWork;
            try
            {
                //var executeFirma = new Firma(_workPath,_dirSeparator);
                //var xml = _workPath + _dirSeparator + request.clave + ".xml";
                //if (!string.IsNullOrWhiteSpace(request.xmlToSign))
                //{
                //    _utility.Base64toFile(request.xmlToSign, xml);
                //    var uploadResult = _digital.UploadFile("fedocumentsstorage/DSign/Temp/" + pathEndFileWork, xml, request.clave + ".xml");
                //    if (!uploadResult)
                //        throw new Exception("Fail to upload cloud xml pre sign");
                //}
                //else if(!System.IO.File.Exists(xml)) 
                //{
                //    var downloadResult = _digital.DowloadFile("fedocumentsstorage/DSign/Temp/" + pathEndFileWork, xml, request.clave + ".xml");
                //    if(!downloadResult.WasDownloaded)
                //        throw new Exception("Fail downloading file");
                //}

                //var result = executeFirma.Firmar(request.base64p12, request.p12pass, request.clave+".p12",xml);
                //var xmlFileInfo = _workPath + _dirSeparator + request.clave + "_signed.xml";
                //var uploadResult2 = _digital.UploadFile("fedocumentsstorage/DSign/Temp/" + pathEndFileWork, xmlFileInfo, request.clave + "_signed.xml");
                //if (!uploadResult2)
                //    throw new Exception("Fail to upload cloud xml signed");
                //var p12File = _workPath + _dirSeparator + request.clave + ".p12";
                //if (System.IO.File.Exists(p12File))
                //    System.IO.File.Delete(p12File);
                //var msg = "{\"Error\": \"0\", \"FileSigned\" : \"" + result.Base64 + "\"}";
                //JObject json = JObject.Parse(msg);
                //return json;
                JObject response = new JObject();
                System.IO.File.WriteAllText(_workPath + _dirSeparator + "TEST.txt", "a");
                var txt = System.IO.File.ReadAllText(_workPath + _dirSeparator + "TEST.txt");
                response.Add("workpath", _workPath);
                response.Add("text", txt);
                return response;
                
            }
            catch (Exception ex)
            {
                String msg = "{\"Error\": \"" + ex.Message.Replace('\\', '/') + "\"}";
                JObject json = JObject.Parse(msg);
                return json;
            }
        }
    }
}