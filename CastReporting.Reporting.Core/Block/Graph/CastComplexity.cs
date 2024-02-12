
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
using CastReporting.Domain.Imaging;
using CastReporting.Domain.Imaging.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Globalization;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("CAST_COMPLEXITY")]
    public class CastComplexity : GraphBlock
    {
        #region METHODS

        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {

            List<string> rowData = new List<string>();
            bool hasPreviousSnapshot = null != reportData.PreviousSnapshot;

            double? previousLowVal = null;
            double? previousAveVal = null;
            double? previousHigVal = null;
            double? previousVhiVal = null;

            if (reportData.CurrentSnapshot != null)
            {

                #region Selected Snapshot

                var selectedName = reportData.CurrentSnapshot.Annotation.Version;
                var selectedLowVal = reportData.CurrentSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                    CostComplexity.CostComplexityArtifacts_Low);
                var selectedAveVal = reportData.CurrentSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                    CostComplexity.CostComplexityArtifacts_Average);
                var selectedHigVal = reportData.CurrentSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                    CostComplexity.CostComplexityArtifacts_High);
                var selectedVhiVal = reportData.CurrentSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                    CostComplexity.CostComplexityArtifacts_VeryHigh);

                #endregion Selected Snapshot

                #region Previous Snapshot

                var previousName = hasPreviousSnapshot ? reportData.PreviousSnapshot.Annotation.Version : "No previous snapshot selected";

                if (hasPreviousSnapshot)
                {
                    previousLowVal = reportData.PreviousSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                        CostComplexity.CostComplexityArtifacts_Low);
                    previousAveVal = reportData.PreviousSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                        CostComplexity.CostComplexityArtifacts_Average);
                    previousHigVal = reportData.PreviousSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                        CostComplexity.CostComplexityArtifacts_High);
                    previousVhiVal = reportData.PreviousSnapshot.GetCostComplexityGrade(QualityDistribution.CostComplexityDistribution,
                        CostComplexity.CostComplexityArtifacts_VeryHigh);
                }
                #endregion Previous Snapshot


                #region Data
                rowData.Add(" ");
                rowData.Add(selectedName);
                if (hasPreviousSnapshot) { rowData.Add(previousName); }

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }

                rowData.Add(Labels.CplxLow);
                rowData.Add(selectedLowVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                if (hasPreviousSnapshot) { rowData.Add(previousLowVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)); }

                rowData.Add(Labels.CplxAverage);
                rowData.Add(selectedAveVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                if (hasPreviousSnapshot) { rowData.Add(previousAveVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)); }

                rowData.Add(Labels.CplxHigh);
                rowData.Add(selectedHigVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                if (hasPreviousSnapshot) { rowData.Add(previousHigVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)); }

                rowData.Add(Labels.CplxVeryHigh);
                rowData.Add(selectedVhiVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture));
                if (hasPreviousSnapshot) { rowData.Add(previousVhiVal.GetValueOrDefault().ToString(CultureInfo.CurrentCulture)); }

                rowData.Add(" ");
                rowData.Add("0");
                if (hasPreviousSnapshot) { rowData.Add("0"); }
                #endregion Data


            }
            TableDefinition back = new TableDefinition
            {
                Data = rowData,
                HasRowHeaders = false,
                HasColumnHeaders = false,
                NbColumns = hasPreviousSnapshot ? 3 : 2,
                NbRows = 7
            };

            return back;
        }
        #endregion METHODS
    }

}

