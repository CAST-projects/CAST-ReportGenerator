using System.Net;
using CastReporting.Mediation.Interfaces.Core;

namespace CastReporting.Mediation.Core
{
    public class ExtendProxy : WebClient, IExtendProxy
    {
        // https://extendng.castsoftware.com/api/package/EXTENSION-ID/latest 
        // https://extendng.castsoftware.com/api/package/download/EXTENSION_ID/VERSION

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
