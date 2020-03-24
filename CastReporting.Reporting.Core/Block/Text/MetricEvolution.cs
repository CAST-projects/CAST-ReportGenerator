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
using System.Collections.Generic;
using CastReporting.BLL.Computing.DTO;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;

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

            EvolutionResult result = MetricsUtility.GetMetricEvolution(reportData, reportData.CurrentSnapshot, reportData.PreviousSnapshot, metricId, true, module, techno, true);
            
            return _format.ToUpper().Equals("ABSOLUTE") ? result.evolution : result.evolutionPercent;
        }
        #endregion METHODS
    }
}
