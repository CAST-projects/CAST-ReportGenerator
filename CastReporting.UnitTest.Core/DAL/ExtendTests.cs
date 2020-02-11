using System;
using System.IO;
using CastReporting.Repositories.Core.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Core.DAL
{
    [TestClass]
    public class ExtendTest
    {
        private static readonly string _extendUrl = "https://extendng.castsoftware.com";
        private readonly Random _random = new Random();

        private string GetTmpPath()
        {
            string rnd = _random.Next(0, 999).ToString();
            string tempDirectory = Path.Combine(Path.GetTempPath(), "RG_extend_tests_" + DateTime.Today.ToString("yyyyMMdd") + rnd);
            if (Directory.Exists(tempDirectory))
            {
                File.SetAttributes(tempDirectory, FileAttributes.Normal);
                Directory.Delete(tempDirectory, true);
            }
            Directory.CreateDirectory(tempDirectory);
            File.SetAttributes(tempDirectory, FileAttributes.Normal);
            return tempDirectory;
        }

        [TestMethod]
        public void TestSearchForLatestVersion()
        {
            using (ExtendRepository extendRepository = new ExtendRepository(_extendUrl, "123456789"))
            {
                string latestVersion = extendRepository.SearchForLatestVersion("com.castsoftware.aip.reportgeneratorfordashboard");
                Assert.IsNotNull(latestVersion);
            }
        }

        [TestMethod]
        [Ignore]
        public void TestGetLatestTemplateVersion()
        {
            string tempDirectory = GetTmpPath();
            // to run test, nugget key should be get from the profile in https://extendng.castsoftware.com
            using (ExtendRepository extendRepository = new ExtendRepository(_extendUrl, "12345678"))
            {
                extendRepository.GetPackageTemplate("com.castsoftware.aip.reportgeneratorfordashboard", tempDirectory, null);
            }
            Assert.IsTrue(Directory.Exists(Path.Combine(tempDirectory, "Templates", "Portfolio")));
            Assert.IsTrue(Directory.Exists(Path.Combine(tempDirectory, "Templates", "Application")));
            Directory.Delete(tempDirectory, true);
        }

        [TestMethod]
        [Ignore]
        public void TestGetSpecificTemplateVersion()
        {
            string tempDirectory = GetTmpPath();
            // to run test, nugget key should be get from the profile in https://extendng.castsoftware.com
            using (ExtendRepository extendRepository = new ExtendRepository(_extendUrl, "12345678"))
            {
                string latestVersion = extendRepository.SearchForLatestVersion("com.castsoftware.aip.reportgeneratorfordashboard");
                extendRepository.GetPackageTemplate("com.castsoftware.aip.reportgeneratorfordashboard", tempDirectory, latestVersion);
            }
            Assert.IsTrue(Directory.Exists(Path.Combine(tempDirectory, "Templates", "Portfolio")));
            Assert.IsTrue(Directory.Exists(Path.Combine(tempDirectory, "Templates", "Application")));
            Directory.Delete(tempDirectory, true);
        }
    }
}
