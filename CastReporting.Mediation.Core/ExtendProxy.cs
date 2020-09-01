using CastReporting.Mediation.Interfaces.Core;
using System.Net;

namespace CastReporting.Mediation.Core
{
    public class ExtendProxy : WebClient, IExtendProxy
    {
        // https://extend.castsoftware.com/api/package/EXTENSION-ID/latest 
        // https://extend.castsoftware.com/api/package/download/EXTENSION_ID/VERSION

        public ExtendProxy(string nugetApiKey)
        {
            // To connect to cast extend to download latest version of reports
            Headers.Add("x-nuget-apikey", nugetApiKey);
        }

        public string DownloadPackageInformation(string query)
        {
            return DownloadString(query);
        }

        public void DownloadExtension(string query, string filename)
        {
            DownloadFile(query, filename);
        }
    }
}
