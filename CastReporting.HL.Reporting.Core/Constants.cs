using CastReporting.Reporting.Block.Table;

namespace CastReporting.Reporting.Core.Highlight.Constants
{
    public enum SoftwareKpi
    {
        Health,
        Resiliency,
        Agility,
        Elegance,
    }

    public enum Snapshot
    {
        Current,
        Previous,
    }

    public static class ShortLabels {
        public const string SoftwareHealth = "SH";
        public const string SoftwareResiliency= "SR";
        public const string SoftwareAgility= "SA";
        public const string SoftwareElegance= "SE";
    }

    public static class Labels
    {
        public const string Evol = "Evolution";
        public const string EvolPercent = "Evolution (%)";

        public const string SoftwareHealth = "Health";
        public const string SoftwareResiliency = "Resiliency";
        public const string SoftwareAgility = "Agility";
        public const string SoftwareElegance = "Elegance";
    }
}
