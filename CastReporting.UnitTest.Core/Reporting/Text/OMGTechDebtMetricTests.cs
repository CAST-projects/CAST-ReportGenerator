using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class OMGTechnicalDebtMetricTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        public void TestCurrentIsoContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "ISO" },
                {"SNAPSHOT", "CURRENT"}
            };
            var component = new OMGTechnicalDebtMetric();
            var str = component.Content(reportData, config);
            Assert.AreEqual("244.3 Days", str);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        public void TestCurrentNoPreviousContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "ISO" },
                {"SNAPSHOT", "PREVIOUS"}
            };
            var component = new OMGTechnicalDebtMetric();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

    }
}
