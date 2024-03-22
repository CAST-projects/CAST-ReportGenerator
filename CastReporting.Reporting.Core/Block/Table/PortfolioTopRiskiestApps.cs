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
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace CastReporting.Reporting.Block.Table
{
    [Block("PF_TOP_RISKIEST_APPS")]
    public class PortfolioTopRiskiestApps : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            int metricId = options.GetIntOption("ALT", (int)Constants.BusinessCriteria.TechnicalQualityIndex);
            int nbLimitTop = options.GetIntOption("COUNT", reportData.Parameter.NbResultDefault);

            List<string> rowData = new List<string>();
            int nbRows = 0;

            if (reportData.Applications == null || reportData.Snapshots == null) return null;
            switch (metricId)
            {
                case 60012:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.Changeability, Labels.SnapshotDate });
                    break;
                case 60014:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.Efficiency, Labels.SnapshotDate });
                    break;
                case 60013:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.Robustness, Labels.SnapshotDate });
                    break;
                case 60016:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.Security, Labels.SnapshotDate });
                    break;
                case 60011:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.Transferability, Labels.SnapshotDate });
                    break;
                case 60017:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.TQI, Labels.SnapshotDate });
                    break;
                default:
                    rowData.AddRange(new[] { Labels.Application, Labels.ViolationsCritical, Labels.TQI, Labels.SnapshotDate });
                    break;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("AppName", typeof(string));
            dt.Columns.Add("CV", typeof(double));
            dt.Columns.Add("BizCritScore", typeof(double));
            dt.Columns.Add("LastAnalysis", typeof(string));


            Application[] _allApps = reportData.Applications;
            foreach (Application _app in _allApps)
            {
                try
                {
                    Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                    if (_snapshot == null) continue;
                    string strAppName = _app.Name;
                    double? _cv = RulesViolationUtility.GetBCEvolutionSummary(_snapshot, metricId).FirstOrDefault()?.TotalCriticalViolations;
                    double? strCurrentBCGrade = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(_snapshot, metricId, false);

                    if (_snapshot.Annotation.Date.DateSnapShot != null)
                    {
                        string strLastAnalysis = Convert.ToDateTime(_snapshot.Annotation.Date.DateSnapShot.Value).ToString("MMM dd yyyy");
                        dt.Rows.Add(strAppName, _cv.Value, strCurrentBCGrade, strLastAnalysis);
                    }
                    nbRows++;
                }
                catch (Exception ex)
                {
                    LogHelper.LogInfo(ex.Message);
                    LogHelper.LogInfo(Labels.NoSnapshot);
                }
            }
            DataView dv = dt.DefaultView;
            dv.Sort = "BizCritScore";
            DataTable dtSorted = dv.ToTable();

            if (nbRows < nbLimitTop)
            {
                nbLimitTop = nbRows;
            }

            for (int i = 0; i < nbLimitTop; i++)
            {
                rowData.AddRange
                (new[] {
                    dtSorted.Rows[i]["AppName"].ToString()
                    , $"{Convert.ToDouble(dtSorted.Rows[i]["CV"]):N0}"
                    , $"{Convert.ToDouble(dtSorted.Rows[i]["BizCritScore"]):N2}"
                    , dtSorted.Rows[i]["LastAnalysis"].ToString()
                });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbLimitTop + 1,
                NbColumns = 4,
                Data = rowData
            };
            return resultTable;
        }
    }
}
