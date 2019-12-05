using System.Collections.Generic;
using System.Linq;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesListViolationsBookmarksTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        // TODO :
        // - test with no count / count = 1
        // - test for metrics by standard tag / bc / tc / metric ids

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestTCmetrics()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
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

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","61028" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Rule Name", "Object Name", "Object Type","Status", "Associated Value", "File path","Start Line", "End Line",
                "Avoid using SQL queries inside a loop", "aedtst_exclusions_central.adg_central_grades_std", "MyObjType", "added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop", "aedtst_exclusions_central.adg_central_grades_std", "MyObjType", "added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop", "aedtst_exclusions_central.adg_central_grades_std", "MyObjType", "added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid Methods with a very low comment/code ratio", "com.castsoftware.aad.common.AadCommandLine.dumpStack","MyObjType","added","2.99", "C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp","2","6"
            };

            TestUtility.AssertTableContent(table, expectedData, 8, 5);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestNocountMetric()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","7846" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            { "Rule Name","Object Name","Object Type","Status","Associated Value","File path","Start Line","End Line",
                "Avoid Methods with a very low comment/code ratio","com.castsoftware.aad.common.AadCommandLine.dumpStack","MyObjType","","2.99","C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp","2","6",
                "Avoid Methods with a very low comment/code ratio","com.castsoftware.aed.common.AedCommandLine.dumpStack","MyObjType","","2.99","C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp","2","6",
                "Avoid Methods with a very low comment/code ratio","com.castsoftware.aad.common.AadCommandLine.logInBase","MyObjType","","2.99","C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp","2","6",
                "Avoid Methods with a very low comment/code ratio","com.castsoftware.aed.common.AedCommandLine.logInBase","MyObjType","","2.99","C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp","2","6",
                "Avoid Methods with a very low comment/code ratio","com.castsoftware.aed.common.AedCommandLine.getFormattedMsg","MyObjType","","2.99","C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp","2","6"
            };
            TestUtility.AssertTableContent(table, expectedData, 8, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestMetricsStdTag()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","CWE" },
                {"COUNT", "-1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            { "Rule Name","Object Name","Object Type","Status","Associated Value","File path","Start Line","End Line",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_init_techno_children","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_init_techno_children","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_init_techno_children","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_m_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_m_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_m_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_m_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_m_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_m_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adgc_delta_debt_added","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adgc_delta_debt_added","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adgc_delta_debt_added","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adgc_delta_debt_removed","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adgc_delta_debt_removed","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adgc_delta_debt_removed","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201"            
            };
            TestUtility.AssertTableContent(table, expectedData, 8, 22);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestBCmetrics()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @".\Data\PreviousBCTC.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
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

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>
            { "Rule Name","Object Name","Object Type","Status","Associated Value","File path","Start Line","End Line",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","added","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201"
            };
            TestUtility.AssertTableContent(table, expectedData, 8, 19);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestBCmetricsCritical()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"CRITICAL","true" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            { "Rule Name","Object Name","Object Type","Status","Associated Value","File path","Start Line","End Line",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_grades_std","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid using SQL queries inside a loop","aedtst_exclusions_central.adg_central_startup_init","MyObjType","","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201"
            };
            TestUtility.AssertTableContent(table, expectedData, 8, 7);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            reportData.ServerVersion = "1.10.5.000";
            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"CRITICAL","true" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Violations",
                "No data found"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 2);
        }


    }
}

