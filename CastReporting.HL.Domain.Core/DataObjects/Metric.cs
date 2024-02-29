namespace CastReporting.HL.Domain;


// Modèle de l'entité HL correspondant à un snapshot d'analyse (contient les KPI HL) 
public class Metric
{
    static readonly DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public long snapshotDate { get; set; }
    public DateTime SnapshotDate => EPOCH + new TimeSpan(0, 0, (int)(snapshotDate / 1000));

    public string SnapshotLabel { get; set; } = string.Empty;
    public double SoftwareAgility { get; set; }
    public double SoftwareElegance { get; set; }
    public double SoftwareResiliency { get; set; }
    public double SoftwareHealth => (SoftwareResiliency + SoftwareAgility + SoftwareElegance) / 3;
    public double OpenSourceSafety { get; set; }
    public OpenSourceSafetyDetail? OssDetail { get; set; }
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

    public IList<Vulnerability> Vulnerabilities { get; set; } = [];

    public int? cveAdvisory = null;
    public int CveAdvisory
    {
        get { return cveAdvisory ?? Vulnerabilities.Count(v => v.CriticityCategory == CriticityCategory.ADVISORY); }
        set { cveAdvisory = value; }
    }

    public int? cveLow = null;
    public int CveLow
    {
        get { return cveLow ?? Vulnerabilities.Count(v => v.CriticityCategory == CriticityCategory.LOW); }
        set { cveLow = value; }
    }

    public int? cveMedium = null;
    public int CveMedium
    {
        get { return cveMedium ?? Vulnerabilities.Count(v => v.CriticityCategory == CriticityCategory.MEDIUM); }
        set { cveMedium = value; }
    }

    public int? cveHigh = null;
    public int CveHigh
    {
        get { return cveHigh ?? Vulnerabilities.Count(v => v.CriticityCategory == CriticityCategory.HIGH); }
        set { cveHigh = value; }
    }

    public int? cveCritical = null;
    public int CveCritical
    {
        get { return cveCritical ?? Vulnerabilities.Count(v => v.CriticityCategory == CriticityCategory.CRITICAL); }
        set { cveCritical = value; }
    }

    public Metric? ComputeTrend(Metric? previous)
    {
        return (previous == null) ? null : new Metric
        {
            SnapshotLabel = SnapshotLabel,
            snapshotDate = snapshotDate,
            SoftwareAgility = SoftwareAgility - previous.SoftwareAgility,
            SoftwareElegance = SoftwareElegance - previous.SoftwareElegance,
            SoftwareResiliency = SoftwareResiliency - previous.SoftwareResiliency,
            OpenSourceSafety = OpenSourceSafety - previous.OpenSourceSafety,
            CloudReady = CloudReady - previous.CloudReady,
            CloudReadyScan = CloudReadyScan - previous.CloudReadyScan,
            BusinessImpact = BusinessImpact - previous.BusinessImpact,
            RoarIndex = RoarIndex - previous.RoarIndex,
            Roadblocks = Roadblocks - previous.Roadblocks,
            TotalLinesOfCode = TotalLinesOfCode - previous.TotalLinesOfCode,
            TotalFiles = TotalFiles - previous.TotalFiles,
            BackFiredFP = BackFiredFP - previous.BackFiredFP,
            TechnicalDebt = TechnicalDebt - previous.TechnicalDebt,
            TechnicalDebtDensity = TechnicalDebtDensity - previous.TechnicalDebtDensity,
            CveAdvisory = CveAdvisory - previous.CveAdvisory,
            CveLow = CveLow - previous.CveLow,
            CveMedium = CveMedium - previous.CveMedium,
            CveHigh = CveHigh - previous.CveHigh,
            CveCritical = CveCritical - previous.CveCritical,
        };
    }
}

