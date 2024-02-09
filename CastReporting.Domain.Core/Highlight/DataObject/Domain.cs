namespace CastReporting.Domain.Highlight
{
  public class HLDomain
  {
    public int Id { get; set; }
    public string ClientRef { get; set; }
    public string Name { get; set; }
    public HLDomain Parent { get; set; }
  }
}