namespace CastReporting.HL.Domain;

// Modèle de l'entité HL pour l'API /domains/{domainId}/applications/{appId}
public class AppInfo
{
    public string Url { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IList<HLDomain> Domains { get; set; } = [];

    public IList<SnapshotResults> Metrics { get; set; } = [];

    public SnapshotResults? CurrentMetrics => (Metrics.Count > 0) ? Metrics[0] : null;
    public SnapshotResults? PreviousMetrics => (Metrics.Count > 1) ? Metrics[1] : null;

    // Calcul des évolutions par rapport à l'analyse précédente 
    public SnapshotResults? Trend => (PreviousMetrics == null) ? null : CurrentMetrics?.ComputeTrend(PreviousMetrics);

    // Calcul des évolutions sur une période donnée
    public SnapshotResults? TrendOneWeek => GetTrendForPeriod(Period.OneWeek);
    public SnapshotResults? TrendTwoWeeks => GetTrendForPeriod(Period.TwoWeeks);
    public SnapshotResults? TrendThreeWeeks => GetTrendForPeriod(Period.ThreeWeeks);
    public SnapshotResults? TrendOneMonth => GetTrendForPeriod(Period.OneMonth);
    public SnapshotResults? TrendThreeMonths => GetTrendForPeriod(Period.ThreeMonths);
    public SnapshotResults? TrendSixMonths => GetTrendForPeriod(Period.SixMonths);
    public SnapshotResults? TrendOneYear => GetTrendForPeriod(Period.OneYear);

    private SnapshotResults? GetTrendForPeriod(Period period) {
        if (CurrentMetrics == null) return null;
        var prevDate = period.GetStartDateFrom(CurrentMetrics.SnapshotDate);
        var previous = Metrics.Where(_ => _.SnapshotDate <= prevDate).OrderByDescending(_ => _.SnapshotDate).FirstOrDefault();
        if (previous == null) return null;
        return CurrentMetrics.ComputeTrend(previous);
    }
}

