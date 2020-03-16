using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class MetricEvolutionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestBCpercent()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=60014,FORMAT=N2
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
             */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60014"},
                {"FORMAT", "PERCENT"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("-0.65%", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestTCabsolute()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=PREVIOUS,ID=61001,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "61007"},
                {"FORMAT", "ABSOLUTE"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("-0.15", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestQR()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=7254,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "7254"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("+0.26%", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestQRModule()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=7254,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "4656"},
                {"MODULE", "sm-central/AppliAEPtran/Shopizer_src content" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("+0.60%", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestQRTechno()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=7254,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL" };
            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "4656"},
                {"TECHNO", "JEE" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("+0.24%", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@".\Data\Snapshot_QIresults2.json", "Data")]
        public void TestBCModuleTechno()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=7254,FORMAT=N2
             * @".\Data\Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @".\Data\Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @".\Data\Modules1.json", @".\Data\Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL" };
            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60012"},
                {"MODULE", "sm-core/AppliAEPtran/Shopizer_src content" },
                {"TECHNO", "JEE" }
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("-0.04%", str);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        public void TestSizing()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,SZID=10151,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "10151"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("+13.0%", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        public void TestSizingAbsolute()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=PREVIOUS,SZID=10154,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "10154"},
                {"FORMAT", "ABSOLUTE"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("94", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@".\Data\DreamTeamSnap1Sample12.json", "Data")]
        [DeploymentItem(@".\Data\BackFacts.json", "Data")]
        [DeploymentItem(@".\Data\BackFactsPrevious.json", "Data")]
        public void TestBackFact()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,BFID=66061,FORMAT=N0
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @".\Data\DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @".\Data\DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            // Needed for background facts, as there are retrieved one by one by url request
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection,reportData.CurrentSnapshot);
            
            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "66061"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("+50%", str);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        public void TestMillionId()
        {
            /*
             * Configuration : TEXT;APPLICATION_METRIC;SNAPSHOT=CURRENT,ID=60014,FORMAT=N2
            * @".\Data\Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @".\Data\Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
             */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new MetricEvolution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "11203569"},
                {"FORMAT", "ABSOLUTE"}
            };
            var str = component.Content(reportData, config);
            Assert.AreEqual("-0.09", str);
        }

    }
}
