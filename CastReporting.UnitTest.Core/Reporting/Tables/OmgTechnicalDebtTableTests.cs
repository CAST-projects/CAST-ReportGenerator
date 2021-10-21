using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class OmgTechnicalDebtTableTests
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
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new OmgTechnicalDebtTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "AIP" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Value" });
            expectedData.AddRange(new List<string> { "Technical Debt (Days)", "766.0" });
            expectedData.AddRange(new List<string> { "Technical Debt Added (Days)", "0.3" });
            expectedData.AddRange(new List<string> { "Technical Debt Removed (Days)", "28.0" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestShortHeaders()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            var component = new OmgTechnicalDebtTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "ISO" },
                {"HEADER","SHORT" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Value" });
            expectedData.AddRange(new List<string> { "Debt", "244.3" });
            expectedData.AddRange(new List<string> { "Debt Added", "0.2" });
            expectedData.AddRange(new List<string> { "Debt Removed", "10.6" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        public void TestPreviousNullContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new OmgTechnicalDebtTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "AIP" },
                {"SNAPSHOT","PREVIOUS" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Value" });
            expectedData.AddRange(new List<string> { "Technical Debt (Days)", "n/a" });
            expectedData.AddRange(new List<string> { "Technical Debt Added (Days)", "n/a" });
            expectedData.AddRange(new List<string> { "Technical Debt Removed (Days)", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

    }
}
