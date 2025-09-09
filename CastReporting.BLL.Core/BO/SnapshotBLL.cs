/*
 *   Copyright (c) 2019 CAST
 *
 * Licensed under a custom license, Version 1.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License, accessible in the main project
 * source code: Empowerment.
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.Domain;
using CastReporting.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
// ReSharper disable AccessToDisposedClosure

namespace CastReporting.BLL
{

    /// <summary>
    /// 
    /// </summary>
    public class SnapshotBLL : BaseBLL, ISnapshotExplorer
    {
        /// <summary>
        /// 
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        // ReSharper disable once InconsistentNaming
        private Snapshot _Snapshot;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="snapshot"></param>
        public SnapshotBLL(WSConnection connection, Snapshot snapshot)
            : base(connection)
        {
            _Snapshot = snapshot;
        }



        /// <summary>
        /// 
        /// </summary>
        public void SetQualityIndicators()
        {
            const string qualityIndicators = "business-criteria,technical-criteria,quality-rules,quality-distributions,quality-measures";
            var qualityIndicatorsResults = GetRepository().GetResultsQualityIndicators(_Snapshot.Href, qualityIndicators, string.Empty, "$all", "$all")
                                                                    .Where(_ => _.ApplicationResults != null)
                                                                    .SelectMany(_ => _.ApplicationResults)
                                                                    .ToList();



            var businessCriteriaResults = new List<ApplicationResult>();
            var qualityDistributionsResults = new List<ApplicationResult>();
            var qualityMeasuresResults = new List<ApplicationResult>();
            var qualityRulesResults = new List<ApplicationResult>();
            var technicalCriteriaResults = new List<ApplicationResult>();

            foreach (var appRes in qualityIndicatorsResults)
            {
                switch (appRes.Type)
                {
                    case "business-criteria": businessCriteriaResults.Add(appRes); break;
                    case "quality-distributions": qualityDistributionsResults.Add(appRes); break;
                    case "quality-measures": qualityMeasuresResults.Add(appRes); break;
                    case "quality-rules": qualityRulesResults.Add(appRes); break;
                    case "technical-criteria": technicalCriteriaResults.Add(appRes); break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }

            _Snapshot.BusinessCriteriaResults = businessCriteriaResults;
            _Snapshot.QualityDistributionsResults = qualityDistributionsResults;
            _Snapshot.QualityMeasuresResults = qualityMeasuresResults;
            _Snapshot.QualityRulesResults = qualityRulesResults;
            _Snapshot.TechnicalCriteriaResults = technicalCriteriaResults;

            SetBusinessCriteriaCCRulesViolations();
            SetBusinessCriteriaNCRulesViolations();
            SetTechnicalCriteriaRulesViolations();
        }


        /// <summary>
        /// 
        /// </summary>
        public void SetModules()
        {
            _Snapshot.Modules = GetRepository().GetModules(_Snapshot.Href);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetSizingMeasure()
        {
            var castRepsitory = GetRepository();
            try
            {
                if (VersionUtil.IsAdgVersion82Compliant(_Snapshot.AdgVersion))
                {
                    const string strSizingMeasures = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics,violation-statistics";
                    _Snapshot.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot.Href, strSizingMeasures, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                }
                else
                {
                    const string strSizingMeasuresOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                    _Snapshot.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot.Href, strSizingMeasuresOld, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
                }
            }
            catch (WebException ex)
            {
                LogHelper.LogInfo(ex.Message);
                const string strSizingMeasuresOld = "technical-size-measures,run-time-statistics,technical-debt-statistics,functional-weight-measures,critical-violation-statistics";
                _Snapshot.SizingMeasuresResults = castRepsitory.GetResultsSizingMeasures(_Snapshot.Href, strSizingMeasuresOld, string.Empty, "$all", "$all").SelectMany(_ => _.ApplicationResults);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>A verifier s'il faut utiliser la conf</remarks>
        public void SetConfigurationBusinessCriterias()
        {
            var castRepsitory = GetRepository();
            _Snapshot.QIBusinessCriterias = castRepsitory.GetConfBusinessCriteriaBySnapshot(_Snapshot.DomainId, _Snapshot.Id);
            List<QIBusinessCriteria> fullQibusinesCriterias = _Snapshot.QIBusinessCriterias.Select(_ => castRepsitory.GetConfBusinessCriteria(_.HRef)).ToList();
            _Snapshot.QIBusinessCriterias = fullQibusinesCriterias;
        }



        /// <summary>
        /// 
        /// </summary>
        public void SetComplexity()
        {
            var values = (int[])Enum.GetValues(typeof(Constants.QualityDistribution));
            List<ApplicationResult> results = new List<ApplicationResult>();

            foreach (int val in values)
            {
                var appResults = GetRepository().GetComplexityIndicators(_Snapshot.Href, val.ToString());
                foreach (var result in appResults)
                {
                    results.AddRange(result.ApplicationResults);
                }
            }

            _Snapshot.CostComplexityResults = results;
        }



        /// <summary>
        /// 
        /// </summary>
        public void SetActionsPlan()
        {
            try
            {
                _Snapshot.ActionsPlan = GetRepository().GetActionPlanBySnapshot(_Snapshot.Href);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                _Snapshot.ActionsPlan = null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Transaction> GetTransactions(string snapshotHref, string businessCriteria, int count)
        {
            try
            {
                return GetRepository().GetTransactions(_Snapshot.Href, businessCriteria, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts)
        {
            try
            {
                return GetRepository().GetResultsBackgroundFacts(snapshotHref, backgroundFacts, string.Empty, string.Empty, string.Empty);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetBackgroundFacts(string snapshotHref, string backgroundFacts, bool modules, bool technologies)
        {
            string modParam = string.Empty;
            if (modules) modParam = "$all";

            string technoParam = string.Empty;
            if (technologies) technoParam = "$all";

            try
            {
                return GetRepository().GetResultsBackgroundFacts(snapshotHref, backgroundFacts, string.Empty, technoParam, modParam);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetSizingMeasureResults(string snapshotHref, string sizingMeasure)
        {
            try
            {
                return GetRepository().GetResultsSizingMeasures(snapshotHref, sizingMeasure, string.Empty, string.Empty, string.Empty);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetQualityIndicatorResults(string snapshotHref, string qualityIndicator)
        {
            try
            {
                return GetRepository().GetResultsQualityIndicators(snapshotHref, qualityIndicator, string.Empty, string.Empty, string.Empty);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Result> GetQualityStandardsRulesResults(string snapshotHref, string standardTag, bool evolutionSummary = false)
        {
            try
            {
                return VersionUtil.IsAdgVersion833Compliant(_Snapshot.AdgVersion) ? GetRepository().GetResultsQualityStandardsRules(snapshotHref, standardTag, string.Empty, string.Empty, evolutionSummary) : null;
            }
            catch (WebException ex)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public IEnumerable<Result> GetQualityStandardsTagsResults(string snapshotHref, string standardTag)
        {
            try
            {
                return VersionUtil.IsAdgVersion833Compliant(_Snapshot.AdgVersion) ? GetRepository().GetResultsQualityStandardsTags(snapshotHref, standardTag) : null;
            }
            catch (WebException ex)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<string> GetQualityStandardsRulesList(string snapshotHref, string standardTag)
        {
            try
            {
                IEnumerable<Result> results = VersionUtil.IsAdgVersion833Compliant(_Snapshot.AdgVersion) ? GetRepository().GetResultsQualityStandardsRules(snapshotHref, standardTag, string.Empty, string.Empty, false) : null;
                if (results == null) return null;
                List<string> metrics = new List<string>();
                foreach (Result _result in results)
                {
                    metrics.AddRange(_result.ApplicationResults.Select(resultApplicationResult => resultApplicationResult.Reference.Key.ToString()));
                }
                return metrics;
            }
            catch (WebException ex)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<CommonCategories> GetCommonCategories(WSConnection connection)
        {
            try
            {
                return GetRepository(connection).GetCommonCategories();
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<IfpugFunction> GetIfpugFunctions(string snapshotHref, int count)
        {
            try
            {
                return GetRepository().GetIfpugFunctions(snapshotHref, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<IfpugFunction> GetIfpugFunctionsEvolutions(string snapshotHref, int count)
        {
            try
            {
                return GetRepository().GetIfpugFunctionsEvolutions(_Snapshot.Href, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<OmgFunction> GetOmgFunctionsEvolutions(string snapshotHref, int count)
        {
            try
            {
                return GetRepository().GetOmgFunctionsEvolutions(snapshotHref, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<OmgFunctionTechnical> GetOmgFunctionsTechnical(string snapshotHref, int count)
        {
            try
            {
                return GetRepository().GetOmgFunctionsTechnical(_Snapshot.Href, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Result> GetOmgTechnicalDebtDetailsForSnapshots(string appHRef, int indexId, string snapshotIds)
        {
            try
            {
                return GetRepository().GetOmgTechnicalDebtDetails(appHRef, indexId.ToString(), snapshotIds);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="ruleId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<MetricTopArtifact> GetMetricTopArtefact(string snapshotHref, string ruleId, int count)
        {
            try
            {
                return GetRepository().GetMetricTopArtefact(_Snapshot.Href, ruleId, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="ruleId"></param>
        /// <param name="bcId"></param>
        /// <param name="count"></param>
        /// <param name="technos"></param>
        /// <returns></returns>
        public IEnumerable<Violation> GetViolationsListIDbyBC(string snapshotHref, string ruleId, string bcId, int count, string technos)
        {
            try
            {
                return GetRepository().GetViolationsListIDbyBC(snapshotHref, ruleId, bcId, count, technos);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        public IEnumerable<Violation> GetRemovedViolationsbyBC(string snapshotHref, string bcId, int count, string criticity)
        {
            try
            {
                return GetRepository().GetRemovedViolations(snapshotHref, bcId, count, criticity);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public IEnumerable<Violation> GetViolationsInActionPlan(string snapshotHref, int count)
        {
            try
            {
                return GetRepository().GetViolationsInActionPlan(snapshotHref, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public TypedComponent GetTypedComponent(string domainId, string componentId, string snapshotId)
        {

            try
            {
                return GetRepository().GetTypedComponent(domainId, componentId, snapshotId);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Component> GetComponents(string snapshotHref, string businessCriteria, int count)
        {

            try
            {
                return GetRepository().GetComponents(_Snapshot.Href, businessCriteria, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshotHref"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="prop1"></param>
        /// <param name="prop2"></param>
        /// <param name="order1"></param>
        /// <param name="order2"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<ComponentWithProperties> GetComponentsByProperties(string snapshotHref, int businessCriteria, string prop1, string prop2, string order1, string order2, int count)
        {
            try
            {
                return GetRepository().GetComponentsWithProperties(snapshotHref, businessCriteria, prop1, prop2, order1, order2, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="moduleId"></param>
        /// <param name="snapshotId"></param>
        /// <param name="businessCriteria"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public IEnumerable<Component> GetComponentsByModule(string domainId, int moduleId, int snapshotId, string businessCriteria, int count)
        {
            try
            {
                return GetRepository().GetComponentsByModule(domainId, moduleId, snapshotId, businessCriteria, count);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetBusinessCriteriaCCRulesViolations()
        {
            foreach (var businessCriteria in _Snapshot.BusinessCriteriaResults)
            {
                var results = GetRepository().GetRulesViolations(_Snapshot.Href, "cc", businessCriteria.Reference.Key.ToString());

                businessCriteria.CriticalRulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetBusinessCriteriaNCRulesViolations()
        {
            foreach (var businessCriteria in _Snapshot.BusinessCriteriaResults)
            {
                var results = GetRepository().GetRulesViolations(_Snapshot.Href, "nc", businessCriteria.Reference.Key.ToString());

                businessCriteria.NonCriticalRulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        private void SetTechnicalCriteriaRulesViolations()
        {
            foreach (var technicalCriteria in _Snapshot.TechnicalCriteriaResults)
            {
                var results = GetRepository().GetRulesViolations(_Snapshot.Href, "c", technicalCriteria.Reference.Key.ToString());
                technicalCriteria.RulesViolation = results?.SelectMany(x => x.ApplicationResults).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="snapshot"></param>
        /// <param name="connection"></param>
        /// <param name="withActionPlan"></param>
        /// <returns></returns>
        public static void BuildSnapshotResult(WSConnection connection, Snapshot snapshot, bool withActionPlan)
        {
            //Build modules
            using (SnapshotBLL snapshotBll = new SnapshotBLL(connection, snapshot))
            {
                Task taskModules = new Task(() => snapshotBll.SetModules());
                taskModules.Start();


                //Build Quality Indicators
                Task taskQualityIndicators = new Task(() => snapshotBll.SetQualityIndicators());
                taskQualityIndicators.Start();

                //Build Sizing Measures
                Task taskSizingMeasure = new Task(() => snapshotBll.SetSizingMeasure());
                taskSizingMeasure.Start();

                //Build Configuration for Business Criteria
                Task taskConfigurationBusinessCriterias = new Task(() => snapshotBll.SetConfigurationBusinessCriterias());
                taskConfigurationBusinessCriterias.Start();

                taskModules.Wait();
                taskQualityIndicators.Wait();
                taskSizingMeasure.Wait();
                taskConfigurationBusinessCriterias.Wait();

                //Build Configuration for Business Criteria
                Task taskComplexity = new Task(() => snapshotBll.SetComplexity());
                taskComplexity.Start();

                //build action plan
                // ReSharper disable once InconsistentNaming
                Task taskAP = null;
                if (withActionPlan)
                {
                    taskAP = new Task(() => snapshotBll.SetActionsPlan());
                    taskAP.Start();
                }

                taskComplexity.Wait();
                taskAP?.Wait();
            }
        }

        public AssociatedValue GetAssociatedValue(string domainId, string componentId, string snapshotId, string metricId)
        {
            try
            {
                return GetRepository().GetAssociatedValue(domainId, snapshotId, componentId, metricId);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public AssociatedValuePath GetAssociatedValuePath(string domainId, string componentId, string snapshotId, string metricId)
        {
            try
            {
                return GetRepository().GetAssociatedValuePath(domainId, snapshotId, componentId, metricId);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public AssociatedValueGroup GetAssociatedValueGroup(string domainId, string componentId, string snapshotId, string metricId)
        {
            try
            {
                return GetRepository().GetAssociatedValueGroup(domainId, snapshotId, componentId, metricId);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public AssociatedValueObject GetAssociatedValueObject(string domainId, string componentId, string snapshotId, string metricId)
        {
            try
            {
                return GetRepository().GetAssociatedValueObject(domainId, snapshotId, componentId, metricId);
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public Dictionary<int, string> GetSourceCodeBookmark(string domainId, CodeBookmark bookmark, int offset)
        {
            string siteId = bookmark.CodeFragment.CodeFile.GetSiteId();
            string fileId = bookmark.CodeFragment.CodeFile.GetFileId();
            int startLine = bookmark.CodeFragment.StartLine;
            int endLine = bookmark.CodeFragment.EndLine;
            try
            {
                Dictionary<int, string> codeLines = new Dictionary<int, string>();
                int idx = startLine - offset;
                if (idx < 0)
                {
                    idx = startLine;
                }

                List<string> lines = GetRepository().GetFileContent(domainId, siteId, fileId, idx, endLine + offset);
                for (int i = 0; i <= 2 * offset; i++)
                {
                    string line = lines[i].Replace("\u001a", "");
                    // Max number of char in a line is 255 to avoid https://jira.castsoftware.com/browse/REPORTGEN-945
                    codeLines.Add(idx, line.Length > 255 ? line.Substring(0, 120) + "(...)" : line);
                    idx++;
                }
                return (codeLines.Count == 0) ? null : codeLines;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is ArgumentException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public List<Tuple<string, Dictionary<int, string>>> GetSourceCode(string domainId, string snapshotId, string componentId, int offset, bool withCodeLines)
        {
            List<Tuple<string, Dictionary<int, string>>> codesAndPath = new List<Tuple<string, Dictionary<int, string>>>();
            try
            {
                var castRepository = GetRepository();
                List<CodeFragment> fragments = castRepository.GetSourceCode(domainId, snapshotId, componentId).ToList();

                if (!fragments.Any()) return null;

                foreach (CodeFragment _fragment in fragments)
                {
                    if (withCodeLines)
                    {
                        string siteId = _fragment.CodeFile.GetSiteId();
                        string fileId = _fragment.CodeFile.GetFileId();
                        int startLine = _fragment.StartLine;
                        int endLine = _fragment.EndLine;
                        int idx = (startLine < 1) ? 1 : startLine;
                        int endIdx = (endLine < 1) ? idx + offset
                            : endLine - idx < offset ? endLine : idx + offset;

                        Dictionary<int, string> codeLines = new Dictionary<int, string>();

                        List<string> lines = castRepository.GetFileContent(domainId, siteId, fileId, idx, endIdx);
                        foreach (string _line in lines)
                        {
                            string line = _line.Replace("\u001a", "");
                            // Max number of char in a line is 255 to avoid https://jira.castsoftware.com/browse/REPORTGEN-945
                            codeLines.Add(idx, line.Length > 255 ? line.Substring(0, 120) + "(...)" : line);
                            idx++;
                        }
                        codesAndPath.Add(new Tuple<string, Dictionary<int, string>>(_fragment.CodeFile.Name, codeLines));
                    }
                    else
                    {
                        codesAndPath.Add(new Tuple<string, Dictionary<int, string>>(_fragment.CodeFile.Name, new Dictionary<int, string>()));
                    }
                }
                return codesAndPath.Count == 0 ? null : codesAndPath;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is ArgumentException)
            {
                LogHelper.LogInfo(ex.Message);
                return null;
            }
        }

        public List<Tuple<string, int, int>> GetComponentFilePath(string domainId, string componentId, string snapshotId)
        {
            List<Tuple<string, int, int>> codesAndLineProps = new List<Tuple<string, int, int>>();
            try
            {
                List<CodeFragment> fragments = GetRepository().GetSourceCode(domainId, snapshotId, componentId).ToList();
                if (!fragments.Any()) return codesAndLineProps;

                foreach (CodeFragment _fragment in fragments)
                {
                    codesAndLineProps.Add(new Tuple<string, int, int>(_fragment.CodeFile.Name, _fragment.StartLine, _fragment.EndLine));
                }
                return codesAndLineProps;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is ArgumentException)
            {
                LogHelper.LogInfo(ex.Message);
                return codesAndLineProps;
            }


        }

    }
}
