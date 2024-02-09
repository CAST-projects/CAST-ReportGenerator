using CastReporting.Domain.Imaging;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class OmgTechnicalDebtEvolutionTests
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
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.ServerVersion = "1.10.0.000";

            var component = new CastReporting.Reporting.Block.Table.OmgTechnicalDebtEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","CISQ" },
                {"MORE","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ", "Technical Debt (Days)",
                "No data found",""
            };

            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestBCAIP()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.OmgTechnicalDebtEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","AIP" },
                {"EVOLUTION", "true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Total Quality Index","Technical Debt (Days)","Technical Debt Added (Days)","Technical Debt Removed (Days)",
                "Architecture - Multi-Layers and Data Access","257.2","10.2","25.7"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestBCCISQMore()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.OmgTechnicalDebtEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","CISQ" },
                {"MORE", "true" },
                {"EVOLUTION", "true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ","Technical Debt (Days)","Technical Debt Added (Days)","Technical Debt Removed (Days)",
                "CISQ-Security","213.2","3.0","31.0",
                "    ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","244.3","24.1","132.8"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 3);

        }

    }
}

