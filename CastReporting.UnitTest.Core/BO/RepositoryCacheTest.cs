using CastReporting.BLL;
using CastReporting.Domain;
using CastReporting.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest
{
    /// <summary>
    /// The repository cache must not hand back a repository built with other credentials
    /// when only the user / password / api key changes for the same url.
    /// </summary>
    [TestClass]
    public class RepositoryCacheTest
    {
        private class TestBll : BaseBLL
        {
            public static ICastRepsitory Get(WSConnection connection) => GetRepository(connection, true);
        }

        private static WSConnection NewConnection(string login, string password = "key", bool apiKey = true)
        {
            return new WSConnection
            {
                Url = "http://repo-cache-test:8082/rest",
                Login = login,
                Password = password,
                ApiKey = apiKey,
                ServerCertificateValidation = true
            };
        }

        [TestMethod]
        public void SameConnection_ReturnsCachedRepository()
        {
            var first = TestBll.Get(NewConnection("cache-same"));
            var second = TestBll.Get(NewConnection("cache-same"));

            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void SameUrlDifferentUser_ReturnsNewRepository()
        {
            var first = TestBll.Get(NewConnection("cache-user-a"));
            var second = TestBll.Get(NewConnection("cache-user-b"));

            Assert.AreNotSame(first, second);
        }

        [TestMethod]
        public void SameUrlDifferentPassword_ReturnsNewRepository()
        {
            var first = TestBll.Get(NewConnection("cache-pwd", "key1"));
            var second = TestBll.Get(NewConnection("cache-pwd", "key2"));

            Assert.AreNotSame(first, second);
        }

        [TestMethod]
        public void SameUrlDifferentApiKeyFlag_ReturnsNewRepository()
        {
            var first = TestBll.Get(NewConnection("cache-flag", "pwd", true));
            var second = TestBll.Get(NewConnection("cache-flag", "pwd", false));

            Assert.AreNotSame(first, second);
        }
    }
}
