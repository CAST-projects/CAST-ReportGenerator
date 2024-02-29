using CastReporting.Domain;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class PortfolioMetricsReleasePerformanceTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestOneAppInPortfolio()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60014|60012|60017" },
                {"TARGETS","2.90|2.90|2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Total Quality Index", "2.62", "2.90", "2.63", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }


        [TestMethod]
        [DeploymentItem(@".\Data\AADMultiCocApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37Snapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp3SnapshotsResults.json", "Data")]
        [DeploymentItem(@".\Data\AADMultiCocApp37SnapshotsResults.json", "Data")]
        public void TestContent()
        {
            List<string> snapList = new List<string> { @".\Data\AADMultiCocApp3Snapshots.json", @".\Data\AADMultiCocApp37Snapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADMultiCocApp3SnapshotsResults.json", @".\Data\AADMultiCocApp37SnapshotsResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADMultiCocApplications.json", snapList, snapResultsList);
            TestUtility.PreparePortfSnapshots(reportData);

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60012|61001|4554" },
                {"TARGETS","2.90|3.10|3.50"},
                {"SLA","5|20"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Changeability", "2.47", "2.90", "2.96", "Good" });
            expectedData.AddRange(new List<string> { "Architecture - Multi-Layers and Data Access", "2.46", "3.10", "2.46", "Poor" });
            expectedData.AddRange(new List<string> { "Avoid large Classes - too many Methods (4554)", "1.98", "3.50", "2.30", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestNoPrevious()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot[] _snapshots = new Snapshot[1];
            _snapshots[0] = _snap0;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","60014|60012" },
                {"TARGETS","2.90|2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "-", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "-", "2.90", "2.37", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestNoCurrent()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot[] _snapshots = new Snapshot[1];
            _snapshots[0] = _snap0;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","60014|60012" },
                {"TARGETS","2.90|2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.84", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.37", "2.90", "2.37", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestVeryOldCurrent()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot[] _snapshots = new Snapshot[1];
            _snapshots[0] = _snap0;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","60014|60012" },
                {"TARGETS","2.90|2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.84", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.37", "2.90", "2.37", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestOldPrevious()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-6) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","60014|60012" },
                {"TARGETS","2.90|2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "2.90", "2.37", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void Test2Current()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddHours(-5) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot _snap2 = reportData.Applications[0].Snapshots.ElementAt(2);
            TimeSpan time2 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date2 = new CastDate { Time = time2.TotalMilliseconds };
            if (_snap2 == null) Assert.Fail();
            _snap2.Annotation.Date = _date2;

            Snapshot[] _snapshots = new Snapshot[3];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            _snapshots[2] = _snap2;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID","60014|60012" },
                {"TARGETS","2.90|2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.37", "2.90", "2.37", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestOneTarget()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60014|60012|60017" },
                {"TARGETS","2.90"},
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Efficiency", "1.85", "2.90", "1.84", "Poor" });
            expectedData.AddRange(new List<string> { "Changeability", "2.35", "2.90", "2.37", "Poor" });
            expectedData.AddRange(new List<string> { "Total Quality Index", "2.62", "2.90", "2.63", "Poor" });
            TestUtility.AssertTableContent(table, expectedData, 5, 4);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestNoTarget()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60014|60012|60017" },
                {"SLA","2|5"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "No target selected. Please review configuration", " ", " ", " ", " " });
            TestUtility.AssertTableContent(table, expectedData, 5, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\AADcocraApplications.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshots.json", "Data")]
        [DeploymentItem(@".\Data\AADcocraSnapshotsBCResults.json", "Data")]
        public void TestNoSLA()
        {
            List<string> snapList = new List<string> { @".\Data\AADcocraSnapshots.json" };
            List<string> snapResultsList = new List<string> { @".\Data\AADcocraSnapshotsBCResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@".\Data\AADcocraApplications.json", snapList, snapResultsList);

            DateTime date = new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            Snapshot _snap0 = reportData.Applications[0].Snapshots.FirstOrDefault();
            TimeSpan time0 = DateTime.Now - date;
            CastDate _date0 = new CastDate { Time = time0.TotalMilliseconds };
            if (_snap0 == null) Assert.Fail();
            _snap0.Annotation.Date = _date0;

            Snapshot _snap1 = reportData.Applications[0].Snapshots.ElementAt(1);
            TimeSpan time1 = DateTime.Now.AddMonths(-3) - date;
            CastDate _date1 = new CastDate { Time = time1.TotalMilliseconds };
            if (_snap1 == null) Assert.Fail();
            _snap1.Annotation.Date = _date1;

            Snapshot[] _snapshots = new Snapshot[2];
            _snapshots[0] = _snap0;
            _snapshots[1] = _snap1;
            reportData.Snapshots = _snapshots;
            reportData.Applications[0].Snapshots = _snapshots;

            var component = new PortfolioMetricsReleasePerformance();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ID", "60014|60012|60017" },
                {"TARGETS","2.90"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Metrics", "Previous score", "Target score", "Current score", "SLA Violations" });
            expectedData.AddRange(new List<string> { "Bad SLA configuration", " ", " ", " ", " " });
            TestUtility.AssertTableContent(table, expectedData, 5, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }
    }
}
