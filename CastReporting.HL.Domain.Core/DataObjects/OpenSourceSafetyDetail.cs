namespace CastReporting.HL.Domain;

// Modèle de l'entité HL correspondant aux résultats détaillés Open Source
public class OpenSourceSafetyDetail
{
    public object? OpenSourceLicense { get; set; }
    public object? OpenSourceObsolescence { get; set; }
    public object? OpenSourceCVE { get; set; }
}
