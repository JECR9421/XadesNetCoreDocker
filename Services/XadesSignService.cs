using FirmaXadesNetCore;
using FirmaXadesNetCore.Signature.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using XadesNetCoreDocker.Models;
using XadesNetCoreDocker.Utils;

namespace XadesNetCoreDocker.Services
{
    public class XadesSignService
    {
        private readonly string _workPath;
        private readonly string _dirSeparator;

        public XadesSignService()
        {
            _dirSeparator = "/";
            var tempFilePath = Path.GetTempFileName();
            FileInfo tempInfo = new FileInfo(tempFilePath);
            _workPath = tempInfo.DirectoryName;
        }
        public SignResult ExecuteSign(string llaveCriptografica, string passP12, string p12FileName, string xmlToSignPath)
        {
            var p12File = p12FileName;
            if (File.Exists(p12File))
                    File.Delete(p12File);
                Utility.Base64toFile(llaveCriptografica, p12File);
            XadesService xadesService = new XadesService();
            SignatureParameters parametros = new SignatureParameters();
            parametros.SignaturePolicyInfo = new SignaturePolicyInfo();
            parametros.SignaturePolicyInfo.PolicyIdentifier = "https://tribunet.hacienda.go.cr/docs/esquemas/2016/v4.1/Resolucion_Comprobantes_Electronicos_DGT-R-48" +
            "-2016.pdf";
            parametros.SignaturePolicyInfo.PolicyHash = "Ohixl6upD6av8N7pEvDABhEL6hM=";
            parametros.SignaturePackaging = SignaturePackaging.ENVELOPED;

            parametros.DataFormat = new DataFormat();

            X509Certificate2 cert = new X509Certificate2(p12File, passP12,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            parametros.Signer = new FirmaXadesNetCore.Crypto.Signer(cert);

            var fs = new FileStream((xmlToSignPath), FileMode.Open);
            FirmaXadesNetCore.Signature.SignatureDocument docFirmado = xadesService.Sign(fs, parametros);
            var xmlFileInfo = Path.GetFileNameWithoutExtension(xmlToSignPath) + "_signed.xml";
            docFirmado.Save(xmlFileInfo);

            return new SignResult()
            {
                Path = xmlFileInfo,
                Base64 = Utility.fileToBase64(xmlFileInfo)
            };
        }
    }
}
