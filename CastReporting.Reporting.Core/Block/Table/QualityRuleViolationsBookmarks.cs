using CastReporting.Domain.Imaging;
using CastReporting.Domain.Imaging.Constants;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("QUALITY_RULE_VIOLATIONS_BOOKMARKS")]
    public class QualityRuleViolationsBookmarks : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;
            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.

            string ruleId = options.GetOption("ID", "7788");
            const int bcId = (int)BusinessCriteria.TechnicalQualityIndex;
            int nbLimitTop = options.GetIntOption("COUNT", 5);
            string showDescription = options.GetOption("DESC", "no").ToLower();

            bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
            RuleDescription rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, ruleId);
            string ruleName = rule.Name;

            rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);
            cellidx++;

            switch (showDescription)
            {
                case "simple":
                    cellidx = MetricsUtility.AddSimpleDescription(rowData, cellProps, cellidx, rule);
                    break;
                case "full":
                    cellidx = MetricsUtility.AddFullDescription(rowData, cellProps, cellidx, rule);
                    break;
                default:
                    break;
            }

            if (reportData.Application.DomainType != null && reportData.Application.DomainType.Equals("AAD"))
            {
                rowData.Add(Labels.BadDomain);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = rowData.Count,
                    NbColumns = 1,
                    Data = rowData
                };
            }

            IEnumerable<Violation> results = reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.CurrentSnapshot.Href, ruleId, bcId, nbLimitTop, "$all");
            if (results != null)
            {
                var _violations = results as Violation[] ?? results.ToArray();
                if (_violations.Length != 0)
                {
                    MetricsUtility.ViolationsBookmarksProperties violationsBookmarksProperties =
                        new MetricsUtility.ViolationsBookmarksProperties(_violations, 0, rowData, ruleName, hasPreviousSnapshot, reportData.CurrentSnapshot.DomainId, reportData.CurrentSnapshot.Id.ToString(), ruleId);
                    MetricsUtility.PopulateViolationsBookmarks(reportData, violationsBookmarksProperties, cellidx, cellProps, true);
                }
                else
                {
                    rowData.Add(Labels.NoItem);
                }
            }
            else
            {
                rowData.Add(Labels.NoItem);
            }

            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = rowData.Count,
                NbColumns = 1,
                Data = rowData,
                CellsAttributes = cellProps
            };

            return table;

        }

    }
}
