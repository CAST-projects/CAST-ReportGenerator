﻿using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("LIST_TAGS_DOC_BYCAT")]
    public class ListTagsDocByCategory : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            List<string> categories = options.GetOption("CAT").Trim().Split('|').ToList();

            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            var headers = new HeaderDefinition();
            headers.Append(Labels.Standards);
            cellidx++;
            headers.Append(Labels.Definition);
            cellidx++;
            headers.Append(Labels.Applicability);
            cellidx++;

            var data = new List<string>();

            if (!VersionUtil.Is112Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.12 at least for component LIST_TAGS_DOC_BYCAT");
                var dataRow = headers.CreateDataRow();
                dataRow.Set(Labels.Standards, Labels.NoData);
                dataRow.Set(Labels.Definition, string.Empty);
                dataRow.Set(Labels.Applicability, string.Empty);
                data.AddRange(dataRow);
                data.InsertRange(0, headers.Labels);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = 2,
                    NbColumns = 3,
                    Data = data
                };
            }

            bool moreThanOne = categories.Count > 1;
            if (categories.Count > 0)
            {
                foreach (var category in categories)
                {
                    if (moreThanOne)
                    {
                        var dataRowCat = headers.CreateDataRow();
                        dataRowCat.Set(Labels.Standards, category);
                        FormatTableHelper.AddGrayAndBold(cellProps, cellidx);
                        cellidx++;
                        dataRowCat.Set(Labels.Definition, "");
                        FormatTableHelper.AddGrayAndBold(cellProps, cellidx);
                        cellidx++;
                        dataRowCat.Set(Labels.Applicability, "");
                        FormatTableHelper.AddGrayAndBold(cellProps, cellidx);
                        cellidx++;
                        data.AddRange(dataRowCat);
                    }
                    List<StandardTag> tagsDoc = reportData.RuleExplorer.GetQualityStandardTagsApplicabilityByCategory(reportData.Application.DomainId, category)?.ToList();
                    if (tagsDoc != null && tagsDoc.Count > 0)
                    {
                        foreach (var doc in tagsDoc)
                        {
                            bool isApplicable = doc.Applicable.Equals("true");
                            var dataRow = headers.CreateDataRow();
                            dataRow.Set(Labels.Standards, doc.Key);
                            FormatTableHelper.AddColorsIfCondition(isApplicable, cellProps, cellidx, "MintCream", "BlanchedAlmond");
                            cellidx++;
                            dataRow.Set(Labels.Definition, doc.Name);
                            FormatTableHelper.AddColorsIfCondition(isApplicable, cellProps, cellidx, "MintCream", "BlanchedAlmond");
                            cellidx++;
                            dataRow.Set(Labels.Applicability, isApplicable ? Labels.Applicable : Labels.NotApplicable);
                            FormatTableHelper.AddColorsIfCondition(isApplicable, cellProps, cellidx, "MintCream", "BlanchedAlmond");
                            cellidx++;
                            data.AddRange(dataRow);
                        }
                    }
                }
            }

            if (data.Count == 0)
            {
                var dataRow = headers.CreateDataRow();
                dataRow.Set(Labels.Standards, Labels.NoRules);
                dataRow.Set(Labels.Definition, string.Empty);
                dataRow.Set(Labels.Applicability, string.Empty);
                data.AddRange(dataRow);
            }

            data.InsertRange(0, headers.Labels);

            return new TableDefinition
            {
                Data = data,
                HasColumnHeaders = true,
                HasRowHeaders = false,
                NbColumns = headers.Count,
                NbRows = data.Count / headers.Count,
                CellsAttributes = cellProps
            };
        }
    }
}
