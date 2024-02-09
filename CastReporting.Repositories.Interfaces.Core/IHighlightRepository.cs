using System;
using System.Collections.Generic;
using CastReporting.Domain.Highlight;

namespace CastReporting.Repositories.Interfaces
{
    /// <summary>
    /// Defines the minimal data access layer methods that must be
    /// enabled in class that inherit of this class.
    /// </summary>
    public interface IHighlightRepository : IDisposable
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsServiceValid();

        string GetServerVersion();

        Company GetCompany(string companyId);
        HLDomain GetDomain(string domainId);
        IList<AppId> GetDomainAppIds(string domainId);
    }
}
