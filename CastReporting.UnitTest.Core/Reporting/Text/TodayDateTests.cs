using CastReporting.Domain;
using CastReporting.Reporting.Block.Text;
using CastReporting.Reporting.ReportingModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CastReporting.UnitTest.Reporting.Text
{
    [TestClass]
    public class TodayDateTests
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }


        [TestMethod]
        public void TestContent()
        {
            CastDate currentDate = new CastDate { Time = 1484953200000 };
            ReportData reportData = new ReportData();

            var component = new TodayDate();
            Dictionary<string, string> config = new Dictionary<string, string>();
            var str = component.Content(reportData, config);
            var todaydate = DateTime.Now.ToString("MMM dd yyyy");
            Assert.AreEqual(todaydate, str);
        }

    }
}
