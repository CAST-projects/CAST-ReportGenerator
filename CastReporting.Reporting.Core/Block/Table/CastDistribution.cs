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
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System;
using System.Collections.Generic;


namespace CastReporting.Reporting.Block.Table
{
    [Block("CAST_DISTRIBUTION")]
    public class CastDistribution : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            TableDefinition result = new TableDefinition();
            List<string> rowData = new List<string>();
            QualityDistribution distributionId;
            bool techno = options.GetOption("TECHNOLOGIES", "N").ToUpper().Equals("Y");
            bool module = options.GetOption("MODULES", "N").ToUpper().Equals("Y");
            int parId;
            if (null != options && options.ContainsKey("PAR") && int.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(QualityDistribution), parId))
            {
                distributionId = (QualityDistribution)parId;
            }
            else
                distributionId = QualityDistribution.CostComplexityDistribution;

            var distributionName = reportData.CurrentSnapshot.GetCostComplexityName(distributionId);
            rowData.AddRange(new[] { distributionName, Labels.Current, Labels.Previous, Labels.Evol, Labels.EvolPercent, Labels.TotalPercent });


            if (reportData?.CurrentSnapshot == null) return result;

            if (!module && !techno)
            {
                ComplexityValuesDTO values = new ComplexityValuesDTO
                {
                    SelectedLowVal = reportData.CurrentSnapshot.GetCostComplexityGrade(distributionId, CategoryType.Low),
                    SelectedAveVal = reportData.CurrentSnapshot.GetCostComplexityGrade(distributionId, CategoryType.Average),
                    SelectedHigVal = reportData.CurrentSnapshot.GetCostComplexityGrade(distributionId, CategoryType.High),
                    SelectedVhiVal = reportData.CurrentSnapshot.GetCostComplexityGrade(distributionId, CategoryType.VeryHigh)
                };

                if (reportData.PreviousSnapshot != null)
                {
                    values.PreviousLowVal = reportData.PreviousSnapshot.GetCostComplexityGrade(distributionId, CategoryType.Low);
                    values.PreviousAveVal = reportData.PreviousSnapshot.GetCostComplexityGrade(distributionId, CategoryType.Average);
                    values.PreviousHigVal = reportData.PreviousSnapshot.GetCostComplexityGrade(distributionId, CategoryType.High);
                    values.PreviousVhiVal = reportData.PreviousSnapshot.GetCostComplexityGrade(distributionId, CategoryType.VeryHigh);
                }

                values.CalculateSelectedTotal();
                rowData.AddRange(values.GetComplexityLow(Labels.ComplexityLow));
                rowData.AddRange(values.GetComplexityAverage(Labels.ComplexityAverage));
                rowData.AddRange(values.GetComplexityHigh(Labels.ComplexityHigh));
                rowData.AddRange(values.GetComplexityExtreme(Labels.ComplexityExtreme));
            }
            else if (module && !techno)
            {
                foreach (Module m in reportData.CurrentSnapshot.Modules)
                {
                    ComplexityValuesDTO modValues = new ComplexityValuesDTO
                    {
                        SelectedLowVal = reportData.CurrentSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.Low, m),
                        SelectedAveVal = reportData.CurrentSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.Average, m),
                        SelectedHigVal = reportData.CurrentSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.High, m),
                        SelectedVhiVal = reportData.CurrentSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.VeryHigh, m)
                    };
                    if (reportData.PreviousSnapshot != null)
                    {
                        modValues.PreviousLowVal = reportData.PreviousSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.Low, m);
                        modValues.PreviousAveVal = reportData.PreviousSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.Average, m);
                        modValues.PreviousHigVal = reportData.PreviousSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.High, m);
                        modValues.PreviousVhiVal = reportData.PreviousSnapshot.GetModuleCostComplexityGrade(distributionId, CategoryType.VeryHigh, m);
                    };
                    rowData.AddRange(new[] { " ", " ", " ", " ", " ", " " });
                    rowData.AddRange(new[] { m.Name, " ", " ", " ", " ", " " });
                    modValues.CalculateSelectedTotal();
                    rowData.AddRange(modValues.GetComplexityLow(Labels.ComplexityLow));
                    rowData.AddRange(modValues.GetComplexityAverage(Labels.ComplexityAverage));
                    rowData.AddRange(modValues.GetComplexityHigh(Labels.ComplexityHigh));
                    rowData.AddRange(modValues.GetComplexityExtreme(Labels.ComplexityExtreme));
                };
            }
            else if (module && techno)
            {
                foreach (Module m in reportData.CurrentSnapshot.Modules)
                {
                    rowData.AddRange(new[] { " ", " ", " ", " ", " ", " " });
                    rowData.AddRange(new[] { m.Name, " ", " ", " ", " ", " " });
                    foreach (string technology in m.Technologies)
                    {
                        ComplexityValuesDTO moduleTechnoValues = new ComplexityValuesDTO
                        {
                            SelectedLowVal = reportData.CurrentSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.Low, m, technology),
                            SelectedAveVal = reportData.CurrentSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.Average, m, technology),
                            SelectedHigVal = reportData.CurrentSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.High, m, technology),
                            SelectedVhiVal = reportData.CurrentSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.VeryHigh, m, technology)
                        };

                        if (reportData.PreviousSnapshot != null)
                        {
                            moduleTechnoValues.PreviousLowVal = reportData.PreviousSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.Low, m, technology);
                            moduleTechnoValues.PreviousAveVal = reportData.PreviousSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.Average, m, technology);
                            moduleTechnoValues.PreviousHigVal = reportData.PreviousSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.High, m, technology);
                            moduleTechnoValues.PreviousVhiVal = reportData.PreviousSnapshot.GetModuleTechnoCostComplexityGrade(distributionId, CategoryType.VeryHigh, m, technology);
                        };
                        rowData.AddRange(new[] { technology, " ", " ", " ", " ", " " });
                        moduleTechnoValues.CalculateSelectedTotal();
                        rowData.AddRange(moduleTechnoValues.GetComplexityLow(Labels.ComplexityLow));
                        rowData.AddRange(moduleTechnoValues.GetComplexityAverage(Labels.ComplexityAverage));
                        rowData.AddRange(moduleTechnoValues.GetComplexityHigh(Labels.ComplexityHigh));
                        rowData.AddRange(moduleTechnoValues.GetComplexityExtreme(Labels.ComplexityExtreme));
                    }
                }
            }
            else if (!module && techno)
            {
                foreach (string technology in reportData.CurrentSnapshot.Technologies)
                {
                    ComplexityValuesDTO technoValues = new ComplexityValuesDTO
                    {
                        SelectedLowVal = reportData.CurrentSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.Low, technology),
                        SelectedAveVal = reportData.CurrentSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.Average, technology),
                        SelectedHigVal = reportData.CurrentSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.High, technology),
                        SelectedVhiVal = reportData.CurrentSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.VeryHigh, technology)
                    };

                    if (reportData.PreviousSnapshot != null)
                    {
                        technoValues.PreviousLowVal = reportData.PreviousSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.Low, technology);
                        technoValues.PreviousAveVal = reportData.PreviousSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.Average, technology);
                        technoValues.PreviousHigVal = reportData.PreviousSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.High, technology);
                        technoValues.PreviousVhiVal = reportData.PreviousSnapshot.GetTechnologyCostComplexityGrade(distributionId, CategoryType.VeryHigh, technology);
                    };
                    rowData.AddRange(new[] { " ", " ", " ", " ", " ", " " });
                    rowData.AddRange(new[] { technology, " ", " ", " ", " ", " " });
                    technoValues.CalculateSelectedTotal();
                    rowData.AddRange(technoValues.GetComplexityLow(Labels.ComplexityLow));
                    rowData.AddRange(technoValues.GetComplexityAverage(Labels.ComplexityAverage));
                    rowData.AddRange(technoValues.GetComplexityHigh(Labels.ComplexityHigh));
                    rowData.AddRange(technoValues.GetComplexityExtreme(Labels.ComplexityExtreme));
                }
            }

            result = new TableDefinition
            {
                Data = rowData,
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbColumns = 6,
                NbRows = rowData.Count / 6
            };
            return result;
        }
    }
}
