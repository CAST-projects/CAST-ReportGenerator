﻿
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


namespace CastReporting.Reporting.Block.Table
{
    [Block("OMG_TECHNICAL_DEBT_TABLE")]
    public class OmgTechnicalDebtTable : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            const string numberFormat = "N1";

            bool displayShortHeader = options.GetOption("HEADER","DAYS").ToUpper().Equals("SHORT");
            string index = options.GetOption("ID", "ISO");
            int indexId = OmgTechnicalDebtUtility.GetOmgIndex(index);
            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ? reportData.PreviousSnapshot ?? null : reportData.CurrentSnapshot ?? null;

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Name, Labels.Value });

            if (snapshot != null)
            {
                OmgTechnicalDebtIdDTO omgTechnicalDebt = OmgTechnicalDebtUtility.GetOmgTechDebt(snapshot, indexId);
                //Build Debt row          
                double? technicalDebtBuild = omgTechnicalDebt?.Total ?? null;
                rowData.AddRange(new[] {
                    displayShortHeader ? Labels.Debt : Labels.TechnicalDebt  + " (" + Labels.Days + ")",
                   technicalDebtBuild?.ToString(numberFormat) ?? Constants.No_Value
                });


                //Build Debt added row            
                double? technicalDebtadded = omgTechnicalDebt?.Added ?? null;
                rowData.AddRange(new[] {
                     displayShortHeader ? Labels.DebtAdded : Labels.TechnicalDebtAdded + " (" + Labels.Days + ")",
                   technicalDebtadded?.ToString(numberFormat) ?? Constants.No_Value
                });

                //Build Debt removed row            
                double? technicalDebtremoved = omgTechnicalDebt?.Removed ?? null;
                rowData.AddRange(new[] {
                     displayShortHeader ? Labels.DebtRemoved : Labels.TechnicalDebtRemoved + " (" + Labels.Days + ")",
                   technicalDebtremoved?.ToString(numberFormat) ?? Constants.No_Value
                });
            }
            if (rowData.Count == 2)
            {
                rowData.AddRange(new[] { displayShortHeader ? Labels.Debt : Labels.TechnicalDebt  + " (" + Labels.Days + ")", Constants.No_Value });
                rowData.AddRange(new[] { displayShortHeader ? Labels.DebtAdded : Labels.TechnicalDebtAdded + " (" + Labels.Days + ")", Constants.No_Value });
                rowData.AddRange(new[] { displayShortHeader ? Labels.DebtRemoved : Labels.TechnicalDebtRemoved + " (" + Labels.Days + ")", Constants.No_Value });

            }

            //Build Table Definition            
            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 3,
                NbColumns = 2,
                Data = rowData
            };

            return resultTable;
        }

    }
}
