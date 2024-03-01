
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
    [Block("OMG_TECHNICAL_DEBT_DETAILS_TABLE")]
    public class OmgTechnicalDebtDetailsTable : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            string index = options.GetOption("ID", "ISO");
            int indexId = OmgTechnicalDebtUtility.GetOmgIndex(index);
            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ? reportData.PreviousSnapshot ?? null : reportData.CurrentSnapshot ?? null;

            var headers = new HeaderDefinition();
            headers.Append(Labels.TechnicalCriterion);
            string techDebtLabel = Labels.TechnicalDebt + " (" + Labels.Days + ")";
            headers.Append(techDebtLabel);
            string techDebtAddedLabel = Labels.TechnicalDebtAdded + " (" + Labels.Days + ")";
            headers.Append(techDebtAddedLabel);
            string techDebtRemovedLabel = Labels.TechnicalDebtRemoved + " (" + Labels.Days + ")";
            headers.Append(techDebtRemovedLabel);

            var data = new List<string>();

            if (snapshot != null)
            {
                Result res = reportData.SnapshotExplorer.GetOmgTechnicalDebtDetailsForSnapshots(reportData.Application.Href, indexId, snapshot.GetId()).FirstOrDefault();
                if (res != null)
                {
                    foreach (ApplicationResult appRes in res.ApplicationResults)
                    {
                        var dataRow = headers.CreateDataRow();
                        dataRow.Set(Labels.TechnicalCriterion, appRes.Reference.Name);
                        OmgTechnicalDebt omgTechDebt = appRes.DetailResult?.OmgTechnicalDebt;
                        if (omgTechDebt != null)
                        {
                            double? techDebt = (double?)omgTechDebt?.Total / 8 / 60;
                            dataRow.Set(techDebtLabel, techDebt.HasValue ? techDebt.Value.ToString("N1") : Constants.No_Value);
                            double? techDebtAdded = (double?)omgTechDebt?.Added / 8 / 60;
                            dataRow.Set(techDebtAddedLabel, techDebtAdded.HasValue ? techDebtAdded.Value.ToString("N1") : Constants.No_Value);
                            double? techDebtRemoved = (double?)omgTechDebt?.Removed / 8 / 60;
                            dataRow.Set(techDebtRemovedLabel, techDebtRemoved.HasValue ? techDebtRemoved.Value.ToString("N1") : Constants.No_Value);
                            data.AddRange(dataRow);
                        }
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(Labels.TechnicalCriterion, Constants.No_Data);
                dataRow.Set(techDebtLabel, Constants.No_Data);
                dataRow.Set(techDebtAddedLabel, Constants.No_Data);
                dataRow.Set(techDebtRemovedLabel, Constants.No_Data);
                data.AddRange(dataRow);
            }

            data.InsertRange(0, headers.Labels);

            //Build Table Definition            
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = data.Count / headers.Count,
                NbColumns = headers.Count,
                Data = data
            };

            return resultTable;
        }

    }
}
