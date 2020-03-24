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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Core.Languages;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using Cast.Util.Log;
using Cast.Util.Date;
using CastReporting.Reporting.Helper;
using Cast.Util;
using CastReporting.BLL.Computing.DTO;

namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_TABLE_RELEASE_PERFORMANCE")]
    public class PortfolioMetricsReleasePerformance : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            List<string> metrics = options.GetOption("ID", string.Empty).Trim().Split('|').ToList();
            List<double> strTargets = options.GetOption("TARGETS", "0").Trim().Split('|').Select(_=> Convert.ToDouble(_, System.Globalization.CultureInfo.InvariantCulture)).ToList();
            bool lonelyTarget = false;

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Metrics, Labels.PreviousScore, Labels.Target, Labels.CurrentScore, Labels.SLAViolations });

            if (metrics.Count != strTargets.Count)
            {
                double first = strTargets.FirstOrDefault();
                if (first == 0)
                {
                    LogHelper.LogError("No target selected. Please review configuration");
                    rowData.AddRange(new[] { "No target selected. Please review configuration", " ", " ", " ", " " });
                    return new TableDefinition
                    {
                        HasRowHeaders = false,
                        HasColumnHeaders = true,
                        NbRows = 2,
                        NbColumns = 5,
                        Data = rowData
                    };
                }
                LogHelper.LogWarn("Not the same number of metrics and targets, the first target will be used");
                lonelyTarget = true;
                strTargets = new List<double> { first };
            }

            List<double> _strSla = options.GetOption("SLA", "0").Trim().Split('|').Select(_=> Convert.ToDouble(_, System.Globalization.CultureInfo.InvariantCulture)).ToList();
            if (_strSla.Count() != 2)
            {
                LogHelper.LogError("Bad SLA configuration");
                rowData.AddRange(new[] { "Bad SLA configuration", " ", " ", " ", " " });
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 5,
                    Data = rowData
                };
            }


            if (reportData.Applications != null && reportData.Snapshots != null)
            {
                Application[] _allApps = reportData.Applications;
                Dictionary<Application, Snapshot> currentApplicationSnapshots = new Dictionary<Application, Snapshot>();
                PortfolioGenericContent.BuildApplicationSnapshots(currentApplicationSnapshots, reportData);
                Dictionary<Application, Snapshot> previousApplicationSnapshots = new Dictionary<Application, Snapshot>();
                PortfolioGenericContent.BuildApplicationPreviousQuarterSnapshots(previousApplicationSnapshots, reportData);

                double? lower = Math.Round(_strSla[0] / 100, 2);
                double? upper = Math.Round(_strSla[1] / 100, 2);

                foreach (string _metricId in metrics)
                {
                    string metricSla = string.Empty;
                    double target = 0;
                    SimpleResult currentRes = MetricsUtility.GetAggregatedMetric(reportData, currentApplicationSnapshots, _metricId, string.Empty, "AVERAGE", true);
                    if (currentRes == null)
                    {
                        continue;
                    }
                    SimpleResult previousRes = null;
                    if (previousApplicationSnapshots.Count() > 0)
                    {
                        previousRes = MetricsUtility.GetAggregatedMetric(reportData, previousApplicationSnapshots, _metricId, string.Empty, "AVERAGE", true);
                    }
                    if (lonelyTarget)
                    {
                        target = strTargets[0];
                        metricSla = (target - currentRes.result) / target > upper ? Labels.Bad 
                            : (target - currentRes.result) / target > lower ? Labels.Acceptable : Labels.Good;
                    }
                    else
                    {
                        int idx = metrics.IndexOf(_metricId);
                        target = strTargets[idx];
                        if (target != 0)
                        {
                            metricSla = (target - currentRes.result) / target > upper ? Labels.Bad
                                : (target - currentRes.result) / target > lower ? Labels.Acceptable : Labels.Good;
                        }
                    }
                    rowData.AddRange(new[] { 
                        currentRes.name,
                        previousRes != null ? previousRes.resultStr : Constants.No_Data,
                        target.ToString("N2"),
                        currentRes.resultStr,
                        metricSla });
                }
                
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowData.Count / 5,
                NbColumns = 5,
                Data = rowData
            };

            return resultTable;
        }
    }
}
