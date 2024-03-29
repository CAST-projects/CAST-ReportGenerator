using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CastReporting.HL.Domain;

namespace CastReporting.HL.Repositories.Interfaces
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

        Company? GetCompany();
        HLDomain? GetDomain(string domainId);
        IList<AppId> GetDomainAppIds(string? domainId = null);
        IList<Snapshot> GetAppSnapshots(string? appId);
        AppInfo GetAppResults(string appId);
    }
}
