using CastReporting.Domain;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class OmgTechnicalDebtRulesEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.ServerVersion = "1.10.0.000";

            var component = new CastReporting.Reporting.Block.Table.OmgTechnicalDebtRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","CISQ" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ", "Technical Debt (Days)","Technical Debt Added (Days)","Technical Debt Removed (Days)",
                "No data found","","",""
            };
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestIdxCISQ()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.OmgTechnicalDebtRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","CISQ-Security" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ-Security","Technical Debt (Days)","Technical Debt Added (Days)","Technical Debt Removed (Days)",
                "ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","244.3","24.1","132.8",
                "    Avoid file path manipulation vulnerabilities","4.4","0.0","4.4"
            };
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestIdxCISQWithDesc()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.OmgTechnicalDebtRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","CISQ-Security" },
                {"DESC", "true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ-Security","Technical Debt (Days)","Technical Debt Added (Days)","Technical Debt Removed (Days)","Rationale","Description","Remediation",
                "ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","244.3","24.1","132.8","","","",
                "    Avoid file path manipulation vulnerabilities","4.4","0.0","4.4",
                "The software does not properly neutralize special elements that are part of paths or file names used in file system operations. This could allow an attacker to access or modify system files or other files that are critical to the application.",
                "Using CAST data-flow engine, this metric detects execution paths from user input methods down to file creation methods, paths which are open vulnerabilities to operating system injection flaws.",
                "Assume all input is malicious."
            };
            TestUtility.AssertTableContent(table, expectedData, 7, 3);
        }

    }
}

