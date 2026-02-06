/*
 *   Copyright (c) 2026 CAST
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
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Text
{

    [Block("TOTAL_VIOLATIONS_EVOLUTION")]
    public class TotalViolationsEvolution : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string strId = options.GetOption("ID", string.Empty);
            bool percent = options.GetOption("FORMAT", string.Empty).ToUpper().Equals("PERCENT");
            bool critical = options.GetOption("CRITICAL", "false").ToLower().Equals("true");

            // for a QR, the total violations correspond to the number of failed checks in violationRatio
            // for TC and BC, it corresponds to the total violations of evolution summary

            if (strId == null || reportData.PreviousSnapshot == null)
            {
                return Constants.No_Value;
            }
            int metricId = int.TryParse(strId, out int id) ? id : -1;
            if (metricId == -1)
            {
                return Constants.No_Value;
            }

            ViolStatMetricIdDTO curResults = RulesViolationUtility.GetViolStat(reportData.CurrentSnapshot, metricId);
            ViolStatMetricIdDTO prevResults = RulesViolationUtility.GetViolStat(reportData.PreviousSnapshot, metricId);

            double? curResult = 0;
            double? prevResult = 0;
            if (critical)
            {
                curResult = curResults?.TotalCriticalViolations;
                prevResult = prevResults?.TotalCriticalViolations;
            }
            else
            {
                curResult = curResults?.TotalViolations;
                prevResult = prevResults?.TotalViolations;
            }
            if (curResult == null || prevResult == null)
            {
                return Constants.No_Value;
            }
            double? evolution = (curResult - prevResult);
            string strEvol = evolution > 0 ? "+" + evolution.Value.ToString("N0") : evolution.Value.ToString("N0");
            double? evp = prevResult > 0.0 ? (curResult - prevResult) / prevResult : null;
            string evolPercent = evp != null ? FormatHelper.FormatPercent(evp, true) : Constants.No_Value;

            return percent ? evolPercent : strEvol;
        }
        #endregion METHODS
    }
}


