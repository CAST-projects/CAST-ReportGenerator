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
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Text
{

    [Block("TOTAL_VIOLATIONS")]
    public class TotalViolations : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string strId = options.GetOption("ID", string.Empty);
            string _snapshot = options.GetOption("SNAPSHOT", "CURRENT");
            bool critical = options.GetOption("CRITICAL", "false").ToLower().Equals("true");

            // for a QR, the total violations correspond to the number of failed checks in violationRatio
            // for TC and BC, it corresponds to the total violations of evolution summary

            if (strId != null)
            {
                int metricId = int.TryParse(strId, out int id) ? id : -1;
                if (metricId == -1)
                {
                    return Constants.No_Value;
                }
                ViolStatMetricIdDTO violStats;
                if (_snapshot == "PREVIOUS")
                {
                    if (reportData.PreviousSnapshot == null)
                    {
                        return Constants.No_Value;
                    }
                    violStats = RulesViolationUtility.GetViolStat(reportData.PreviousSnapshot, metricId);
                }
                else
                {
                    violStats = RulesViolationUtility.GetViolStat(reportData.CurrentSnapshot, metricId);
                }

                return critical ? violStats?.TotalCriticalViolations?.ToString("N0") ?? Constants.No_Value : violStats?.TotalViolations?.ToString("N0") ?? Constants.No_Value;
            }

            return Constants.No_Value;
        }
        #endregion METHODS
    }
}


