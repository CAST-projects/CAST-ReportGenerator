using CastReporting.Domain;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class QualityTagsRulesEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagsCWE78results.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestBadServerVersion()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            reportData.ServerVersion = "1.10.5.000";
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-2011-Top25",
                "No data found"
            };

            TestUtility.AssertTableContent(table, expectedData, 1, 2);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagsCWE78results.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWE()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-2011-Top25","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0","0","0",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 10);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsSTIGv4R8CAT1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagsOWASPresults1.json", "Data")]
        [DeploymentItem(@".\Data\StandardTagsSTIG.json", "Data")]
        [DeploymentItem(@".\Data\RulePatterns.json", "Data")]
        public void TestStgTagDetailSTIGWithDescription()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTagsSTIG.json");
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

            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","STIG-V4R8-CAT1" },
                {"DESC","true" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "STIG-V4R8-CAT1", "Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities","Rationale","Description","Remediation",
                "STIG-V-70245 The application must protect the confidentiality and integrity of transmitted information.","0","0","0","","","",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                    "For portability reasons, 'java.lang.Runtime.exec()' should not be used since it means being dependant on the environment where the application is deployed.\nFor security reasons, 'java.lang.Runtime.exec()' can lead to malicious file execution resulting in devastating attacks such as total server compromise.\n\nThere are uses correct uses of Runtime.exec for example when the method call is platform neutral.\n\nSuch examples of the correct use of Runtime.exec are:\n- Invocation of a Java compiler, with the name of the compiler specified as a\nuser-settable Property.\n- Execution of a command the user typed in (a \"shell\").\n- Invocation of a browser, configured as part of the installation of the\nprogram, when the user presses a \"Help\" button.",
                    "Java artifacts should not use 'java.lang.Runtime.exec()'","",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                    "The software constructs all or part of an OS command using externally-influenced input from an upstream component, but it does not neutralize or incorrectly neutralizes special elements that could modify the intended OS command when it is sent to a downstream component.\nThis could allow attackers to execute unexpected, dangerous commands directly on the operating system. This weakness can lead to a vulnerability in environments in which the attacker does not have direct access to the operating system, such as in web applications.",
                    "The software constructs all or part of an OS command using externally-influenced input from an upstream component, but it does not neutralize or incorrectly neutralizes special elements that could modify the intended OS command when it is sent to a downstream component.\n\nUsing CAST data-flow engine, this metric detects paths from user input methods down to system call API methods, paths which are open vulnerabilities to operating system injection flaws.",
                    "Assume all input is malicious. \nAvoid using inputs. If it is not possible, use an \"accept known good\" input validation strategy, i.e., use stringent white-lists that limit the character set based on the expected value of the parameter in the request. This will indirectly limit the scope of an attack.",
                "STIG-V-70261 The application must protect from command injection.","0","0","0","","","",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                    "For portability reasons, 'java.lang.Runtime.exec()' should not be used since it means being dependant on the environment where the application is deployed.\nFor security reasons, 'java.lang.Runtime.exec()' can lead to malicious file execution resulting in devastating attacks such as total server compromise.\n\nThere are uses correct uses of Runtime.exec for example when the method call is platform neutral.\n\nSuch examples of the correct use of Runtime.exec are:\n- Invocation of a Java compiler, with the name of the compiler specified as a\nuser-settable Property.\n- Execution of a command the user typed in (a \"shell\").\n- Invocation of a browser, configured as part of the installation of the\nprogram, when the user presses a \"Help\" button.",
                    "Java artifacts should not use 'java.lang.Runtime.exec()'","",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                    "The software constructs all or part of an OS command using externally-influenced input from an upstream component, but it does not neutralize or incorrectly neutralizes special elements that could modify the intended OS command when it is sent to a downstream component.\nThis could allow attackers to execute unexpected, dangerous commands directly on the operating system. This weakness can lead to a vulnerability in environments in which the attacker does not have direct access to the operating system, such as in web applications.",
                    "The software constructs all or part of an OS command using externally-influenced input from an upstream component, but it does not neutralize or incorrectly neutralizes special elements that could modify the intended OS command when it is sent to a downstream component.\n\nUsing CAST data-flow engine, this metric detects paths from user input methods down to system call API methods, paths which are open vulnerabilities to operating system injection flaws.",
                    "Assume all input is malicious. \nAvoid using inputs. If it is not possible, use an \"accept known good\" input validation strategy, i.e., use stringent white-lists that limit the character set based on the expected value of the parameter in the request. This will indirectly limit the scope of an attack."
            };

            TestUtility.AssertTableContent(table, expectedData, 7, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagsOWASPresults1.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWEHeaders()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
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
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 10);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTC.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagResultsCWE.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_StdTagsOWASPresults1.json", "Data")]
        [DeploymentItem(@".\Data\StandardTags.json", "Data")]
        public void TestStgTagCWENoHeaders()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTC.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, null, null, null, null, null);
            reportData = TestUtility.AddStandardTags(reportData, @".\Data\StandardTags.json");
            WSImagingConnection connection = new WSImagingConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CWE-2011-Top25" },
                {"HEADER","NO" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CWE-22 Improper Limitation of a Pathname to a Restricted Directory ('Path Traversal')","0","0","0",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                "CWE-78 Improper Neutralization of Special Elements used in an OS Command ('OS Command Injection')","7","7","5",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0",
                "CWE-79 Improper Neutralization of Input During Web Page Generation ('Cross-site Scripting')","7","7","2",
                "    Avoid using 'java.lang.Runtime.exec()'","5","2","1",
                "    Avoid OS command injection vulnerabilities","0","0","0"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 9);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestIdxCISQ()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","CISQ-Security" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ-Security","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "ASCSM-CWE-78 - OS Command Injection Improper Input Neutralization","17","3","10",
                "    Avoid thread injection vulnerabilities","5","5","3",
                "    Avoid using eval() (Javascript)","4","3","2",
                "ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","15","9","6",
                "    Avoid file path manipulation vulnerabilities","10","7","3"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 6);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentBCTCindex.json", "Data")]
        public void TestIdxCisqId()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @".\Data\CurrentBCTCindex.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
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
            var component = new CastReporting.Reporting.Block.Table.QualityTagsRulesEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"STD","1062104" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "CISQ-Security","Total Vulnerabilities","Added Vulnerabilities","Removed Vulnerabilities",
                "ASCSM-CWE-78 - OS Command Injection Improper Input Neutralization","17","3","10",
                "    Avoid thread injection vulnerabilities","5","5","3",
                "    Avoid using eval() (Javascript)","4","3","2",
                "ASCSM-CWE-22 - Path Traversal Improper Input Neutralization","15","9","6",
                "    Avoid file path manipulation vulnerabilities","10","7","3"
            };

            TestUtility.AssertTableContent(table, expectedData, 4, 6);
        }

    }
}

