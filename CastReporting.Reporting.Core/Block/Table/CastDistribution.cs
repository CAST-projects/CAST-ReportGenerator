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
    public class CastDistribution : ImagingTableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            TableDefinition result = new TableDefinition();
            List<string> rowData = new List<string>();
            Constants.QualityDistribution distributionId;
            bool techno = options.GetOption("TECHNOLOGIES", "N").ToUpper().Equals("Y");
            bool module = options.GetOption("MODULES", "N").ToUpper().Equals("Y");
            int parId;
            if (null != options && options.ContainsKey("PAR") && int.TryParse(options["PAR"], out parId) && Enum.IsDefined(typeof(Constants.QualityDistribution), parId))
            {
                distributionId = (Constants.QualityDistribution)parId;
            }
            else
                distributionId = Constants.QualityDistribution.CostComplexityDistribution;

            var distributionName = CastComplexityUtility.GetCostComplexityName(reportData.CurrentSnapshot, distributionId.GetHashCode());
            rowData.AddRange(new[] { distributionName, Labels.Current, Labels.Previous, Labels.Evol, Labels.EvolPercent, Labels.TotalPercent });


            if (reportData?.CurrentSnapshot == null) return result;

            if (!module && !techno)
            {
                ComplexityValuesDTO values = new ComplexityValuesDTO
                {
                    SelectedLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "low"),
                    SelectedAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "average"),
                    SelectedHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "high"),
                    SelectedVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "very_high")
                };

                if (reportData.PreviousSnapshot != null)
                {
                    values.PreviousLowVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "low");
                    values.PreviousAveVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "average");
                    values.PreviousHigVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "high");
                    values.PreviousVhiVal = CastComplexityUtility.GetCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "very_high");
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
                        SelectedLowVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "low", m),
                        SelectedAveVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "average", m),
                        SelectedHigVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "high", m),
                        SelectedVhiVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "very_high", m)
                    };
                    if (reportData.PreviousSnapshot != null)
                    {
                        modValues.PreviousLowVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "low", m);
                        modValues.PreviousAveVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "average", m);
                        modValues.PreviousHigVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "high", m);
                        modValues.PreviousVhiVal = CastComplexityUtility.GetModuleCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "very_high", m);
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
                            SelectedLowVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "low", m, technology),
                            SelectedAveVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "average", m, technology),
                            SelectedHigVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "high", m, technology),
                            SelectedVhiVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "very_high", m, technology)
                        };

                        if (reportData.PreviousSnapshot != null)
                        {
                            moduleTechnoValues.PreviousLowVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "low", m, technology);
                            moduleTechnoValues.PreviousAveVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "average", m, technology);
                            moduleTechnoValues.PreviousHigVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "high", m, technology);
                            moduleTechnoValues.PreviousVhiVal = CastComplexityUtility.GetModuleTechnoCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "very_high", m, technology);
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
                        SelectedLowVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "low", technology),
                        SelectedAveVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "average", technology),
                        SelectedHigVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "high", technology),
                        SelectedVhiVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.CurrentSnapshot, distributionId.GetHashCode(), "very_high", technology)
                    };

                    if (reportData.PreviousSnapshot != null)
                    {
                        technoValues.PreviousLowVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "low", technology);
                        technoValues.PreviousAveVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "average", technology);
                        technoValues.PreviousHigVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "high", technology);
                        technoValues.PreviousVhiVal = CastComplexityUtility.GetTechnologyCostComplexityGrade(reportData.PreviousSnapshot, distributionId.GetHashCode(), "very_high", technology);
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
