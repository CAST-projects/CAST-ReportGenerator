using CastReporting.Domain;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class TransactionsChartTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60013WebGoat.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4", currentDate,
               null, null, null, null, null, null);

            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new TransactionsChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TRI", "Security", "Efficiency", "Robustness" });
            expectedData.AddRange(new List<string> { "lessoninfo.mvc/", "0", "160", "329" });
            expectedData.AddRange(new List<string> { "Page_Load", "0", "0", "178" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60013WebGoat.json", "Data")]
        public void TestFullNames()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4", currentDate,
               null, null, null, null, null, null);

            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new TransactionsChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" },
                {"NAME","FULL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TRI", "Security", "Efficiency", "Robustness" });
            expectedData.AddRange(new List<string> { "C:\\Users\\ABD\\CAST\\AipNode\\data\\deploy\\WebGoatMulti\\main_sources\\WebGoat\\webgoat-container\\src\\main\\java\\org\\owasp\\webgoat\\service\\LessonInfoService.java//PORT/ANY/lessoninfo.mvc/", "0", "160", "329" });
            expectedData.AddRange(new List<string> { "WebSite.Redirect.Page_Load", "0", "0", "178" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60016WebGoat.json", "Data")]
        public void TestSecurity()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4", currentDate,
               null, null, null, null, null, null);

            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new TransactionsChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" },
                {"FILTER","SECU" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TRI", "Security", "Efficiency", "Robustness" });
            expectedData.AddRange(new List<string> { "btnPlaceOrder_Click", "530", "0", "0" });
            expectedData.AddRange(new List<string> { "Page_Load", "520", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60014WebGoat.json", "Data")]
        public void TestEfficiency()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4", currentDate,
               null, null, null, null, null, null);

            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new TransactionsChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" },
                {"FILTER","EFF" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TRI", "Security", "Efficiency", "Robustness" });
            expectedData.AddRange(new List<string> { "main.jsp", "0", "200", "0" });
            expectedData.AddRange(new List<string> { "lessoninfo.mvc/", "0", "160", "329" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60013WebGoat.json", "Data")]
        public void TestNoSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4", currentDate,
               null, null, null, null, null, null);

            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new TransactionsChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" },
                {"FILTER","ROBU" },
                {"SNAPSHOT","PREVIOUS" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TRI", "Security", "Efficiency", "Robustness" });
            expectedData.AddRange(new List<string> { "No snapshot found for this application", "0", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        [DeploymentItem(@".\Data\Transactions60013WebGoat.json", "Data")]
        public void TestPreviousRobustness()
        {
            ImagingData reportData = TestUtility.PrepaReportData("ReportGenerator",
               null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
               null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");

            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new TransactionsChart();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","2" },
                {"FILTER","ROBU" },
                {"SNAPSHOT","PREVIOUS" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "TRI", "Security", "Efficiency", "Robustness" });
            expectedData.AddRange(new List<string> { "lessoninfo.mvc/", "0", "160", "329" });
            expectedData.AddRange(new List<string> { "Page_Load", "0", "0", "178" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

            Assert.IsNull(table.GraphOptions);
        }
    }
}
