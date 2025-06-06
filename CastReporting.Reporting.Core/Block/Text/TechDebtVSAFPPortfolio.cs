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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Text
{
    [Block("PF_TECHDEBT_VS_AFP")]
    public class TechDebtVsafpPortfolio : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            if (reportData?.Applications == null) return Constants.No_Value;
            Application[] _allApps = reportData.Applications;
            double? _allTechDebt = 0;
            double? _afpAll = 0;

            foreach (Application _app in _allApps)
            {
                try
                {
                    Snapshot _snapshot = _app.Snapshots.OrderByDescending(_ => _.Annotation.Date.DateSnapShot).First();
                    if (_snapshot == null) continue;
                    double? result = MeasureUtility.GetTechnicalDebtMetric(_snapshot);
                    if (result != null)
                    {
                        _allTechDebt = _allTechDebt + result;
                    }

                    double? _resultAfp = MeasureUtility.GetAutomatedIFPUGFunction(_snapshot);
                    if (_resultAfp != null)
                    {
                        _afpAll = _afpAll + _resultAfp;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogInfo(ex.Message);
                    LogHelper.LogInfo(Labels.NoSnapshot);
                }
            }

            if (!(_allTechDebt > 0) || !(_afpAll > 0)) return Labels.NoData;
            double? _finalValue = _allTechDebt / _afpAll;
            return $"{_finalValue.Value:N0} {reportData.CurrencySymbol}";

        }
        #endregion METHODS
    }
}
