﻿using CastReporting.Domain;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class QualityRuleViolationsBookmarksTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        // TODO :
        // - test with no count / count = 2
        // - test with only one snapshot (previous does not exists= => no status column)
        // - test with each type of metric type : integer, text, percentage, null, group, path, bookmarks


        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings7392.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7424.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "",
                "Violation #1    Avoid using SQL queries inside a loop",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "Associated Value: 3",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 30);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(28, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings7392.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCresults.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7424.json", "Data")]
        public void TestTwoSnapshotNoCount()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCresults.json", "AED/applications/3/snapshots/5", "Version 1.4.1", "V-1.4.1", previousDate);
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(151, table.NbRows);
            Assert.AreEqual("Violation #1    Avoid using SQL queries inside a loop", table.Data.ElementAt(2));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adg_central_grades_std", table.Data.ElementAt(3));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(4));
            Assert.AreEqual("Status: added", table.Data.ElementAt(5));
            Assert.AreEqual("Associated Value: 3", table.Data.ElementAt(6));
            Assert.AreEqual("Violation #4    Avoid using SQL queries inside a loop", table.Data.ElementAt(92));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adg_m_central_grades_std", table.Data.ElementAt(93));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(94));
            Assert.AreEqual("Status: updated", table.Data.ElementAt(95));
            Assert.AreEqual("Associated Value: 3", table.Data.ElementAt(96));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(145, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings7392.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7424.json", "Data")]
        public void TestAllViolations()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","-1" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(204, table.NbRows);
            Assert.AreEqual("Violation #1    Avoid using SQL queries inside a loop", table.Data.ElementAt(2));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adg_central_grades_std", table.Data.ElementAt(3));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(4));
            Assert.AreEqual("Associated Value: 3", table.Data.ElementAt(5));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql", table.Data.ElementAt(6));
            Assert.AreEqual("Violation #7    Avoid using SQL queries inside a loop", table.Data.ElementAt(176));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adgc_delta_debt_removed", table.Data.ElementAt(177));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(178));
            Assert.AreEqual("Associated Value: 3", table.Data.ElementAt(179));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql", table.Data.ElementAt(180));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(196, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings7392.json", "Data")]
        [DeploymentItem(@"Data/findings_bookmarks.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7424.json", "Data")]
        public void TestBookmark()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "",
                "Violation #1    Avoid using SQL queries inside a loop",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "Associated Value: 3",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 30);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(28, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_integer.json", "Data")]
        [DeploymentItem(@"Data/RulePatterns.json", "Data")]
        public void TestTypeIntegerNull()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7390" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid having multiple Artifacts inserting data on the same SQL Table",
                "",
                "Violation #1    Avoid having multiple Artifacts inserting data on the same SQL Table",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "Associated Value: 2",
                "File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp",
                "4904 :      m_bGridModified = FALSE;",
                "4905 :  }",
                "4906 : ",
                "4907 :  void CMetricTreePageDet::Validate()",
                "4908 :  {",
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 13);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(11, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_groups.json", "Data")]
        [DeploymentItem(@"Data/RulePatterns.json", "Data")]
        public void TestTypeGroup()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7156" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid Too Many Copy Pasted Artifacts",
                "",
                "Violation #1    Avoid Too Many Copy Pasted Artifacts",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "C:\\JENKINS6_SLAVE\\WORKSPACE\\CAIP_8.3.3_TESTE2E_CSS_ADG\\WORK\\CAST\\DEPLOY\\JURASSIC PARK\\WEBDYNPRO\\SIOS\\ABAP/CLASSPOOL/CX_IOS_APPLICATIONPROPERTIES/CLASS/CX_IOS_APPLICATIONPROPERTIES/METHOD/CONSTRUCTOR",
                "C:\\JENKINS6_SLAVE\\WORKSPACE\\CAIP_8.3.3_TESTE2E_CSS_ADG\\WORK\\CAST\\DEPLOY\\JURASSIC PARK\\WEBDYNPRO\\SIOS\\ABAP/CLASSPOOL/CX_IOS_WORDPROCESSING/CLASS/CX_IOS_WORDPROCESSING/METHOD/CONSTRUCTOR",
                ""
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 8);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(5, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_null.json", "Data")]
        [DeploymentItem(@"Data/RulePatterns.json", "Data")]
        public void TestTypeNull()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7210" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid instantiations inside loops",
                "",
                "Violation #1    Avoid instantiations inside loops",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "File path: c:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Jurassic Park\\JSPBookDemo\\WEB-INF\\classes\\com\\castsoftware\\FrameworkOld\\util\\db\\DBUtils.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: c:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Jurassic Park\\JSPBookDemo\\WEB-INF\\classes\\com\\castsoftware\\FrameworkOld\\util\\db\\DBUtils.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 21);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(19, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_objects.json", "Data")]
        [DeploymentItem(@"Data/RulePatterns.json", "Data")]
        public void TestTypeObject()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","4722" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Track Classes referencing Database objects",
                "",
                "Violation #1    Track Classes referencing Database objects",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "O11JNK.LINDBERGH_CENTRAL.VIEWER_ACTIONS",
                "O11JNK.LINDBERGH_CENTRAL.VIEWER_DATA_PERSISTERS"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 7);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(5, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_path.json", "Data")]
        [DeploymentItem(@"Data/RulePatterns.json", "Data")]
        public void TestTypePath()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7740" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule CWE-79: Avoid cross-site scripting DOM vulnerabilities",
                "",
                "Violation #1    CWE-79: Avoid cross-site scripting DOM vulnerabilities",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "Violation Path #1",
                "File path: c:\\jenkins6_slave\\workspace\\caip_8.3.3_teste2e_css_adg\\work\\cast\\deploy\\jurassic park\\jspbookdemo\\pages\\newsales.jsp",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: c:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Jurassic Park\\WASecu\\WASecurityForm\\Default.aspx.cs",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                ""
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 23);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(20, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_text.json", "Data")]
        [DeploymentItem(@"Data/RulePatterns.json", "Data")]
        public void TestTypeText()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","1596" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using \"nullable\" Columns except in the last position in a Table",
                "",
                "Violation #1    Avoid using \"nullable\" Columns except in the last position in a Table",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp",
                "4904 :      m_bGridModified = FALSE;",
                "4905 :  }",
                "4906 : ",
                "4907 :  void CMetricTreePageDet::Validate()",
                "4908 :  {",
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 12);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(10, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings_percentage.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7846.json", "Data")]
        public void TestTypePercentage()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7846" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid Methods with a very low comment/code ratio",
                "",
                "Violation #1    Avoid Methods with a very low comment/code ratio",
                "Object Name: com.castsoftware.aad.common.AadCommandLine.dumpStack",
                "Object Type: MyObjType",
                "File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp",
                "4904 :      m_bGridModified = FALSE;",
                "4905 :  }",
                "4906 : ",
                "4907 :  void CMetricTreePageDet::Validate()",
                "4908 :  {",
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 12);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(10, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings7392.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7424.json", "Data")]
        public void TestSimpleDescription()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","1" },
                {"DESC", "Simple" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "",
                "Rationale: ",
                "Having an SQL query inside a loop is usually the source of performance and scalability problems especially if the number of iterations become very high (for example if it is dependent on the data returned from the database).",
                "This iterative pattern has proved to be very dangerous for application performance and scalability. Database servers perform much better in set-oriented patterns rather than pure iterative ones.",
                "Description: ",
                "This metric retrieves all artifacts using at least one SQL query inside a loop statement.",
                "Remediation: ",
                "The remediation is often to replace the iterative approach based on a loop with a set-oriented one and thus modify the query.",
                "",
                "Violation #1    Avoid using SQL queries inside a loop",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "Associated Value: 3",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 38);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(35, cellsProperties.Count);

        }

        [TestMethod]
        [DeploymentItem(@"Data/Violations7424_60017.json", "Data")]
        [DeploymentItem(@"Data/Violations7846_60016.json", "Data")]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/findings7392.json", "Data")]
        [DeploymentItem(@"Data/RulePattern7424.json", "Data")]
        public void TestFullDescription()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityRuleViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","7424" },
                {"COUNT","1" },
                {"DESC", "full" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "",
                "Rationale: ",
                "Having an SQL query inside a loop is usually the source of performance and scalability problems especially if the number of iterations become very high (for example if it is dependent on the data returned from the database).","This iterative pattern has proved to be very dangerous for application performance and scalability. Database servers perform much better in set-oriented patterns rather than pure iterative ones.",
                "Description: ",
                "This metric retrieves all artifacts using at least one SQL query inside a loop statement.",
                "Remediation: ",
                "The remediation is often to replace the iterative approach based on a loop with a set-oriented one and thus modify the query.",
                "Sample: ",
                "Oracle:","for x in ( select * from t1 )","loop","  for y in ( select * from t2 where t2.col = x.COL )","  loop ","    for z in (select * from t3 where t3.col = y.SOMETHING )","    loop","      update table_name set co1 = z.SOMETHING_ELSE where table_name.col2 = z.KeyName;","    end loop;","  end loop;","end loop;","","Microsoft SQL Server:","WHILE @Counter <= @MaxOscars","BEGIN","SET @NumFilms =","(","SELECT COUNT(*)","FROM tblFilm","WHERE FilmOscarWins = @Counter",")","PRINT","CAST(@NumFilms AS VARCHAR(3)) +","' films have won ' +","CAST(@Counter AS VARCHAR(2)) +","' Oscars.'","SET @Counter += 1","END","","PreparedStatement updateSales;","String updateString = \"update COFFEES \" +","                      \"set SALES = ? where COF_NAME like ?\";","updateSales = con.prepareStatement(updateString);","","int len = coffees.length;","for(int i = 0; i < len; i++) {","                updateSales.setInt(1, salesForWeek[i]);","                updateSales.setString(2, coffees[i]);","                updateSales.executeUpdate();    // VIOLATION","        }",
                "Remediation Sample: ",
                "Oracle:"," update table_name"," set co1 = (select z.SOMETHING_ELSE","              from t3 z","                  join t2 y on z.col = y.SOMETHING","                  join t1 x on y.col = x.COL","              where table_name.col2 = z.KeyName)","where exists(select 1","              from t3 z","                  join t2 y on z.col = y.SOMETHING","                  join t1 x on y.col = x.COL","              where table_name.col2 = z.KeyName);","","Microsoft SQL Server:","SELECT","FilmOscarWins",",COUNT(*) AS [NumberOfFilms]","FROM","tblFilm","GROUP BY","FilmOscarWins",
                "Output: ",
                "Associated to each client-server artifact with violations, the Quality Rule provides:","- Number of violation patterns","- Bookmarks for violation patterns found in the source code:","  - SQL query","  - loop starting line",
                "Associated Value: ",
                "Number of violation patterns",
                "Total: ",
                "Number of client-server artifacts using tables and views",
                "",
                "Violation #1    Avoid using SQL queries inside a loop",
                "Object Name: aedtst_exclusions_central.adg_central_grades_std",
                "Object Type: MyObjType",
                "Associated Value: 3",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }",
                "File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java",
                "1197 : PreparedStatement statement = null;",
                "1198 :         try",
                "1199 :         {",
                "1200 :             statement = consolidatedConn.prepareStatement(insertMessage); ",
                "1201 :             statement.setString(1, message); ",
                "1202 :             statement.executeUpdate(); ",
                "1203 :         }"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 111);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(108, cellsProperties.Count);

        }

    }
}

