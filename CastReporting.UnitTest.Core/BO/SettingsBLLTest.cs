using CastReporting.BLL;
using CastReporting.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest
{
    [TestClass]
    public class SettingsBLLTest
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void IsUserMissingForApiKey_ApiKeyWithoutUser_ReturnsTrue(string login)
        {
            var connection = new WSConnection { Url = "http://localhost:8090/dashboards/rest", Login = login, Password = "key", ApiKey = true };

            Assert.IsTrue(SettingsBLL.IsUserMissingForApiKey(connection));
        }

        [TestMethod]
        public void IsUserMissingForApiKey_ApiKeyWithUser_ReturnsFalse()
        {
            var connection = new WSConnection { Url = "http://localhost:8090/dashboards/rest", Login = "admin", Password = "key", ApiKey = true };

            Assert.IsFalse(SettingsBLL.IsUserMissingForApiKey(connection));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("admin")]
        public void IsUserMissingForApiKey_NoApiKey_ReturnsFalse(string login)
        {
            var connection = new WSConnection { Url = "http://localhost:8090/dashboards/rest", Login = login, Password = "pwd", ApiKey = false };

            Assert.IsFalse(SettingsBLL.IsUserMissingForApiKey(connection));
        }

        [TestMethod]
        public void IsUserMissingForApiKey_NullConnection_ReturnsFalse()
        {
            Assert.IsFalse(SettingsBLL.IsUserMissingForApiKey(null));
        }
    }
}
