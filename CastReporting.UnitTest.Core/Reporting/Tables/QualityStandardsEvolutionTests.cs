using CastReporting.Domain;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class QualityStandardsEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWE()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-2011-Top25","Total Vulnerabilities",
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7"
            };

            TestUtility.AssertTableContent(table, expectedData, 2, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        public void TestStgTagDetailSTIG()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8" },
                {"MORE","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8", "Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "STIG-V4R8-CAT1 ","0","0","0",
                "    STIG-V-70245 The application must protect the confidentiality and integrity of transmitted information.","0","0","0",
                "    STIG-V-70261 The application must protect from command injection.","0","0","0",
                "STIG-V4R8-CAT3 ","589","31","6",
                "    STIG-V-70385 The application development team must follow a set of coding standards.","589","31","6"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 6);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        public void TestStgTagDetailSTIGWithEvolution()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8" },
                {"MORE","true" },
                {"EVOLUTION", "true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8", "Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "STIG-V4R8-CAT1 ","0","0","0",
                "    STIG-V-70245 The application must protect the confidentiality and integrity of transmitted information.","0","0","0",
                "    STIG-V-70261 The application must protect from command injection.","0","0","0",
                "STIG-V4R8-CAT3 ","589","31","6",
                "    STIG-V-70385 The application development team must follow a set of coding standards.","589","31","6"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 6);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        public void TestStgTagDetailSTIGWithoutEvolution()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8" },
                {"MORE","true" },
                {"EVOLUTION", "false" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8", "Total Vulnerabilities",
                "STIG-V4R8-CAT1 ","0",
                "    STIG-V-70245 The application must protect the confidentiality and integrity of transmitted information.","0",
                "    STIG-V-70261 The application must protect from command injection.","0",
                "STIG-V4R8-CAT3 ","589",
                "    STIG-V-70385 The application development team must follow a set of coding standards.","589"
            };

            TestUtility.AssertTableContent(table, expectedData, 2, 6);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWEHeaders()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" },
                {"LBL","violations" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-2011-Top25","Total Violations","Added Violations","Removed Violations",
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 4);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWENoHeaders()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" },
                {"HEADER","no" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT3.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
            reportData.ServerVersion = "1.10.0.000";
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8" },
                {"MORE","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8", "Total Vulnerabilities",
                "No data found",""
            };

            TestUtility.AssertTableContent(table, expectedData, 2, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestBCOWASP2013()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","OWASP-2013" },
                {"EVOLUTION", "true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "OWASP-2013","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "ASCSM-CWE-78 - OS Command Injection Improper Input Neutralization","17","3","10",
                "ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","15","9","6"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 3);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestBCCISQMore()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.QualityStandardsEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CISQ" },
                {"MORE", "true" },
                {"EVOLUTION", "true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "CISQ-Security","11","11","2",
                "    ASCSM-CWE-78 - OS Command Injection Improper Input Neutralization","17","3","10",
                "    ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","15","9","6"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 4);

        }

    }
}

