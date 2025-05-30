﻿using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class GenericTableTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@"Data/Sample1Current.json", "Data")]
        public void TestSample1()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=60014|60017|60013,SNAPSHOTS=CURRENT
             * @"Data/Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
             */

            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @"Data/Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60014|60017|60013"},
                {"SNAPSHOTS", "CURRENT"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Efficiency", "Total Quality Index", "Robustness" });
            expectedData.AddRange(new List<string> { "PreVersion 1.5.0 sprint 2 shot 2 - V-1.5.0_Sprint 2_2", "2.59", "2.78", "3.19" });
            TestUtility.AssertTableContent(table, expectedData, 4, 2);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Sample1Current.json", "Data")]
        [DeploymentItem(@"Data/Sample1Previous.json", "Data")]
        public void TestSample2()
        {
            /*
            * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=60014|60017|60013,SNAPSHOTS=CURRENT|PREVIOUS
            * @"Data/Sample1Current.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/6/results?quality-indicators=(60013,60014,60017)
            * @"Data/Sample1Previous.json" => http://localhost:7070/CAST-AAD-AED/rest/AED/applications/3/snapshots/3/results?quality-indicators=(60013,60014,60017)
            */
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @"Data/Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @"Data/Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60014|60017|60013"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Efficiency", "Total Quality Index", "Robustness" });
            expectedData.AddRange(new List<string> { "PreVersion 1.5.0 sprint 2 shot 2 - V-1.5.0_Sprint 2_2", "2.59", "2.78", "3.19" });
            expectedData.AddRange(new List<string> { "PreVersion 1.4.1 before release - V-1.4.1", "2.61", "2.61", "2.88" });
            TestUtility.AssertTableContent(table, expectedData, 4, 3);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults2.json", "Data")]
        public void TestSample3()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS
             * @"Data/Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @"Data/Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @"Data/Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @"Data/Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", "3.12", "2.98", "2.55", "1.88", "1.70" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", "3.13", "2.98", "2.55", "1.88", "1.70" });
            TestUtility.AssertTableContent(table, expectedData, 6, 3);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Snapshot_QIresults1.json", "Data")]
        public void TestSample3OneSnapshot()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS
             * @"Data/Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @"Data/Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @"Data/Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, null, null, null, null);
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", "3.12", "2.98", "2.55", "1.88", "1.70" });
            TestUtility.AssertTableContent(table, expectedData, 6, 2);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Modules1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults2.json", "Data")]
        public void TestSample4()
        {
            /*
             * Configuration = TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=SNAPSHOTS,ROW11=MODULES,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @"Data/Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @"Data/Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @"Data/Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"ROW11", "MODULES"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    SHOPIZER/AppliAEPtran/Shopizer_sql content", "3.36", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "    sm-central/AppliAEPtran/Shopizer_src content", "3.00", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "    sm-core/AppliAEPtran/Shopizer_src content", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "    sm-shop/AppliAEPtran/Shopizer_src content", "2.96", "3.04", "2.51", "1.84", "1.76" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    SHOPIZER/AppliAEPtran/Shopizer_sql content", "3.37", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "    sm-central/AppliAEPtran/Shopizer_src content", "3.01", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "    sm-core/AppliAEPtran/Shopizer_src content", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "    sm-shop/AppliAEPtran/Shopizer_src content", "2.96", "3.03", "2.51", "1.84", "1.76" });
            TestUtility.AssertTableContent(table, expectedData, 6, 11);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Modules1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults2.json", "Data")]
        public void TestSample5()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=MODULES,ROW11=SNAPSHOTS,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @"Data/Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @"Data/Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @"Data/Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "MODULES"},
                {"ROW11", "SNAPSHOTS"},
                {"METRICS", "HEALTH_FACTOR"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "SHOPIZER/AppliAEPtran/Shopizer_sql content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "3.36", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "3.37", "3.54", "3.70", "3.26", "3.67" });
            expectedData.AddRange(new List<string> { "sm-central/AppliAEPtran/Shopizer_src content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "3.00", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "3.01", "3.11", "2.55", "1.84", "1.75" });
            expectedData.AddRange(new List<string> { "sm-core/AppliAEPtran/Shopizer_src content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "3.16", "2.97", "2.67", "1.88", "2.33" });
            expectedData.AddRange(new List<string> { "sm-shop/AppliAEPtran/Shopizer_src content", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "2.96", "3.04", "2.51", "1.84", "1.76" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "2.96", "3.03", "2.51", "1.84", "1.76" });
            TestUtility.AssertTableContent(table, expectedData, 6, 13);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Modules1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults2.json", "Data")]
        public void TestSample6()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,COL11=MODULES,ROW1=SNAPSHOTS,METRICS=60017|60014,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @"Data/Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @"Data/Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @"Data/Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"COL11", "MODULES"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots",
                "Total Quality Index - SHOPIZER/AppliAEPtran/Shopizer_sql content",
                "Total Quality Index - sm-central/AppliAEPtran/Shopizer_src content",
                "Total Quality Index - sm-core/AppliAEPtran/Shopizer_src content",
                "Total Quality Index - sm-shop/AppliAEPtran/Shopizer_src content",
                "Efficiency - SHOPIZER/AppliAEPtran/Shopizer_sql content",
                "Efficiency - sm-central/AppliAEPtran/Shopizer_src content",
                "Efficiency - sm-core/AppliAEPtran/Shopizer_src content",
                "Efficiency - sm-shop/AppliAEPtran/Shopizer_src content" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", "3.47", "2.53", "2.68", "2.49", "3.26", "1.84", "1.88", "1.84" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", "3.47", "2.53", "2.68", "2.49", "3.26", "1.84", "1.88", "1.84" });
            TestUtility.AssertTableContent(table, expectedData, 9, 3);

        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample7.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap1Sample7.json", "Data")]
        public void TestSample7()
        {
            /*
             * Configuration :TABLE;GENERIC_TABLE;COL1=MODULES,COL11=METRICS,ROW1=SNAPSHOTS,METRICS=60017|60014,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * ModulesDreamTeam.json : AED3/applications/7/modules
             * DreamTeamSnap4Sample7.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             * DreamTeamSnap1Sample7.json : AED3/applications/7/snapshots/3/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
               @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap4Sample7.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
               @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap1Sample7.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "MODULES"},
                {"COL11", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots",
                "Adg - Total Quality Index", "Adg - Efficiency",
                "Central - Total Quality Index","Central - Efficiency",
                "DssAdmin - Total Quality Index", "DssAdmin - Efficiency",
                "Pchit - Total Quality Index", "Pchit - Efficiency" });
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_4 - 4", "2.35", "2.64", "2.40", "1.71", "3.08", "3.31", "2.62", "2.14" });
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_1 - 1", "2.43", "2.64", "2.40", "1.71", "3.08", "3.32", "2.63", "2.09" });
            TestUtility.AssertTableContent(table, expectedData, 9, 3);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample7.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap1Sample7.json", "Data")]
        public void TestSample8()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=MODULES,COL11=METRICS,ROW1=SNAPSHOTS,ROW11=TECHNOLOGIES,
             * METRICS=60017|60014,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL,TECHNOLOGIES=ALL
             * ModulesDreamTeam.json : AED3/applications/7/modules
             * DreamTeamSnap4Sample7.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             * DreamTeamSnap1Sample7.json : AED3/applications/7/snapshots/3/results?quality-indicators=(60017,60014)&modules=$all&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
               @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap4Sample7.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
               @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap1Sample7.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "MODULES"},
                {"COL11", "METRICS"},
                {"ROW1", "SNAPSHOTS"},
                {"ROW11", "TECHNOLOGIES"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"},
                {"TECHNOLOGIES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots",
                "Adg - Total Quality Index","Adg - Efficiency",
                "Central - Total Quality Index","Central - Efficiency",
                "DssAdmin - Total Quality Index","DssAdmin - Efficiency",
                "Pchit - Total Quality Index", "Pchit - Efficiency"  });
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_4 - 4", " ", " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "2.35", "2.64", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "n/a", "n/a", "2.40", "1.71", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    C++", "n/a", "n/a", "n/a", "n/a", "3.08", "3.31", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    .NET", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "2.62", "2.14" });
            expectedData.AddRange(new List<string> { "ADGAutoSnap_Dream Team_1 - 1", " ", " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "2.43", "2.64", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "n/a", "n/a", "2.40", "1.71", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    C++", "n/a", "n/a", "n/a", "n/a", "3.08", "3.32", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    .NET", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "2.63", "2.09" });
            TestUtility.AssertTableContent(table, expectedData, 9, 11);
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample9.json", "Data")]
        public void TestSample9()
        {
            /*
             * Configuration :TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=CRITICAL_VIOLATIONS,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Sample9.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
              null, @"Data/DreamTeamSnap4Sample9.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
              null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "CRITICAL_VIOLATIONS"},
                {"SNAPSHOTS", "CURRENT"}
            };
            TestUtility.SetCulture("fr-FR");
            var tableFr = component.Content(reportData, config);
            var expectedDataFr = new List<string>();
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques", "23", "160", "187", "206", "200" });
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques ajoutées", "0", "6", "11", "20", "22" });
            expectedDataFr.AddRange(new List<string> { "Non-conformités critiques supprimées", "0", "0", "0", "1", "0" });
            TestUtility.AssertTableContent(tableFr, expectedDataFr, 6, 4);

            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Critical Violations", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Total Critical Violations", "23", "160", "187", "206", "200" });
            expectedData.AddRange(new List<string> { "Added Critical Violations", "0", "6", "11", "20", "22" });
            expectedData.AddRange(new List<string> { "Removed Critical Violations", "0", "0", "0", "1", "0" });
            TestUtility.AssertTableContent(table, expectedData, 6, 4);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample10.json", "Data")]
        public void TestSample10()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=CRITICAL_VIOLATIONS,ROW11=MODULES,
             * METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS=ALL,MODULES=ALL,SNAPSHOTS=CURRENT
             * ModulesDreamTeam.json : AED3/applications/7/modules
             * DreamTeamSnap4Sample10.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)&modules=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
             @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap4Sample10.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
             null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "VIOLATIONS"},
                {"ROW11", "MODULES"},
                {"METRICS", "HEALTH_FACTOR" },
                {"VIOLATIONS", "ALL" },
                {"MODULES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Violations", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "Total Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Adg", "20,765", "8,907", "4,018", "754", "1,394" });
            expectedData.AddRange(new List<string> { "    Central", "930", "854", "569", "559", "129" });
            expectedData.AddRange(new List<string> { "    DssAdmin", "3,377", "3,856", "1,251", "80", "851" });
            expectedData.AddRange(new List<string> { "    Pchit", "256", "247", "132", "58", "78" });
            expectedData.AddRange(new List<string> { "Added Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Adg", "915", "534", "306", "94", "144" });
            expectedData.AddRange(new List<string> { "    Central", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    DssAdmin", "14", "12", "6", "0", "2" });
            expectedData.AddRange(new List<string> { "    Pchit", "111", "104", "66", "30", "35" });
            expectedData.AddRange(new List<string> { "Removed Violations", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Adg", "375", "287", "203", "101", "104" });
            expectedData.AddRange(new List<string> { "    Central", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    DssAdmin", "46", "35", "31", "0", "19" });
            expectedData.AddRange(new List<string> { "    Pchit", "31", "15", "15", "6", "9" });
            TestUtility.AssertTableContent(table, expectedData, 6, 16);
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample11.json", "Data")]
        public void TestSample11()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=TECHNOLOGIES,ROW11=CRITICAL_VIOLATIONS,
             * METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS =ADDED|REMOVED,TECHNOLOGIES=ALL,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Sample11.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
            null, @"Data/DreamTeamSnap4Sample11.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
            null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "TECHNOLOGIES"},
                {"ROW11", "CRITICAL_VIOLATIONS"},
                {"METRICS", "HEALTH_FACTOR" },
                {"CRITICAL_VIOLATIONS", "ADDED|REMOVED" },
                {"TECHNOLOGIES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
            TestUtility.SetCulture("en-US");
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "JEE", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "3", "11", "17", "22" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "PL/SQL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "C++", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "0", "0" });
            expectedData.AddRange(new List<string> { ".NET", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Added Critical Violations", "0", "3", "0", "3", "0" });
            expectedData.AddRange(new List<string> { "    Removed Critical Violations", "0", "0", "0", "1", "0" });
            TestUtility.AssertTableContent(table, expectedData, 6, 13);
        }


        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample12.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap1Sample12.json", "Data")]
        public void TestSample12()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=10151|10107|10152|10154|10161,SNAPSHOTS=ALL
             * DreamTeamSnap4Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             * DreamTeamSnap1Sample12.json : AED3/applications/7/snapshots/15/results?sizing-measures=(10151,10107,10152,10154,10161)
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Sample12.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, @"Data/DreamTeamSnap1Sample12.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "10151|10107|10152|10154|10161" },
                {"SNAPSHOTS", "ALL"}
            };
            TestUtility.SetCulture("invariant");
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "ADGAutoSnap_Dream Team_4 - 4", "ADGAutoSnap_Dream Team_1 - 1", "Evolution", "% Evolution" });
            expectedData.AddRange(new List<string> { "Number of Code Lines", "104,851", "92,762", "12,089", "+13.0 %" });
            expectedData.AddRange(new List<string> { "Number of Comment Lines", "10,626", "10,009", "617", "+6.16 %" });
            expectedData.AddRange(new List<string> { "Number of Artifacts", "6,727", "6,089", "638", "+10.5 %" });
            expectedData.AddRange(new List<string> { "Number of Files", "579", "485", "94", "+19.4 %" });
            expectedData.AddRange(new List<string> { "Number of Methods", "6,477", "5,883", "594", "+10.1 %" });
            TestUtility.AssertTableContent(table, expectedData, 5, 6);
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestTechnicalDebtType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=TECHNICAL_DEBT,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "TECHNICAL_DEBT" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsTrue(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestTechnicalSizingType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=TECHNICAL_SIZING,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "TECHNICAL_SIZING" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control"));
            Assert.IsTrue(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestFunctionalWeightType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=FUNCTIONAL_WEIGHT,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "FUNCTIONAL_WEIGHT" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsTrue(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestViolationType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=VIOLATION,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "VIOLATION" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsTrue(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestCriticalViolationType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=CRITICAL_VIOLATION,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "CRITICAL_VIOLATION" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsTrue(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestBusinessCriteriaType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=BUSINESS_CRITERIA,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "BUSINESS_CRITERIA" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control (550)"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsTrue(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestTechnicalCriteriaType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=TECHNICAL_CRITERIA,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "TECHNICAL_CRITERIA" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(2, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsFalse(data.Contains("Class naming convention - case control (550)"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsTrue(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        public void TestQualityRuleType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=QUALITY_RULES,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "QUALITY_RULES" },
                {"SNAPSHOTS", "CURRENT"}
            };

            var table = component.Content(reportData, config);
            Assert.AreEqual(2, table.NbColumns);
            Assert.AreEqual(3, table.NbRows);

            var data = table.Data.ToList();
            Assert.IsTrue(data.Contains("ADGAutoSnap_Dream Team_4 - 4"));
            Assert.IsTrue(data.Contains("Class naming convention - case control (550)"));
            Assert.IsTrue(data.Contains("My Critical quality rule (556)"));
            Assert.IsFalse(data.Contains("Number of Code Lines"));
            Assert.IsFalse(data.Contains("OMG-Compliant Automated Function Points"));
            Assert.IsFalse(data.Contains("Efficiency"));
            Assert.IsFalse(data.Contains("Architecture - OS and Platform Independence"));
            Assert.IsFalse(data.Contains("Number of violations to critical quality rules"));
            Assert.IsFalse(data.Contains("Number of quality rules with violations"));
            Assert.IsFalse(data.Contains("Technical Debt"));
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics.json", "Data")]
        [DeploymentItem(@"Data/BusinessValue.json", "Data")]
        public void TestBackgroundFactType()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=SNAPSHOTS,ROW1=METRICS,METRICS=66061,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Metrics.json : AED3/applications/7/snapshots/15/results?quality-indicators=(60014,61004,550)&sizing-measures=(10151,68001,10202,67210,67011)
             */

            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                null, @"Data/DreamTeamSnap4Metrics.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "SNAPSHOTS"},
                {"ROW1", "METRICS"},
                {"METRICS", "66061" },
                {"SNAPSHOTS", "CURRENT"}
            };

            // Needed for background facts, as there are retrieved one by one by url request
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);

            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "ADGAutoSnap_Dream Team_4 - 4" });
            expectedData.AddRange(new List<string> { "Business Value", "3" });
            TestUtility.AssertTableContent(table, expectedData, 2, 2);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Modules1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults1.json", "Data")]
        [DeploymentItem(@"Data/Snapshot_QIresults2.json", "Data")]
        public void TestStandardTag()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=MODULES,ROW11=SNAPSHOTS,METRICS=HEALTH_FACTOR,SNAPSHOTS=CURRENT|PREVIOUS,MODULES=ALL
             * @"Data/Modules1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/modules
             * @"Data/Snapshot_QIresults1.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/4/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             * @"Data/Snapshot_QIresults2.json" is the result of http://localhost:7070/CAST-AAD-AED/rest/AED2/applications/3/snapshots/3/results?quality-indicators=(60011,60012,60013,60014,60016,60017,61001,61003,61007,1576,1596,4656,7254)&modules=$all&technologies=$all&categories=$all
             */

            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults1.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                @"Data/Modules1.json", @"Data/Snapshot_QIresults2.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "MODULES"},
                {"ROW11", "SNAPSHOTS"},
                {"METRICS", "OWASP"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"},
                {"MODULES", "ALL"}
            };
            WSConnection connection = new WSConnection
            {
                Url = "http://tests/CAST-RESTAPI/rest/",
                Login = "admin",
                Password = "cast",
                IsActive = true,
                Name = "Default"
            };
            reportData.SnapshotExplorer = new SnapshotBLLStub(connection, reportData.CurrentSnapshot);
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "Avoid using \"nullable\" Columns except in the last position in a Table (1596)", "Avoid declaring throwing an exception and not throwing it (4656)" });
            expectedData.AddRange(new List<string> { "SHOPIZER/AppliAEPtran/Shopizer_sql content", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "1.42", "n/a" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "1.42", "n/a" });
            expectedData.AddRange(new List<string> { "sm-central/AppliAEPtran/Shopizer_src content", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "n/a", "2.95" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "n/a", "2.93" });
            expectedData.AddRange(new List<string> { "sm-core/AppliAEPtran/Shopizer_src content", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "n/a", "2.98" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "n/a", "2.98" });
            expectedData.AddRange(new List<string> { "sm-shop/AppliAEPtran/Shopizer_src content", " ", " " });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.4 - v1.1.4", "n/a", "3.87" });
            expectedData.AddRange(new List<string> { "    Snap_v1.1.3 - v1.1.3", "n/a", "3.87" });
            TestUtility.AssertTableContent(table, expectedData, 3, 13);
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics2.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics2Previous.json", "Data")]
        public void TestCustomExpressionsForApplication()
        {
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @"Data/DreamTeamSnap4Metrics2.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @"Data/DreamTeamSnap4Metrics2Previous.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "SNAPSHOTS"},
                {"CUSTOM_EXPRESSIONS", "a/b|(c+d)/2"},
                {"PARAMS", "Sz a SZ b QR c QR d"},
                {"a", "10151"},
                {"b", "10202"},
                {"c", "550"},
                {"d", "556"},
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "a/b", "(c+d)/2" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", "92.30", "3.89" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", "103.81", "2.82" });
            TestUtility.AssertTableContent(table, expectedData, 3, 3);
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics2.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Metrics2Previous.json", "Data")]
        [DeploymentItem(@"Data/ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@"Data/ComplexitySnapPrevious.json", "Data")]
        public void TestCustomExpressionsForApplicationWithCategories()
        {
            ReportData reportData = TestUtility.PrepaReportData("AppliAEP",
                null, @"Data/DreamTeamSnap4Metrics2.json", "AED3/applications/3/snapshots/4", "Snap_v1.1.4", "v1.1.4",
                null, @"Data/DreamTeamSnap4Metrics2Previous.json", "AED3/applications/3/snapshots/3", "Snap_v1.1.3", "v1.1.3");
            reportData = TestUtility.AddApplicationComplexity(reportData, @"Data/ComplexitySnapCurrent.json", @"Data/ComplexitySnapPrevious.json");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "SNAPSHOTS"},
                {"CUSTOM_EXPRESSIONS", "a+b"},
                {"PARAMS", "Sz a SZ b"},
                {"a", "65104"},
                {"b", "65103"},
                {"FORMAT", "N0" },
                {"SNAPSHOTS", "CURRENT|PREVIOUS"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Snapshots", "a+b" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.4 - v1.1.4", "357" });
            expectedData.AddRange(new List<string> { "Snap_v1.1.3 - v1.1.3", "346" });
            TestUtility.AssertTableContent(table, expectedData, 2, 3);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap15MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap3MetricsCustom.json", "Data")]
        public void TestCustomExpressionsForModules()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap15MetricsCustom.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap3MetricsCustom.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "MODULES"},
                {"CUSTOM_EXPRESSIONS", "a/b|(c+d)/2"},
                {"PARAMS", "Sz a SZ b QR c QR d"},
                {"a", "67211"},
                {"b", "10151"},
                {"c", "60013"},
                {"d", "60014"},
                {"MODULES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "a/b", "(c+d)/2" });
            expectedData.AddRange(new List<string> { "Adg", "0.44", "2.27" });
            expectedData.AddRange(new List<string> { "Central", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "DssAdmin", "0.22", "3.31" });
            expectedData.AddRange(new List<string> { "Pchit", "0.15", "2.80" });
            TestUtility.AssertTableContent(table, expectedData, 3, 5);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap15MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap3MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/SizeDistributionSnapCurrent.json", "Data")]
        public void TestCustomExpressionsForModulesWithCategories()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap15MetricsCustom.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap3MetricsCustom.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @"Data/SizeDistributionSnapCurrent.json", null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "MODULES"},
                {"CUSTOM_EXPRESSIONS", "a+b"},
                {"PARAMS", "Sz a SZ b"},
                {"a", "65505"},
                {"b", "65504"},
                {"FORMAT", "N0"},
                {"MODULES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "a+b" });
            expectedData.AddRange(new List<string> { "Adg", "64" });
            expectedData.AddRange(new List<string> { "Central", "0" });
            expectedData.AddRange(new List<string> { "DssAdmin", "61" });
            expectedData.AddRange(new List<string> { "Pchit", "8" });
            TestUtility.AssertTableContent(table, expectedData, 2, 5);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap15MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap3MetricsCustom.json", "Data")]
        public void TestCustomExpressionsForTechnologies()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap15MetricsCustom.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap3MetricsCustom.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "TECHNOLOGIES"},
                {"CUSTOM_EXPRESSIONS", "a/b|(c+d)/2"},
                {"PARAMS", "Sz a SZ b QR c QR d"},
                {"a", "67211"},
                {"b", "10151"},
                {"c", "60013"},
                {"d", "60014"},
                {"TECHNOLOGIES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "a/b", "(c+d)/2" });
            expectedData.AddRange(new List<string> { "JEE", "0.44", "2.27" });
            expectedData.AddRange(new List<string> { "PL/SQL", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "C++", "0.22", "3.31" });
            expectedData.AddRange(new List<string> { ".NET", "0.15", "2.80" });
            TestUtility.AssertTableContent(table, expectedData, 3, 5);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap15MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap3MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/SizeDistributionSnapCurrent.json", "Data")]
        public void TestCustomExpressionsForTechnologiesWithCategory()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap15MetricsCustom.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap3MetricsCustom.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            reportData = TestUtility.AddApplicationComplexity(reportData, @"Data/SizeDistributionSnapCurrent.json", null);
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "TECHNOLOGIES"},
                {"CUSTOM_EXPRESSIONS", "a+b"},
                {"PARAMS", "Sz a SZ b"},
                {"a", "65505"},
                {"b", "65504"},
                {"FORMAT", "N0"},
                {"TECHNOLOGIES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "a+b"});
            expectedData.AddRange(new List<string> { "JEE", "64" });
            expectedData.AddRange(new List<string> { "PL/SQL", "0" });
            expectedData.AddRange(new List<string> { "C++", "61" });
            expectedData.AddRange(new List<string> { ".NET", "8" });
            TestUtility.AssertTableContent(table, expectedData, 2, 5);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap15MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap3MetricsCustom.json", "Data")]
        public void TestCustomExpressionsForModulesAndTechnologies()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap15MetricsCustom.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap3MetricsCustom.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "CUSTOM_EXPRESSIONS"},
                {"ROW1", "MODULES"},
                {"ROW11", "TECHNOLOGIES"},
                {"CUSTOM_EXPRESSIONS", "a/b|(c+d)/2"},
                {"PARAMS", "Sz a SZ b QR c QR d"},
                {"a", "67211"},
                {"b", "10151"},
                {"c", "60013"},
                {"d", "60014"},
                {"TECHNOLOGIES", "ALL"},
                {"MODULES", "ALL"},
                {"SNAPSHOTS", "PREVIOUS"}
            };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "a/b", "(c+d)/2" });
            expectedData.AddRange(new List<string> { "Adg", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "0.45", "2.27" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    .NET", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "Central", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    .NET", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "DssAdmin", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "0.22", "3.32" });
            expectedData.AddRange(new List<string> { "    .NET", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "Pchit", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "No data found", "No data found" });
            expectedData.AddRange(new List<string> { "    .NET", "0.18", "2.75" });
            TestUtility.AssertTableContent(table, expectedData, 3, 21);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap15MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap3MetricsCustom.json", "Data")]
        [DeploymentItem(@"Data/SizeDistributionSnapCurrent.json", "Data")]
        public void TestCustomExpressionsForModulesAndTechnologiesWithCategory()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap15MetricsCustom.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
                @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap3MetricsCustom.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            reportData = TestUtility.AddApplicationComplexity(reportData, null, @"Data/SizeDistributionSnapCurrent.json");
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
                {
                    {"COL1", "CUSTOM_EXPRESSIONS"},
                    {"ROW1", "MODULES"},
                    {"ROW11", "TECHNOLOGIES"},
                    {"CUSTOM_EXPRESSIONS", "a+b"},
                    {"PARAMS", "Sz a SZ b"},
                    {"a", "65505"},
                    {"b", "65504"},
                    {"FORMAT", "N0"},
                    {"TECHNOLOGIES", "ALL"},
                    {"MODULES", "ALL"},
                    {"SNAPSHOTS", "PREVIOUS"}
                };
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
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Modules", "a+b" });
            expectedData.AddRange(new List<string> { "Adg", " "});
            expectedData.AddRange(new List<string> { "    JEE", "64" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "No data found" });
            expectedData.AddRange(new List<string> { "    .NET", "No data found" });
            expectedData.AddRange(new List<string> { "Central", " " });
            expectedData.AddRange(new List<string> { "    JEE", "No data found" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "0" });
            expectedData.AddRange(new List<string> { "    C++", "No data found" });
            expectedData.AddRange(new List<string> { "    .NET", "No data found" });
            expectedData.AddRange(new List<string> { "DssAdmin", " " });
            expectedData.AddRange(new List<string> { "    JEE", "No data found" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "61" });
            expectedData.AddRange(new List<string> { "    .NET", "No data found" });
            expectedData.AddRange(new List<string> { "Pchit", " " });
            expectedData.AddRange(new List<string> { "    JEE", "No data found" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "No data found" });
            expectedData.AddRange(new List<string> { "    C++", "No data found" });
            expectedData.AddRange(new List<string> { "    .NET", "8" });
            TestUtility.AssertTableContent(table, expectedData, 2, 21);
        }

        [TestMethod]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample11.json", "Data")]
        public void TestTechDebtTechnoAddedRemoved()
        {
            /*
             * Configuration : TABLE;GENERIC_TABLE;COL1=METRICS,ROW1=TECHNOLOGIES,ROW11=CRITICAL_VIOLATIONS,
             * METRICS=HEALTH_FACTOR,CRITICAL_VIOLATIONS =ADDED|REMOVED,TECHNOLOGIES=ALL,SNAPSHOTS=CURRENT
             * DreamTeamSnap4Sample11.json : AED3/applications/7/snapshots/15/results?quality-indicators=(business-criteria)&select=(evolutionSummary)&technologies=$all
             */
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
            null, @"Data/DreamTeamSnap4Sample11.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
            null, null, null, null, null);
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "TECHNOLOGIES"},
                {"ROW11", "OMG_TECHNICAL_DEBT"},
                {"METRICS", "HEALTH_FACTOR" },
                {"OMG_TECHNICAL_DEBT", "ADDED|REMOVED" },
                {"TECHNOLOGIES", "ALL"},
                {"SNAPSHOTS", "CURRENT"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technologies", "Transferability", "Changeability", "Robustness", "Efficiency", "Security" });
            expectedData.AddRange(new List<string> { "JEE", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Technical Debt Added (Days)", "2.4", "n/a", "n/a", "n/a", "5.3" });
            expectedData.AddRange(new List<string> { "    Technical Debt Removed (Days)", "2.8", "n/a", "n/a", "n/a", "7.0" });
            expectedData.AddRange(new List<string> { "PL/SQL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Technical Debt Added (Days)", "n/a", "n/a", "n/a", "n/a", "10.2" });
            expectedData.AddRange(new List<string> { "    Technical Debt Removed (Days)", "n/a", "n/a", "n/a", "n/a", "25.7" });
            expectedData.AddRange(new List<string> { "C++", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Technical Debt Added (Days)", "n/a", "n/a", "n/a", "n/a", "3.0" });
            expectedData.AddRange(new List<string> { "    Technical Debt Removed (Days)", "n/a", "n/a", "n/a", "n/a", "31.0" });
            expectedData.AddRange(new List<string> { ".NET", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    Technical Debt Added (Days)", "5.3", "n/a", "n/a", "n/a", "2.4" });
            expectedData.AddRange(new List<string> { "    Technical Debt Removed (Days)", "7.0", "n/a", "n/a", "n/a", "2.8" });
            TestUtility.AssertTableContent(table, expectedData, 6, 13);
        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample10.json", "Data")]
        public void TestTechDebtModules()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
             @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap4Sample10.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
             null, null, null, null, null);

            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "MODULES"},
                {"ROW1", "OMG_TECHNICAL_DEBT"},
                {"METRICS", "60017" },
                {"OMG_TECHNICAL_DEBT", "ALL" },
                {"MODULES", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical Debt (Days)", "Adg", "Central", "DssAdmin", "Pchit" });
            expectedData.AddRange(new List<string> { "Technical Debt (Days)", "257.2", "55.3", "213.2", "205.0" });
            expectedData.AddRange(new List<string> { "Technical Debt Added (Days)", "10.2", "5.3", "3.0", "2.4" });
            expectedData.AddRange(new List<string> { "Technical Debt Removed (Days)", "25.7", "7.0", "31.0", "2.8" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);

        }

        [TestMethod]
        [DeploymentItem(@"Data/ModulesDreamTeam.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap4Sample7.json", "Data")]
        [DeploymentItem(@"Data/DreamTeamSnap1Sample7.json", "Data")]
        public void TestOmgTechDebtModulesTechnos()
        {
            ReportData reportData = TestUtility.PrepaReportData("Dream Team",
               @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap4Sample7.json", "AED3/applications/7/snapshots/15", "ADGAutoSnap_Dream Team_4", "4",
               @"Data/ModulesDreamTeam.json", @"Data/DreamTeamSnap1Sample7.json", "AED3/applications/7/snapshots/3", "ADGAutoSnap_Dream Team_1", "1");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", "PL/SQL", "C++", ".NET" };
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "MODULES"},
                {"COL11", "METRICS"},
                {"ROW1", "OMG_TECHNICAL_DEBT"},
                {"ROW11", "TECHNOLOGIES"},
                {"METRICS", "60017|60014"},
                {"SNAPSHOTS", "PREVIOUS"},
                {"MODULES", "ALL"},
                {"TECHNOLOGIES", "ALL"},
                {"OMG_TECHNICAL_DEBT", "REMOVED" }
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical Debt (Days)",
                "Adg - Total Quality Index","Adg - Efficiency",
                "Central - Total Quality Index","Central - Efficiency",
                "DssAdmin - Total Quality Index","DssAdmin - Efficiency",
                "Pchit - Total Quality Index", "Pchit - Efficiency"  });
            expectedData.AddRange(new List<string> { "Technical Debt Removed (Days)", " ", " ", " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "    JEE", "2.8", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    PL/SQL", "n/a", "n/a", "7.0", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    C++", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "    .NET", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 9, 6);
        }

        [TestMethod]
        [DeploymentItem(@"Data/Sample1Current.json", "Data")]
        public void TestTechDebt()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @"Data/Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            var component = new GenericTable();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"COL1", "METRICS"},
                {"ROW1", "OMG_TECHNICAL_DEBT"},
                {"METRICS", "60017"},
                {"OMG_TECHNICAL_DEBT", "ALL"}
            };
            var table = component.Content(reportData, config);
            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Technical Debt (Days)", "Total Quality Index" });
            expectedData.AddRange(new List<string> { "Technical Debt (Days)", "55.3" });
            expectedData.AddRange(new List<string> { "Technical Debt Added (Days)", "5.3" });
            expectedData.AddRange(new List<string> { "Technical Debt Removed (Days)", "7.0" });
            TestUtility.AssertTableContent(table, expectedData, 2, 4);
        }

    }
}

