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
    [Block("OMG_TECHNICAL_DEBT_EVOLUTION")]
    public class OmgTechnicalDebtEvolution : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string index = options.GetOption("ID","ISO");
            bool detail = options.GetOption("MORE", "false").ToLower().Equals("true");
            string displayAddedRemoved = reportData.PreviousSnapshot != null ? "true" : "false";
            bool displayEvolution = options.GetOption("EVOLUTION", displayAddedRemoved).ToLower().Equals("true");
            bool displayHeader = !options.GetOption("HEADER", "YES").ToUpper().Equals("NO");

            string lbltotal = Labels.TechnicalDebt + " (" + Labels.Days + ")";
            string lbladded = Labels.TechnicalDebtAdded + " (" + Labels.Days + ")";
            string lblremoved = Labels.TechnicalDebtRemoved + " (" + Labels.Days + ")";

            string indicatorName = reportData.CurrentSnapshot.BusinessCriteriaResults
                .Where(_ => _.Reference.Key == OmgTechnicalDebtUtility.GetOmgIndex(index)).Select(_ => _.Reference.Name).FirstOrDefault();

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
            if (displayEvolution)
            {
                headers.Append(lbladded);
                cellidx++;
                headers.Append(lblremoved);
                cellidx++;
            }

            var data = new List<string>();

            if (!VersionUtil.Is231Compatible(reportData.ServerVersion) && detail)
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 2.3.1 at least for component OMG_TECHNICAL_DEBT_EVOLUTION");
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
                    List<ApplicationResult> bcresults = reportData.CurrentSnapshot.BusinessCriteriaResults.Where(_ => _.Reference.ShortName != null && _.Reference.ShortName.Contains(indicatorName + "-")).OrderByDescending(_ => _.DetailResult.OmgTechnicalDebt?.Total).ToList();
                    foreach (ApplicationResult bcres in bcresults)
                    {
                        OmgTechnicalDebtIdDTO omgTechnicalDebt = OmgTechnicalDebtUtility.GetOmgTechDebt(reportData.CurrentSnapshot, bcres.Reference.Key);
                        cellidx = AddDataRow(true, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, omgTechnicalDebt, bcres.Reference.Name);

                        List<int?> technicalCriterionIds = reportData.RuleExplorer.GetCriteriaContributors(reportData.CurrentSnapshot.DomainId, bcres.Reference.Key.ToString(), reportData.CurrentSnapshot.Id).Select(_ => _.Key).ToList();
                        List<ApplicationResult> tcResults = technicalCriterionIds.Count > 0 ?
                            reportData.CurrentSnapshot.TechnicalCriteriaResults.Where(_ => technicalCriterionIds.Contains(_.Reference.Key)).OrderByDescending(_ => _.DetailResult.OmgTechnicalDebt?.Total).ToList()
                            : null;
                        if (tcResults?.Count > 0)
                        {
                            foreach (ApplicationResult tcres in tcResults)
                            {
                                string tcName = "    " + tcres.Reference?.Name;
                                OmgTechnicalDebtIdDTO tcOmgTechDebt = OmgTechnicalDebtUtility.GetOmgTechDebt(reportData.CurrentSnapshot, tcres.Reference.Key);
                                cellidx = AddDataRow(false, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, tcOmgTechDebt, tcName);
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
                            OmgTechnicalDebtIdDTO tcOmgTechDebt = OmgTechnicalDebtUtility.GetOmgTechDebt(reportData.CurrentSnapshot, tcres.Reference.Key);
                            cellidx = AddDataRow(false, displayEvolution, lbltotal, lbladded, lblremoved, indicatorName, cellProps, cellidx, headers, data, tcOmgTechDebt, tcName);
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

        private static int AddDataRow(bool detail, bool displayEvolution, string lbltotal, string lbladded, string lblremoved, string indicatorName, List<CellAttributes> cellProps, int cellidx, HeaderDefinition headers, List<string> data, OmgTechnicalDebtIdDTO omgTechDebt, string stdTagName)
        {
            if (omgTechDebt == null) return cellidx;
            var dataRow = headers.CreateDataRow();
            dataRow.Set(indicatorName, stdTagName);
            int total = omgTechDebt.Total > 0.0 ? 1 : 0;
            FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, total);
            cellidx++;
            dataRow.Set(lbltotal, omgTechDebt.Total != null ? omgTechDebt.Total.Value.ToString("N1") : Constants.No_Value);
            FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, total);
            cellidx++;
            if (displayEvolution)
            {
                dataRow.Set(lbladded, omgTechDebt.Added != null ? omgTechDebt.Added.Value.ToString("N1") : Constants.No_Value);
                FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, total);
                cellidx++;
                dataRow.Set(lblremoved, omgTechDebt.Removed != null ? omgTechDebt.Removed.Value.ToString("N1") : Constants.No_Value);
                FormatTableHelper.AddGrayOrBold(detail, cellProps, cellidx, total);
                cellidx++;
            }
            data.AddRange(dataRow);
            return cellidx;
        }
    }
}
