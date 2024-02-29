namespace CastReporting.HL.Domain;

public class AuditLog
{
    public string CompanyId { get; set; } = string.Empty;
    public IList<AuditLine> Result { get; set; } = [];
}

