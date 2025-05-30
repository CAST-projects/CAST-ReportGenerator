﻿using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    // ReSharper disable once InconsistentNaming
    public class PF_IgnoredSnapshotsTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@"Data/AADApplications.json", "Data")]
        public void TestIgnoredSnapshots()
        {
            ReportData reportData = TestUtility.PrepaEmptyPortfolioReportData();

            var component = new PF_IgnoredSnapshots();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Ignored Snapshots" });
            expectedData.AddRange(new List<string> { "Snap Ignor 1" });
            expectedData.AddRange(new List<string> { "Snap 2 Ignore" });
            TestUtility.AssertTableContent(table, expectedData, 1, 3);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@"Data/AADApplications.json", "Data")]
        [DeploymentItem(@"Data/AADApplication1Snap.json", "Data")]
        [DeploymentItem(@"Data/AADApplication1SnapResults.json", "Data")]
        [DeploymentItem(@"Data/AADApplication2Snap.json", "Data")]
        [DeploymentItem(@"Data/AADApplication2SnapResults.json", "Data")]
        public void TestNoIgnoredSnapshots()
        {
            List<string> snapList = new List<string> { @"Data/AADApplication1Snap.json", @"Data/AADApplication2Snap.json" };
            List<string> snapResultsList = new List<string> { @"Data/AADApplication1SnapResults.json", @"Data/AADApplication2SnapResults.json" };
            ReportData reportData = TestUtility.PrepaPortfolioReportData(@"Data/AADApplications.json", snapList, snapResultsList);

            var component = new PF_IgnoredSnapshots();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Ignored Snapshots" });
            expectedData.AddRange(new List<string> { "No Ignored Snapshots" });
            TestUtility.AssertTableContent(table, expectedData, 1, 2);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
