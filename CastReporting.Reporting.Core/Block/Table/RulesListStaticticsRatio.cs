﻿/*
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
    [Block("RULES_LIST_STATISTICS_RATIO")]
    public class RulesListStatisticsRatio : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> metrics = options.GetOption("METRICS").Trim().Split('|').ToList();

            bool critical = options.GetOption("CRITICAL", "false").Equals("true");
            bool displayCompliance = options.GetBoolOption("COMPLIANCE");
            bool sortedByCompliance = displayCompliance && options.GetOption("SORTED", "TOTAL").Equals("COMPLIANCE");

            string displayAddedRemoved = reportData.PreviousSnapshot != null ? "true" : "false";
            bool displayEvolution = options.GetOption("EVOLUTION", displayAddedRemoved).ToLower().Equals("true");

            bool vulnerability = options.GetOption("LBL", "vulnerabilities").ToLower().Equals("vulnerabilities");
            string lbltotal = vulnerability ? Labels.TotalVulnerabilities : Labels.TotalViolations;
            string lbladded = vulnerability ? Labels.AddedVulnerabilities : Labels.AddedViolations;
            string lblremoved = vulnerability ? Labels.RemovedVulnerabilities : Labels.RemovedViolations;

            bool showDescription = options.GetOption("DESC", "false").Equals("true");
            bool displayHeader = !options.GetOption("HEADER", "YES").ToUpper().Equals("NO");
            bool showNoViolationRules = options.GetOption("NOVIOLATIONS", "true").Equals("true");

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(Labels.CASTRules);
            cellidx++;

            if (!VersionUtil.Is111Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.11 at least for component RULES_LIST_STATISTICS_RATIO");
                var _row = headers.CreateDataRow();
                var _data = new List<string>();
                _row.Set(Labels.CASTRules, Labels.NoData);
                _data.AddRange(_row);
                if (displayHeader) _data.InsertRange(0, headers.Labels);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = displayHeader,
                    NbRows = displayHeader ? 2 : 1,
                    NbColumns = 1,
                    Data = _data
                };
            }

            headers.Append(lbltotal);
            cellidx++;
            headers.Append(lbladded, displayEvolution);
            headers.Append(lblremoved, displayEvolution);
            if (displayEvolution)
            {
                cellidx++; // for added
                cellidx++; // for removed
            }
            headers.Append(Labels.ComplianceScorePercent, displayCompliance);
            if (displayCompliance) cellidx++;

            headers.Append(Labels.Rationale, showDescription);
            headers.Append(Labels.Description, showDescription);
            headers.Append(Labels.Remediation, showDescription);
            if (showDescription)
            {
                cellidx++; // for Rationale
                cellidx++; // for Description
                cellidx++; // for Remediation
            }
            var dataRow = headers.CreateDataRow();
            var data = new List<string>();

            List<string> qualityRules = MetricsUtility.BuildRulesList(reportData, metrics, critical);

            List<ApplicationResult> results = sortedByCompliance ?
                    reportData.CurrentSnapshot?.QualityRulesResults.Where(_ => qualityRules.Contains(_.Reference.Key.ToString())).OrderBy(_ => _.DetailResult.ViolationRatio.Ratio).ToList()
                    : reportData.CurrentSnapshot?.QualityRulesResults.Where(_ => qualityRules.Contains(_.Reference.Key.ToString())).OrderByDescending(_ => _.DetailResult.ViolationRatio.FailedChecks).ToList();
            string colorBeige = "Beige";
            if (results?.Count > 0)
            {
                foreach (var result in results)
                {
                    var detailResult = result.DetailResult;
                    if (detailResult == null) continue;
                    int nbViolations = detailResult.ViolationRatio.FailedChecks ?? 0;

                    if (!showNoViolationRules && nbViolations == 0)
                    {
                        continue; // skip rules with no violations
                    }

                    dataRow.Set(Labels.CASTRules, (result.Reference?.Name + " (" + result.Reference?.Key + ")").NAIfEmpty());
                    if (nbViolations > 0)
                    {
                        cellProps.Add(new CellAttributes(cellidx, colorBeige));
                    }
                    cellidx++;
                    dataRow.Set(lbltotal, detailResult.ViolationRatio.FailedChecks.HasValue ? detailResult.ViolationRatio?.FailedChecks.Value.ToString("N0") : Constants.No_Value);
                    if (nbViolations > 0)
                    {
                        cellProps.Add(new CellAttributes(cellidx, colorBeige));
                    }
                    cellidx++;
                    if (displayEvolution)
                    {
                        dataRow.Set(lbladded, detailResult.EvolutionSummary?.AddedViolations.NAIfEmpty("N0"));
                        if (nbViolations > 0)
                        {
                            cellProps.Add(new CellAttributes(cellidx, colorBeige));
                        }
                        cellidx++;
                        dataRow.Set(lblremoved, detailResult.EvolutionSummary?.RemovedViolations.NAIfEmpty("N0"));
                        if (nbViolations > 0)
                        {
                            cellProps.Add(new CellAttributes(cellidx, colorBeige));
                        }
                        cellidx++;
                    }
                    if (displayCompliance)
                    {
                        dataRow.Set(Labels.ComplianceScorePercent, detailResult.ViolationRatio?.Ratio.FormatPercent(false));
                        if (nbViolations > 0)
                        {
                            cellProps.Add(new CellAttributes(cellidx, colorBeige));
                        }
                        cellidx++;
                    }
                    if (showDescription)
                    {
                        RuleDescription rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, result.Reference.Key.ToString());
                        // Rationale
                        if (!string.IsNullOrWhiteSpace(rule.Rationale))
                        {
                            dataRow.Set(Labels.Rationale, rule.Rationale);
                        }
                        else
                        {
                            dataRow.Set(Labels.Rationale, string.Empty);
                        }
                        if (nbViolations > 0)
                        {
                            cellProps.Add(new CellAttributes(cellidx, colorBeige));
                        }
                        cellidx++;
                        // Description
                        if (!string.IsNullOrWhiteSpace(rule.Description))
                        {
                            dataRow.Set(Labels.Description, rule.Description);
                        }
                        else
                        {
                            dataRow.Set(Labels.Description, string.Empty);
                        }
                        if (nbViolations > 0)
                        {
                            cellProps.Add(new CellAttributes(cellidx, colorBeige));
                        }
                        cellidx++;
                        // Remediation
                        if (!string.IsNullOrWhiteSpace(rule.Remediation))
                        {
                            dataRow.Set(Labels.Remediation, rule.Remediation);
                        }
                        else
                        {
                            dataRow.Set(Labels.Remediation, string.Empty);
                        }
                        if (nbViolations > 0)
                        {
                            cellProps.Add(new CellAttributes(cellidx, colorBeige));
                        }
                        cellidx++;
                    }

                    data.AddRange(dataRow);
                }
            }

            if (data.Count == 0)
            {
                dataRow.Reset();
                dataRow.Set(0, Labels.NoRules);
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
