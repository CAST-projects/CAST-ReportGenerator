using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class OmgTechDebtBubbleTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Metrics2.json", "Data")]
        public void TestContentSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Metrics2.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4", currentDate,
                null, null, null, null, null, null);

            var component = new OmgTechDebtBubble();
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
                {"ID", "AIP" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Grade", "Technical Debt (Days)", "Size (kLoC)" });
            expectedData.AddRange(new List<string> { "2.1", "244.3", "104851" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Modules1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults1_Extended.json", "Data")]
        public void TestContentSnapshotModule()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("DreamTeam",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults1_Extended.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4", currentDate,
               null, null, null, null, null, null);

            var component = new OmgTechDebtBubble();
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
                {"M", "5"},
                {"ID", "AIP" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Grade", "Technical Debt (Days)", "Size (kLoC)" });
            expectedData.AddRange(new List<string> { "2.68", "244.3", "12345" });
            TestUtility.AssertTableContent(table, expectedData, 3, 2);

        }
    }
}
