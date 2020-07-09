using System;

namespace CastReporting.Mediation.Interfaces.Core
{
    public interface IExtendProxy : IDisposable
    {
        string DownloadPackageInformation(string query);

        void DownloadExtension(string query, string filename);
    }
}
