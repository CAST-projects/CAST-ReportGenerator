using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Block.Table;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Tables
{
    [TestClass]
    public class TechnicalSizingTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new TechnicalSizing();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "Name", "Value" });
            expectedData.AddRange(new List<string> { "kLoC", "62" });
            expectedData.AddRange(new List<string> { "  Files", "476" });
            expectedData.AddRange(new List<string> { "  Classes", "351" });
            expectedData.AddRange(new List<string> { "SQL Art.", "553" });
            expectedData.AddRange(new List<string> { "  Tables", "349" });
            TestUtility.AssertTableContent(table, expectedData, 2, 6);
            Assert.IsTrue(table.HasColumnHeaders);
        }

        [TestMethod]
        [DeploymentItem(@".\Data\CurrentTechSizeResultsModTechno.json", "Data")]
        public void TestNoHeader()
        {
            CastDate currentDate = new CastDate { Time = 1492984800000 };
            ImagingData reportData = TestUtility.PrepareApplicationReportData("CoCRestAPI",
                null, @".\Data\CurrentTechSizeResultsModTechno.json", "AED/applications/3/snapshots/4", "Snap4_CAIP-8.3ra_RG-1.5.a", "8.3.ra", currentDate,
               null, null, null, null, null, null);

            var component = new TechnicalSizing();
            Dictionary<string, string> config = new Dictionary<string, string>() { { "HEADER", "NO" } };
            var table = component.Content(reportData, config);

            var expectedData = new List<string>();
            expectedData.AddRange(new List<string> { "kLoC", "62" });
            expectedData.AddRange(new List<string> { "  Files", "476" });
            expectedData.AddRange(new List<string> { "  Classes", "351" });
            expectedData.AddRange(new List<string> { "SQL Art.", "553" });
            expectedData.AddRange(new List<string> { "  Tables", "349" });
            TestUtility.AssertTableContent(table, expectedData, 2, 5);
            Assert.IsFalse(table.HasColumnHeaders);
        }

    }
}
