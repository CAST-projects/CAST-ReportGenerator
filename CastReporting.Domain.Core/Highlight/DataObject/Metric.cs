
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Domain.Highlight
{
    // Modèle de l'entité HL correspondant à un snapshot d'analyse (contient les KPI HL) 
    public class Metric
    {
        static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public long snapshotDate { get; set; }

        public string SnapshotLabel { get; set; }
        public DateTime SnapshotDate => EPOCH + new TimeSpan(0, 0, (int)(snapshotDate / 1000));
        public double SoftwareAgility { get; set; }
        public double SoftwareElegance { get; set; }
        public double SoftwareResiliency { get; set; }
        public double SoftwareHealth => (SoftwareResiliency + SoftwareAgility + SoftwareElegance) / 3;
        public double OpenSourceSafety { get; set; }
        public OpenSourceSafetyDetail OssDetail { get; set; }
        public double CloudReady { get; set; }
        public double CloudReadyScan { get; set; }
        public double Roadblocks { get; set; }
        public int TotalLinesOfCode { get; set; }
        public int TotalFiles { get; set; }
        public double BackFiredFP { get; set; }
        public double BusinessImpact { get; set; }
        public double RoarIndex { get; set; }
        public double TechnicalDebt { get; set; }

        // calcul de la densité de dette technique par ligne de code
        private double? _technicalDebtDensity = null;
        public double TechnicalDebtDensity
        {
            get
            {
                if (!_technicalDebtDensity.HasValue && TotalLinesOfCode > 0)
                {
                    _technicalDebtDensity = TechnicalDebt / TotalLinesOfCode;
                }
                return _technicalDebtDensity.HasValue ? _technicalDebtDensity.Value : 0;
            }
            set
            {
                _technicalDebtDensity = value;
            }
        }

        public IList<Vulnerability> Vulnerabilities { get; set; }

        public int? cveAdvisory = null;
        public int? CveAdvisory
        {
            get { return cveAdvisory ?? Vulnerabilities?.Count(v => v.CriticityCategory == CriticityCategory.ADVISORY); }
            set { cveAdvisory = value; }
        }

        public int? cveLow = null;
        public int? CveLow
        {
            get { return cveLow ?? Vulnerabilities?.Count(v => v.CriticityCategory == CriticityCategory.LOW); }
            set { cveLow = value; }
        }

        public int? cveMedium = null;
        public int? CveMedium
        {
            get { return cveMedium ?? Vulnerabilities?.Count(v => v.CriticityCategory == CriticityCategory.MEDIUM); }
            set { cveMedium = value; }
        }

        public int? cveHigh = null;
        public int? CveHigh
        {
            get { return cveHigh ?? Vulnerabilities?.Count(v => v.CriticityCategory == CriticityCategory.HIGH); }
            set { cveHigh = value; }
        }

        public int? cveCritical = null;
        public int? CveCritical
        {
            get { return cveCritical ?? Vulnerabilities?.Count(v => v.CriticityCategory == CriticityCategory.CRITICAL); }
            set { cveCritical = value; }
        }

        public static Metric ComputeTrend(Metric current, Metric previous)
        {
            if (previous == null) return null;
            var trend = new Metric();
            trend.SnapshotLabel = previous.SnapshotLabel;
            trend.snapshotDate = previous.snapshotDate;
            trend.SoftwareAgility = current.SoftwareAgility - previous.SoftwareAgility;
            trend.SoftwareElegance = current.SoftwareElegance - previous.SoftwareElegance;
            trend.SoftwareResiliency = current.SoftwareResiliency - previous.SoftwareResiliency;
            trend.OpenSourceSafety = current.OpenSourceSafety - previous.OpenSourceSafety;
            trend.CloudReady = current.CloudReady - previous.CloudReady;
            trend.CloudReadyScan = current.CloudReadyScan - previous.CloudReadyScan;
            trend.BusinessImpact = current.BusinessImpact - previous.BusinessImpact;
            trend.RoarIndex = current.RoarIndex - previous.RoarIndex;
            trend.Roadblocks = current.Roadblocks - previous.Roadblocks;
            trend.TotalLinesOfCode = current.TotalLinesOfCode - previous.TotalLinesOfCode;
            trend.TotalFiles = current.TotalFiles - previous.TotalFiles;
            trend.BackFiredFP = current.BackFiredFP - previous.BackFiredFP;
            trend.TechnicalDebt = current.TechnicalDebt - previous.TechnicalDebt;
            trend.TechnicalDebtDensity = current.TechnicalDebtDensity - previous.TechnicalDebtDensity;

            if (current.CveAdvisory != null)
            {
                trend.CveAdvisory = current.CveAdvisory - (previous.CveAdvisory ?? 0);
            }
            if (current.CveLow != null)
            {
                trend.CveLow = current.CveLow - (previous.CveLow ?? 0);
            }
            if (current.CveMedium != null)
            {
                trend.CveMedium = current.CveMedium - (previous.CveMedium ?? 0);
            }
            if (current.CveHigh != null)
            {
                trend.CveHigh = current.CveHigh - (previous.CveHigh ?? 0);
            }
            if (current.CveCritical != null)
            {
                trend.CveCritical = current.CveCritical - (previous.CveCritical ?? 0);
            }

            return trend;
        }
    }
}
