using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class OmgTechnicalDebtDetailsTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/4", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new OmgTechnicalDebtDetailsTable();
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "CISQ" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical Criterion", "Technical Debt (Days)", "Technical Debt Added (Days)", "Technical Debt Removed (Days)" });
            expectedData.AddRange(new List<string> { "CWE-22 - Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')", "4.4", "0.0", "4.4" });
            expectedData.AddRange(new List<string> { "CWE-23 - Relative Path Traversal", "4.4", "0.0", "4.4" });
            expectedData.AddRange(new List<string> { "CWE-36 - Absolute Path Traversal", "4.4", "0.0", "4.4" });
            TestUtility.AssertTableContent(table, expectedData, 4, 4);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestNullContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new OmgTechnicalDebtDetailsTable();
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "CISQ" },
                {"SNAPSHOT", "PREVIOUS" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical Criterion", "Technical Debt (Days)", "Technical Debt Added (Days)", "Technical Debt Removed (Days)" });
            expectedData.AddRange(new List<string> { "-", "-", "-", "-" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }
    }
}
