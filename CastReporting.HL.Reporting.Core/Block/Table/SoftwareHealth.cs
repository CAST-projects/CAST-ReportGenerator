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
using CastReporting.HL.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Core.Highlight.Constants;
using CastReporting.Reporting.Core.ReportingModel;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;

namespace CastReporting.Reporting.Block.Table
{
    [Block("HL.SOFTWARE_HEALTH")]
    public class SoftwareHealth : HighlightTableBlock
    {
        public override TableDefinition Content(HighlightData reportData, Dictionary<string, string> options) {
            bool showEvol = options.GetBoolOption("SHOW_EVOL");
            bool showEvolPercent = options.GetBoolOption("SHOW_EVOL_PERCENT");
            bool displayShortHeader = options.GetOption("HEADER") == "SHORT";

            List<string> rowData = new List<string>(displayShortHeader
                ? [" ", ShortLabels.SoftwareHealth, ShortLabels.SoftwareResiliency, ShortLabels.SoftwareAgility, ShortLabels.SoftwareElegance]
                : [" ", Labels.SoftwareHealth, Labels.SoftwareResiliency, Labels.SoftwareAgility, Labels.SoftwareElegance]
            );

            var curSnapshot = reportData.Results.CurrentMetrics;
            var prevSnapshot = reportData.Results.PreviousMetrics;

            rowData.AddRange([
                curSnapshot?.SnapshotLabel ?? FormatHelper.No_Value,
                curSnapshot?.SoftwareHealth.ToString("F2") ?? FormatHelper.No_Value,
                curSnapshot?.SoftwareResiliency.ToString("F2") ?? FormatHelper.No_Value,
                curSnapshot?.SoftwareAgility.ToString("F2")?? FormatHelper.No_Value,
                curSnapshot?.SoftwareElegance.ToString("F2")?? FormatHelper.No_Value,
            ]);

            if (curSnapshot != null && prevSnapshot != null) {
                var trend = curSnapshot.ComputeTrend(prevSnapshot);

                rowData.AddRange([
                    prevSnapshot.SnapshotLabel,
                    prevSnapshot?.SoftwareHealth.ToString("F2") ?? FormatHelper.No_Value,
                    prevSnapshot?.SoftwareResiliency.ToString("F2") ?? FormatHelper.No_Value,
                    prevSnapshot?.SoftwareAgility.ToString("F2")?? FormatHelper.No_Value,
                    prevSnapshot?.SoftwareElegance.ToString("F2")?? FormatHelper.No_Value,
                ]);

                if (showEvol) {
                    rowData.AddRange([
                        Labels.Evol,
                        trend.SoftwareHealth.ToString("F2") ?? FormatHelper.No_Value,
                        trend.SoftwareResiliency.ToString("F2") ?? FormatHelper.No_Value,
                        trend.SoftwareAgility.ToString("F2")?? FormatHelper.No_Value,
                        trend.SoftwareElegance.ToString("F2")?? FormatHelper.No_Value,
                    ]);
                }

                if (showEvolPercent) {
                    rowData.AddRange([
                        Labels.EvolPercent,
                        FormatPercent(trend.SoftwareHealth / curSnapshot.SoftwareHealth),
                        FormatPercent(trend.SoftwareResiliency/curSnapshot.SoftwareResiliency),
                        FormatPercent(trend.SoftwareAgility/curSnapshot.SoftwareAgility),
                        FormatPercent(trend.SoftwareElegance/curSnapshot.SoftwareElegance),
                    ]);
                }
            }

            return new TableDefinition {
                HasRowHeaders = true,
                HasColumnHeaders = true,
                NbRows = rowData.Count / 5,
                NbColumns = 5,
                Data = rowData
            };
        }
    }
}
