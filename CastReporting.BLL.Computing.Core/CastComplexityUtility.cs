using CastReporting.Domain;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    public static class CastComplexityUtility
    {

        /// <summary>
        /// return Fack Value --> Mock should be deleted on S06
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="categorieId"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        public static double? GetCostComplexityGrade(Snapshot snapshot, int categorieId, int metricId)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            var category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.key == metricId);
            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetCostComplexityGrade(Snapshot snapshot, int categorieId, string position)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            Category category = null;
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case "low":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("low"))
                               ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("small"));
                    break;
                case "average":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("average"))
                        ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("moderate"));
                    break;
                case "high":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && !_.Name.ToLower().Contains("very"))
                               ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && !_.Name.ToLower().Contains("very"));
                    break;
                case "very_high":
                    category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && _.Name.ToLower().Contains("very"))
                               ?? result?.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && _.Name.ToLower().Contains("very"));
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetModuleCostComplexityGrade(Snapshot snapshot, int categorieId, string position, Module module)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            Category category = null;
            ModuleResult moduleResults = result?.ModulesResult.FirstOrDefault(_ => _.Module.Name.Equals(module.Name));
            if (moduleResults == null) return null;
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case "low":
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("low"))
                               ?? moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("small"));
                    break;
                case "average":
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("average"))
                        ?? moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("moderate"));
                    break;
                case "high":
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && !_.Name.ToLower().Contains("very"))
                               ?? moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && !_.Name.ToLower().Contains("very"));
                    break;
                case "very_high":
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && _.Name.ToLower().Contains("very"))
                               ?? moduleResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && _.Name.ToLower().Contains("very"));
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetTechnologyCostComplexityGrade(Snapshot snapshot, int categorieId, string position, string technology)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            Category category = null;
            TechnologyResult technoResults = result?.TechnologyResult.FirstOrDefault(_ => _.Technology.Equals(technology));
            if (technoResults == null) return null;
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case "low":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("low"))
                               ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("small"));
                    break;
                case "average":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("average"))
                        ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("moderate"));
                    break;
                case "high":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && !_.Name.ToLower().Contains("very"))
                               ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && !_.Name.ToLower().Contains("very"));
                    break;
                case "very_high":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && _.Name.ToLower().Contains("very"))
                               ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && _.Name.ToLower().Contains("very"));
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetModuleTechnoCostComplexityGrade(Snapshot snapshot, int categorieId, string position, Module module, string technology)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == categorieId);
            Category category = null;
            ModuleResult moduleResults = result?.ModulesResult.FirstOrDefault(_ => _.Module.Name.Equals(module.Name));
            if (moduleResults == null) return null;
            TechnologyResult technoResults = moduleResults.TechnologyResults.FirstOrDefault(_ => _.Technology.Equals(technology));
            if (technoResults == null) return null;

            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case "low":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("low"))
                               ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("small"));
                    break;
                case "average":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("average"))
                        ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("moderate"));
                    break;
                case "high":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && !_.Name.ToLower().Contains("very"))
                               ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && !_.Name.ToLower().Contains("very"));
                    break;
                case "very_high":
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("high") && _.Name.ToLower().Contains("very"))
                               ?? technoResults.DetailResult?.Categories?.FirstOrDefault(_ => _.Name.ToLower().Contains("large") && _.Name.ToLower().Contains("very"));
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="categorieId"></param>
        /// <returns></returns>
        public static string GetCostComplexityName(Snapshot snapshot, int categorieId)
        {
            var result = snapshot.CostComplexityResults.FirstOrDefault(_ => _.Reference.Key == categorieId);
            return result?.Reference?.Name;
        }

    }
}
