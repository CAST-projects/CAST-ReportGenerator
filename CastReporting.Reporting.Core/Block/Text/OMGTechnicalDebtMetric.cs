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
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Helper;
using System.Collections.Generic;
using CastReporting.Domain;
using CastReporting.Reporting.Core.Languages;

namespace CastReporting.Reporting.Block.Text
{
    [Block("OMG_TECHNICAL_DEBT")]
    public class OMGTechnicalDebtMetric : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
            string index = options.GetOption("ID", "ISO");
            Snapshot snapshot = options.GetOption("SNAPSHOT", "CURRENT").ToUpper().Equals("PREVIOUS") ? reportData.PreviousSnapshot ?? null : reportData.CurrentSnapshot ?? null;
            if (snapshot == null) return Constants.No_Value;
            OmgTechnicalDebt omgTechDebt = reportData.SnapshotExplorer.GetOmgTechnicalDebt(reportData.Application.Href, index, snapshot.GetId());

            if (omgTechDebt?.Total != null)
            {
                double? result = (double) omgTechDebt.Total / 8 / 60;
                return result.HasValue ? $"{result.Value:N1} {Labels.Days}" : Constants.No_Value;
            }
            return Constants.No_Value;
        }
        #endregion METHODS
    }
}
