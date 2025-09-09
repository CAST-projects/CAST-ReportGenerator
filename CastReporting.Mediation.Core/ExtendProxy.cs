using CastReporting.Mediation.Interfaces.Core;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CastReporting.Mediation.Core
{
    public class ExtendProxy : IExtendProxy
    {
        // https://extend.castsoftware.com/api/package/EXTENSION-ID/latest 
        // https://extend.castsoftware.com/api/package/download/EXTENSION_ID/VERSION

        private readonly HttpClient _httpClient;

        public ExtendProxy(string nugetApiKey)
        {
            _httpClient = new HttpClient();
            // To connect to cast extend to download latest version of reports
            _httpClient.DefaultRequestHeaders.Add("x-nuget-apikey", nugetApiKey);
        }

        public string DownloadPackageInformation(string query)
        {
            return _httpClient.GetStringAsync(query).GetAwaiter().GetResult();
        }

        public void DownloadExtension(string query, string filename)
        {
            using (var response = _httpClient.GetAsync(query, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
            {
                response.EnsureSuccessStatusCode();
                using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
                using (var fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    stream.CopyToAsync(fileStream).GetAwaiter().GetResult();
                }
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

    }
}
