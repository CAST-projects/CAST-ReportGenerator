/*
 *   Copyright (c) 2020 CAST
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
using System.Collections.Generic;
using System.Linq;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.ReportingModel;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Helper;
// ReSharper disable InconsistentNaming

namespace CastReporting.Reporting.Block.Text
{
	[Block("METRIC_EVOLUTION")]
    public class MetricEvolution : TextBlock
    {
        #region METHODS
        public override string Content(ReportData reportData, Dictionary<string, string> options)
        {
          
            int metricId = options.GetIntOption("ID");
            int metricSzId = options.GetIntOption("SZID");
            int metricBfId = options.GetIntOption("BFID");
            string _format = options.GetOption("FORMAT", "PERCENT");

            if (reportData?.CurrentSnapshot == null || reportData?.PreviousSnapshot == null) return Constants.No_Value;

            if (metricId != 0)
            {
                double? curResult = BusinessCriteriaUtility.GetMetricValue(reportData.CurrentSnapshot, metricId);
                double? prevResult = BusinessCriteriaUtility.GetMetricValue(reportData.PreviousSnapshot, metricId);
                if (prevResult == null || curResult == null) return Constants.No_Value;
                double? variation = _format.ToUpper().Equals("PERCENT") ? (curResult - prevResult) / prevResult
                    : curResult - prevResult;

                return _format.ToUpper().Equals("ABSOLUTE") ? variation.Value.ToString("N2") : FormatHelper.FormatPercent(variation);
            }

            if (metricSzId != 0)
            {
                double? curSize = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, metricSzId);
                double? prevSize = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot, metricSzId);
                if (prevSize == null || curSize == null) return Constants.No_Value;
                double? varSize = _format.ToUpper().Equals("PERCENT") ? (curSize - prevSize) / prevSize
                    : curSize - prevSize;
                return _format.ToUpper().Equals("ABSOLUTE") ? varSize.Value.ToString("N0") : FormatHelper.FormatPercent(varSize);
            }

            if (metricBfId != 0)
            {
                Result curBf = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.CurrentSnapshot.Href, metricBfId.ToString()).FirstOrDefault();
                Result prevBf = reportData.SnapshotExplorer.GetBackgroundFacts(reportData.PreviousSnapshot.Href, metricBfId.ToString()).FirstOrDefault();
                if (prevBf == null || !prevBf.ApplicationResults.Any() || curBf == null || !curBf.ApplicationResults.Any())
                    return Constants.No_Value;

                double? curBfValue = curBf.ApplicationResults[0].DetailResult?.Value;
                double? prevBfValue = prevBf.ApplicationResults[0].DetailResult?.Value;
                if (curBfValue == null || prevBfValue == null) return Constants.No_Value;
                double? varBf = _format.ToUpper().Equals("PERCENT") ? (curBfValue - prevBfValue) / prevBfValue
                    : curBfValue - prevBfValue;
                return _format.ToUpper().Equals("ABSOLUTE") ? varBf.Value.ToString("N0") : FormatHelper.FormatPercent(varBf);
            }

            return Constants.No_Value;
        }
        #endregion METHODS
    }
}
