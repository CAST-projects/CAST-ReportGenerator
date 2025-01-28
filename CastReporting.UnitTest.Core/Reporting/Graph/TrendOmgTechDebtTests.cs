using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class TrendOmgTechDebtTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/2", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new TrendOmgTechDebt();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "ISO" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Debt Removed (Days)", "Debt Added (Days)", "Debt (Days)" });
            expectedData.AddRange(new List<string> { "42756", "-10.6", "0.2", "244.3" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        public void TestNoValue()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new TrendOmgTechDebt();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "12345" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Debt Removed (Days)", "Debt Added (Days)", "Debt (Days)" });
            expectedData.AddRange(new List<string> { "42756", "0", "0", "0" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCresults.json", "Data")]
        public void TestTwoSnapshots()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/2", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCresults.json", "AED/applications/3/snapshots/1", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new TrendOmgTechDebt();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "ISO" }
            };

            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Debt Removed (Days)", "Debt Added (Days)", "Debt (Days)" });
            expectedData.AddRange(new List<string> { "42755", "-0.2", "10.6", "254.7" });
            expectedData.AddRange(new List<string> { "42756", "-10.6", "0.2", "244.3" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);

            Assert.IsNull(table.GraphOptions);

        }

    }
}
