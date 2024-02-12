
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
using CastReporting.BLL.Computing;
using CastReporting.Domain.Imaging;
using CastReporting.Domain.Imaging.Constants;
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
    [Block("TRANSACTIONS_CHART")]
    public class TransactionsChart : GraphBlock
    {

        #region METHODS

        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop = options.GetIntOption("COUNT", 20);
            string filteringBc = options.GetOption("FILTER", "ROB");
            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ? reportData.PreviousSnapshot : reportData.CurrentSnapshot;
            bool fullNames = options.GetOption("NAME", "SHORT").ToUpper().Equals("FULL");

            List<string> rowData = new List<string>(new[] {
                Labels.TRI,
                Labels.Security,
                Labels.Efficiency,
                Labels.Robustness
            });
            int rowcount = 0;
            if (snapshot == null)
            {
                LogHelper.LogError(Labels.NoSnapshot);
                rowData.AddRange(new[] { Labels.NoSnapshot, "0", "0", "0" });
                rowcount++;
            }
            else
            {
                List<TransactionDetailsDTO> transactionsDetails = new List<TransactionDetailsDTO>();
                switch (filteringBc)
                {
                    case "SECU":
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Security, snapshot);
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Performance, snapshot);
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Robustness, snapshot);
                        transactionsDetails = nbLimitTop != -1 ?
                            transactionsDetails.OrderByDescending(_ => _.TriSecurity).Take(nbLimitTop).ToList()
                            : transactionsDetails.OrderByDescending(_ => _.TriSecurity).ToList();
                        break;
                    case "EFF":
                    case "PERF":
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Performance, snapshot);
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Robustness, snapshot);
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Security, snapshot);
                        transactionsDetails = nbLimitTop != -1 ?
                            transactionsDetails.OrderByDescending(_ => _.TriEfficiency).Take(nbLimitTop).ToList()
                            : transactionsDetails.OrderByDescending(_ => _.TriEfficiency).ToList();
                        break;
                    default:
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Robustness, snapshot);
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Security, snapshot);
                        RegisterTransactionsDetails(transactionsDetails, reportData, nbLimitTop, BusinessCriteria.Performance, snapshot);
                        transactionsDetails = nbLimitTop != -1 ?
                            transactionsDetails.OrderByDescending(_ => _.TriRobustness).Take(nbLimitTop).ToList()
                            : transactionsDetails.OrderByDescending(_ => _.TriRobustness).ToList();
                        break;
                }

                foreach (TransactionDetailsDTO trDetails in transactionsDetails)
                {
                    rowData.AddRange(new[] {
                        fullNames ? trDetails.Name : trDetails.ShortName,
                        trDetails.TriSecurity.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                        trDetails.TriEfficiency.GetValueOrDefault().ToString(CultureInfo.CurrentCulture),
                        trDetails.TriRobustness.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)
                    });
                }
                rowcount = transactionsDetails.Count;
            }

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = rowcount + 1,
                NbColumns = 4,
                Data = rowData,
                GraphOptions = null
            };
            return resultTable;
        }

        private void RegisterTransactionsDetails(List<TransactionDetailsDTO> transactionsDetails, ImagingData reportData, int nbLimitTop, BusinessCriteria bc, Snapshot snapshot)
        {
            // Get the transactions list
            List<Transaction> transactions = reportData.SnapshotExplorer.GetTransactions(snapshot.Href, bc.GetHashCode().ToString(), nbLimitTop)?.ToList();
            if (transactions != null)
            {
                if (!transactionsDetails.Any())
                {
                    foreach (Transaction tr in transactions)
                    {
                        TransactionDetailsDTO trDetails = new TransactionDetailsDTO
                        {
                            HRef = tr.HRef,
                            Name = tr.Name,
                            ShortName = tr.ShortName,
                            TriRobustness = 0,
                            TriEfficiency = 0,
                            TriSecurity = 0
                        };
                        switch (bc)
                        {
                            case BusinessCriteria.Security:
                                trDetails.TriSecurity = tr.TransactionRiskIndex;
                                break;
                            case BusinessCriteria.Performance:
                                trDetails.TriEfficiency = tr.TransactionRiskIndex;
                                break;
                            default:
                                trDetails.TriRobustness = tr.TransactionRiskIndex;
                                break;
                        }
                        transactionsDetails.Add(trDetails);
                    }
                }
                else
                {
                    foreach (Transaction tr in transactions)
                    {
                        TransactionDetailsDTO trItem = transactionsDetails.Where(_ => _.HRef.Equals(tr.HRef)).FirstOrDefault();
                        if (trItem == null)
                        {
                            trItem = new TransactionDetailsDTO
                            {
                                HRef = tr.HRef,
                                Name = tr.Name,
                                ShortName = tr.ShortName,
                                TriRobustness = 0,
                                TriEfficiency = 0,
                                TriSecurity = 0
                            };
                            transactionsDetails.Add(trItem);
                        }
                        switch (bc)
                        {
                            case BusinessCriteria.Security:
                                trItem.TriSecurity = tr.TransactionRiskIndex;
                                break;
                            case BusinessCriteria.Performance:
                                trItem.TriEfficiency = tr.TransactionRiskIndex;
                                break;
                            default:
                                trItem.TriRobustness = tr.TransactionRiskIndex;
                                break;
                        }
                    }
                }
            }
        }

        #endregion METHODS

    }
}

