using System;

namespace CastReporting.Repositories.Interfaces
{
    /// <summary>
    /// Defines the minimal data access layer methods that must be
    /// enabled in class that inherit of this class.
    /// </summary>
    public interface IExtendRepository : IDisposable
    {
        /// <summary>
        /// For downloading the latest version, version parameter should be null or empty, or latest
        /// </summary>
        /// <param name="packageId"></param>
        /// <param name="targetPath"></param>
        /// <param name="version"></param>
        void GetPackageTemplate(string packageId, string targetPath, string version);
        string SearchForLatestVersion(string packageId);
    }
}
