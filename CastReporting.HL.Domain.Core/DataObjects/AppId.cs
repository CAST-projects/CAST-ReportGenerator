namespace CastReporting.HL.Domain;

// Modèle simplifié de l'entité HL pour l'API /domains/{domainId}/applications
public class AppId
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DomainId { get; set; } = string.Empty;
}

