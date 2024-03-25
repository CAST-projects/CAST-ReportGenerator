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
    [Block("VIOLATION_STATISTICS_EVOLUTION")]
    public class ViolationStatisticsEvolution : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            const string metricFormat = "N0";
            if (reportData?.CurrentSnapshot == null) return null;

            #region currentSnapshot

            double? criticalViolation = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, SizingInformations.ViolationsToCriticalQualityRulesNumber);
            double? numCritPerFile = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, SizingInformations.ViolationsToCriticalQualityRulesPerFileNumber);
            string numCritPerFileIfNegative;
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- special case
            if (numCritPerFile == -1)
                numCritPerFileIfNegative = FormatHelper.No_Value;
            else
                numCritPerFileIfNegative = numCritPerFile?.ToString("N2") ?? FormatHelper.No_Value;
            double? _numCritPerKloc = MeasureUtility.GetSizingMeasure(reportData.CurrentSnapshot, SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);

            double? veryHighCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_VeryHigh);
            double? highCostComplexityViolations = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_High);

            double? veryHighCostComplexityArtefacts = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_VeryHigh);
            double? highCostComplexityArtefacts = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_High);

            #endregion currentSnapshot


            #region PreviousSnapshot

            double? criticalViolationPrev = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot,
                SizingInformations.ViolationsToCriticalQualityRulesNumber);

            double? numCritPerFilePrev = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot,
                SizingInformations.ViolationsToCriticalQualityRulesPerFileNumber);
            string numCritPerFilePrevIfNegative;
            // ReSharper disable once CompareOfFloatsByEqualityOperator -- special case
            if (numCritPerFilePrev == -1)
                numCritPerFilePrevIfNegative = FormatHelper.No_Value;
            else
                numCritPerFilePrevIfNegative = numCritPerFilePrev?.ToString("N2") ?? FormatHelper.No_Value;

            double? _numCritPerKlocPrev = MeasureUtility.GetSizingMeasure(reportData.PreviousSnapshot,
                SizingInformations.ViolationsToCriticalQualityRulesPerKLOCNumber);

            double? veryHighCostComplexityViolationsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_VeryHigh);

            double? highCostComplexityViolationsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                (int)QualityDistribution.DistributionOfDefectsToCriticalDiagnosticBasedMetricsPerCostComplexity,
                (int)DefectsToCriticalDiagnosticBasedMetricsPerCostComplexity.CostComplexityDefects_High);

            double? veryHighCostComplexityArtefactsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_VeryHigh);

            double? highCostComplexityArtefactsPrev = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot,
                (int)QualityDistribution.CostComplexityDistribution,
                (int)CostComplexity.CostComplexityArtifacts_High);

            #endregion PreviousSnapshot
            #region SumMetric

            double? _highveryHighCostComplexityArtefacts = MathUtility.GetSum(veryHighCostComplexityArtefacts, highCostComplexityArtefacts);
            double? _highveryHighCostComplexityViolations = MathUtility.GetSum(veryHighCostComplexityViolations, highCostComplexityViolations);
            double? _highveryHighCostComplexityArtefactsPrev = MathUtility.GetSum(veryHighCostComplexityArtefactsPrev, highCostComplexityArtefactsPrev);
            double? _highveryHighCostComplexityViolationsPrev = MathUtility.GetSum(veryHighCostComplexityViolationsPrev, highCostComplexityViolationsPrev);

            #endregion SumMetric

            #region evolutionPercMetric

            double? criticalViolationEvolPerc = MathUtility.GetVariationPercent(criticalViolation, criticalViolationPrev);
            double? numCritPerFileEvolPerc = MathUtility.GetVariationPercent(numCritPerFile, numCritPerFilePrev);
            double? _numCritPerKlocEvolPerc = MathUtility.GetVariationPercent(_numCritPerKloc, _numCritPerKlocPrev);
            double? _highveryHighCostComplexityViolationsEvolPerc = MathUtility.GetVariationPercent(_highveryHighCostComplexityViolations, _highveryHighCostComplexityViolationsPrev);
            double? _highveryHighCostComplexityArtefactsEvolPerc = MathUtility.GetVariationPercent(_highveryHighCostComplexityArtefacts, _highveryHighCostComplexityArtefactsPrev);

            #endregion evolutionPercMetric

            var rowData = new List<string>
            { Labels.Name
                , Labels.Current
                , Labels.Previous
                , Labels.EvolutionPercent
                , Labels.ViolationsCritical
                , criticalViolation?.ToString(metricFormat) ?? FormatHelper.No_Value
                , criticalViolationPrev?.ToString(metricFormat) ?? FormatHelper.No_Value
                , criticalViolationEvolPerc.HasValue ? FormatPercent(criticalViolationEvolPerc.Value): FormatHelper.No_Value

                , "  " + Labels.PerFile
                , numCritPerFileIfNegative
                , numCritPerFilePrevIfNegative
                , numCritPerFileEvolPerc.HasValue ? FormatPercent(numCritPerFileEvolPerc.Value) : FormatHelper.No_Value

                , "  " + Labels.PerkLoC
                , _numCritPerKloc?.ToString("N2") ?? FormatHelper.No_Value
                , _numCritPerKlocPrev?.ToString("N2") ?? FormatHelper.No_Value
                , _numCritPerKlocEvolPerc.HasValue ? FormatPercent(_numCritPerKlocEvolPerc.Value) : FormatHelper.No_Value

                , Labels.ComplexObjects
                , _highveryHighCostComplexityArtefacts?.ToString(metricFormat) ?? FormatHelper.No_Value
                , _highveryHighCostComplexityArtefactsPrev?.ToString(metricFormat) ?? FormatHelper.No_Value
                , _highveryHighCostComplexityArtefactsEvolPerc.HasValue ? FormatPercent(_highveryHighCostComplexityArtefactsEvolPerc.Value) : FormatHelper.No_Value

                , "  " + Labels.WithViolations
                , _highveryHighCostComplexityViolations?.ToString(metricFormat) ?? FormatHelper.No_Value
                , _highveryHighCostComplexityViolationsPrev?.ToString(metricFormat) ?? FormatHelper.No_Value
                , _highveryHighCostComplexityViolationsEvolPerc.HasValue ? FormatPercent(_highveryHighCostComplexityViolationsEvolPerc.Value) : FormatHelper.No_Value
            };

            var resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = false,
                NbRows = 6,
                NbColumns = 4,
                Data = rowData
            };
            return resultTable;
        }
    }
}
