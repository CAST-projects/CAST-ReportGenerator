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
using CastReporting.BLL.Computing;
using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace CastReporting.Reporting.Block.Text
{
    [Block("APPLICATION_RULE"), Block("APPLICATION_METRIC")]
    public class ApplicationRule : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {

            string metricId = options.GetOption("ID") ?? options.GetOption("SZID") ?? options.GetOption("BFID") ?? null;
            string _format = options.GetOption("FORMAT", "N0");

            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ?
                reportData.PreviousSnapshot
                : reportData.CurrentSnapshot;

            if (string.IsNullOrEmpty(metricId)) return Constants.No_Value;
            if (snapshot == null) return Constants.No_Value;

            SimpleResult res = MetricsUtility.GetMetricNameAndResult(reportData, snapshot, metricId, null, string.Empty, true);
            return res.result.HasValue ? res.result.Value.ToString(_format) : Constants.No_Value;

        }
        #endregion METHODS
    }
}
