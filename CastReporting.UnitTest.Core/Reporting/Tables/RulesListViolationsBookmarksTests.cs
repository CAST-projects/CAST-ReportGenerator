using CastReporting.Domain;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class RulesListViolationsBookmarksTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestTCmetrics()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","61028" },
                {"COUNT","1" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Violations",
                "",
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "# Violations: 86",
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
                "1203 :         }",
                "",
                "",
                "",
                "",
                "",
                "Objects in violation for rule Avoid Methods with a very low comment/code ratio",
                "# Violations: 128",
                "",
                "Rationale: ",
                "Maintainability of the code is facilitated if there is documentation in the code. This rule will ensure there are comments within the Artifact",
                "Description: ",
                "Methods should have at least a ratio comment/code > X %",
                "The threshold is a parameter and can be changed at will.",
                "Remediation: ",
                "Enrich Artifact code with comments",
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
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;",
                "",
                "",
                "",
                ""
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 71);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(56, cellsProperties.Count);
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
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","7846" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(71, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Avoid Methods with a very low comment/code ratio", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 128", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #5    Avoid Methods with a very low comment/code ratio", table.Data.ElementAt(57));
            Assert.AreEqual("Object Name: com.castsoftware.aed.common.AedCommandLine.getFormattedMsg", table.Data.ElementAt(58));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(59));
            Assert.AreEqual("File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp", table.Data.ElementAt(60));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(59, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestNoCodeLines()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","7846" },
                {"WITHCODELINES", "N" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(41, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Avoid Methods with a very low comment/code ratio", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 128", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #5    Avoid Methods with a very low comment/code ratio", table.Data.ElementAt(33));
            Assert.AreEqual("Object Name: com.castsoftware.aed.common.AedCommandLine.getFormattedMsg", table.Data.ElementAt(34));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(35));
            Assert.AreEqual("File path: C:\\jenkins6_slave\\workspace\\CAIP_8.3.3_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\Dream Team\\DssAdmin\\DssAdmin\\MetricTree.cpp", table.Data.ElementAt(36));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(29, cellsProperties.Count);
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
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","CWE" },
                {"COUNT", "-1" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(219, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Avoid using SQL queries inside a loop", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 86", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #7    Avoid using SQL queries inside a loop", table.Data.ElementAt(187));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adgc_delta_debt_removed", table.Data.ElementAt(188));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(189));
            Assert.AreEqual("Associated Value: 3", table.Data.ElementAt(190));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql", table.Data.ElementAt(191));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java", table.Data.ElementAt(199));
            Assert.AreEqual("1203 :         }", table.Data.ElementAt(214));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(205, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestMetricsStdTagNoHeader()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","CWE" },
                {"COUNT", "-1" },
                {"HEADER","no" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(218, table.NbRows);
            Assert.AreEqual("Objects in violation for rule Avoid using SQL queries inside a loop", table.Data.ElementAt(1));
            Assert.AreEqual("# Violations: 86", table.Data.ElementAt(2));
            Assert.AreEqual("Violation #7    Avoid using SQL queries inside a loop", table.Data.ElementAt(186));
            Assert.AreEqual("Object Name: aedtst_exclusions_central.adgc_delta_debt_removed", table.Data.ElementAt(187));
            Assert.AreEqual("Object Type: MyObjType", table.Data.ElementAt(188));
            Assert.AreEqual("Associated Value: 3", table.Data.ElementAt(189));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql", table.Data.ElementAt(190));
            Assert.AreEqual("File path: D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java", table.Data.ElementAt(206));
            Assert.AreEqual("1203 :         }", table.Data.ElementAt(213));

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(205, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\BaseQI60011.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        public void TestBCmetrics()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(218, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Action Mappings should have few forwards", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 77", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #1    Action Mappings should have few forwards", table.Data.ElementAt(10));
            Assert.AreEqual("Violation #2    Action Mappings should have few forwards", table.Data.ElementAt(39));
            Assert.AreEqual("Objects in violation for rule Avoid accessing data by using the position and length", table.Data.ElementAt(72));
            Assert.AreEqual("# Violations: 6", table.Data.ElementAt(73));
            Assert.AreEqual("Objects in violation for rule Avoid artifacts having recursive calls", table.Data.ElementAt(146));
            Assert.AreEqual("# Violations: 12", table.Data.ElementAt(147));
            Assert.AreEqual("1203 :         }", table.Data.ElementAt(213));


            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(193, cellsProperties.Count);

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
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","60011" },
                {"CRITICAL","true" },
                {"COUNT", "2" }
            };
            var table = component.Content(reportData, config);

            Assert.AreEqual(1, table.NbColumns);
            Assert.AreEqual(71, table.NbRows);
            Assert.AreEqual("Violations", table.Data.ElementAt(0));
            Assert.AreEqual("Objects in violation for rule Action Mappings should have few forwards", table.Data.ElementAt(2));
            Assert.AreEqual("# Violations: 77", table.Data.ElementAt(3));
            Assert.AreEqual("Violation #1    Action Mappings should have few forwards", table.Data.ElementAt(10));
            Assert.AreEqual("Violation #2    Action Mappings should have few forwards", table.Data.ElementAt(39));


            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(62, cellsProperties.Count);
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
            WSImagingConnection connection = new WSImagingConnection
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
            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
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

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestTCmetricsDescFull()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","61028" },
                {"COUNT","1" },
                {"DESC", "full" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Violations",
                "",
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "# Violations: 86",
                "",
                "Rationale: ",
                "Having an SQL query inside a loop is usually the source of performance and scalability problems especially if the number of iterations become very high (for example if it is dependent on the data returned from the database).",
                "This iterative pattern has proved to be very dangerous for application performance and scalability. Database servers perform much better in set-oriented patterns rather than pure iterative ones.",
                "Description: ",
                "This metric retrieves all artifacts using at least one SQL query inside a loop statement.",
                "Remediation: ",
                "The remediation is often to replace the iterative approach based on a loop with a set-oriented one and thus modify the query.",
                "Sample: ",
                "Oracle:",
                "for x in ( select * from t1 )",
                "loop",
                "  for y in ( select * from t2 where t2.col = x.COL )",
                "  loop ",
                "    for z in (select * from t3 where t3.col = y.SOMETHING )",
                "    loop",
                "      update table_name set co1 = z.SOMETHING_ELSE where table_name.col2 = z.KeyName;",
                "    end loop;",
                "  end loop;",
                "end loop;",
                "",
                "Microsoft SQL Server:",
                "WHILE @Counter <= @MaxOscars",
                "BEGIN",
                "SET @NumFilms =",
                "(",
                "SELECT COUNT(*)",
                "FROM tblFilm",
                "WHERE FilmOscarWins = @Counter",
                ")",
                "PRINT",
                "CAST(@NumFilms AS VARCHAR(3)) +",
                "' films have won ' +",
                "CAST(@Counter AS VARCHAR(2)) +",
                "' Oscars.'",
                "SET @Counter += 1",
                "END",
                "",
                "PreparedStatement updateSales;",
                "String updateString = \"update COFFEES \" +",
                "                      \"set SALES = ? where COF_NAME like ?\";",
                "updateSales = con.prepareStatement(updateString);",
                "",
                "int len = coffees.length;",
                "for(int i = 0; i < len; i++) {",
                "                updateSales.setInt(1, salesForWeek[i]);",
                "                updateSales.setString(2, coffees[i]);",
                "                updateSales.executeUpdate();    // VIOLATION",
                "        }",
                "Remediation Sample: ",
                "Oracle:",
                " update table_name",
                " set co1 = (select z.SOMETHING_ELSE",
                "              from t3 z",
                "                  join t2 y on z.col = y.SOMETHING",
                "                  join t1 x on y.col = x.COL",
                "              where table_name.col2 = z.KeyName)",
                "where exists(select 1",
                "              from t3 z",
                "                  join t2 y on z.col = y.SOMETHING",
                "                  join t1 x on y.col = x.COL",
                "              where table_name.col2 = z.KeyName);",
                "",
                "Microsoft SQL Server:",
                "SELECT",
                "FilmOscarWins",
                ",COUNT(*) AS [NumberOfFilms]",
                "FROM",
                "tblFilm",
                "GROUP BY",
                "FilmOscarWins",
                "Output: ",
                "Associated to each client-server artifact with violations, the Quality Rule provides:",
                "- Number of violation patterns",
                "- Bookmarks for violation patterns found in the source code:",
                "  - SQL query",
                "  - loop starting line",
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
                "1203 :         }",
                "",
                "",
                "",
                "",
                "",
                "Objects in violation for rule Avoid Methods with a very low comment/code ratio",
                "# Violations: 128",
                "",
                "Rationale: ",
                "Maintainability of the code is facilitated if there is documentation in the code. This rule will ensure there are comments within the Artifact",
                "Description: ",
                "Methods should have at least a ratio comment/code > X %",
                "The threshold is a parameter and can be changed at will.",
                "Remediation: ",
                "Enrich Artifact code with comments",
                "Output: ",
                "This report lists all Methods (excluding getters and setters) with comment/code ratio lower than X %",
                "It provides the following information: Method full name, comment/code ratio",
                "For .net, Getters and setters must not be considered as breaking the rule if the property/indexer itself is commented.",
                "Associated Value: ",
                "Associated Value",
                "Total: ",
                "Total number of methods",
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
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;",
                "",
                "",
                "",
                ""
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 152);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(137, cellsProperties.Count);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Violations7424_60017.json", "Data")]
        [DeploymentItem(@".\Data\Violations7846_60016.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7846.json", "Data")]
        [DeploymentItem(@".\Data\RulePattern7424.json", "Data")]
        [DeploymentItem(@".\Data\findings7392.json", "Data")]
        [DeploymentItem(@".\Data\findings_percentage.json", "Data")]
        public void TestTCmetricsNoDesc()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            reportData.RuleExplorer = new RuleBLLStub();

            var component = new CastReporting.Reporting.Block.Table.RulesListViolationsBookmarks();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"METRICS","61028" },
                {"COUNT","1" },
                {"DESC", "no" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Violations",
                "",
                "Objects in violation for rule Avoid using SQL queries inside a loop",
                "# Violations: 86",
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
                "1203 :         }",
                "",
                "",
                "",
                "",
                "",
                "Objects in violation for rule Avoid Methods with a very low comment/code ratio",
                "# Violations: 128",
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
                "4909 :      int i, index, nAggregate, nAggregateCentral, nType, nLastLine;",
                "",
                "",
                "",
                ""
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 55);

            var cellsProperties = table.CellsAttributes;
            Assert.AreEqual(42, cellsProperties.Count);
        }

    }
}

