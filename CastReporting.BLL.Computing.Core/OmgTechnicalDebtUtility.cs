using CastReporting.Domain;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    /// <summary>
    /// 
    /// </summary>
    public static class OmgTechnicalDebtUtility
    {
        public static OmgTechnicalDebtIdDTO GetOmgTechDebt(Snapshot snapshot, string index)
        {
            return GetOmgTechDebt(snapshot, GetOmgIndex(index));
        }


        public static OmgTechnicalDebtIdDTO GetOmgTechDebt(Snapshot snapshot, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdBc = resbc?.DetailResult?.OmgTechnicalDebt;
            if (omgTdBc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdBc?.Total / 8 / 60,
                    Added = (double?)omgTdBc?.Added / 8 / 60,
                    Removed = (double?)omgTdBc?.Removed / 8 / 60
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdTc = restc?.DetailResult?.OmgTechnicalDebt;
            if (omgTdTc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdTc?.Total / 8 / 60,
                    Added = (double?)omgTdTc?.Added / 8 / 60,
                    Removed = (double?)omgTdTc?.Removed / 8 / 60
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdQr = resqr?.DetailResult?.OmgTechnicalDebt;
            if (omgTdQr != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdQr?.Total / 8 / 60,
                    Added = (double?)omgTdQr?.Added / 8 / 60,
                    Removed = (double?)omgTdQr?.Removed / 8 / 60
                };
            }

            return null;
        }

        public static OmgTechnicalDebtIdDTO GetOmgTechDebtModule(Snapshot snapshot, int modId, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdBc = resbc?.ModulesResult?.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdBc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdBc?.Total / 8 / 60,
                    Added = (double?)omgTdBc?.Added / 8 / 60,
                    Removed = (double?)omgTdBc?.Removed / 8 / 60
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdTc = restc?.ModulesResult?.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdTc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdTc?.Total / 8 / 60,
                    Added = (double?)omgTdTc?.Added / 8 / 60,
                    Removed = (double?)omgTdTc?.Removed / 8 / 60
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdQr = resqr?.ModulesResult?.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdQr != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdQr?.Total / 8 / 60,
                    Added = (double?)omgTdQr?.Added / 8 / 60,
                    Removed = (double?)omgTdQr?.Removed / 8 / 60
                };
            }

            return null;
        }

        public static OmgTechnicalDebtIdDTO GetOmgTechDebtTechno(Snapshot snapshot, string techno, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdBc = resbc?.TechnologyResult?.FirstOrDefault(t => t.Technology == techno)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdBc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdBc?.Total / 8 / 60,
                    Added = (double?)omgTdBc?.Added / 8 / 60,
                    Removed = (double?)omgTdBc?.Removed / 8 / 60
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdTc = restc?.TechnologyResult?.FirstOrDefault(t => t.Technology == techno)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdTc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdTc?.Total / 8 / 60,
                    Added = (double?)omgTdTc?.Added / 8 / 60,
                    Removed = (double?)omgTdTc?.Removed / 8 / 60
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdQr = resqr?.TechnologyResult?.FirstOrDefault(t => t.Technology == techno)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdQr != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdQr?.Total / 8 / 60,
                    Added = (double?)omgTdQr?.Added / 8 / 60,
                    Removed = (double?)omgTdQr?.Removed / 8 / 60
                };
            }

            return null;
        }

        public static OmgTechnicalDebtIdDTO GetOmgTechDebtModuleTechno(Snapshot snapshot, int modId, string techno, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdBc = resbc?.ModulesResult?.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults?.FirstOrDefault(t => t.Technology == techno)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdBc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdBc?.Total / 8 / 60,
                    Added = (double?)omgTdBc?.Added / 8 / 60,
                    Removed = (double?)omgTdBc?.Removed / 8 / 60
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdTc = restc?.ModulesResult?.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults?.FirstOrDefault(t => t.Technology == techno)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdTc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdTc?.Total / 8 / 60,
                    Added = (double?)omgTdTc?.Added / 8 / 60,
                    Removed = (double?)omgTdTc?.Removed / 8 / 60
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            OmgTechnicalDebt omgTdQr = resqr?.ModulesResult?.FirstOrDefault(m => m.Module.Id == modId)?.TechnologyResults?.FirstOrDefault(t => t.Technology == techno)?.DetailResult?.OmgTechnicalDebt;
            if (omgTdQr != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)omgTdQr?.Total / 8 / 60,
                    Added = (double?)omgTdQr?.Added / 8 / 60,
                    Removed = (double?)omgTdQr?.Removed / 8 / 60
                };
            }

            return null;
        }

        public static int GetOmgIndex(string indexId)
        {
            int idx;
            switch (indexId)
            {
                case "CISQ":
                    idx = 1062100;
                    break;
                case "AIP":
                    idx = 60017;
                    break;
                case "ISO":
                    idx = 1061000;
                    break;
                default:
                    idx = int.TryParse(indexId, out int id) ? id : 0;
                    break;
            }

            return idx;
        }

    }
}
