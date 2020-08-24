/*
 *   Copyright (c) 2020 CAST
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
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Text
{
    [Block("METRIC_EVOLUTION")]
    public class MetricEvolution : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {

            string metricId = options.GetOption("ID", "60017");
            string _format = options.GetOption("FORMAT", "PERCENT");
            string moduleName = options.GetOption("MODULE", null);
            string techno = options.GetOption("TECHNO", null);

            string[] lstParams = options.GetOption("PARAMS", string.Empty).Split(' ');
            string _expr = options.GetOption("EXPR", string.Empty);

            if (reportData?.CurrentSnapshot == null || reportData?.PreviousSnapshot == null) return Constants.No_Value;

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

            EvolutionResult result = null;
            if (lstParams.Length > 0 && _expr != string.Empty)
            {
                double? curResult = MetricsUtility.CustomExpressionDoubleEvaluation(reportData, options, lstParams, reportData.CurrentSnapshot, _expr, module, techno);
                double? prevResult = MetricsUtility.CustomExpressionDoubleEvaluation(reportData, options, lstParams, reportData.PreviousSnapshot, _expr, module, techno);
                if (curResult == null || prevResult == null)
                {
                    return Labels.NoData;
                }
                string evolution = (curResult.Value - prevResult.Value).ToString("N2");
                double? evp = Math.Abs((double)prevResult) > 0.0 ? (curResult - prevResult) / prevResult : null;
                string evolPercent = evp != null ? evp.FormatPercent() : Constants.No_Value;
                result = new EvolutionResult()
                {
                    name = _expr,
                    type = MetricType.NotKnown,
                    curResult = curResult.ToString(),
                    prevResult = prevResult.ToString(),
                    evolution = evolution,
                    evolutionPercent = evolPercent
                };
            }
            else if (metricId != null)
            {
                result = MetricsUtility.GetMetricEvolution(reportData, reportData.CurrentSnapshot, reportData.PreviousSnapshot, metricId, true, module, techno, true);
            }

            if (result == null)
            {
                return Labels.NoData;
            }
            return _format.ToUpper().Equals("ABSOLUTE") ? result.evolution : result.evolutionPercent;
        }
        #endregion METHODS
    }
}
