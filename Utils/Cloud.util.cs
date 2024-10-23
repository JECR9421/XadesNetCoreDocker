using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.IO;

namespace XadesNetCoreDocker.Utils
{
    public class CloudUtil
    {
        private string _urlCloudApi = Environment.GetEnvironmentVariable("URL_CLOUD_API");
        private string _bucket = Environment.GetEnvironmentVariable("BUCKET_BASE");
        private string _apiKey = Environment.GetEnvironmentVariable("API_KEY");

        public async Task<bool> doUploadToCloudAsync(String filePath, String cloudPath, String fileName)
        {

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("key", _apiKey);
            using var form = new MultipartFormDataContent();

            // Add the file content
            var fileStream = File.OpenRead(filePath);
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            form.Add(fileContent, "file", fileName);

            // Add other form fields (text)
            form.Add(new StringContent(_bucket), "bucket");
            form.Add(new StringContent(cloudPath), "path");
            form.Add(new StringContent(fileName), "fileName");
            // Send the request
            var response = await httpClient.PostAsync($"{_urlCloudApi}/api/upload-file", form);

            // Read the response
            var responseContent = await response.Content.ReadAsStringAsync();

            bool result = Convert.ToBoolean(responseContent);

            return result;
        }

        public async Task<bool> doDownloadFileToPath(String cloudPath, String fileName, String localPath)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("key", _apiKey);
            var urlRequest = $"{_urlCloudApi}/api/download-file?bucket={_bucket}&folder={cloudPath}&fileName={fileName}";
            var response = await httpClient.GetAsync(urlRequest, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            // Read the response content as a stream
            using var contentStream = await response.Content.ReadAsStreamAsync();

            // Save the stream to a file
            using var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
            await contentStream.CopyToAsync(fileStream);
            return File.Exists(localPath);
        }
    }
}
