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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;


namespace CastReporting.Reporting.Block.Table
{
    [Block("QUALITY_STANDARDS_EVOLUTION")]
    public class QualityStandardsEvolution : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            string standard = options.GetOption("STD");
            bool detail = options.GetOption("MORE", "false").ToLower().Equals("true");
            bool vulnerability = options.GetOption("LBL", "vulnerabilities").ToLower().Equals("vulnerabilities");
            string displayAddedRemoved = reportData.PreviousSnapshot != null ? "true" : "false";
            bool displayEvolution = options.GetOption("EVOLUTION", displayAddedRemoved).ToLower().Equals("true");
            bool displayHeader = !options.GetOption("HEADER", "YES").ToUpper().Equals("NO");

            string lbltotal = vulnerability ? Labels.TotalVulnerabilities : Labels.TotalViolations;
            string lbladded = vulnerability ? Labels.AddedVulnerabilities : Labels.AddedViolations;
            string lblremoved = vulnerability ? Labels.RemovedVulnerabilities : Labels.RemovedViolations;

            string indicatorName = int.TryParse(standard, out int id) ?
                reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == id).Select(_ => _.Reference.Name).FirstOrDefault()
    : standard;

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(indicatorName);
            cellidx++;
            headers.Append(lbltotal);
            cellidx++;
            if (displayEvolution)
            {
                headers.Append(lbladded);
                cellidx++;
                headers.Append(lblremoved);
                cellidx++;
            }

            var data = new List<string>();

            if (!VersionUtil.Is111Compatible(reportData.ServerVersion) && detail)
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.11 at least for component QUALITY_STANDARDS_EVOLUTION and MORE option");
                var dataRow = headers.CreateDataRow();
                dataRow.Set(indicatorName, Labels.NoData);
                dataRow.Set(lbltotal, string.Empty);
                if (reportData.PreviousSnapshot != null)
                {
                    dataRow.Set(lbladded, string.Empty);
                    dataRow.Set(lblremoved, string.Empty);
                }
                data.AddRange(dataRow);
                if (displayHeader) data.InsertRange(0, headers.Labels);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = displayHeader,
                    NbRows = displayHeader ? 2 : 1,
                    NbColumns = headers.Count,
                    Data = data
                };
            }

            // REPORTGEN-885 : 
            // this component initially designed for standard category, should now wotk for a BC, because in new extension the standards are becoming BC
            // to avoid changing the reports, the parameter should accept : BC name or short name, BC id, category name
            int metricBcIdFromName = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.Name == indicatorName).Select(_ => _.Reference.Key).FirstOrDefault();
            if (metricBcIdFromName == 0)
            {
                // if we don't get it with the name, we try with the short name
                metricBcIdFromName = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.ShortName == indicatorName).Select(_ => _.Reference.Key).FirstOrDefault();
            }
            if (metricBcIdFromName != 0)
            {
                // Case of BC
                // In this case, the MORE option will be used to find the special global index as CISQ, because the purpose
                // is to separate it into its BC CISQ-Security, CISQ-Reliability,....
                // so if MORE=true, we do not get the results for CISQ, but we search for all CISQ-xxx BCs
                if (detail)
                {
                    List<ApplicationResult> bcresults = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.ShortName != null && _.Reference.ShortName.Contains(indicatorName + "-")).OrderByDescending(_ => _.DetailResult.EvolutionSummary.TotalViolations).ToList();
                    foreach (ApplicationResult bcres in bcresults)
                    {
                        string bcName = bcres.Reference.Name;
                        int? nbbcViolations = bcres.DetailResult?.EvolutionSummary?.TotalViolations;
                        cellidx = AddDataRow(true, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, bcres.DetailResult, nbbcViolations, bcName);

                        List<int?> technicalCriterionIds = reportData.RuleExplorer.GetCriteriaContributors(reportData.CurrentSnapshot.DomainId, bcres.Reference.Key.ToString(), reportData.CurrentSnapshot.Id).Select(_ => _.Key).ToList();
                        List<ApplicationResult> tcResults = technicalCriterionIds.Count > 0 ?
                            reportData.CurrentSnapshot.TechnicalCriteriaResults.Where(_ => technicalCriterionIds.Contains(_.Reference.Key)).OrderByDescending(_ => _.DetailResult.EvolutionSummary.TotalViolations).ToList()
                            : null;
                        if (tcResults?.Count > 0)
                        {
                            foreach (ApplicationResult tcres in tcResults)
                            {
                                string tcName = "    " + tcres.Reference?.Name;
                                int? nbtcViolations = tcres.DetailResult?.EvolutionSummary?.TotalViolations;
                                cellidx = AddDataRow(false, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, tcres.DetailResult, nbtcViolations, tcName);
                            }
                        }
                    }
                }
                else
                {
                    List<int?> technicalCriterionIds = reportData.RuleExplorer.GetCriteriaContributors(reportData.CurrentSnapshot.DomainId, metricBcIdFromName.ToString(), reportData.CurrentSnapshot.Id).Select(_ => _.Key).ToList();
                    List<ApplicationResult> tcResults = technicalCriterionIds.Count > 0 ?
                        reportData.CurrentSnapshot.TechnicalCriteriaResults.Where(_ => technicalCriterionIds.Contains(_.Reference.Key)).OrderByDescending(_ => _.DetailResult.EvolutionSummary.TotalViolations).ToList()
                        : null;
                    if (tcResults?.Count > 0)
                    {
                        foreach (ApplicationResult tcres in tcResults)
                        {
                            string tcName = tcres.Reference?.Name;
                            int? nbtcViolations = tcres.DetailResult?.EvolutionSummary?.TotalViolations;
                            cellidx = AddDataRow(false, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, tcres.DetailResult, nbtcViolations, tcName);
                        }
                    }
                }
            }
            else
            {
                // Case of standard
                List<ApplicationResult> results = reportData.SnapshotExplorer.GetQualityStandardsTagsResults(reportData.CurrentSnapshot.Href, standard)?.FirstOrDefault()?.ApplicationResults?.ToList();

                if (results?.Count > 0)
                {
                    foreach (var result in results)
                    {
                        var detailResult = result.DetailResult;
                        if (detailResult == null) continue;
                        int? nbViolations = detailResult.EvolutionSummary?.TotalViolations;
                        // usefull when the STD is a tag. when STD is a category it is not in the standardTags list for application, so only STD name is displayed
                        string stdTagName = result.Reference?.Name + " " + reportData.Application.StandardTags?.Where(_ => _.Key == result.Reference?.Name).FirstOrDefault()?.Name;

                        cellidx = AddDataRow(detail, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, detailResult, nbViolations, stdTagName);

                        // add lines for all sub tags if detail version (case of an upper category that should list all tags contains in the sub category
                        if (!detail) continue;
                        {
                            List<ApplicationResult> stdresults = reportData.SnapshotExplorer.GetQualityStandardsTagsResults(reportData.CurrentSnapshot.Href, result.Reference?.Name)?.FirstOrDefault()?.ApplicationResults?.ToList();
                            if (!(stdresults?.Count > 0)) continue;
                            foreach (var stdres in stdresults)
                            {
                                var detailStdResult = stdres.DetailResult;
                                if (detailStdResult == null) continue;
                                int? nbStdViolations = detailStdResult.EvolutionSummary?.TotalViolations;
                                string stdresTagName = "    " + stdres.Reference?.Name + " " + reportData.Application.StandardTags?.Where(_ => _.Key == stdres.Reference?.Name).FirstOrDefault()?.Name;
                                cellidx = AddDataRow(false, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, detailStdResult, nbStdViolations, stdresTagName);
                            }
                        }
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(indicatorName, Labels.NoRules);
                dataRow.Set(lbltotal, string.Empty);
                if (displayEvolution)
                {
                    dataRow.Set(lbladded, string.Empty);
                    dataRow.Set(lblremoved, string.Empty);
                }
                data.AddRange(dataRow);
            }

            if (displayHeader) data.InsertRange(0, headers.Labels);

            return new TableDefinition
            {
                Data = data,
                HasColumnHeaders = displayHeader,
                HasRowHeaders = false,
                NbColumns = headers.Count,
                NbRows = data.Count / headers.Count,
                CellsAttributes = cellProps
            };
        }

        private static int AddDataRow(bool detail, bool displayEvolution, string lbltotal, string lbladded, string lblremoved, string indicatorName, List<CellAttributes> cellProps, int cellidx, HeaderDefinition headers, List<string> data, ResultDetail detailResult, int? nbViolations, string stdTagName)
        {
            var dataRow = headers.CreateDataRow();
            dataRow.Set(indicatorName, stdTagName);
            FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
            cellidx++;
            dataRow.Set(lbltotal, detailResult.EvolutionSummary?.TotalViolations.NAIfEmpty("N0"));
            FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
            cellidx++;
            if (displayEvolution)
            {
                dataRow.Set(lbladded, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty("N0"));
                FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
                cellidx++;
                dataRow.Set(lblremoved, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty("N0"));
                FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, nbViolations);
                cellidx++;
            }
            data.AddRange(dataRow);
            return cellidx;
        }
    }
}
