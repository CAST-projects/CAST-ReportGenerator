namespace CastReporting.HL.Domain;

public class HLDomain
{
  public int Id { get; set; }
  public string ClientRef { get; set; } = string.Empty;
  public string Name { get; set; } = string.Empty;
  public HLDomain? Parent { get; set; }
}
