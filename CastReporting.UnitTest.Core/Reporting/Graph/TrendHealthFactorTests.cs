﻿using CastReporting.Domain;
using CastReporting.Reporting.Block.Graph;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Graph
{
    [TestClass]
    public class TrendHealthFactorTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        public void TestOneSnapshot()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
               null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
               null, null, null, null, null, null);
            var component = new TrendHealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Trans.", "Chang.", "Robu.", "Efcy", "Secu.", "LoC" });
            expectedData.AddRange(new List<string> { "42756", "2.92", "1.93", "3.19", "2.59", "3.17", "21261" });
            expectedData.AddRange(new List<string> { "42756", "2.92", "1.93", "3.19", "2.59", "3.17", "21261" });
            TestUtility.AssertTableContent(table, expectedData, 7, 3);

            Assert.IsNull(table.GraphOptions);
        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCresults.json", "Data")]
        public void TestTwoSnapshots()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new TrendHealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Trans.", "Chang.", "Robu.", "Efcy", "Secu.", "LoC" });
            expectedData.AddRange(new List<string> { "42755", "2.82", "1.93", "3.15", "2.6", "3.13", "22347" });
            expectedData.AddRange(new List<string> { "42756", "2.92", "1.93", "3.19", "2.59", "3.17", "21261" });
            TestUtility.AssertTableContent(table, expectedData, 7, 3);

            Assert.IsNull(table.GraphOptions);

        }

        [TestMethod]
        [DeploymentItem(@"Data/CurrentBCresults.json", "Data")]
        [DeploymentItem(@"Data/PreviousBCresults.json", "Data")]
        public void TestZoom()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            CastDate previousDate = new CastDate { Time = 1484866800000 };
            ReportData reportData = TestUtility.PrepareApplicationReportData("ReportGenerator",
                null, @"Data/CurrentBCresults.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2", currentDate,
                null, @"Data/PreviousBCresults.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1", previousDate);

            var component = new TrendHealthFactor();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"ZOOM", "3"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { " ", "Trans.", "Chang.", "Robu.", "Efcy", "Secu.", "LoC" });
            expectedData.AddRange(new List<string> { "42755", "2.82", "1.93", "3.15", "2.6", "3.13", "22347" });
            expectedData.AddRange(new List<string> { "42756", "2.92", "1.93", "3.19", "2.59", "3.17", "21261" });
            TestUtility.AssertTableContent(table, expectedData, 7, 3);

            Assert.AreEqual(-1.1, table.GraphOptions.AxisConfiguration.VerticalAxisMinimal);
            Assert.AreEqual(6.2, table.GraphOptions.AxisConfiguration.VerticalAxisMaximal);

        }
    }
}
