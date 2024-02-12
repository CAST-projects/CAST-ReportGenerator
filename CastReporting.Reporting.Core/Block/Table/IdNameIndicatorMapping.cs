
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
using CastReporting.Domain.Imaging;
using CastReporting.Domain.Imaging.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Table
{
    [Block("ID_NAME_INDICATOR_MAPPING")]
    public class IdNameIndicatorMapping : TableBlock
    {

        #region METHODS

        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.Name, Labels.Id });

            if (reportData?.CurrentSnapshot == null) return null;
            rowData.AddRange(
                new[]
                {

                    BusinessCriteria.TechnicalQualityIndex.ToString(),
                    BusinessCriteria.TechnicalQualityIndex.GetHashCode().ToString(),
                    BusinessCriteria.Security.ToString(),
                    BusinessCriteria.Security.GetHashCode().ToString(),
                    BusinessCriteria.Robustness.ToString(),
                    BusinessCriteria.Robustness.GetHashCode().ToString(),
                    BusinessCriteria.Performance.ToString(),
                    BusinessCriteria.Performance.GetHashCode().ToString(),
                    BusinessCriteria.Changeability.ToString(),
                    BusinessCriteria.Changeability.GetHashCode().ToString(),
                    BusinessCriteria.Transferability.ToString(),
                    BusinessCriteria.Transferability.GetHashCode().ToString(),
                    BusinessCriteria.ProgrammingPractices.ToString(),
                    BusinessCriteria.ProgrammingPractices.GetHashCode().ToString(),
                    BusinessCriteria.ArchitecturalDesign.ToString(),
                    BusinessCriteria.ArchitecturalDesign.GetHashCode().ToString(),
                    BusinessCriteria.Documentation.ToString(),
                    BusinessCriteria.Documentation.GetHashCode().ToString(),
                    BusinessCriteria.SEIMaintainability.ToString(),
                    BusinessCriteria.SEIMaintainability.GetHashCode().ToString(),
                    QualityDistribution.CostComplexityDistribution.ToString(),
                    QualityDistribution.CostComplexityDistribution.GetHashCode().ToString(),
                    QualityDistribution.CyclomaticComplexityDistribution.ToString(),
                    QualityDistribution.CyclomaticComplexityDistribution.GetHashCode().ToString(),
                    QualityDistribution.OOComplexityDistribution.ToString(),
                    QualityDistribution.OOComplexityDistribution.GetHashCode().ToString(),
                    QualityDistribution.SQLComplexityDistribution.ToString(),
                    QualityDistribution.SQLComplexityDistribution.GetHashCode().ToString(),
                    QualityDistribution.CouplingDistribution.ToString(),
                    QualityDistribution.CouplingDistribution.GetHashCode().ToString(),
                    QualityDistribution.ClassFanOutDistribution.ToString(),
                    QualityDistribution.ClassFanOutDistribution.GetHashCode().ToString(),
                    QualityDistribution.ClassFanInDistribution.ToString(),
                    QualityDistribution.ClassFanInDistribution.GetHashCode().ToString(),
                    QualityDistribution.SizeDistribution.ToString(),
                    QualityDistribution.SizeDistribution.GetHashCode().ToString()

                });


            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = 37,
                NbColumns = 2,
                Data = rowData
            };
            return resultTable;
        }
        #endregion METHODS
    }
}
