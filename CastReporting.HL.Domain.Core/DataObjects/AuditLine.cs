namespace CastReporting.HL.Domain;

public class AuditLine
{
    static readonly DateTime EPOCH = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public string Guid { get; set; } = string.Empty;
    public long date { get; set; }
    public DateTime Date => EPOCH + new TimeSpan(0, 0, (int)(date / 1000));
    public long UserId { get; set; }
    public long CompanyId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string IpSource { get; set; } = string.Empty;
    public string Params { get; set; } = string.Empty;
}

