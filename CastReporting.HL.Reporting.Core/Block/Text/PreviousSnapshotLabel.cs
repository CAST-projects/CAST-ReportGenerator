/*
 *   Copyright (c) 2024 CAST
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
using CastReporting.Reporting.Core.ReportingModel;

namespace CastReporting.Reporting.Highlight.Block.Text
{
    [Block("HL.PREVIOUS_SNAPSHOT_LABEL")]
    public class PreviousSnapshotLabel: HighlightTextBlock
    {
        public override string Content(HighlightData reportData, Dictionary<string, string> options)
        {
            var label = reportData?.Results.PreviousMetrics?.SnapshotLabel?.Trim() ?? "";
            return string.IsNullOrEmpty(label) ? FormatHelper.No_Value : label;
        }
    }
}
