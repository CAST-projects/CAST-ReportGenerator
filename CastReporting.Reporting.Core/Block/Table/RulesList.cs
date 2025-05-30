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
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("RULES_LIST")]
    public class RulesList : TableBlock
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reportData"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string srBusinessCriterias = options != null && options.ContainsKey("PAR") ? options["PAR"] : null;
            bool showNoViolationRules = options.GetOption("NOVIOLATIONS", "true").Equals("true");

            int count;
            if (options == null || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out count))
            {
                count = -1;
            }

            if (string.IsNullOrWhiteSpace(srBusinessCriterias)) return null;
            // Parse business criterias Ids
            List<int> businessCriteriasIds = new List<int>();
            string[] parentMetrics = srBusinessCriterias.Split('|');
            foreach (var metric in parentMetrics.Distinct())
            {
                int metricId;
                if (int.TryParse(metric, out metricId))
                {
                    businessCriteriasIds.Add(metricId);
                }
            }

            //Build result
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Criticality, Labels.Weight, Labels.Grade, Labels.TechnicalCriterion, Labels.RuleName, Labels.ViolCount, Labels.TotalChecks });

            var results = RulesViolationUtility.GetNbViolationByRule(reportData.CurrentSnapshot, reportData.RuleExplorer, businessCriteriasIds, count);
            int nbRows = 0;
            foreach (var item in results)
            {
                if (!showNoViolationRules && item.TotalFailed == 0)
                {
                    continue;
                }

                rowData.Add(item.Rule.Critical ? "y" : string.Empty);
                rowData.Add(item.Rule.CompoundedWeight.ToString());
                rowData.Add(item.Grade?.ToString("N2"));
                rowData.Add(item.TechnicalCriteriaName);
                rowData.Add(item.Rule.Name);

                rowData.Add(item.TotalFailed?.ToString("N0") ?? Constants.No_Value);
                rowData.Add(item.TotalChecks?.ToString("N0") ?? Constants.No_Value);

                nbRows++;
            }

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 7,
                Data = rowData
            };

            return resultTable;
        }




    }
}
