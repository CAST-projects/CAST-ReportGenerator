﻿using CastReporting.Domain;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TopNonCriticalViolationsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCTC.json", "Data")]
        [DeploymentItem(@"Data/cc60011.json", "Data")]
        [DeploymentItem(@"Data/cc60011previous.json", "Data")]
        [DeploymentItem(@"Data/nc60011.json", "Data")]
        [DeploymentItem(@"Data/nc60011previous.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @"Data/cc60011.json", @"Data/cc60011previous.json");
            reportData = TestUtility.AddNonCriticalRuleViolations(reportData, 60011, @"Data/nc60011.json", @"Data/nc60011previous.json");

            var component = new CastReporting.Reporting.Block.Table.TopNonCriticalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "# Violations" });
            expectedData.AddRange(new List<string> { "Avoid unreferenced Tables", "209" });
            expectedData.AddRange(new List<string> { "Enumerations naming convention - case and character set control", "1" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCTC.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCTC.json", "Data")]
        [DeploymentItem(@"Data/cc60011.json", "Data")]
        [DeploymentItem(@"Data/cc60011previous.json", "Data")]
        [DeploymentItem(@"Data/nc60011.json", "Data")]
        [DeploymentItem(@"Data/nc60011previous.json", "Data")]
        public void TestLimitCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCTC.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);
            reportData = TestUtility.AddCriticalRuleViolations(reportData, 60011, @"Data/cc60011.json", @"Data/cc60011previous.json");
            reportData = TestUtility.AddNonCriticalRuleViolations(reportData, 60011, @"Data/nc60011.json", @"Data/nc60011previous.json");

            var component = new CastReporting.Reporting.Block.Table.TopNonCriticalViolations();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR","60011" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Rule Name", "# Violations" });
            expectedData.AddRange(new List<string> { "Avoid unreferenced Tables", "209" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }

    }
}

