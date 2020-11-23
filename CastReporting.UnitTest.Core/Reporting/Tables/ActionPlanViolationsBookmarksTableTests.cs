using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class ActionPlanViolationsBookmarksTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484953100000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Rule Name", "Object Name", "Object Type","Status", "Priority", "Associated Value", "File path","Start Line", "End Line",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Cast.Util.ExpressionEvaluator.Eval", "MyObjType", "added", "low", "3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Cast.Util.ExpressionEvaluator.Eval", "MyObjType", "added", "low","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "Cast.Util.ExpressionEvaluator.Eval", "MyObjType", "added", "low","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.CastDomainBLL.GetCategories", "MyObjType", "pending", "moderate","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.CastDomainBLL.GetCategories", "MyObjType", "pending", "moderate","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.CastDomainBLL.GetCategories", "MyObjType", "pending", "moderate","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "extreme","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "extreme","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "extreme","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Builder.WorksheetAccessorExt","MyObjType","added", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Builder.WorksheetAccessorExt","MyObjType","added", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Builder.WorksheetAccessorExt","MyObjType","added", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper","MyObjType","solved", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper","MyObjType","solved", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper","MyObjType","solved", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.UI.WPF.Utilities.PasswordBoxAssistant","MyObjType","pending", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.UI.WPF.Utilities.PasswordBoxAssistant","MyObjType","pending", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.UI.WPF.Utilities.PasswordBoxAssistant","MyObjType","pending", "high","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
            };
            TestUtility.AssertTableContent(table, expectedData, 9, 19);

        }


        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestFilter()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484953100000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","ALL" },
                {"FILTER","solved" },
                {"TAG", "NO" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Rule Name", "Object Name", "Object Type","Status", "Priority", "Associated Value", "File path","Start Line", "End Line",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "low","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "low","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "low","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper","MyObjType","solved", "low","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper","MyObjType","solved", "low","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Provide a private default Constructor for utility Classes", "CastReporting.Reporting.Helper.StreamHelper","MyObjType","solved", "low","3", "D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201"
            };
            TestUtility.AssertTableContent(table, expectedData, 9, 7);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\ActionPlanViolations1.json", "Data")]
        [DeploymentItem(@".\Data\CurrentBCresults.json", "Data")]
        [DeploymentItem(@".\Data\PreviousBCresults.json", "Data")]
        public void TestNoHeader()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484953100000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @".\Data\CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, @".\Data\PreviousBCresults.json", "AED/applications/3/snapshots/5", "PreVersion 1.5.0 sprint 2 shot 1", "V-1.5.0_Sprint 2_1", previousDate);
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var component = new CastReporting.Reporting.Block.Table.ActionPlanViolationsBookmarksTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COUNT","1" },
                {"FILTER","solved" },
                {"HEADER", "no" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>
            {
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "extreme","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\SQL\\central.sql","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "extreme","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201",
                "Avoid catching an exception of type Exception, RuntimeException, or Throwable", "CastReporting.BLL.SnapshotBLL.GetBackgroundFacts", "MyObjType", "solved", "extreme","3","D:\\CASTMS\\TST834\\Deploy\\Team\\AADAED\\Java\\AADAdmin\\AadSite\\sources\\com\\castsoftware\\aad\\site\\AadSite.java","1200","1201"
            };
            TestUtility.AssertTableContent(table, expectedData, 9, 3);

        }

    }
}
