namespace CastReporting.HL.Domain;

// Modèle de l'entité HL pour l'API /domains/{domainId}/applications/{appId}
public class AppInfo
{
    public string Url { get; set; } = string.Empty;

    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IList<Metric> Metrics { get; set; } = [];

    public Metric? CurrentMetrics => (Metrics.Count > 0) ? Metrics[0] : null;
    public Metric? PreviousMetrics => (Metrics.Count > 1) ? Metrics[1] : null;

    // Calcul des évolutions par rapport à l'analyse précédente 
    public Metric? Trend => CurrentMetrics?.ComputeTrend(PreviousMetrics);

    // Calcul des évolutions sur une période donnée
    public Metric? TrendOneWeek => GetTrendForPeriod(Period.OneWeek);
    public Metric? TrendTwoWeeks => GetTrendForPeriod(Period.TwoWeeks);
    public Metric? TrendThreeWeeks => GetTrendForPeriod(Period.ThreeWeeks);
    public Metric? TrendOneMonth => GetTrendForPeriod(Period.OneMonth);
    public Metric? TrendThreeMonths => GetTrendForPeriod(Period.ThreeMonths);

    private Metric? GetTrendForPeriod(Period period)
    {
        if (CurrentMetrics == null) return null;
        var prevDate = period.GetStartDateFrom(CurrentMetrics.SnapshotDate);
        var previous = Metrics.Where(_ => _.SnapshotDate <= prevDate).OrderByDescending(_ => _.SnapshotDate).FirstOrDefault();
        return CurrentMetrics.ComputeTrend(previous);
    }
}

