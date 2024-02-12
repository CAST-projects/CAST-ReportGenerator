using Cast.Util;
using CastReporting.Domain.Imaging;
using System;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public static class SnapshotExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static string GetSnapshotVersionNumber(this Snapshot snapshot)
        {
            return snapshot?.Annotation?.Version ?? FormatHelper.No_Value;
        }

        public static string GetSnapshotNameVersion(this Snapshot snapshot)
        {
            var annotation = snapshot?.Annotation;
            return annotation == null ? FormatHelper.No_Value : $"{annotation.Name} - {annotation.Version}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <returns></returns>
        public static DateTime? GetSnapshotDate(this Snapshot snapshot)
        {
            return snapshot?.Annotation?.Date?.DateSnapShot;
        }

        public static bool IsLatestSnapshot(this Application application, Snapshot snapshot)
        {
            int nbSnapshot = application.Snapshots.Count();
            if (nbSnapshot <= 0) return false;
            Snapshot latest = nbSnapshot == 1 ? application.Snapshots.FirstOrDefault() : application.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).FirstOrDefault();
            return latest?.Equals(snapshot) ?? false;
        }

    }
}
