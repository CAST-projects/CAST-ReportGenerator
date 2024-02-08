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
using Cast.Util;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("BC_BY_TECHNO")]
    public class TQIbyTechno : TableBlock
    {

        /// <summary>
        /// 
        /// </summary>
        private const string MetricFormat = "N2";


        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbTot = 0;
            List<string> rowData = new List<string>();
            int? metricId = options != null && options.ContainsKey("ID") ? Convert.ToInt32(options["ID"]) : (int?)Constants.BusinessCriteria.TechnicalQualityIndex;

            var result = reportData?.CurrentSnapshot?.BusinessCriteriaResults.FirstOrDefault(r => r.Reference.Key == metricId);

            if (result != null)
            {
                string value = Text(metricId.Value);
                rowData.AddRange(new[] { Labels.Techno, value });
                foreach (var res in result.TechnologyResult)
                {
                    rowData.AddRange(new[] { res.Technology, res.DetailResult.Grade?.ToString(MetricFormat) ?? FormatHelper.No_Value });
                }
                nbTot = result.TechnologyResult.Length;
            }
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbTot + 1,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }

        private static string Text(int value)
        {
            // Begin the switch.
            switch (value)
            {
                case (int)Constants.BusinessCriteria.TechnicalQualityIndex: return Labels.TQI;
                case (int)Constants.BusinessCriteria.ArchitecturalDesign: return Labels.ArchitecturalDesign;
                case (int)Constants.BusinessCriteria.Changeability: return Labels.Changeability;
                case (int)Constants.BusinessCriteria.Documentation: return Labels.Documentation;
                case (int)Constants.BusinessCriteria.Performance: return Labels.Efficiency;
                case (int)Constants.BusinessCriteria.ProgrammingPractices: return Labels.ProgrammingPractices;
                case (int)Constants.BusinessCriteria.Robustness: return Labels.Robustness;
                case (int)Constants.BusinessCriteria.Security: return Labels.Security;
                case (int)Constants.BusinessCriteria.SEIMaintainability: return Labels.SEIMaintainability;
                case (int)Constants.BusinessCriteria.Transferability: return Labels.Transferability;
                default: return FormatHelper.No_Value + " (" + value + ")";
            }
        }



    }
}
