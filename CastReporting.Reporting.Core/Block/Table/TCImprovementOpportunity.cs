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
using CastReporting.Domain;
using CastReporting.Domain.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Table
{
    [Block("TC_IMPROVEMENT_OPPORTUNITY")]
    public class TCImprovementOpportunity : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            int rowCount = 0;
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.TechnicalCriterionName, Labels.ViolationsCount, Labels.TotalChecks, Labels.Grade });

            #region Options

            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }

            int bcCriteriaId;
            if (null == options || !options.ContainsKey("PAR") || !int.TryParse(options["PAR"], out bcCriteriaId))
            {
                throw new ArgumentException("Impossible to build TC_IMPROVEMENT_OPPORTUNITY : Need business criterion id.");
            }
            #endregion Options


            var technicalCriticalViolation = RulesViolationUtility.GetTechnicalCriteriaViolations(reportData.CurrentSnapshot,
                                                                                                     (BusinessCriteria)bcCriteriaId,
                                                                                                     nbLimitTop);
            if (technicalCriticalViolation != null)
            {
                foreach (var item in technicalCriticalViolation)
                {
                    rowData.AddRange(new[]
                                    {
                                          item.Name
                                        , item.TotalFailed?.ToString("N0") ?? FormatHelper.No_Value
                                        , item.TotalChecks?.ToString("N0") ?? FormatHelper.No_Value
                                        , item.Grade?.ToString("N2") ?? FormatHelper.No_Value
                                   }
                                   );
                }

                rowCount = technicalCriticalViolation.Count;
            }

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowCount + 1,
                NbColumns = 4,
                Data = rowData
            };
            return resultTable;
        }
    }
}
