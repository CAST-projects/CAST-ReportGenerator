
using System.Reflection;

namespace Cast.Util.Version
{
    public class VersionUtil
    {
        private VersionUtil()
        {
            // Avoid instanciation of the class
        }

        public static string GetRGVersion(Assembly asm)
        {
            if (asm == null)
                return null;
            var n = asm.GetName();
            return n.Version == null ? null : n.Version.ToString();
        }

        private static bool IsVersionCompatible(string targetVersion, string serviceVersion)
        {
            if ("X.X.X-XXX".Equals(serviceVersion)) return false;
            // due to new version format of rest api since 2.0, the '-' lead to an input string exception for System.Version
            string checkedVersion = serviceVersion.Replace('-', '.');
            return new System.Version(checkedVersion).CompareTo(new System.Version(targetVersion)) >= 0;
        }

        public static bool IsAdgVersion833Compliant(string version)
        {
            return IsVersionCompatible("8.3.3", version);
        }

        public static bool IsAdgVersion82Compliant(string version)
        {
            return IsVersionCompatible("8.2.0", version);
        }

        public static bool Is111Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.11.0.000", serviceVersion);
        }

        public static bool Is112Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.12.0.000", serviceVersion);
        }

        public static bool Is19Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.9.0.000", serviceVersion);
        }

        public static bool Is18Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.8.0.000", serviceVersion);
        }

        public static bool Is17Compatible(string serviceVersion)
        {
            return IsVersionCompatible("1.7.0.000", serviceVersion);
        }

    }
}
