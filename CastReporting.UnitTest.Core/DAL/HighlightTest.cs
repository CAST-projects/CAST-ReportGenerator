using System.Net;
using CastReporting.HL.Domain;
using CastReporting.HL.Repositories;
using CastReporting.HL.Repositories.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CastReporting.UnitTest.Repositories
{


    /// <summary>
    ///This is a test class for HLContextTest and is intended
    ///to contain all HLContextTest Unit Tests
    ///</summary>
    [Ignore]
    [TestClass()]
    public class HLContextTest
    {

        private readonly HLWSConnection _connection = new HLWSConnection
        {
            Url = "https://demo.casthighlight.com/",
            Login = "***",
            Password = "***",
            IsActive = true,
            Name = "Default"
        };

        private const string companyId = "22400";

        [TestMethod()]
        public void IsServiceValidTest()
        {
            IHighlightRepository context = new HighlightRepository(_connection, null);
            bool result = context.IsServiceValid();
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void GetCompanyInfo()
        {
            IHighlightRepository context = new HighlightRepository(_connection, null);
            try
            {
                var result = context.GetCompany(companyId);
                Assert.IsNotNull(result);
            }
            catch (WebException ex)
            {
                // the demo account is not allowed to get the company info
                Assert.AreEqual(ex.Status, WebExceptionStatus.ProtocolError);
            }
        }

        [TestMethod()]
        public void GetDomainInfo()
        {
            IHighlightRepository context = new HighlightRepository(_connection, null);
            // but the demo account is allowed to get the domain info :)
            var result = context.GetDomain(companyId);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetDomainAppInfo()
        {
            IHighlightRepository context = new HighlightRepository(_connection, null);
            var result = context.GetDomainAppIds(companyId);
            Assert.IsNotNull(result);
        }
    }
}