
using Cast.Util;
using Cast.Util.Log;
using CastReporting.BLL.Computing;
using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Module = CastReporting.Domain.Imaging.Module;

namespace CastReporting.Reporting.Helper
{
    public static class MetricsUtility
    {
        private const string ColorGainsboro = "Gainsboro";
        private const string ColorWhite = "White";
        private const string ColorLavander = "Lavender";
        private const string ColorLightYellow = "LightYellow";

        /// <summary>
        /// This method return the name of the metric from getting it in the results of the snapshot
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <returns></returns>
        public static string GetMetricName(ImagingData reportData, Snapshot snapshot, string metricId)
        {
            string name = ((snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault() ??
                            snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault()) ??
                           snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault()) ??
                          snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
            if (snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault() != null)
            {
                name = name + " (" + metricId + ")";
            }
            if (name != null) return name;
            var bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, metricId, true, true).FirstOrDefault();
            if (bfResult == null || !bfResult.ApplicationResults.Any()) return FormatHelper.No_Value;
            name = bfResult.ApplicationResults[0].Reference.Name ?? FormatHelper.No_Value;
            return name;
        }

        /// <summary>
        /// This method return the name, type and result (grade or value) of the metric from getting it in the results of the snapshot
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="snapshot"></param>
        /// <param name="metricId"></param>
        /// <param name="module"></param>
        /// <param name="technology"></param>
        /// <param name="format"></param> should be false for graph component
        /// <returns></returns>
        public static SimpleResult GetMetricNameAndResult(ImagingData reportData, Snapshot snapshot, string metricId, Module module, string technology, bool format)
        {
            MetricType type = MetricType.NotKnown;
            Result bfResult = null;
            double? result = null;
            string resStr = string.Empty;

            try
            {
                string name = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                if (name != null) type = MetricType.BusinessCriteria;
                // if metricId is not a Business Criteria
                if (name == null)
                {
                    name = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                    if (name != null) type = MetricType.TechnicalCriteria;
                }
                // if metricId is not a technical criteria
                if (name == null)
                {
                    name = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                    if (name != null) type = MetricType.QualityRule;
                }
                // if metricId is not a quality rule
                if (name == null)
                {
                    name = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId)).Select(_ => _.Reference.Name).FirstOrDefault();
                    if (name != null) type = MetricType.SizingMeasure;
                }
                // if metricId is not a sizing measure, perhaps a category
                if (name == null)
                {
                    name = snapshot.GetCategoryName(int.Parse(metricId));
                    if (name != null) type = MetricType.Category;
                }
                // if metricId is not a category, perhaps a background fact
                if (name == null)
                {
                    bfResult = reportData.SnapshotExplorer.GetBackgroundFacts(snapshot.Href, metricId, true, true).FirstOrDefault();
                    if (bfResult == null || !bfResult.ApplicationResults.Any()) return null;
                    name = bfResult.ApplicationResults[0].Reference.Name;
                    if (name != null) type = MetricType.BackgroundFact;
                }
                // we don't know what is this metric
                if (name == null) return null;

