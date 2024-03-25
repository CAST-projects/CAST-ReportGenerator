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
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Table
{
    [Block("CAST_COMPLEXITY_WITH_VIOL")]
    public class CastComplexityWithViolation : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            TableDefinition back = new TableDefinition();
            List<string> rowData = new List<string>();
            const string numberFormat = "N0";

            if (reportData?.CurrentSnapshot?.CostComplexityResults == null) return back;

            #region Selected Snapshot


            double? nbArtifactLow = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_Low);
            double? nbArtifactAve = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_Average);
            double? nbArtifactHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_High);
            double? nbArtifactVeryHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_VeryHigh);

            double? nbViolationLow = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_Low);
            double? nbViolationAve = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_Average);
            double? nbViolationHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_High);
            double? nbViolationVeryHigh = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_VeryHigh);

            #endregion Selected Snapshot

            #region Data


            rowData.AddRange(new[] { Labels.Complexity, Labels.Artifacts, Labels.WithViolations });
            rowData.AddRange(new[] { Labels.CplxExtreme, nbArtifactVeryHigh?.ToString(numberFormat) ?? FormatHelper.No_Value, nbViolationVeryHigh?.ToString(numberFormat) ?? FormatHelper.No_Value });
            rowData.AddRange(new[] { Labels.CplxHigh, nbArtifactHigh?.ToString(numberFormat) ?? FormatHelper.No_Value, nbViolationHigh?.ToString(numberFormat) ?? FormatHelper.No_Value });
            rowData.AddRange(new[] { Labels.CplxAverage, nbArtifactAve?.ToString(numberFormat) ?? FormatHelper.No_Value, nbViolationAve?.ToString(numberFormat) ?? FormatHelper.No_Value });
            rowData.AddRange(new[] { Labels.CplxLow, nbArtifactLow?.ToString(numberFormat) ?? FormatHelper.No_Value, nbViolationLow?.ToString(numberFormat) ?? FormatHelper.No_Value });

            #endregion Data

            back = new TableDefinition
            {
                Data = rowData,
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbColumns = 3,
                NbRows = 4
            };
            return back;
        }
    }
}
