using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class CastDistributionTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("invariant");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestCastComplexityNoPar()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cost Complexity distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,881", "n/a", "n/a", "n/a", "86.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1,167", "n/a", "n/a", "n/a", "11.3 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "172", "n/a", "n/a", "n/a", "1.67 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "71", "n/a", "n/a", "n/a", "0.69 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);

        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        public void TestCastComplexityOneSnapshot()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, null, null, null, null);
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", null);

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67001"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cost Complexity distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,881", "n/a", "n/a", "n/a", "86.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1,167", "n/a", "n/a", "n/a", "11.3 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "172", "n/a", "n/a", "n/a", "1.67 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "71", "n/a", "n/a", "n/a", "0.69 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestCastComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67001"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cost Complexity distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,881", "8,824", "+57", "+0.65 %", "86.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1,167", "1,140", "+27", "+2.37 %", "11.3 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "172", "170", "+2", "+1.18 %", "1.67 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "71", "68", "+3", "+4.41 %", "0.69 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestCyclomaticComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65501"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cyclomatic Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,305", "8,235", "+70", "+0.85 %", "88.6 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "838", "824", "+14", "+1.70 %", "8.94 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "161", "159", "+2", "+1.26 %", "1.72 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "71", "68", "+3", "+4.41 %", "0.76 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestFourGLComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65601"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "4GL Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "n/a" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestClassComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66015"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Class Complexity Distribution (WMC)", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "434", "433", "+1", "+0.23 %", "74.1 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "76", "78", "-2", "-2.56 %", "13.0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "59", "54", "+5", "+9.26 %", "10.1 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "17", "17", "0", "0 %", "2.90 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestOOComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65701"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "OO Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "569", "565", "+4", "+0.71 %", "97.1 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "17", "17", "0", "0 %", "2.90 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestSQLComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65801"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "SQL Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "522", "509", "+13", "+2.55 %", "97.6 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1", "1", "0", "0 %", "0.19 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "12", "12", "0", "0 %", "2.24 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestCouplingComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65350"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Coupling Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "9,413", "9,345", "+68", "+0.73 %", "91.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "666", "653", "+13", "+1.99 %", "6.47 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "170", "163", "+7", "+4.29 %", "1.65 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "42", "41", "+1", "+2.44 %", "0.41 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestClassFanOutComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66020"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Class Fan-Out Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "411", "412", "-1", "-0.24 %", "70.1 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "132", "130", "+2", "+1.54 %", "22.5 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "40", "37", "+3", "+8.11 %", "6.83 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "3", "3", "0", "0 %", "0.51 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestClassFanInComplexityTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66021"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Class Fan-In Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "527", "525", "+2", "+0.38 %", "89.9 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "18", "17", "+1", "+5.88 %", "3.07 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "19", "19", "0", "0 %", "3.24 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "22", "21", "+1", "+4.76 %", "3.75 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestSizeDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65105"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Size Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "8,619", "8,558", "+61", "+0.71 %", "83.8 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "1,315", "1,298", "+17", "+1.31 %", "12.8 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "321", "310", "+11", "+3.55 %", "3.12 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "36", "36", "0", "0 %", "0.35 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestReusebyCallDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "66010"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Reuse by Call Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "9,413", "9,345", "+68", "+0.73 %", "91.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "666", "653", "+13", "+1.99 %", "6.47 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "170", "163", "+7", "+4.29 %", "1.65 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "42", "41", "+1", "+2.44 %", "0.41 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestViolationsToCriticalDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67020"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Distribution of violations to critical diagnostic-based metrics per cost complexity", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "227", "226", "+1", "+0.44 %", "32.4 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "308", "299", "+9", "+3.01 %", "43.9 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "107", "106", "+1", "+0.94 %", "15.3 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "59", "55", "+4", "+7.27 %", "8.42 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\Sample1Current.json", "Data")]
        [DeploymentItem(@".\Data\Sample1Previous.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapCurrent.json", "Data")]
        [DeploymentItem(@".\Data\ComplexitySnapPrevious.json", "Data")]
        public void TestDefectsToCriticalDistributionTwoSnapshots()
        {
            ReportData reportData = TestUtility.PrepaReportData("ReportGenerator",
                null, @".\Data\Sample1Current.json", "AED/applications/3/snapshots/6", "PreVersion 1.5.0 sprint 2 shot 2", "V-1.5.0_Sprint 2_2",
                null, @".\Data\Sample1Previous.json", "AED/applications/3/snapshots/3", "PreVersion 1.4.1 before release", "V-1.4.1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\ComplexitySnapCurrent.json", @".\Data\ComplexitySnapPrevious.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "67030"}
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Distribution of defects to critical diagnostic-based metrics per cost complexity", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { "Low Complexity", "221", "221", "0", "0 %", "37.2 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "270", "262", "+8", "+3.05 %", "45.5 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "74", "73", "+1", "+1.37 %", "12.5 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "29", "26", "+3", "+11.5 %", "4.88 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 5);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\WebGoatMultiCurrentResults.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiPreviousResults.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiModules.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiCurrentComplexity.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiPreviousComplexity.json", "Data")]
        public void TestModules()
        {
            ReportData reportData = TestUtility.PrepaReportData("WebGoatMulti",
                @".\Data\WebGoatMultiModules.json", @".\Data\WebGoatMultiCurrentResults.json", "69f58395-7dd6-4509-98d2-6501028e7150/applications/3/snapshots/2", "Snapshot-2020-03-02T15-55-00", "Version-2",
                @".\Data\WebGoatMultiModules.json", @".\Data\WebGoatMultiPreviousResults.json", "69f58395-7dd6-4509-98d2-6501028e7150/applications/3/snapshots/1", "Snapshot-v1", "Version-1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\WebGoatMultiCurrentComplexity.json", @".\Data\WebGoatMultiPreviousComplexity.json");

            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65501"},
                {"MODULES","Y" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cyclomatic Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_dotnet", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "113", "232", "-119", "-51.3 %", "88.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "15", "15", "0", "0 %", "11.7 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_html5", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "790", "808", "-18", "-2.23 %", "94.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "39", "44", "-5", "-11.4 %", "4.67 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "3", "3", "0", "0 %", "0.36 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "4", "3", "+1", "+33.3 %", "0.48 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_Jee", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "615", "601", "+14", "+2.33 %", "89.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "69", "66", "+3", "+4.55 %", "10.0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "3", "3", "0", "0 %", "0.44 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "1", "-1", "-100 %", "0 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_PHP", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "7,418", "7,418", "0", "0 %", "87.6 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "785", "785", "0", "0 %", "9.27 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "153", "153", "0", "0 %", "1.81 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "113", "113", "0", "0 %", "1.33 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_shell", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_Sql", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "9", "9", "0", "0 %", "100 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 37);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\WebGoatMultiCurrentResults.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiPreviousResults.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiModules.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiCurrentComplexity.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiPreviousComplexity.json", "Data")]
        public void TestTechnologies()
        {
            ReportData reportData = TestUtility.PrepaReportData("WebGoatMulti",
                @".\Data\WebGoatMultiModules.json", @".\Data\WebGoatMultiCurrentResults.json", "69f58395-7dd6-4509-98d2-6501028e7150/applications/3/snapshots/2", "Snapshot-2020-03-02T15-55-00", "Version-2",
                @".\Data\WebGoatMultiModules.json", @".\Data\WebGoatMultiPreviousResults.json", "69f58395-7dd6-4509-98d2-6501028e7150/applications/3/snapshots/1", "Snapshot-v1", "Version-1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\WebGoatMultiCurrentComplexity.json", @".\Data\WebGoatMultiPreviousComplexity.json");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", ".NET", "HTML5", "PHP", "SHELL", "SQL" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", ".NET", "HTML5", "PHP", "SHELL", "SQL" };
            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65501"},
                {"TECHNOLOGIES","Y" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cyclomatic Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "JEE", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "615", "601", "+14", "+2.33 %", "89.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "69", "66", "+3", "+4.55 %", "10.0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "3", "3", "0", "0 %", "0.44 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "1", "-1", "-100 %", "0 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { ".NET", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "113", "232", "-119", "-51.3 %", "88.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "15", "15", "0", "0 %", "11.7 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "HTML5", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "790", "808", "-18", "-2.23 %", "94.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "39", "44", "-5", "-11.4 %", "4.67 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "3", "3", "0", "0 %", "0.36 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "4", "3", "+1", "+33.3 %", "0.48 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "PHP", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "7,418", "7,418", "0", "0 %", "87.6 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "785", "785", "0", "0 %", "9.27 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "153", "153", "0", "0 %", "1.81 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "113", "113", "0", "0 %", "1.33 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "SHELL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "SQL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "9", "9", "0", "0 %", "100 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 37);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\WebGoatMultiCurrentResults.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiPreviousResults.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiModules.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiCurrentComplexity.json", "Data")]
        [DeploymentItem(@".\Data\WebGoatMultiPreviousComplexity.json", "Data")]
        public void TestModulesTechnologies()
        {
            ReportData reportData = TestUtility.PrepaReportData("WebGoatMulti",
                @".\Data\WebGoatMultiModules.json", @".\Data\WebGoatMultiCurrentResults.json", "69f58395-7dd6-4509-98d2-6501028e7150/applications/3/snapshots/2", "Snapshot-2020-03-02T15-55-00", "Version-2",
                @".\Data\WebGoatMultiModules.json", @".\Data\WebGoatMultiPreviousResults.json", "69f58395-7dd6-4509-98d2-6501028e7150/applications/3/snapshots/1", "Snapshot-v1", "Version-1");
            reportData = TestUtility.AddApplicationComplexity(reportData, @".\Data\WebGoatMultiCurrentComplexity.json", @".\Data\WebGoatMultiPreviousComplexity.json");
            reportData.CurrentSnapshot.Technologies = new[] { "JEE", ".NET", "HTML5", "PHP", "SHELL", "SQL" };
            reportData.PreviousSnapshot.Technologies = new[] { "JEE", ".NET", "HTML5", "PHP", "SHELL", "SQL" };
            var component = new CastDistribution();
            Dictionary<string, string> config = new Dictionary<string, string>
            {
                {"PAR", "65501"},
                {"MODULES","Y" },
                {"TECHNOLOGIES","Y" }
            };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Cyclomatic Complexity Distribution", "Current", "Previous", "Evol.", "% Evol.", "% Total" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_dotnet", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { ".NET", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "113", "232", "-119", "-51.3 %", "88.3 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "15", "15", "0", "0 %", "11.7 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_html5", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "HTML5", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "790", "808", "-18", "-2.23 %", "94.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "39", "44", "-5", "-11.4 %", "4.67 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "3", "3", "0", "0 %", "0.36 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "4", "3", "+1", "+33.3 %", "0.48 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_Jee", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "JEE", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "615", "601", "+14", "+2.33 %", "89.5 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "69", "66", "+3", "+4.55 %", "10.0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "3", "3", "0", "0 %", "0.44 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "1", "-1", "-100 %", "0 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_PHP", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "PHP", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "7,418", "7,418", "0", "0 %", "87.6 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "785", "785", "0", "0 %", "9.27 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "153", "153", "0", "0 %", "1.81 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "113", "113", "0", "0 %", "1.33 %" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_shell", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "SHELL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "n/a" });
            expectedData.AddRange(new List<string> { " ", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Module_Sql", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "SQL", " ", " ", " ", " ", " " });
            expectedData.AddRange(new List<string> { "Low Complexity", "9", "9", "0", "0 %", "100 %" });
            expectedData.AddRange(new List<string> { "Average Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "High Complexity", "0", "0", "0", "n/a", "0 %" });
            expectedData.AddRange(new List<string> { "Extreme Complexity", "0", "0", "0", "n/a", "0 %" });
            TestUtility.AssertTableContent(table, expectedData, 6, 43);
            Assert.IsTrue(table.HasColumnHeaders);
        }

    }
}
