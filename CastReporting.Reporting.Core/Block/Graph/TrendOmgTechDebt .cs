
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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("TREND_OMG_TECH_DEBT")]
    public class TrendOmgTechDebt : GraphBlock
    {

        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string index = options.GetOption("ID", "ISO");

            int count = 0;
            var rowData = new List<string>();
            rowData.AddRange(new[] {
                " ",
                Labels.DebtRemoved + " (" + Labels.Days + ")",
                Labels.DebtAdded + " (" + Labels.Days + ")",
                Labels.Debt + " (" + Labels.Days + ")"
            });


            int nbSnapshots = reportData.Application?.Snapshots.Count() ?? 0;
            if (nbSnapshots > 0)
            {
                var _snapshots = reportData.Application?.Snapshots.OrderBy(_ => _.Annotation.Date.DateSnapShot);
                if (_snapshots != null)
                {
                    List<string> listIds = _snapshots.Select(s => s.GetId()).ToList();
                    IEnumerable<Result> omgDebtsResults = reportData.SnapshotExplorer.GetOmgTechnicalDebtForSnapshots(reportData.Application.Href, index, string.Join(",", listIds));
                    
                    foreach (Snapshot snapshot in _snapshots)
                    {
                        double? snapshotDate = snapshot.Annotation.Date.DateSnapShot?.ToOADate() ?? 0;
                        rowData.Add(snapshotDate.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                        OmgTechnicalDebt snapshotTechDebt = omgDebtsResults?
                            .Where(r => r.Snapshot.GetId().Equals(snapshot.GetId()))
                            .Select(r => r.ApplicationResults.FirstOrDefault().DetailResult.OmgTechnicalDebt)
                            .FirstOrDefault();
                        if (snapshotTechDebt != null)
                        {
                            double? omgTechDebtRemoved = (double) snapshotTechDebt.Removed / 8 / 60 * -1;
                            double? omgTechDebtAdded = (double) snapshotTechDebt.Added / 8 / 60;
                            double? omgTechDebtValue = (double) snapshotTechDebt.Total / 8 / 60;

                            rowData.Add(omgTechDebtRemoved.GetValueOrDefault().ToString("N1"));
                            rowData.Add(omgTechDebtAdded.GetValueOrDefault().ToString("N1"));
                            rowData.Add(omgTechDebtValue.GetValueOrDefault().ToString("N1"));
                        } 
                        else
                        {
                            rowData.Add("0");
                            rowData.Add("0");
                            rowData.Add("0");
                        }
                    }
                    count = nbSnapshots;
                }
            }

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = count + 1,
                NbColumns = 4,
                Data = rowData,
                GraphOptions = null
            };
            return resultTable;
        }

        #endregion METHODS

    }
}

