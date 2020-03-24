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

using System;
using System.Collections.Generic;
using System.Linq;
using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;


namespace CastReporting.Reporting.Block.Table
{
    [Block("QUALITY_TAGS_RULES_EVOLUTION")]
    public class QualityTagsRulesEvolution : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string standard = options.GetOption("STD");
            bool vulnerability = options.GetOption("LBL", "vulnerabilities").ToLower().Equals("vulnerabilities");
            string lbltotal = vulnerability ? Labels.TotalVulnerabilities : Labels.TotalViolations;
            string lbladded = vulnerability ? Labels.AddedVulnerabilities : Labels.AddedViolations;
            string lblremoved = vulnerability ? Labels.RemovedVulnerabilities : Labels.RemovedViolations;
            bool displayHeader = !options.GetOption("HEADER", "YES").ToUpper().Equals("NO");
            bool showDescription = options.GetOption("DESC", "false").Equals("true");

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(standard);
            cellidx++;
            var data = new List<string>();

            if (!VersionUtil.Is111Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.11 at least for component QUALITY_TAGS_RULES_EVOLUTION");
                var dataRow = headers.CreateDataRow();
                dataRow.Set(standard, Labels.NoData);
                data.AddRange(dataRow);
                if (displayHeader) data.InsertRange(0, headers.Labels);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = displayHeader,
                    NbRows = displayHeader ? 2 : 1,
                    NbColumns = 1,
                    Data = data
                };
            }

            headers.Append(lbltotal);
            cellidx++;
            headers.Append(lbladded);
            cellidx++;
            headers.Append(lblremoved);
            cellidx++;

            headers.Append(Labels.Rationale, showDescription);
            headers.Append(Labels.Description, showDescription);
            headers.Append(Labels.Remediation, showDescription);
            if (showDescription)
            {
                cellidx++; // for Rationale
                cellidx++; // for Description
                cellidx++; // for Remediation
            }

            List<ApplicationResult> results = reportData.SnapshotExplorer.GetQualityStandardsTagsResults(reportData.CurrentSnapshot.Href, standard)?.FirstOrDefault()?.ApplicationResults?.ToList();

            if (results?.Count > 0)
            {
                foreach (var result in results)
                {
                    var dataRow = headers.CreateDataRow();
                    var detailResult = result.DetailResult;
                    if (detailResult == null) continue;
                    int? _nbTagViolations = detailResult.EvolutionSummary?.TotalViolations;
                    string stdTagName = result.Reference?.Name + " " + reportData.Application.StandardTags?.Where(_ => _.Key == result.Reference?.Name).FirstOrDefault()?.Name;
                    // Add a line for each tag of the category
                    dataRow.Set(standard, stdTagName);
                    FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _nbTagViolations);
                    cellidx++;
                    dataRow.Set(lbltotal, detailResult.EvolutionSummary?.TotalViolations.NAIfEmpty("N0"));
                    FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _nbTagViolations);
                    cellidx++;
                    dataRow.Set(lbladded, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty("N0"));
                    FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _nbTagViolations);
                    cellidx++;
                    dataRow.Set(lblremoved, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty("N0"));
                    FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _nbTagViolations);
                    cellidx++;
                    if (showDescription)
                    {
                        dataRow.Set(Labels.Rationale, string.Empty);
                        cellidx++;
                        dataRow.Set(Labels.Description, string.Empty);
                        cellidx++;
                        dataRow.Set(Labels.Remediation, string.Empty);
                        cellidx++;
                    }

                    data.AddRange(dataRow);

                    // for each tag of the category add lines for all cast rules associated to this tag
                    List<ApplicationResult> stdresults = reportData.SnapshotExplorer.GetQualityStandardsRulesResults(reportData.CurrentSnapshot.Href, result.Reference?.Name, true)?.FirstOrDefault()?.ApplicationResults?.ToList();
                    if (stdresults?.Count > 0)
                    {
                        foreach (var ruleresult in stdresults)
                        {
                            var _ruleDr = headers.CreateDataRow();

                            var _resultDetail = ruleresult.DetailResult;
                            if (_resultDetail == null) continue;
                            int? _nbViolations = _resultDetail.ViolationRatio?.FailedChecks;
                            string ruleName = ruleresult.Reference?.Name;
                            _ruleDr.Set(standard, "    " + ruleName);
                            FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _nbViolations);
                            cellidx++;
                            _ruleDr.Set(lbltotal, _resultDetail.ViolationRatio?.FailedChecks.NAIfEmpty("N0"));
                            FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _nbViolations);
                            cellidx++;
                            _ruleDr.Set(lbladded, _resultDetail.EvolutionSummary?.AddedViolations.NAIfEmpty("N0"));
                            FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _nbViolations);
                            cellidx++;
                            _ruleDr.Set(lblremoved, _resultDetail.EvolutionSummary?.RemovedViolations.NAIfEmpty("N0"));
                            FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _nbViolations);
                            cellidx++;
                            if (showDescription)
                            {
                                RuleDescription desc = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, ruleresult.Reference.Key.ToString());
                                if (!string.IsNullOrWhiteSpace(desc.Rationale))
                                {
                                    _ruleDr.Set(Labels.Rationale, desc.Rationale);
                                }
                                else
                                {
                                    _ruleDr.Set(Labels.Rationale, string.Empty);
                                }
                                cellidx++;
                                if (!string.IsNullOrWhiteSpace(desc.Rationale))
                                {
                                    _ruleDr.Set(Labels.Description, desc.Description);
                                }
                                else
                                {
                                    _ruleDr.Set(Labels.Description, string.Empty);
                                }
                                cellidx++;
                                if (!string.IsNullOrWhiteSpace(desc.Rationale))
                                {
                                    _ruleDr.Set(Labels.Remediation, desc.Remediation);
                                }
                                else
                                {
                                    _ruleDr.Set(Labels.Remediation, string.Empty);
                                }
                                cellidx++;
                            }

                            data.AddRange(_ruleDr);
                        }
                    }
                    else
                    {
                        var _ruleDr = headers.CreateDataRow();
                        _ruleDr.Set(standard, Labels.NoRules);
                        _ruleDr.Set(lbltotal, string.Empty);
                        _ruleDr.Set(lbladded, string.Empty);
                        _ruleDr.Set(lblremoved, string.Empty);
                        if (showDescription)
                        {
                            _ruleDr.Set(Labels.Rationale, string.Empty);
                            _ruleDr.Set(Labels.Description, string.Empty);
                            _ruleDr.Set(Labels.Remediation, string.Empty);
                        }
                        cellidx++;
                        data.AddRange(_ruleDr);
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(0, Labels.NoRules);
                for (int i=1; i < headers.Count; i++)
                {
                    dataRow.Set(i, string.Empty);
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

    }
}
