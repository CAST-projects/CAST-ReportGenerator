using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class TotalViolationsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        // Business Criteria Tests
        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestCurrentBusinessCriteria()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60011" }  // Transferability (Business Criteria)
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("2,157", str);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCTC.json", "Data")]
        public void TestPreviousBusinessCriteria()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60011" },  // Transferability (Business Criteria)
                {"SNAPSHOT", "PREVIOUS" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("2,375", str);
        }

        // Technical Criteria Tests
        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestCurrentTechnicalCriteria()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "61001" }  // Architecture - Multi-Layers and Data Access (Technical Criteria)
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("7", str);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCTC.json", "Data")]
        public void TestPreviousTechnicalCriteria()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "61001" },  // Architecture - Multi-Layers and Data Access (Technical Criteria)
                {"SNAPSHOT", "PREVIOUS" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("7", str);
        }

        // Quality Rule Tests
        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestCurrentQualityRule()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "7424" }  // Avoid using SQL queries inside a loop (Quality Rule)
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("86", str);
        }

        // Edge Cases
        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestNoPreviousSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60011" },
                {"SNAPSHOT", "PREVIOUS" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestInvalidMetricId()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "invalid" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestMissingMetricId()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        public void TestNonExistentMetricId()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);

            var component = new TotalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "99999" }  // Non-existent metric
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("n/a", str);
        }
    }
}
