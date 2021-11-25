
/*
 *   Copyright (c) 2021 CAST
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
using System.Globalization;
using System.Linq;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("OMG_TECH_DEBT_BUBBLE")]
    public class OmgTechDebtBubble : GraphBlock
    {
        #region METHODS

        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {


            #region Required Options
            string index = options.GetOption("ID", "ISO");
            int moduleId = options.GetIntOption("M", -1);
            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ? reportData.PreviousSnapshot ?? null : reportData.CurrentSnapshot ?? null;
            #endregion Required Options

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Grade, Labels.TechnicalDebt + " (" + Labels.Days + ")", Labels.Size });

            if (snapshot != null)
            {
                int idxId = OmgTechnicalDebtUtility.GetOmgIndex(index);
                double? _idxValue = 0;
                double? _omgTechDebtValue = 0;
                double? _locValue = 0;
                
                if (moduleId > 0)
                {
                    Module module = snapshot.Modules.FirstOrDefault(m => m.Id.Equals(moduleId));
                    if (module != null)
                    {
                        // snapshot is already in the module href, we have to send null as snapshot id (to avoid exception)
                        OmgTechnicalDebtIdDTO omgTechDebt = OmgTechnicalDebtUtility.GetOmgTechDebtModule(snapshot, module.Id, idxId);
                        if (omgTechDebt != null)
                        {
                            _idxValue = BusinessCriteriaUtility.GetBusinessCriteriaModuleGrade(snapshot, moduleId, idxId, true);
                            _omgTechDebtValue = omgTechDebt.Total ?? 0;
                            _locValue = MeasureUtility.GetSizingMeasureModule(snapshot, moduleId, Constants.SizingInformations.CodeLineNumber.GetHashCode());
                        }
                    }
                }
                else
                {
                    OmgTechnicalDebtIdDTO omgTechDebt = OmgTechnicalDebtUtility.GetOmgTechDebt(snapshot, idxId);
                    if (omgTechDebt != null)
                    {
                        _idxValue = BusinessCriteriaUtility.GetSnapshotBusinessCriteriaGrade(snapshot, idxId, true);
                        _omgTechDebtValue = omgTechDebt.Total ?? 0;
                        _locValue = MeasureUtility.GetSizingMeasure(snapshot, Constants.SizingInformations.CodeLineNumber);
                    }
                }

                rowData.Add(_idxValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                rowData.Add(_omgTechDebtValue.GetValueOrDefault().ToString("N1"));
                rowData.Add(_locValue.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                
            }
            else
            {
                rowData.AddRange(new[] { "0", "0.0", "0" });
            }

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 2,
                NbColumns = 3,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}



