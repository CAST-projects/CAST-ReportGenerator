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
using CastReporting.BLL.Computing.DTO;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace CastReporting.Reporting.Block.Text
{
    [Block("APPLICATION_RULE"), Block("APPLICATION_METRIC")]
    public class ApplicationRule: ImagingTextBlock
    {
        #region METHODS
        public override string Content(ImagingData reportData, Dictionary<string, string> options)
        {

            string metricId = options.GetOption("ID") ?? options.GetOption("SZID") ?? options.GetOption("BFID") ?? null;
            string _format = options.GetOption("FORMAT", "N0");

            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ?
                reportData.PreviousSnapshot
                : reportData.CurrentSnapshot;

            string moduleName = options.GetOption("MODULE", null);
            string techno = options.GetOption("TECHNO", null);

            string[] lstParams = options.GetOption("PARAMS", string.Empty).Split(' ');
            string _expr = options.GetOption("EXPR", string.Empty);

            Module module = null;
            if (moduleName != null)
            {
                foreach (Module snapModule in reportData.CurrentSnapshot.Modules)
                {
                    if (snapModule.Name.Equals(moduleName))
                    {
                        module = snapModule;
                    }
                }
            }

            if (snapshot == null) return FormatHelper.No_Value;
            if (lstParams.Length > 0 && !string.IsNullOrEmpty(_expr))
            {
                double? exprRes = MetricsUtility.CustomExpressionDoubleEvaluation(reportData, options, lstParams, reportData.CurrentSnapshot, _expr, module, techno);
                return exprRes.HasValue ? exprRes.Value.ToString(_format) : FormatHelper.No_Value;
            }
            else if (string.IsNullOrEmpty(metricId)) return FormatHelper.No_Value;

            SimpleResult res = MetricsUtility.GetMetricNameAndResult(reportData, snapshot, metricId, module, techno, true);
            if (res == null) return FormatHelper.No_Value;
            return res.result.HasValue ? res.result.Value.ToString(_format) : FormatHelper.No_Value;

        }
        #endregion METHODS
    }
}
