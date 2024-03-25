﻿using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Domain.Constants;
using CastReporting.UnitTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class ComputingTest
    {
        [TestInitialize()]
        public void Initialize()
        {
            TestUtility.SetCulture("en-US");
        }

        [TestMethod]
        [DeploymentItem(@".\Data\ComputingTest1.json", "Data")]
        public void GetDCGradesTest()
        {

            Snapshot selectedSnapshot = new Snapshot
            {
                Name = "Test",
                Href = "AED1/applications/3/snapshots/3",
                Annotation = new Annotation { Name = "My Snapshot", Version = "2.1" },
                BusinessCriteriaResults = TestUtility.GetSampleResult<ApplicationResult>(@".\Data\ComputingTest1.json")
            };

            var result = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(selectedSnapshot, (int)BusinessCriteria.TechnicalQualityIndex, true);
            if (result == null) Assert.Fail();
            Assert.AreEqual(3.4, result.Value);
            result = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(selectedSnapshot, (int)BusinessCriteria.TechnicalQualityIndex, false);
            Assert.AreEqual("3.40", result?.ToString("N2"));
        }



    }
}
