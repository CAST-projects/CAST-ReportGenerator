/*
 *   Copyright (c) 2021 CAST
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
using CastReporting.BLL.Computing;
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
    [Block("OMG_TECHNICAL_DEBT_RULES_EVOLUTION")]
    public class OmgTechnicalDebtRulesEvolution : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            string standard = options.GetOption("ID", "ISO");
            string lbltotal = Labels.TechnicalDebt + " (" + Labels.Days + ")";
            string lbladded = Labels.TechnicalDebtAdded + " (" + Labels.Days + ")";
            string lblremoved = Labels.TechnicalDebtRemoved + " (" + Labels.Days + ")";
            bool displayHeader = !options.GetOption("HEADER", "YES").ToUpper().Equals("NO");
            bool showDescription = options.GetOption("DESC", "false").Equals("true");

            string indicatorName = int.TryParse(standard, out int id) ?
                reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.Key == id).Select(_ => _.Reference.Name).FirstOrDefault()
                : standard;
            if (indicatorName == null)
            {
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = displayHeader,
                    NbRows = 1,
                    NbColumns = 1,
                    Data = new List<string>() { Labels.NoData }
                };
            }
            if (indicatorName.Contains("-Index"))
            {
                int idx = indicatorName.IndexOf("-Index");
                if (idx != -1)
                {
                    indicatorName = indicatorName.Substring(0, idx);
                }
            }

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(indicatorName);
            cellidx++;
            headers.Append(lbltotal);
            cellidx++;
            headers.Append(lbladded);
            cellidx++;
            headers.Append(lblremoved);
            cellidx++;
            var data = new List<string>();

            if (!VersionUtil.Is231Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 2.3.1 at least for component OMG_TECHNICAL_DEBT_ERULES_VOLUTION");
                var dataRow = headers.CreateDataRow();
                dataRow.Set(indicatorName, Labels.NoData);
                dataRow.Set(lbltotal, string.Empty);
                dataRow.Set(lbladded, string.Empty);
                dataRow.Set(lblremoved, string.Empty);
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

            headers.Append(Labels.Rationale, showDescription);
            headers.Append(Labels.Description, showDescription);
            headers.Append(Labels.Remediation, showDescription);
            if (showDescription)
            {
                cellidx++; // for Rationale
                cellidx++; // for Description
                cellidx++; // for Remediation
            }

            int metricBcIdFromName = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.Name == indicatorName).Select(_ => _.Reference.Key).FirstOrDefault();
            if (metricBcIdFromName == 0)
            {
                // if we don't get it with the name, we try with the short name
                metricBcIdFromName = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.ShortName == indicatorName).Select(_ => _.Reference.Key).FirstOrDefault();
            }
            if (metricBcIdFromName != 0)
            {
                // Case of BC
                List<int?> technicalCriterionIds = reportData.RuleExplorer.GetCriteriaContributors(reportData.CurrentSnapshot.DomainId, metricBcIdFromName.ToString(), reportData.CurrentSnapshot.Id).Select(_ => _.Key).ToList();
                List<ApplicationResult> tcResults = technicalCriterionIds.Count > 0 ?
                    reportData.CurrentSnapshot.TechnicalCriteriaResults.Where(_ => technicalCriterionIds.Contains(_.Reference.Key) && _.DetailResult.OmgTechnicalDebt != null).OrderByDescending(_ => _.DetailResult.OmgTechnicalDebt?.Total).ToList()
                    : null;
                if (tcResults?.Count > 0)
                {
                    foreach (ApplicationResult tcres in tcResults)
                    {
                        string tcName = tcres.Reference?.Name;
                        OmgTechnicalDebtIdDTO omgTechnicalDebt = OmgTechnicalDebtUtility.GetOmgTechDebt(reportData.CurrentSnapshot, tcres.Reference.Key);
                        cellidx = AddRowForTCorTag(indicatorName, lbltotal, lbladded, lblremoved, showDescription, cellProps, cellidx, headers, data, tcName, omgTechnicalDebt);

                        List<int?> rulesIds = reportData.RuleExplorer.GetCriteriaContributors(reportData.CurrentSnapshot.DomainId, tcres.Reference.Key.ToString(), reportData.CurrentSnapshot.Id).Select(_ => _.Key).ToList();
                        List<ApplicationResult> rulesResults = rulesIds.Count > 0 ?
                            reportData.CurrentSnapshot.QualityRulesResults.Where(_ => rulesIds.Contains(_.Reference.Key)).OrderByDescending(_ => _.DetailResult.OmgTechnicalDebt?.Total).ToList()
                            : null;
                        cellidx = AddRowsForRules(reportData, indicatorName, lbltotal, lbladded, lblremoved, showDescription, cellProps, cellidx, headers, data, rulesResults);
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(0, Labels.NoRules);
                for (int i = 1; i < headers.Count; i++)
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

        private static int AddRowForTCorTag(string indicatorName, string lbltotal, string lbladded, string lblremoved, bool showDescription, List<CellAttributes> cellProps, int cellidx, HeaderDefinition headers, List<string> data, string tcName, OmgTechnicalDebtIdDTO tcOmgTechDebt)
        {
            if (tcOmgTechDebt == null) return cellidx;
            int? _totalNotNull = tcOmgTechDebt.Total > 0.0 ? 1 : 0;
            // Add a line for each TC from BC
            var dataRow = headers.CreateDataRow();
            dataRow.Set(indicatorName, tcName);
            FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _totalNotNull);
            cellidx++;
            dataRow.Set(lbltotal, tcOmgTechDebt.Total != null ? tcOmgTechDebt.Total.Value.ToString("N1") : Constants.No_Value);
            FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _totalNotNull);
            cellidx++;
            dataRow.Set(lbladded, tcOmgTechDebt.Added != null ? tcOmgTechDebt.Added.Value.ToString("N1") : Constants.No_Value);
            FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _totalNotNull);
            cellidx++;
            dataRow.Set(lblremoved, tcOmgTechDebt.Removed != null ? tcOmgTechDebt.Removed.Value.ToString("N1") : Constants.No_Value);
            FormatTableHelper.AddGrayOrBold(true, cellProps, cellidx, _totalNotNull);
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
            return cellidx;
        }

        private static int AddRowsForRules(ImagingData reportData, string indicatorName, string lbltotal, string lbladded, string lblremoved, bool showDescription, List<CellAttributes> cellProps, int cellidx, HeaderDefinition headers, List<string> data, List<ApplicationResult> rulesResults)
        {
            if (rulesResults?.Count > 0)
            {
                foreach (ApplicationResult qres in rulesResults)
                {
                    var _ruleDr = headers.CreateDataRow();
                    var _resultDetail = OmgTechnicalDebtUtility.GetOmgTechDebt(reportData.CurrentSnapshot, qres.Reference.Key);
                    if (_resultDetail == null) continue;
                    int? _totalNotNull = _resultDetail.Total > 0.0 ? 1 : 0; ;
                    string ruleName = qres.Reference?.Name;
                    _ruleDr.Set(indicatorName, "    " + ruleName);
                    FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _totalNotNull);
                    cellidx++;
                    _ruleDr.Set(lbltotal, _resultDetail.Total != null ? _resultDetail.Total.Value.ToString("N1") : Constants.No_Value);
                    FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _totalNotNull);
                    cellidx++;
                    _ruleDr.Set(lbladded, _resultDetail.Added != null ? _resultDetail.Added.Value.ToString("N1") : Constants.No_Value);
                    FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _totalNotNull);
                    cellidx++;
                    _ruleDr.Set(lblremoved, _resultDetail.Removed != null ? _resultDetail.Removed.Value.ToString("N1") : Constants.No_Value);
                    FormatTableHelper.AddGrayOrBold(false, cellProps, cellidx, _totalNotNull);
                    cellidx++;
                    if (showDescription)
                    {
                        RuleDescription desc = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, qres.Reference.Key.ToString());
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
                _ruleDr.Set(indicatorName, Labels.NoRules);
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

            return cellidx;
        }
    }
}
