using CastReporting.Domain.Imaging;
using CastReporting.Domain.Imaging.Constants;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.BLL.Computing
{
    public enum CategoryType
    {
        Low,
        Average,
        High,
        VeryHigh,
    };

    public static class CastComplexityExtension
    {

        public static double? GetCostComplexityGrade(this Snapshot snapshot, QualityDistribution categorieId, CostComplexity metricId)
                => GetCostComplexityGrade(snapshot, categorieId, (int)metricId);

        public static double? GetCostComplexityGrade(this Snapshot snapshot, QualityDistribution categorieId, DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity metricId)
                => GetCostComplexityGrade(snapshot, categorieId, (int)metricId);

        /// <summary>
        /// return Fack Value --> Mock should be deleted on S06
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="categorieId"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        private static double? GetCostComplexityGrade(Snapshot snapshot, QualityDistribution categorieId, int metricId)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == (int)categorieId);
            var category = result?.DetailResult?.Categories?.FirstOrDefault(_ => _.key == metricId);
            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        private static bool IsLow(Category category)
        {
            var name = category.Name.ToLower();
            return name.Contains("low") || name.Contains("small");
        }

        private static bool IsAverage(Category category)
        {
            var name = category.Name.ToLower();
            return name.Contains("average") || name.Contains("moderate");
        }

        private static bool IsHigh(Category category)
        {
            var name = category.Name.ToLower();
            return !name.Contains("very") && (name.Contains("high") || name.Contains("large"));
        }

        private static bool IsVeryHigh(Category category)
        {
            var name = category.Name.ToLower();
            return name.Contains("very") && (name.Contains("high") || name.Contains("large"));
        }


        public static double? GetCostComplexityGrade(this Snapshot snapshot, QualityDistribution categorieId, CategoryType position)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == (int)categorieId);
            Category category = null;
            var categoryNames = result?.DetailResult?.Categories;
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case CategoryType.Low:
                    category = categoryNames?.FirstOrDefault(IsLow);
                    break;
                case CategoryType.Average:
                    category = categoryNames?.FirstOrDefault(IsAverage);
                    break;
                case CategoryType.High:
                    category = categoryNames?.FirstOrDefault(IsHigh);
                    break;
                case CategoryType.VeryHigh:
                    category = categoryNames?.FirstOrDefault(IsVeryHigh);
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetModuleCostComplexityGrade(this Snapshot snapshot, QualityDistribution categorieId, CategoryType position, Module module)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == (int)categorieId);
            Category category = null;
            ModuleResult moduleResults = result?.ModulesResult.FirstOrDefault(_ => _.Module.Name.Equals(module.Name));
            if (moduleResults == null) return null;
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case CategoryType.Low:
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(IsLow);
                    break;
                case CategoryType.Average:
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(IsAverage);
                    break;
                case CategoryType.High:
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(IsHigh);
                    break;
                case CategoryType.VeryHigh:
                    category = moduleResults.DetailResult?.Categories?.FirstOrDefault(IsVeryHigh);
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetTechnologyCostComplexityGrade(this Snapshot snapshot, QualityDistribution categorieId, CategoryType position, string technology)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == (int)categorieId);
            Category category = null;
            TechnologyResult technoResults = result?.TechnologyResult.FirstOrDefault(_ => _.Technology.Equals(technology));
            if (technoResults == null) return null;
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case CategoryType.Low:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsLow);
                    break;
                case CategoryType.Average:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsAverage);
                    break;
                case CategoryType.High:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsHigh);
                    break;
                case CategoryType.VeryHigh:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsVeryHigh);
                    break;
            }

            return category != null ? MathUtility.GetRound(category.Value) : null;
        }

        public static double? GetModuleTechnoCostComplexityGrade(this Snapshot snapshot, QualityDistribution categorieId, CategoryType position, Module module, string technology)
        {
            var result = snapshot?.CostComplexityResults?.FirstOrDefault(_ => _.Reference.Key == (int)categorieId);
            Category category = null;
            ModuleResult moduleResults = result?.ModulesResult.FirstOrDefault(_ => _.Module.Name.Equals(module.Name));
            if (moduleResults == null) return null;
            TechnologyResult technoResults = moduleResults.TechnologyResults.FirstOrDefault(_ => _.Technology.Equals(technology));
            if (technoResults == null) return null;

            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (position)
            {
                case CategoryType.Low:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsLow);
                    break;
                case CategoryType.Average:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsAverage);
                    break;
                case CategoryType.High:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsHigh);
                    break;
                case CategoryType.VeryHigh:
                    category = technoResults.DetailResult?.Categories?.FirstOrDefault(IsVeryHigh);
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
        public static string GetCostComplexityName(this Snapshot snapshot, QualityDistribution categorieId)
        {
            var result = snapshot.CostComplexityResults.FirstOrDefault(_ => _.Reference.Key == (int)categorieId);
            return result?.Reference?.Name;
        }


        public static double? GetCategoryValue(this Snapshot snapshot, int categorieId)
        {
            double? value = null;
            List<Category[]> categories = snapshot?.CostComplexityResults.Select(_ => _.DetailResult.Categories).ToList();
            foreach (Category[] distrib in categories)
            {
                if (distrib == null) continue;
                foreach (Category c in distrib)
                {
                    if (c.key == categorieId)
                    {
                        value = c.Value;
                    }
                }
            }
            return value;
        }

        public static string GetCategoryName(this Snapshot snapshot, int categorieId)
        {
            string name = null;
            List<Category[]> categories = snapshot?.CostComplexityResults?.Select(_ => _.DetailResult.Categories)?.ToList();
            if (categories == null) return null;
            foreach (Category[] distrib in categories)
            {
                if (distrib == null) continue;
                foreach (Category c in distrib)
                {
                    if (c.key == categorieId)
                    {
                        name = c.Name;
                    }
                }
            }
            return name;
        }

        public static double? GetModuleCategoryValue(this Snapshot snapshot, int categorieId, Module module)
        {
            double? value = null;
            List<Category[]> categories = snapshot?.CostComplexityResults?
                .Select(_ => _.ModulesResult.Where(m => m.Module.Name.Equals(module.Name)).FirstOrDefault())?
                .Select(_ => _.DetailResult.Categories)?.ToList();
            if (categories == null) return null;
            foreach (Category[] distrib in categories)
            {
                foreach (Category c in distrib)
                {
                    if (c.key == categorieId)
                    {
                        value = c.Value;
                    }
                }
            }
            return value;
        }

        public static double? GetTechnoCategoryValue(this Snapshot snapshot, int categorieId, string technology)
        {
            double? value = null;
            List<Category[]> categories = snapshot?.CostComplexityResults?
                .Select(_ => _.TechnologyResult.Where(t => t.Technology.Equals(technology)).FirstOrDefault())?
                .Select(_ => _?.DetailResult.Categories)?.ToList();
            if (categories == null) return null;
            foreach (Category[] distrib in categories)
            {
                if (distrib == null) continue;
                foreach (Category c in distrib)
                {
                    if (c.key == categorieId)
                    {
                        value = c.Value;
                    }
                }
            }
            return value;
        }

        public static double? GetModuleTechnoCategoryValue(this Snapshot snapshot, int categorieId, Module module, string technology)
        {
            double? value = null;
            List<Category[]> categories = snapshot?.CostComplexityResults?
                .Select(_ => _.ModulesResult.Where(m => m.Module.Name.Equals(module.Name)).FirstOrDefault())?
                .Select(_ => _.TechnologyResults.Where(t => t.Technology.Equals(technology)).FirstOrDefault())?
                .Select(_ => _?.DetailResult.Categories)?.ToList();
            if (categories == null) return null;
            foreach (Category[] distrib in categories)
            {
                if (distrib == null) continue;
                foreach (Category c in distrib)
                {
                    if (c.key == categorieId)
                    {
                        value = c.Value;
                    }
                }
            }
            return value;
        }
    }
}
