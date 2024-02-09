

namespace CastReporting.Domain.Highlight
{
    // Modèle de l'entité HL correspondant aux résultats détaillés Open Source
    public class OpenSourceSafetyDetail
    {
        public dynamic OpenSourceLicense { get; set; }
        public dynamic OpenSourceObsolescence { get; set; }
        public dynamic OpenSourceCVE { get; set; }
    }
}
