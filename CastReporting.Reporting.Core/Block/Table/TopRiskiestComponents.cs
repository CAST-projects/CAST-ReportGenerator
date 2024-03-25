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
using CastReporting.Domain;
using CastReporting.Domain.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;


namespace CastReporting.Reporting.Block.Table
{
    [Block("TOP_RISKIEST_COMPONENTS")]
    public class TopRiskiestComponents : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            const string metricFormat = "N0";
            int nbLimitTop;
            string dataSource = string.Empty;
            int moduleId = -1;

            #region Business Criteria
            if (options != null &&
                options.ContainsKey("SRC"))
            {
                dataSource = options["SRC"];
            }
            #endregion Business Criteria

            #region Module Id
            if (options != null &&
                options.ContainsKey("MOD") &&
                !int.TryParse(options["MOD"], out moduleId))
            {
                moduleId = -1;
            }
            #endregion Module Id

            #region Item Count
            if (options == null ||
                !options.ContainsKey("COUNT") ||
                !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = reportData.Parameter.NbResultDefault;
            }
            #endregion Item Count

            List<string> rowData = new List<string>();
            int nbRows = 1;

            if (reportData.CurrentSnapshot == null) return null;
            rowData.AddRange(new[] { Labels.ArtifactName, Labels.PRI });
            BusinessCriteria bizCrit;
            switch (dataSource)
            {
                case "ARCH": { bizCrit = BusinessCriteria.ArchitecturalDesign; } break;
                case "CHAN": { bizCrit = BusinessCriteria.Changeability; } break;
                case "DOC": { bizCrit = BusinessCriteria.Documentation; } break;
                case "PERF": { bizCrit = BusinessCriteria.Performance; } break;
                case "PROG": { bizCrit = BusinessCriteria.ProgrammingPractices; } break;
                case "ROB": { bizCrit = BusinessCriteria.Robustness; } break;
                case "SEC": { bizCrit = BusinessCriteria.Security; } break;
                case "MAIN": { bizCrit = BusinessCriteria.SEIMaintainability; } break;
                case "TQI": { bizCrit = BusinessCriteria.TechnicalQualityIndex; } break;
                case "TRAN": { bizCrit = BusinessCriteria.Transferability; } break;
                default: { bizCrit = BusinessCriteria.Transferability; } break;
            }


            IEnumerable<Component> components = moduleId > 0
                ? reportData.SnapshotExplorer.GetComponentsByModule(reportData.CurrentSnapshot.DomainId, moduleId, (int)reportData.CurrentSnapshot.Id, ((int)bizCrit).ToString(), nbLimitTop)?.ToList()
                : reportData.SnapshotExplorer.GetComponents(reportData.CurrentSnapshot.Href, ((int)bizCrit).ToString(), nbLimitTop)?.ToList();

            if (components != null && components.Any())
            {
                foreach (var component in components)
                {
                    rowData.AddRange(new[] { component.Name, component.PropagationRiskIndex.ToString(metricFormat) });
                    nbRows++;
                }
            }
            else
            {
                rowData.AddRange(new[] { Labels.NoItem, string.Empty });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows + 1,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }
    }
}
