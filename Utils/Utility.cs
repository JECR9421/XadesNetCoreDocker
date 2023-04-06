using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XadesNetCoreDocker.Utils
{
    public class Utility
    {
        public static void Base64toFile(String base64, String path)
        {
            try
            {
                File.WriteAllBytes(path, Convert.FromBase64String(base64));
                if (new FileInfo(path).Length < 100 || new FileInfo(path).Length == 0)
                {
                    throw new Exception("Archivo base64 corrupto");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"error base64 to file:${ex.Message}");


            }
        }

        public static String fileToBase64(String path)
        {
            try
            {
                Byte[] bytes = File.ReadAllBytes(path);
                String file = Convert.ToBase64String(bytes);
                return file;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating base 64 string from file:{path}\n{ex.Message}");
            }
        }

    }
}
