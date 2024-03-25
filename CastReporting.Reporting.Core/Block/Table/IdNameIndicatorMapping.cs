
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
using CastReporting.Domain.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Table
{
    [Block("ID_NAME_INDICATOR_MAPPING")]
    public class IdNameIndicatorMapping : ImagingTableBlock
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
                    ((int)BusinessCriteria.TechnicalQualityIndex).ToString(),
                    BusinessCriteria.Security.ToString(),
                    ((int)BusinessCriteria.Security).ToString(),
                    BusinessCriteria.Robustness.ToString(),
                    ((int)BusinessCriteria.Robustness).ToString(),
                    BusinessCriteria.Performance.ToString(),
                    ((int)BusinessCriteria.Performance).ToString(),
                    BusinessCriteria.Changeability.ToString(),
                    ((int)BusinessCriteria.Changeability).ToString(),
                    BusinessCriteria.Transferability.ToString(),
                    ((int)BusinessCriteria.Transferability).ToString(),
                    BusinessCriteria.ProgrammingPractices.ToString(),
                    ((int)BusinessCriteria.ProgrammingPractices).ToString(),
                    BusinessCriteria.ArchitecturalDesign.ToString(),
                    ((int)BusinessCriteria.ArchitecturalDesign).ToString(),
                    BusinessCriteria.Documentation.ToString(),
                    ((int)BusinessCriteria.Documentation).ToString(),
                    BusinessCriteria.SEIMaintainability.ToString(),
                    ((int)BusinessCriteria.SEIMaintainability).ToString(),
                    QualityDistribution.CostComplexityDistribution.ToString(),
                    ((int)QualityDistribution.CostComplexityDistribution).ToString(),
                    QualityDistribution.CyclomaticComplexityDistribution.ToString(),
                    ((int)QualityDistribution.CyclomaticComplexityDistribution).ToString(),
                    QualityDistribution.OOComplexityDistribution.ToString(),
                    ((int)QualityDistribution.OOComplexityDistribution).ToString(),
                    QualityDistribution.SQLComplexityDistribution.ToString(),
                    ((int)QualityDistribution.SQLComplexityDistribution).ToString(),
                    QualityDistribution.CouplingDistribution.ToString(),
                    ((int)QualityDistribution.CouplingDistribution).ToString(),
                    QualityDistribution.ClassFanOutDistribution.ToString(),
                    ((int)QualityDistribution.ClassFanOutDistribution).ToString(),
                    QualityDistribution.ClassFanInDistribution.ToString(),
                    ((int)QualityDistribution.ClassFanInDistribution).ToString(),
                    QualityDistribution.SizeDistribution.ToString(),
                    ((int)QualityDistribution.SizeDistribution).ToString()
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
