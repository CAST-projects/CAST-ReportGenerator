
namespace CastReporting.HL.Domain;

public class Period(int days = 0, int months = 0)
{
    public int Days { get; private set; } = days;
    public int Months { get; private set; } = months;

    public static readonly Period OneWeek = new(days: 7);
    public static readonly Period TwoWeeks = new(days: 2 * 7);
    public static readonly Period ThreeWeeks = new(days: 3 * 7);
    public static readonly Period OneMonth = new(months: 1);
    public static readonly Period ThreeMonths = new(months: 3);

    public DateTime GetStartDateFrom(DateTime date) => date.AddMonths(-Months).AddDays(-Days);
}