                switch (type)
                {
                    case MetricType.BusinessCriteria:
                        if (module == null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                                .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                        }
                        else if (module != null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        else if (module == null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                                .SelectMany(_ => _.TechnologyResult)
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        else if (module != null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        resStr = result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                        break;
                    case MetricType.TechnicalCriteria:
                        if (module == null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                                .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                        }
                        else if (module != null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        else if (module == null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                                .SelectMany(_ => _.TechnologyResult)
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        else if (module != null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        resStr = result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                        break;
                    case MetricType.QualityRule:
                        if (module == null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                                .Select(_ => _.DetailResult.Grade).FirstOrDefault();
                        }
                        else if (module != null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        else if (module == null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                                .SelectMany(_ => _.TechnologyResult)
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        else if (module != null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Grade;
                        }
                        resStr = result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");

                        break;
                    case MetricType.SizingMeasure:
                        if (module == null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId))
                                .Select(_ => _.DetailResult.Value).FirstOrDefault();
                        }
                        else if (module != null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.DetailResult != null)?.DetailResult.Value;
                        }
                        else if (module == null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.TechnologyResult != null)
                                .SelectMany(_ => _.TechnologyResult)
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Value;
                        }
                        else if (module != null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.SizingMeasuresResults?.Where(_ => _.Reference.Key == int.Parse(metricId) && _.ModulesResult != null)
                                .SelectMany(_ => _.ModulesResult)
                                .FirstOrDefault(_ => _.Module.Id == module.Id && _.TechnologyResults != null)?.TechnologyResults
                                .FirstOrDefault(_ => _.Technology == technology && _.DetailResult != null)?.DetailResult.Value;
                        }
                        resStr = format ? result?.ToString("N0") ?? FormatHelper.No_Value : result?.ToString() ?? "0";
                        break;
                    case MetricType.Category:
                        if (module == null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.GetCategoryValue(int.Parse(metricId));
                        }
                        else if (module != null && string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.GetModuleCategoryValue(int.Parse(metricId), module);
                        }
                        else if (module == null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.GetTechnoCategoryValue(int.Parse(metricId), technology);
                        }
                        else if (module != null && !string.IsNullOrEmpty(technology))
                        {
                            result = snapshot.GetModuleTechnoCategoryValue(int.Parse(metricId), module, technology);
                        }
                        resStr = format ? result?.ToString("N0") ?? FormatHelper.No_Value : result?.ToString() ?? "0";
                        break;
                    case MetricType.BackgroundFact:
                        if (module == null && string.IsNullOrEmpty(technology))
                        {
                            result = bfResult?.ApplicationResults[0].DetailResult.Value;
                        }
                        else if (module != null && string.IsNullOrEmpty(technology))
                        {
                            result = bfResult?.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id)?
                                .DetailResult.Value;
                        }
                        else if (module == null && !string.IsNullOrEmpty(technology))
                        {
                            result = bfResult?.ApplicationResults[0].TechnologyResult.FirstOrDefault(_ => _.Technology == technology)?
                                .DetailResult.Value;
                        }
                        else if (module != null && !string.IsNullOrEmpty(technology))
                        {
                            result = bfResult?.ApplicationResults[0].ModulesResult.FirstOrDefault(_ => _.Module.Id == module.Id)?
                                .TechnologyResults.FirstOrDefault(_ => _.Technology == technology)?
                                .DetailResult.Value;
                        }
                        resStr = format ? result?.ToString("N0") ?? FormatHelper.No_Value : result?.ToString() ?? "0";
                        break;
                    case MetricType.NotKnown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (MetricType.QualityRule == type)
                {
                    name = name + " (" + metricId + ")";
                }
                SimpleResult res = new SimpleResult { name = name, type = type, result = result, resultStr = resStr };
                return res;
            }
            catch (NullReferenceException e)
            {
                // for linux
                if (snapshot != null && metricId != null)
                {
                    LogHelper.LogInfo("No data for snapshot " + snapshot.ToString() + " and metric id " + metricId);
                }
                LogHelper.LogWarn(e.Message);
                return null;
            }
        }

        /// <summary>
        /// This method return the evolution of a metric between 2 snapshots results (name, type, current result, previous result, absolute evolution and percent evolution
        /// The metric id can be a BC, TC, QR, sizing or background fact measure
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="curSnapshot"></param>
        /// <param name="prevSnapshot"></param>
        /// <param name="metricId"></param>
        /// <param name="evol"></param>
        /// <param name="module"></param>
        /// <param name="technology"></param>
        /// <param name="format"></param> format is true for table component and false for graph component
        /// <returns></returns>
        public static EvolutionResult GetMetricEvolution(ImagingData reportData, Snapshot curSnapshot, Snapshot prevSnapshot, string metricId, bool evol, Module module, string technology, bool format)
        {
            SimpleResult curResult = null;
            SimpleResult prevResult = null;
            if (curSnapshot != null) curResult = GetMetricNameAndResult(reportData, curSnapshot, metricId, module, technology, format);
            if (curResult == null) return null;
            if (prevSnapshot != null) prevResult = GetMetricNameAndResult(reportData, prevSnapshot, metricId, module, technology, format);
            if (!evol && (curResult?.result != null || prevResult?.result != null))
            {
                string name = curResult?.name ?? prevResult?.name ?? FormatHelper.No_Value;
                MetricType type = curResult?.type ?? prevResult.type;
                string curRes = format ? FormatHelper.No_Value : "0";
                string prevRes = format ? FormatHelper.No_Value : "0";
                switch (type)
                {
                    case MetricType.BusinessCriteria:
                    case MetricType.TechnicalCriteria:
                    case MetricType.QualityRule:
                        curRes = curResult?.result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                        prevRes = prevResult?.result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                        break;
                    case MetricType.SizingMeasure:
                    case MetricType.Category:
                    case MetricType.BackgroundFact:
                        if (format)
                        {
                            curRes = curResult?.result?.ToString("N0") ?? FormatHelper.No_Value;
                            prevRes = prevResult?.result?.ToString("N0") ?? FormatHelper.No_Value;
                        }
                        else
                        {
                            curRes = curResult?.result?.ToString() ?? "0";
                            prevRes = prevResult?.result?.ToString() ?? "0";

                        }
                        break;
                    case MetricType.NotKnown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return new EvolutionResult
                {
                    name = name,
                    type = type,
                    curResult = curRes,
                    prevResult = prevRes,
                    evolution = format ? FormatHelper.No_Value : "0",
                    evolutionPercent = format ? FormatHelper.No_Value : "0"
                };
            }

            if (curResult?.result == null || prevResult?.result == null)
            {
                string name = curResult?.name ?? prevResult?.name ?? FormatHelper.No_Value;
                MetricType type = curResult?.type ?? prevResult?.type ?? MetricType.NotKnown;
                string curRes = format ? FormatHelper.No_Value : "0";
                string prevRes = format ? FormatHelper.No_Value : "0";
                switch (type)
                {
                    case MetricType.BusinessCriteria:
                    case MetricType.TechnicalCriteria:
                    case MetricType.QualityRule:
                        curRes = curResult?.result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                        prevRes = prevResult?.result?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                        break;
                    case MetricType.SizingMeasure:
                    case MetricType.Category:
                    case MetricType.BackgroundFact:
                        if (format)
                        {
                            curRes = curResult?.result?.ToString("N0") ?? FormatHelper.No_Value;
                            prevRes = prevResult?.result?.ToString("N0") ?? FormatHelper.No_Value;
                        }
                        else
                        {
                            curRes = curResult?.result?.ToString() ?? "0";
                            prevRes = prevResult?.result?.ToString() ?? "0";
                        }
                        break;
                    case MetricType.NotKnown:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new EvolutionResult
                {
                    name = name,
                    type = type,
                    curResult = curRes,
                    prevResult = prevRes,
                    evolution = format ? FormatHelper.No_Value : "0",
                    evolutionPercent = format ? FormatHelper.No_Value : "0"
                };

            }

            string evolution;
            string evolPercent;
            string finalCurRes;
            string finalPrevRes;
            double? evp;
            switch (curResult.type)
            {
                case MetricType.BusinessCriteria:
                case MetricType.TechnicalCriteria:
                case MetricType.QualityRule:
                    if (curResult.result != null && prevResult.result != null)
                    {
                        finalCurRes = curResult.result.Value.ToString("N2");
                        finalPrevRes = prevResult.result.Value.ToString("N2");
                        evolution = (curResult.result - prevResult.result).Value.ToString("N2");
                        evp = Math.Abs((double)prevResult.result) > 0.0 ? (curResult.result - prevResult.result) / prevResult.result : null;
                        evolPercent = evp != null ? evp.FormatPercent() : format ? FormatHelper.No_Value : "0";
                    }
                    else
                    {
                        finalCurRes = format ? FormatHelper.No_Value : "0";
                        finalPrevRes = format ? FormatHelper.No_Value : "0";
                        evolution = format ? FormatHelper.No_Value : "0";
                        evolPercent = format ? FormatHelper.No_Value : "0";
                    }
                    break;
                case MetricType.SizingMeasure:
                case MetricType.Category:
                case MetricType.BackgroundFact:
                    if (curResult.result != null && prevResult.result != null)
                    {
                        if (format)
                        {
                            finalCurRes = curResult.result.Value.ToString("N0");
                            finalPrevRes = prevResult.result.Value.ToString("N0");
                            evolution = (curResult.result - prevResult.result).Value.ToString("N0");
                        }
                        else
                        {
                            finalCurRes = curResult.result.ToString();
                            finalPrevRes = prevResult.result.ToString();
                            evolution = (curResult.result - prevResult.result).ToString();
                        }
                        evp = Math.Abs((double)prevResult.result) > 0.0 ? (curResult.result - prevResult.result) / prevResult.result : null;
                        evolPercent = evp != null ? evp.FormatPercent() : format ? FormatHelper.No_Value : "0";
                    }
                    else
                    {
                        finalCurRes = format ? FormatHelper.No_Value : "0";
                        finalPrevRes = format ? FormatHelper.No_Value : "0";
                        evolution = format ? FormatHelper.No_Value : "0";
                        evolPercent = format ? FormatHelper.No_Value : "0";
                    }
                    break;
                case MetricType.NotKnown:
                    finalCurRes = format ? FormatHelper.No_Value : "0";
                    finalPrevRes = format ? FormatHelper.No_Value : "0";
                    evolution = format ? FormatHelper.No_Value : "0";
                    evolPercent = format ? FormatHelper.No_Value : "0";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new EvolutionResult
            {
                name = curResult.name,
                type = curResult.type,
                curResult = finalCurRes,
                prevResult = finalPrevRes,
                evolution = evolution,
                evolutionPercent = evolPercent
            };
        }

        public static SimpleResult GetAggregatedMetric(ImagingData reportData, Dictionary<Application, Snapshot> lastSnapshotList, string metricId, string techno, string aggregator, bool format)
        {

            List<SimpleResult> results = new List<SimpleResult>();
            foreach (Application application in reportData.Applications)
            {
                Snapshot appCurSnap;
                try
                {
                    appCurSnap = lastSnapshotList[application];
                }
                catch (KeyNotFoundException)
                {
                    appCurSnap = null;
                }
                if (appCurSnap != null)
                {
                    SimpleResult appRes = GetMetricNameAndResult(reportData, appCurSnap, metricId, null, techno, format);
                    if (appRes != null)
                    {
                        results.Add(appRes);
                    }
                }
            }

            string metName = results.FirstOrDefault()?.name;
            if (metName == null) return null;

            MetricType metType = results.FirstOrDefault()?.type ?? MetricType.NotKnown;

            double? curResult = 0;

            if (string.IsNullOrEmpty(aggregator))
            {
                // ReSharper disable once SwitchStatementMissingSomeCases default case is managed in next switch
                switch (metType)
                {
                    case MetricType.QualityRule:
                    case MetricType.TechnicalCriteria:
                    case MetricType.BusinessCriteria:
                        aggregator = "AVERAGE";
                        break;
                    case MetricType.SizingMeasure:
                    case MetricType.Category:
                    case MetricType.BackgroundFact:
                        aggregator = "SUM";
                        break;
                }
            }

            switch (aggregator)
            {
                case "SUM":
                    curResult = results.Aggregate(curResult, (current, result) => result.result != null ? current + result.result : current);
                    break;
                case "AVERAGE":
                    int nbCurRes = 0;
                    foreach (var _result in results)
                    {
                        if (_result.result == null) continue;
                        curResult = curResult + _result.result;
                        nbCurRes++;
                    }
                    curResult = nbCurRes != 0 ? curResult / nbCurRes : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // format curResult and prevResult in case of metricType
            string res;
            switch (metType)
            {
                case MetricType.BusinessCriteria:
                case MetricType.TechnicalCriteria:
                case MetricType.QualityRule:
                    res = curResult?.ToString("N2") ?? (format ? FormatHelper.No_Value : "0");
                    break;
                case MetricType.SizingMeasure:
                case MetricType.Category:
                case MetricType.BackgroundFact:
                    res = format ? curResult?.ToString("N0") ?? FormatHelper.No_Value : curResult?.ToString() ?? "0";
                    break;
                case MetricType.NotKnown:
                    res = curResult?.ToString() ?? (format ? FormatHelper.No_Value : "0");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new SimpleResult
            {
                name = metName,
                type = metType,
                result = curResult,
                resultStr = res
            };
        }

        public static List<string> BuildRulesList(ImagingData reportData, List<string> metrics, bool critical)
        {
            return BuildRulesList(reportData, metrics, critical, false);
        }

        public static List<string> BuildRulesList(ImagingData reportData, List<string> metrics, bool critical, bool omg)
        {
            List<string> qualityRules = new List<string>();

            foreach (string _metric in metrics)
            {
                // If metric can not be parsed as integer, this is potentially a string containing a standard tag for quality rule selection
                if (!int.TryParse(_metric, out int _))
                {
                    // From 8.3.21 and new index extensions, a quality standard can be a BC or TC (as shortName)
                    Snapshot snapshot = reportData.CurrentSnapshot;
                    int? metricBcIdFromShortName = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Name == _metric).Select(_ => _.Reference.Key).FirstOrDefault();
                    if (metricBcIdFromShortName != null && metricBcIdFromShortName != 0)
                    {
                        List<RuleDetails> rules = reportData.RuleExplorer.GetRulesDetails(snapshot.DomainId, metricBcIdFromShortName.Value, snapshot.Id).ToList();
                        if (omg)
                        {
                            qualityRules.AddRange(rules.Where(_ =>
                            reportData.CurrentSnapshot.QualityRulesResults.Where(qr => qr.Reference.Key == _.Key && qr.DetailResult.OmgTechnicalDebt != null) != null)
                                .Select(_ => _.Key.ToString()).ToList());
                        }
                        else
                        {
                            qualityRules.AddRange(critical ? rules.Where(_ => _.Critical).Select(_ => _.Key.ToString()).ToList() : rules.Select(_ => _.Key.ToString()).ToList());
                        }
                    }
                    else
                    {
                        int? metricTcIdFromShortName = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.ShortName == _metric).Select(_ => _.Reference.Key).FirstOrDefault();
                        if (metricTcIdFromShortName != null && metricTcIdFromShortName != 0)
                        {
                            List<Contributor> rules = reportData.RuleExplorer.GetCriteriaContributors(snapshot.DomainId, metricTcIdFromShortName.ToString(), snapshot.Id).ToList();
                            if (omg)
                            {
                                qualityRules.AddRange(rules.Where(_ =>
                                reportData.CurrentSnapshot.QualityRulesResults.Where(qr => qr.Reference.Key == _.Key && qr.DetailResult.OmgTechnicalDebt != null) != null)
                                    .Select(_ => _.Key.ToString()).ToList());
                            }
                            else
                            {
                                qualityRules.AddRange(critical ? rules.Where(_ => _.Critical).Select(_ => _.Key.ToString()).ToList() : rules.Select(_ => _.Key.ToString()).ToList());
                            }
                        }
                        else
                        {
                            List<string> stdTagMetrics = reportData.SnapshotExplorer.GetQualityStandardsRulesList(reportData.CurrentSnapshot.Href, _metric);
                            if (stdTagMetrics != null) qualityRules.AddRange(stdTagMetrics);
                        }
                    }
                }
                else
                {
                    Snapshot snapshot = reportData.CurrentSnapshot;
                    int metricId = int.Parse(_metric);
                    string name = snapshot.BusinessCriteriaResults?.Where(_ => _.Reference.Key == metricId).Select(_ => _.Reference.Name).FirstOrDefault();

                    if (name != null)
                    {
                        // This is a Business criteria
                        List<RuleDetails> rules = reportData.RuleExplorer.GetRulesDetails(snapshot.DomainId, metricId, snapshot.Id).ToList();
                        qualityRules.AddRange(critical ? rules.Where(_ => _.Critical).Select(_ => _.Key.ToString()).ToList() : rules.Select(_ => _.Key.ToString()).ToList());
                    }
                    else
                    {
                        // if metricId is not a Business Criteria
                        name = snapshot.TechnicalCriteriaResults?.Where(_ => _.Reference.Key == metricId).Select(_ => _.Reference.Name).FirstOrDefault();
                        if (name != null)
                        {
                            // This is a Technical criteria
                            List<Contributor> rules = reportData.RuleExplorer.GetCriteriaContributors(snapshot.DomainId, _metric, snapshot.Id).ToList();
                            qualityRules.AddRange(critical ? rules.Where(_ => _.Critical).Select(_ => _.Key.ToString()).ToList() : rules.Select(_ => _.Key.ToString()));
                        }
                        else
                        {
                            // if metricId is not a Technical Criteria
                            name = snapshot.QualityRulesResults?.Where(_ => _.Reference.Key == metricId).Select(_ => _.Reference.Name).FirstOrDefault();
                            if (name != null)
                            {
                                qualityRules.Add(_metric);
                            }
                        }

                    }

                }
            }

            return qualityRules.Distinct().ToList();
        }

        public class ViolationsBookmarksProperties
        {
            public Violation[] Violations { get; }
            public int ViolationCounter { get; }
            public List<string> RowData { get; }
            public string RuleName { get; }
            public bool HasPreviousSnapshot { get; }
            public string DomainId { get; }
            public string SnapshotId { get; }
            public string Metric { get; }

            public ViolationsBookmarksProperties(Violation[] violations, int violationCounter, List<string> rowData, string ruleName, bool hasPreviousSnapshot, string domainId, string snapshotId, string metric)
            {
                Violations = violations;
                ViolationCounter = violationCounter;
                RowData = rowData;
                RuleName = ruleName;
                HasPreviousSnapshot = hasPreviousSnapshot;
                DomainId = domainId;
                SnapshotId = snapshotId;
                Metric = metric;
            }
        }

        public static int PopulateViolationsBookmarks(ImagingData reportData, ViolationsBookmarksProperties violationsBookmarksProperties, int cellidx, List<CellAttributes> cellProps, bool withCodeLines)
        {
            Violation[] violations = violationsBookmarksProperties.Violations;
            int violationCounter = violationsBookmarksProperties.ViolationCounter;
            List<string> rowData = violationsBookmarksProperties.RowData;
            string ruleName = violationsBookmarksProperties.RuleName;
            bool hasPreviousSnapshot = violationsBookmarksProperties.HasPreviousSnapshot;
            string domainId = violationsBookmarksProperties.DomainId;
            string snapshotId = violationsBookmarksProperties.SnapshotId;
            string metric = violationsBookmarksProperties.Metric;
            foreach (Violation _violation in violations)
            {
                // rule name is empty if violations comes from action plan (where every action can have a different rule
                string ruleStr = (ruleName != "actionPlan" && ruleName != "actionPlanPriority") ? ruleName : _violation.RulePattern.Name;
                string key;
                if (ruleName == "actionPlan" || ruleName == "actionPlanPriority")
                {
                    string[] fragments = _violation.RulePattern.Href.Split('/');
                    key = fragments[fragments.Length - 1];
                }
                else
                {
                    key = metric;
                }
                violationCounter++;
                rowData.Add("");
                cellidx++;
                rowData.Add($"{Labels.Violation} #{violationCounter}    {ruleStr}");
                cellProps.Add(new CellAttributes(cellidx, ColorGainsboro));
                cellidx++;
                rowData.Add($"{Labels.ObjectName}: {_violation.Component.Name}");
                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                cellidx++;

                TypedComponent objectComponent = reportData.SnapshotExplorer.GetTypedComponent(reportData.CurrentSnapshot.DomainId, _violation.Component.GetComponentId(), reportData.CurrentSnapshot.GetId());
                if (objectComponent != null)
                {
                    rowData.Add($"{Labels.IFPUG_ObjectType}: {objectComponent.Type.Label}");
                }
                else
                {
                    rowData.Add("");
                }
                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                cellidx++;

                if (hasPreviousSnapshot)
                {
                    // if ruleName is empty, this is a violation in action plan
                    string status = !string.IsNullOrEmpty(ruleName) ? _violation.Diagnosis.Status : _violation.RemedialAction.Status;
                    rowData.Add($"{Labels.Status}: {status}");
                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                    cellidx++;
                }
                if (ruleName == "actionPlan")
                {
                    rowData.Add($"{Labels.Priority}: {_violation.RemedialAction.Tag}");
                    cellidx++;
                }
                else if (ruleName == "actionPlanPriority")
                {
                    rowData.Add($"{Labels.Priority}: {_violation.RemedialAction.Priority}");
                    cellidx++;
                }
                AssociatedValue associatedValue = reportData.SnapshotExplorer.GetAssociatedValue(domainId, _violation.Component.GetComponentId(), snapshotId, key);
                if (associatedValue == null) continue;

                if (associatedValue.Type == null || associatedValue.Type.Equals("integer"))
                {
                    if (associatedValue.Values != null && associatedValue.Values.Length > 0)
                    {
                        var value = associatedValue.Values[0];
                        rowData.Add($"{Labels.AssociatedValue}: {value}");
                        cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                        cellidx++;
                    }
                    IEnumerable<IEnumerable<CodeBookmark>> bookmarks = associatedValue.Bookmarks;
                    if (bookmarks == null || !bookmarks.Any())
                    {
                        cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation, withCodeLines);
                    }
                    else
                    {
                        foreach (IEnumerable<CodeBookmark> _codeBookmarks in bookmarks)
                        {
                            IEnumerable<CodeBookmark> _bookmarks = _codeBookmarks.ToList();
                            foreach (CodeBookmark _bookmark in _bookmarks)
                            {
                                rowData.Add($"{Labels.FilePath}: {_bookmark.CodeFragment.CodeFile.Name}");
                                cellProps.Add(new CellAttributes(cellidx, ColorLavander));
                                cellidx++;
                                if (withCodeLines)
                                {
                                    Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookmark, 3);
                                    if (codeLines == null) continue;
                                    foreach (KeyValuePair<int, string> codeLine in codeLines)
                                    {
                                        rowData.Add($"{codeLine.Key} : {codeLine.Value}");
                                        cellProps.Add(codeLine.Key >= _bookmark.CodeFragment.StartLine && codeLine.Key <= _bookmark.CodeFragment.EndLine
                                            ? new CellAttributes(cellidx, ColorLightYellow)
                                            : new CellAttributes(cellidx, ColorWhite));
                                        cellidx++;
                                    }
                                }
                                else
                                {
                                    rowData.Add($"{Labels.StartLine} : {_bookmark.CodeFragment.StartLine}");
                                    cellidx++;
                                    rowData.Add($"{Labels.EndLine} : {_bookmark.CodeFragment.EndLine}");
                                    cellidx++;
                                }
                            }
                        }
                    }
                }

                if (associatedValue.Type != null && associatedValue.Type.Equals("path"))
                {
                    // manage case when type="path" and values contains the different path
                    AssociatedValuePath associatedValueEx = reportData.SnapshotExplorer.GetAssociatedValuePath(domainId, _violation.Component.GetComponentId(), snapshotId, key);
                    IEnumerable<IEnumerable<CodeBookmark>> values = associatedValueEx?.Values;
                    if (values == null || !values.Any())
                    {
                        cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation, withCodeLines);
                    }
                    else
                    {
                        int pathCounter = 0;
                        foreach (IEnumerable<CodeBookmark> _value in values)
                        {
                            pathCounter++;
                            IEnumerable<CodeBookmark> _bookmarksValue = _value.ToList();
                            rowData.Add($"{Labels.ViolationPath} #{pathCounter}");
                            cellProps.Add(new CellAttributes(cellidx, ColorLavander));
                            cellidx++;
                            string previousFile = string.Empty;
                            foreach (CodeBookmark _bookval in _bookmarksValue)
                            {
                                if (string.IsNullOrEmpty(previousFile) || !previousFile.Equals(_bookval.CodeFragment.CodeFile.Name))
                                {
                                    previousFile = _bookval.CodeFragment.CodeFile.Name;
                                    rowData.Add($"{Labels.FilePath}: {_bookval.CodeFragment.CodeFile.Name}");
                                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                                    cellidx++;
                                }

                                if (withCodeLines)
                                {
                                    Dictionary<int, string> codeLines = reportData.SnapshotExplorer.GetSourceCodeBookmark(domainId, _bookval, 0);
                                    if (codeLines == null) continue;
                                    foreach (KeyValuePair<int, string> codeLine in codeLines)
                                    {
                                        rowData.Add($"{codeLine.Key} : {codeLine.Value}");
                                        cellProps.Add(codeLine.Key == _bookval.CodeFragment.StartLine
                                            ? new CellAttributes(cellidx, ColorLightYellow)
                                            : new CellAttributes(cellidx, ColorWhite));
                                        cellidx++;
                                    }
                                }
                                else
                                {
                                    rowData.Add($"{Labels.StartLine} : {_bookval.CodeFragment.StartLine}");
                                    cellidx++;
                                    rowData.Add($"{Labels.EndLine} : {_bookval.CodeFragment.EndLine}");
                                    cellidx++;
                                }
                            }
                            rowData.Add("");
                            cellidx++;
                        }
                    }
                }
                if (associatedValue.Type != null && associatedValue.Type.Equals("group"))
                {
                    // manage case when type="group" and values contains an array of array of components
                    AssociatedValueGroup associatedValueEx = reportData.SnapshotExplorer.GetAssociatedValueGroup(domainId, _violation.Component.GetComponentId(), snapshotId, key);
                    IEnumerable<IEnumerable<CodeBookmark>> values = associatedValueEx?.Values;
                    if (values == null || !values.Any())
                    {
                        cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation, withCodeLines);
                    }
                    else
                    {
                        foreach (IEnumerable<CodeBookmark> components in values)
                        {
                            foreach (CodeBookmark _component in components)
                            {
                                rowData.Add(_component.Component.Name);
                                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                                cellidx++;
                            }
                            rowData.Add("");
                            cellidx++;
                        }
                    }
                }
                if (associatedValue.Type != null && (associatedValue.Type.Equals("object") || associatedValue.Type.Equals("text") || associatedValue.Type.Equals("percentage")))
                {
                    // manage case when type="object", "text" or "percentage"
                    cellidx = AddSourceCode(reportData, rowData, cellidx, cellProps, domainId, snapshotId, _violation, withCodeLines);
                }

            }
            return cellidx;
        }

        private static int AddSourceCode(ImagingData reportData, List<string> rowData, int cellidx, List<CellAttributes> cellProps, string domainId, string snapshotId, Violation violation, bool withCodeLines)
        {
            List<Tuple<string, Dictionary<int, string>>> codes = reportData.SnapshotExplorer.GetSourceCode(domainId, snapshotId, violation.Component.GetComponentId(), 6, withCodeLines);
            if (codes == null) return cellidx;

            foreach (Tuple<string, Dictionary<int, string>> _code in codes)
            {
                rowData.Add($"{Labels.FilePath}: {_code.Item1}");
                cellProps.Add(new CellAttributes(cellidx, ColorLavander));
                cellidx++;

                Dictionary<int, string> codeLines = _code.Item2;
                if (codeLines == null) return cellidx;
                foreach (KeyValuePair<int, string> codeLine in codeLines)
                {
                    rowData.Add($"{codeLine.Key} : {codeLine.Value}");
                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                    cellidx++;
                }
            }

            return cellidx;
        }

        public static string GetPropertyName(string prop)
        {
            switch (prop)
            {
                case "codeLines":
                    return Labels.codeLines;
                case "commentedCodeLines":
                    return Labels.commentedCodeLines;
                case "commentLines":
                    return Labels.commentLines;
                case "coupling":
                    return Labels.coupling;
                case "fanIn":
                    return Labels.fanIn;
                case "fanOut":
                    return Labels.fanOut;
                case "cyclomaticComplexity":
                    return Labels.cyclomaticComplexity;
                case "ratioCommentLinesCodeLines":
                    return Labels.ratioCommentLinesCodeLines;
                case "halsteadProgramLength":
                    return Labels.halsteadProgramLength;
                case "halsteadProgramVocabulary":
                    return Labels.halsteadProgramVocabulary;
                case "halsteadVolume":
                    return Labels.halsteadVolume;
                case "distinctOperators":
                    return Labels.distinctOperators;
                case "distinctOperands":
                    return Labels.distinctOperands;
                case "integrationComplexity":
                    return Labels.integrationComplexity;
                case "essentialComplexity":
                    return Labels.essentialComplexity;
                default:
                    return string.Empty;
            }
        }


        // for sending in parameters to the script. It is mandatory for that class to be public. If private all tests failed and script cannot be executed.
        public class Globals
        {
            // improbable name to avoid conflicts when replacing parameters
            public dynamic _ { get; set; }
        }

        private static ScriptState<object> ExecuteScript(ImagingData reportData, Dictionary<string, string> options, string[] lstParams, Snapshot snapshot, string expr, Module module, string technology)
        {
            dynamic expando = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;

            for (int i = 0; i < lstParams.Length; i += 2)
            {
                string param = lstParams[i + 1];
                expr = expr.Replace(param, "_." + param);

                string _id = options.GetOption(lstParams[i + 1], "0");
                if (string.IsNullOrEmpty(_id))
                    continue;

                double? _value = GetMetricNameAndResult(reportData, snapshot, _id, module, technology, true)?.result;
                if (_value == null) continue;

                dictionary.Add(param, _value);
            }

            // setup references needed
            var refs = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.CompilerServices.DynamicAttribute).GetTypeInfo().Assembly.Location)
            };
            var script = CSharpScript.Create(expr, options: ScriptOptions.Default.AddReferences(refs), globalsType: typeof(Globals));
            script.Compile();

            // create new global that will contain the data we want to send into the script
            var g = new Globals() { _ = expando };

            //Execute and return
            return script.RunAsync(g).Result;
        }

        public static string CustomExpressionEvaluation(ImagingData reportData, Dictionary<string, string> options, string[] lstParams, Snapshot snapshot, string expr, string metricFormat, Module module, string technology, bool portfolio = false)
        {
            try
            {
                var r = ExecuteScript(reportData, options, lstParams, snapshot, expr, module, technology);
                double? res = (double)r.ReturnValue;
                if (metricFormat.Equals("graph"))
                {
                    return res.Value.Equals(double.NaN) ? "0" : res.Value.ToString();
                }
                return res.Value.Equals(double.NaN) ? Labels.NoData : res.Value.ToString(metricFormat);
            }
            catch (Exception ex) when (ex is CompilationErrorException || ex is ArgumentException || ex is AggregateException)
            {
                if (portfolio) return null;
                LogHelper.LogError("Expression cannot be evaluate : " + ex.Message);
                return metricFormat.Equals("graph") ? "0" : Labels.NoData;
            }
        }

        public static double? CustomExpressionDoubleEvaluation(ImagingData reportData, Dictionary<string, string> options, string[] lstParams, Snapshot snapshot, string expr, Module module, string technology)
        {
            try
            {
                var r = ExecuteScript(reportData, options, lstParams, snapshot, expr, module, technology);
                return (double)r.ReturnValue;
            }
            catch (Exception ex) when (ex is CompilationErrorException || ex is ArgumentException || ex is AggregateException)
            {
                LogHelper.LogError("Expression cannot be evaluate : " + ex.Message);
                return null;
            }
        }

        public static string CustomExpressionEvaluationAggregated(ImagingData reportData, Dictionary<string, string> options, string[] lstParams, List<Snapshot> snapshots, string expr, string metricFormat, Module module, string technology, string aggregator)
        {
            List<double?> results = snapshots.Select(snapshot => CustomExpressionDoubleEvaluation(reportData, options, lstParams, snapshot, expr, module, technology)).ToList();
            double? res = AggregateValues(aggregator, results);
            if (metricFormat.Equals("graph"))
            {
                return res == null ? "0" : res.ToString();
            }
            return res?.ToString(metricFormat) ?? Labels.NoData;
        }

        public static double? AggregateValues(string aggregator, List<double?> values)
        {
            double? res = 0;
            switch (aggregator)
            {
                case "SUM":
                    res = values.Aggregate(res, (current, result) => result.HasValue ? current + result.Value : current);
                    break;
                case "AVERAGE":
                    int nbCurRes = 0;
                    foreach (var _result in values)
                    {
                        if (!_result.HasValue) continue;
                        res = res + _result.Value;
                        nbCurRes++;
                    }
                    res = nbCurRes != 0 ? res / nbCurRes : null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return res;
        }

        public static List<string> PopulateViolationsBookmarksRow(ImagingData reportData, List<Violation> results, HeaderDefinition headers, string ruleId)
        {
            List<string> rowData = new List<string>();
            string metric = ruleId;
            string ruleName = results.FirstOrDefault()?.RulePattern.Name;
            if (ruleName == null) return rowData;

            bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
            string domainId = reportData.CurrentSnapshot.DomainId;
            string snapshotId = reportData.CurrentSnapshot.Id.ToString();
            string status = string.Empty;
            string assoValue = string.Empty;
            foreach (Violation _violation in results)
            {
                if (hasPreviousSnapshot)
                {
                    status = (ruleId != "actionPlan" && ruleId != "actionPlanPriority") ? _violation.Diagnosis.Status : _violation.RemedialAction.Status;
                }

                if (ruleId == "actionPlan" || ruleId == "actionPlanPriority")
                {
                    // case of violations in action plan
                    ruleName = _violation.RulePattern.Name;
                    string[] fragments = _violation.RulePattern.Href.Split('/');
                    metric = fragments[fragments.Length - 1];
                }
                TypedComponent objectComponent = reportData.SnapshotExplorer.GetTypedComponent(reportData.CurrentSnapshot.DomainId, _violation.Component.GetComponentId(), reportData.CurrentSnapshot.GetId());

                AssociatedValue associatedValue = reportData.SnapshotExplorer.GetAssociatedValue(domainId, _violation.Component.GetComponentId(), snapshotId, metric);
                if (associatedValue == null)
                {
                    var _row = headers.CreateDataRow();
                    _row.Set(Labels.RuleName, ruleName);
                    _row.Set(Labels.ObjectName, _violation.Component.Name);
                    _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                    _row.Set(Labels.Status, status);
                    if (ruleId == "actionPlan")
                    {
                        _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                    }
                    else if (ruleId == ("actionPlanPriority"))
                    {
                        _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                    }
                    _row.Set(Labels.AssociatedValue, assoValue);
                    _row.Set(Labels.FilePath, string.Empty);
                    _row.Set(Labels.StartLine, string.Empty);
                    _row.Set(Labels.EndLine, string.Empty);
                    rowData.AddRange(_row);
                }
                else
                {
                    string st = status;

                    if (associatedValue.Type == null || associatedValue.Type.Equals("integer"))
                    {
                        if (associatedValue.Values != null && associatedValue.Values.Length > 0)
                        {
                            assoValue = associatedValue.Values[0].ToString();
                        }

                        string av = assoValue;

                        IEnumerable<IEnumerable<CodeBookmark>> bookmarks = associatedValue.Bookmarks;
                        if (bookmarks == null || !bookmarks.Any())
                        {
                            List<Tuple<string, int, int>> paths = reportData.SnapshotExplorer.GetComponentFilePath(domainId, _violation.Component.GetComponentId(), snapshotId);
                            if (paths.Count == 0)
                            {
                                var _row = headers.CreateDataRow();
                                _row.Set(Labels.RuleName, ruleName);
                                _row.Set(Labels.ObjectName, _violation.Component.Name);
                                _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                _row.Set(Labels.Status, status);
                                if (ruleId == "actionPlan")
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                }
                                else if (ruleId == ("actionPlanPriority"))
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                }
                                _row.Set(Labels.AssociatedValue, assoValue);
                                _row.Set(Labels.FilePath, string.Empty);
                                _row.Set(Labels.StartLine, string.Empty);
                                _row.Set(Labels.EndLine, string.Empty);
                                rowData.AddRange(_row);
                            }
                            paths.ForEach(_path =>
                            {
                                var _row = headers.CreateDataRow();
                                _row.Set(Labels.RuleName, ruleName);
                                _row.Set(Labels.ObjectName, _violation.Component.Name);
                                _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                _row.Set(Labels.Status, st);
                                if (ruleId == "actionPlan")
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                }
                                else if (ruleId == ("actionPlanPriority"))
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                }
                                _row.Set(Labels.AssociatedValue, av);
                                _row.Set(Labels.FilePath, _path.Item1);
                                _row.Set(Labels.StartLine, _path.Item2.ToString());
                                _row.Set(Labels.EndLine, _path.Item3.ToString());
                                rowData.AddRange(_row);
                            });
                        }
                        else
                        {
                            foreach (IEnumerable<CodeBookmark> _codeBookmarks in bookmarks)
                            {
                                _codeBookmarks.ToList().ForEach(_ =>
                                {
                                    var _row = headers.CreateDataRow();
                                    _row.Set(Labels.RuleName, ruleName);
                                    _row.Set(Labels.ObjectName, _violation.Component.Name);
                                    _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                    _row.Set(Labels.Status, st);
                                    if (ruleId == "actionPlan")
                                    {
                                        _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                    }
                                    else if (ruleId == ("actionPlanPriority"))
                                    {
                                        _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                    }
                                    _row.Set(Labels.AssociatedValue, av);
                                    _row.Set(Labels.FilePath, _.CodeFragment?.CodeFile.Name);
                                    _row.Set(Labels.StartLine, _.CodeFragment?.StartLine.ToString());
                                    _row.Set(Labels.EndLine, _.CodeFragment?.EndLine.ToString());
                                    rowData.AddRange(_row);
                                });
                            }
                        }
                    }

                    if (associatedValue.Type != null && associatedValue.Type.Equals("path"))
                    {
                        // manage case when type="path" and values contains the different path
                        AssociatedValuePath associatedValueEx = reportData.SnapshotExplorer.GetAssociatedValuePath(domainId, _violation.Component.GetComponentId(), snapshotId, metric);
                        IEnumerable<IEnumerable<CodeBookmark>> values = associatedValueEx?.Values;

                        if (values == null || !values.Any())
                        {
                            List<Tuple<string, int, int>> paths = reportData.SnapshotExplorer.GetComponentFilePath(domainId, _violation.Component.GetComponentId(), snapshotId);
                            paths.ForEach(_path =>
                            {
                                var _row = headers.CreateDataRow();
                                _row.Set(Labels.RuleName, ruleName);
                                _row.Set(Labels.ObjectName, _violation.Component.Name);
                                _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                _row.Set(Labels.Status, st);
                                if (ruleId == "actionPlan")
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                }
                                else if (ruleId == ("actionPlanPriority"))
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                }
                                _row.Set(Labels.AssociatedValue, string.Empty);
                                _row.Set(Labels.FilePath, _path.Item1);
                                _row.Set(Labels.StartLine, _path.Item2.ToString());
                                _row.Set(Labels.EndLine, _path.Item3.ToString());
                                rowData.AddRange(_row);
                            });
                        }
                        else
                        {
                            foreach (IEnumerable<CodeBookmark> _value in values)
                            {
                                List<CodeBookmark> bookList = _value.ToList();
                                bookList.ForEach(_bookval =>
                                {
                                    var _row = headers.CreateDataRow();
                                    _row.Set(Labels.RuleName, ruleName);
                                    _row.Set(Labels.ObjectName, _violation.Component.Name);
                                    _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                    _row.Set(Labels.Status, st);
                                    if (ruleId == "actionPlan")
                                    {
                                        _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                    }
                                    else if (ruleId == ("actionPlanPriority"))
                                    {
                                        _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                    }
                                    _row.Set(Labels.AssociatedValue, $"path #{bookList.IndexOf(_bookval)}");
                                    _row.Set(Labels.FilePath, _bookval.CodeFragment?.CodeFile.Name);
                                    _row.Set(Labels.StartLine, _bookval.CodeFragment?.StartLine.ToString());
                                    _row.Set(Labels.EndLine, _bookval.CodeFragment?.EndLine.ToString());
                                    rowData.AddRange(_row);
                                });
                            }
                        }
                    }
                    if (associatedValue.Type != null && associatedValue.Type.Equals("group"))
                    {
                        // manage case when type="group" and values contains an array of array of components
                        AssociatedValueGroup associatedValueEx = reportData.SnapshotExplorer.GetAssociatedValueGroup(domainId, _violation.Component.GetComponentId(), snapshotId, metric);
                        IEnumerable<IEnumerable<CodeBookmark>> values = associatedValueEx?.Values;
                        if (values == null || !values.Any())
                        {
                            List<Tuple<string, int, int>> paths = reportData.SnapshotExplorer.GetComponentFilePath(domainId, _violation.Component.GetComponentId(), snapshotId);
                            paths.ForEach(_ =>
                            {
                                var _row = headers.CreateDataRow();
                                _row.Set(Labels.RuleName, ruleName);
                                _row.Set(Labels.ObjectName, _violation.Component.Name);
                                _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                _row.Set(Labels.Status, st);
                                if (ruleId == "actionPlan")
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                }
                                else if (ruleId == ("actionPlanPriority"))
                                {
                                    _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                }
                                _row.Set(Labels.AssociatedValue, string.Empty);
                                _row.Set(Labels.FilePath, _.Item1);
                                _row.Set(Labels.StartLine, _.Item2.ToString());
                                _row.Set(Labels.EndLine, _.Item3.ToString());
                                rowData.AddRange(_row);
                            });
                        }
                        else
                        {
                            foreach (IEnumerable<CodeBookmark> components in values)
                            {
                                components.ToList().ForEach(_component =>
                                {
                                    var _row = headers.CreateDataRow();
                                    _row.Set(Labels.RuleName, ruleName);
                                    _row.Set(Labels.ObjectName, _violation.Component.Name);
                                    _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                                    _row.Set(Labels.Status, st);
                                    if (ruleId == "actionPlan")
                                    {
                                        _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                                    }
                                    else if (ruleId == ("actionPlanPriority"))
                                    {
                                        _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                                    }
                                    _row.Set(Labels.AssociatedValue, _component.Component.Name);
                                    _row.Set(Labels.FilePath, _component.CodeFragment?.CodeFile.Name);
                                    _row.Set(Labels.StartLine, _component.CodeFragment?.StartLine.ToString());
                                    _row.Set(Labels.EndLine, _component.CodeFragment?.EndLine.ToString());
                                });
                            }
                        }
                    }
                    if (associatedValue.Type != null && (associatedValue.Type.Equals("object") || associatedValue.Type.Equals("text")))
                    {
                        // manage case when type="object", "text"
                        List<Tuple<string, int, int>> paths = reportData.SnapshotExplorer.GetComponentFilePath(domainId, _violation.Component.GetComponentId(), snapshotId);
                        paths.ForEach(_ =>
                        {
                            var _row = headers.CreateDataRow();
                            _row.Set(Labels.RuleName, ruleName);
                            _row.Set(Labels.ObjectName, _violation.Component.Name);
                            _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                            _row.Set(Labels.Status, st);
                            if (ruleId == "actionPlan")
                            {
                                _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                            }
                            else if (ruleId == ("actionPlanPriority"))
                            {
                                _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                            }
                            _row.Set(Labels.AssociatedValue, associatedValue.Values == null ? " " : string.Join(',', associatedValue.Values));
                            _row.Set(Labels.FilePath, _.Item1);
                            _row.Set(Labels.StartLine, _.Item2.ToString());
                            _row.Set(Labels.EndLine, _.Item3.ToString());
                            rowData.AddRange(_row);
                        });
                    }
                    if (associatedValue.Type != null && associatedValue.Type.Equals("percentage"))
                    {
                        decimal? value = (decimal?)associatedValue.Values?.GetValue(0);
                        // manage case when type= "percentage"
                        List<Tuple<string, int, int>> paths = reportData.SnapshotExplorer.GetComponentFilePath(domainId, _violation.Component.GetComponentId(), snapshotId);
                        paths.ForEach(_ =>
                        {
                            var _row = headers.CreateDataRow();
                            _row.Set(Labels.RuleName, ruleName);
                            _row.Set(Labels.ObjectName, _violation.Component.Name);
                            _row.Set(Labels.IFPUG_ObjectType, objectComponent.Type.Label);
                            _row.Set(Labels.Status, st);
                            if (ruleId == "actionPlan")
                            {
                                _row.Set(Labels.Priority, _violation.RemedialAction.Tag);
                            }
                            else if (ruleId == ("actionPlanPriority"))
                            {
                                _row.Set(Labels.Priority, _violation.RemedialAction.Priority);
                            }
                            _row.Set(Labels.AssociatedValue, value?.ToString("N2"));
                            _row.Set(Labels.FilePath, _.Item1);
                            _row.Set(Labels.StartLine, _.Item2.ToString());
                            _row.Set(Labels.EndLine, _.Item3.ToString());
                            rowData.AddRange(_row);
                        });
                    }
                }
            }
            return rowData;
        }

        public static int AddSimpleDescription(List<string> rowData, List<CellAttributes> cellProps, int cellidx, RuleDescription rule)
        {
            return AddDescription(rowData, cellProps, cellidx, rule, false);
        }

        public static int AddFullDescription(List<string> rowData, List<CellAttributes> cellProps, int cellidx, RuleDescription rule)
        {
            return AddDescription(rowData, cellProps, cellidx, rule, true);
        }

        public static int AddDescription(List<string> rowData, List<CellAttributes> cellProps, int cellidx, RuleDescription rule, bool full)
        {
            string ColorWhite = "White";
            string ColorLightGray = "LightGrey";

            rowData.Add("");
            cellidx++;
            if (!string.IsNullOrWhiteSpace(rule.Rationale))
            {
                rowData.Add(Labels.Rationale + ": ");
                cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                cellidx++;
                cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.Rationale, ColorWhite);
            }
            rowData.Add(Labels.Description + ": ");
            cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
            cellidx++;
            cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.Description, ColorWhite);
            if (!string.IsNullOrWhiteSpace(rule.Remediation))
            {
                rowData.Add(Labels.Remediation + ": ");
                cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                cellidx++;
                cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.Remediation, ColorWhite);
            }

            if (full)
            {
                if (!string.IsNullOrWhiteSpace(rule.Reference))
                {
                    rowData.Add(Labels.Reference + ": ");
                    cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                    cellidx++;
                    rowData.Add(rule.Reference);
                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                    cellidx++;
                }
                if (!string.IsNullOrWhiteSpace(rule.Sample))
                {
                    rowData.Add(Labels.Sample + ": ");
                    cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                    cellidx++;
                    cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.Sample, ColorWhite);
                }
                if (!string.IsNullOrWhiteSpace(rule.RemediationSample))
                {
                    rowData.Add(Labels.RemediationSample + ": ");
                    cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                    cellidx++;
                    cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.RemediationSample, ColorWhite);
                }
                if (!string.IsNullOrWhiteSpace(rule.Output))
                {
                    rowData.Add(Labels.Output + ": ");
                    cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                    cellidx++;
                    cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.Output, ColorWhite);
                }
                if (!string.IsNullOrWhiteSpace(rule.AssociatedValueName))
                {
                    rowData.Add(Labels.AssociatedValue + ": ");
                    cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                    cellidx++;
                    cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.AssociatedValueName, ColorWhite);
                }
                if (!string.IsNullOrWhiteSpace(rule.Total))
                {
                    rowData.Add(Labels.Total + ": ");
                    cellProps.Add(new CellAttributes(cellidx, ColorLightGray));
                    cellidx++;
                    cellidx = FormatStringWithLinesBreak(rowData, cellProps, cellidx, rule.Total, ColorWhite);
                }
            }
            return cellidx;
        }

        private static int FormatStringWithLinesBreak(List<string> rowData, List<CellAttributes> cellProps, int cellidx, string desc, string ColorWhite)
        {
            string[] sampleFragments = desc.Split("\n");
            int nbFragments = sampleFragments.Count();
            for (int i = 0; i < nbFragments; i++)
            {
                rowData.Add(sampleFragments[i]);
                cellProps.Add(new CellAttributes(cellidx, ColorWhite));
                cellidx++;
            }

            return cellidx;
        }
    }

}