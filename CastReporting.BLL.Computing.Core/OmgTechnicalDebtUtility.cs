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
            if (resbc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)resbc.DetailResult.OmgTechnicalDebt?.Total / 8 / 60,
                    Added = (double?)resbc.DetailResult.OmgTechnicalDebt?.Added / 8 / 60,
                    Removed = (double?)resbc.DetailResult.OmgTechnicalDebt?.Removed / 8 / 60
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (restc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)restc.DetailResult.OmgTechnicalDebt?.Total / 8 / 60,
                    Added = (double?)restc.DetailResult.OmgTechnicalDebt?.Added / 8 / 60,
                    Removed = (double?)restc.DetailResult.OmgTechnicalDebt?.Removed / 8 / 60
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resqr != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)resqr.DetailResult.OmgTechnicalDebt?.Total / 8 / 60,
                    Added = (double?)resqr.DetailResult.OmgTechnicalDebt?.Added / 8 / 60,
                    Removed = (double?)resqr.DetailResult.OmgTechnicalDebt?.Removed / 8 / 60
                };
            }

            return null;
        }

        public static OmgTechnicalDebtIdDTO GetOmgTechDebtModule(Snapshot snapshot, int modId, int metricId)
        {
            ApplicationResult resbc = snapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resbc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Total / 8 / 60,
                    Added = (double?)resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Added / 8 / 60,
                    Removed = (double?)resbc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Removed / 8 / 60
                };
            }

            ApplicationResult restc = snapshot.TechnicalCriteriaResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (restc != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Total / 8 / 60,
                    Added = (double?)restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Added / 8 / 60,
                    Removed = (double?)restc.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Removed / 8 / 60
                };
            }

            ApplicationResult resqr = snapshot.QualityRulesResults.FirstOrDefault(_ => _.Reference.Key == metricId);
            if (resqr != null)
            {
                return new OmgTechnicalDebtIdDTO
                {
                    Id = metricId,
                    Total = (double?)resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Total / 8 / 60,
                    Added = (double?)resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Added / 8 / 60,
                    Removed = (double?)resqr.ModulesResult.FirstOrDefault(m => m.Module.Id == modId)?.DetailResult.OmgTechnicalDebt?.Removed / 8 / 60
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
