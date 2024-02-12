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

using CastReporting.Domain.Imaging.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TOP_RISKIEST_TRANSACTIONS")]
    public class TopRiskiestTransactions : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            const string metricFormat = "N0";
            int nbLimitTop;
            int nbRows = 0;
            List<string> rowData = new List<string>(new[] { Labels.TransactionEP, Labels.TRI });

            // Default Options
            BusinessCriteria businessCriteria = BusinessCriteria.Robustness;
            if (options != null && options.TryGetValue("SRC", out string source))
            {
                switch (source)
                {
                    case "PERF": businessCriteria = BusinessCriteria.Performance; break;
                    case "ROB": businessCriteria = BusinessCriteria.Robustness; break;
                    case "SEC": businessCriteria = BusinessCriteria.Security; break;
                    default: throw new ArgumentOutOfRangeException("SRC");
                }
            }

            if (options == null ||
                !options.ContainsKey("COUNT") ||
                !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
            }

            var bc = reportData.CurrentSnapshot.BusinessCriteriaResults.FirstOrDefault(_ => _.Reference.Key == (int)businessCriteria);

            if (bc != null)
            {
                bc.Transactions = reportData.SnapshotExplorer.GetTransactions(reportData.CurrentSnapshot.Href, bc.Reference.Key.ToString(), nbLimitTop)?.ToList();
                if (bc.Transactions != null && bc.Transactions.Any())
                {
                    foreach (var transaction in bc.Transactions)
                    {
                        rowData.Add(transaction.Name);
                        rowData.Add(transaction.TransactionRiskIndex.Value.ToString(metricFormat));
                        nbRows += 1;
                    }
                }
                else
                {
                    rowData.AddRange(new[] { Labels.NoItem, string.Empty });
                }
            }
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }

    }
}
