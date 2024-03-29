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
using CastReporting.HL.Domain;
using CastReporting.HL.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Core.Highlight.Constants;
using CastReporting.Reporting.Core.ReportingModel;
using CastReporting.Reporting.Helper;
using Snapshot = CastReporting.Reporting.Core.Highlight.Constants.Snapshot;

namespace CastReporting.Reporting.Highlight.Block.Text
{
    [ Block("HL.SOFTWARE_HEALTH")]
    public class SoftwareHealth: HighlightTextBlock
    {
        public override string Content(HighlightData reportData, Dictionary<string, string> options)
        {
            SnapshotResults? snapshot;
            var targetSnapshot = options.GetOption("SNAPSHOT", Snapshot.Current);
            switch (targetSnapshot) {
                case Snapshot.Previous:
                    snapshot = reportData.Results.PreviousMetrics;
                    break;
                case Snapshot.Current:
                case Snapshot.Last:
                default:
                    snapshot = reportData.Results.CurrentMetrics;
                    break;
            }
            if (snapshot == null) return FormatHelper.No_Data;
            double? kpi = null;
            var indicator = options.GetOption("KPI", SoftwareKpi.Health);
            switch (indicator) {
                case SoftwareKpi.Health:
                    kpi = snapshot.SoftwareHealth;
                    break;
                case SoftwareKpi.Resiliency:
                    kpi = snapshot.SoftwareResiliency;
                    break;
                case SoftwareKpi.Agility:
                    kpi = snapshot.SoftwareAgility;
                    break;
                case SoftwareKpi.Elegance:
                    kpi = snapshot.SoftwareElegance;
                    break;
            }
            return kpi.HasValue? kpi.Value.ToString("F2"): FormatHelper.No_Value;
        }
    }
}
