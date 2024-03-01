using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class PortfolioTagNameTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        public void TestContent()
        {
            ImagingData reportData = TestUtility.PrepaEmptyPortfolioReportData();
            reportData.Tag = "UnitTests";

            var component = new PortfolioTagName();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("UnitTests", str);
        }

        [TestMethod]
        public void TestNoCategory()
        {
            ImagingData reportData = TestUtility.PrepaEmptyPortfolioReportData();

            var component = new PortfolioTagName();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            Assert.AreEqual("All", str);
        }

        [TestMethod]
        public void TestNoResult()
        {
            var component = new PortfolioTagName();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(null, config);
            Assert.AreEqual("n/a", str);
        }


    }
}
