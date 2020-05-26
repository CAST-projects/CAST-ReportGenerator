using CastReporting.Domain;
using CastReporting.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace CastReporting.UnitTest.Repositories
{
    [TestClass()]
    public class CsvSerializerTest
    {
        /// <summary>
        ///
        ///</summary>
        [TestMethod()]
        [DeploymentItem(@".\Data\OmgFunctionsTechnicalEscaped.csv", "Data")]
        public void DoubleQuotedEscapedTest()
        {
            var csvString = File.ReadAllText(@".\Data\OmgFunctionsTechnicalEscaped.csv");
            var serializer = new CsvSerializer<OmgFunctionTechnical>();
            OmgFunctionTechnical[] functions = serializer.ReadObjects(csvString, -1, null).ToArray();
            Assert.AreEqual(functions.Count(), 3);
            
            OmgFunctionTechnical expect1 = initFunction(new string[] { "497", "getDays",
                "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].getDays",
                "eFunction","added","0.05","45.42553191489362","2.2712766295893396" });
            Assert.IsTrue(Compare(expect1, functions[0]));

            OmgFunctionTechnical expect2 = initFunction(new string[] { "4087", "getM;onthName",
                "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].getM;onthName",
                "eFunction","added","0.05","45.42553191489362","2.2712766295893396"});
            Assert.IsTrue(Compare(expect2,functions[1]));

            OmgFunctionTechnical expect3 = initFunction(new string[] { "1348", "leapYear",
                "[C:\\jenkins7_slave\\workspace\\CAIP_Trunk_TestE2E_CSS_ADG\\Work\\CAST\\Deploy\\IFPUG\\Sol2005\\solution_complete_for_stats\\WebApplicationVB\\appli_web_asp\\calendar.htc].leapYear",
                "eFunction","added","0.05","45.42553191489362","2.2712766295893396"});
            Assert.IsTrue(Compare(expect3,functions[2]));
        }

        private OmgFunctionTechnical initFunction(string[] parameters)
        {
            OmgFunctionTechnical func = new OmgFunctionTechnical();
            func.ObjectId = parameters[0];
            func.ObjectName = parameters[1];
            func.ObjectFullName = parameters[2];
            func.ObjectType = parameters[3];
            func.ObjectStatus = parameters[4];
            func.EffortComplexity = parameters[5];
            func.EquivalenceRatio = parameters[6];
            func.AepCount = parameters[7];
            return func;
        }

        public bool Compare(OmgFunctionTechnical func1, OmgFunctionTechnical func2)
        {
            return string.Equals(func1.ObjectId, func2.ObjectId)
                && string.Equals(func1.ObjectName, func2.ObjectName)
                && string.Equals(func1.ObjectFullName, func2.ObjectFullName)
                && string.Equals(func1.ObjectType, func2.ObjectType)
                && string.Equals(func1.ObjectStatus, func2.ObjectStatus)
                && string.Equals(func1.EffortComplexity, func2.EffortComplexity)
                && string.Equals(func1.EquivalenceRatio, func2.EquivalenceRatio)
                && string.Equals(func1.AepCount, func2.AepCount);
        }

    }
}
