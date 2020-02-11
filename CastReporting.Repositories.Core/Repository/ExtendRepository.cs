using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using Cast.Util;
using Cast.Util.Log;
using CastReporting.Domain.Core.WSObjects;
using CastReporting.Mediation.Core;
using CastReporting.Mediation.Interfaces.Core;
using CastReporting.Repositories.Interfaces;

namespace CastReporting.Repositories.Core.Repository
{
    public class ExtendRepository : IExtendRepository
    {
        protected IExtendProxy Proxy;
        private readonly string _url;
        private const string QUERY_GET_PACKAGE_LATEST_VERSION = "{0}/api/package/{1}/latest";
        private const string QUERY_DOWNLOAD_VERSION = "{0}/api/package/download/{1}/{2}";

        public ExtendRepository(string url, string nugetKey)
        {
            Proxy = new ExtendProxy(nugetKey);
            _url = url.EndsWith("/") ? url.Substring(0, url.Length - 1) : url;
        }

        public void GetPackageTemplate(string packageId, string targetPath, string version)
        {
            string workTmpPath = GetWorkTempPath();
            string archive = GetVersion(packageId, _url, version, workTmpPath);
            PathUtil.UnzipAndCopy(archive, targetPath);
            LogHelper.LogInfo("Extension " + packageId + " version " + version + " downloaded and installed in " + targetPath);
            DeleteWorkTempPath(workTmpPath);
        }

        public string SearchForLatestVersion(string packageId)
        {

            string query = string.Format(QUERY_GET_PACKAGE_LATEST_VERSION, _url, packageId);
            try
            {
                string jsonString = Proxy.DownloadPackageInformation(query);
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ExtendPackage));
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(jsonString));
                try
                {
                    ExtendPackage res = serializer.ReadObject(ms) as ExtendPackage;
                    return res?.Version;
                }
                finally
                {
                    ms.Close();
                }
            }
            catch (WebException e)
            {
                LogHelper.LogError(e.Message);
                return null;
            }
        }

        private string GetVersion(string packageId, string extendUrl, string version, string targetPath)
        {
            if (string.IsNullOrEmpty(version))
            {
                version = SearchForLatestVersion(packageId);
            }
            string query = string.Format(QUERY_DOWNLOAD_VERSION, extendUrl, packageId, version);
            string filename = Path.Combine(targetPath, packageId + "." + version + ".nupkg");
            try
            {
                Proxy.DownloadExtension(query, filename);
            }
            catch (WebException e)
            {
                LogHelper.LogError(e.Message);
            }
            return filename;
        }

        private static string GetWorkTempPath()
        {
            var random = new Random();
            string rnd = random.Next(0, 999).ToString();
            string tempDirectory = Path.Combine(Path.GetTempPath(), "RG_work_" + DateTime.Today.ToString("yyyyMMdd") + rnd);
            if (Directory.Exists(tempDirectory))
            {
                File.SetAttributes(tempDirectory, FileAttributes.Normal);
                Directory.Delete(tempDirectory, true);
            }
            Directory.CreateDirectory(tempDirectory);
            File.SetAttributes(tempDirectory, FileAttributes.Normal);
            return tempDirectory;
        }

        private static void DeleteWorkTempPath(string tempDirectory)
        {
            if (!Directory.Exists(tempDirectory)) return;
            File.SetAttributes(tempDirectory, FileAttributes.Normal);
            Directory.Delete(tempDirectory, true);
        }

        public void Dispose()
        {
            Proxy.Dispose();
        }
    }

}
