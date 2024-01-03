using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XadesNetCoreDocker.Models
{
    public class SignRequest
    {
        public string clave { get; set; }
        public string Tipo { get; set; }
        public string base64p12 { get; set; }
        public string p12pass { get; set; }
        public string xmlToSign { get; set; }
        public string tipoEnvio { set; get; }
        public string tipoidemisor { set; get; }
        public string tipoidreceptor { set; get; }
        public string path { set; get; }
        public string isExternal { set; get; }
        public string callback { set; get; }

        public string cloudOriginPath { set; get; }

        public string cloudDestinationPath { set; get; }
    }
}
