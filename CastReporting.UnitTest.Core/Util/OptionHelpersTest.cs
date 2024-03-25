using Cast.Util.Date;
using CastReporting.Reporting.Helper;
using CastReporting.UnitTest.Reporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class OptionHelpersTests
    {
        private Dictionary<String, String> getOptions() {
            var options = new Dictionary<String, String>();

            options["SPACE"] = " ";
            options["STR1"] = "STR1";
            options["STR2"] = "  STR2  ";
            options["STR3"] = "  STR3 \n ";
            options["INT1"] = "1";
            options["INT2"] = "  2  ";
            options["INT3"] = "  3 \n ";

            return options;
        }

        [TestMethod]
        public void TestGetOption() {
            var options = getOptions();
            Assert.AreEqual("STR1", options.GetOption("STR1"));
            Assert.AreEqual("  STR2  ", options.GetOption("STR2")); // note: current implementation only replaces characters \t \r \n with ""
            Assert.AreEqual("  STR3  ", options.GetOption("STR3")); // note: current implementation only replaces characters \t \r \n with ""

            // blank entry
            Assert.AreEqual(" ", options.GetOption("SPACE"));
        }

        [TestMethod]
        public void TestGetOptionDefaultValue() {
            var options = getOptions();

            // blank entry
            Assert.AreEqual(" ", options.GetOption("SPACE", "default"));
            Assert.AreEqual(" ", options.GetOption("SPACE", "default\n"));

            // non-existing entry
            Assert.AreEqual(null, options.GetOption("X"));
            Assert.AreEqual("default", options.GetOption("X", "default"));
            Assert.AreEqual("default", options.GetOption("X", "default\n"));
        }

        [TestMethod]
        public void TestGetIntOption() {
            var options = getOptions();
            Assert.AreEqual(1, options.GetIntOption("INT1"));
            Assert.AreEqual(2, options.GetIntOption("INT2"));
            Assert.AreEqual(3, options.GetIntOption("INT3"));

            // blank entry
            Assert.AreEqual(0, options.GetIntOption("SPACE")); // default value is default(int) == 0
        }

        [TestMethod]
        public void TestGetIntOptionDefaultValue() {
            var options = getOptions();

            // blank entry
            Assert.AreEqual(1, options.GetIntOption("SPACE", 1));
            Assert.AreEqual(2, options.GetIntOption("SPACE", 2));

            // non-existing entry
            Assert.AreEqual(0, options.GetIntOption("X"));
            Assert.AreEqual(1, options.GetIntOption("X", 1));
            Assert.AreEqual(2, options.GetIntOption("X", 2));
        }
    }
}