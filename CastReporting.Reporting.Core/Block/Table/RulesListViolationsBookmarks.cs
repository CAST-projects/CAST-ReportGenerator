using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.BLL.Computing;
using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("LIST_RULES_VIOLATIONS_BOOKMARKS")]
    public class RulesListViolationsBookmarks : ImagingTableBlock
    {
        private const string ColorWhite = "White";
        private const string ColorGray = "Gray";

        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;
            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.

            List<string> metrics = options.GetOption("METRICS").Trim().Split('|').ToList();
            bool critical = options.GetOption("CRITICAL", "false").ToLower().Equals("true");
            bool displayHeader = !options.GetOption("HEADER", "YES").ToUpper().Equals("NO");
            string showDescription = options.GetOption("DESC", "simple").ToLower();
            bool omg = options.GetOption("OMG", "false").ToLower().Equals("true");

            if (!VersionUtil.Is111Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.11 at least for component LIST_RULES_VIOLATIONS_BOOKMARKS");
                if (displayHeader) rowData.Add(Labels.Violations);
                rowData.Add(Labels.NoData);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = displayHeader,
                    NbRows = displayHeader ? 2 : 1,
                    NbColumns = 1,
                    Data = rowData
                };
            }

            List<string> qualityRules = MetricsUtility.BuildRulesList(reportData, metrics, critical, omg);

            if (displayHeader)
            {
                rowData.Add(Labels.Violations);
                cellidx++;
            }

            if (reportData.Application.DomainType != null && reportData.Application.DomainType.Equals("AAD"))
            {
                rowData.Add(Labels.BadDomain);
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = displayHeader,
                    NbRows = rowData.Count,
                    NbColumns = 1,
                    Data = rowData
                };
            }

            if (qualityRules.Count > 0)
            {
                const string bcId = "60017";
                int nbLimitTop = options.GetIntOption("COUNT", 5);
                bool hasPreviousSnapshot = reportData.PreviousSnapshot != null;
                bool displaySourceCode = options.GetOption("WITHCODELINES", "Y").Equals("Y");

                foreach (string _metric in qualityRules)
                {
                    RuleDescription rule = reportData.RuleExplorer.GetSpecificRule(reportData.Application.DomainId, _metric);
                    string ruleName = rule.Name;
                    if (ruleName == null) continue;
                    if (!int.TryParse(_metric, out int metricId)) continue;
                    ViolStatMetricIdDTO violStats = RulesViolationUtility.GetViolStat(reportData.CurrentSnapshot, metricId);
                    if (violStats == null) continue;

                    // if no violations, do not display anything for this rule
                    if (violStats.TotalViolations < 1) continue;

                    rowData.Add("");
                    cellidx++;

                    rowData.Add(Labels.ObjectsInViolationForRule + " " + ruleName);
                    cellProps.Add(new CellAttributes(cellidx, ColorGray, Color.White, "bold"));
                    cellidx++;
                    rowData.Add(Labels.ViolationsCount + ": " + violStats.TotalViolations);
                    cellProps.Add(new CellAttributes(cellidx, ColorWhite));
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

                    IEnumerable<Violation> results = reportData.SnapshotExplorer.GetViolationsListIDbyBC(reportData.CurrentSnapshot.Href, _metric, bcId, nbLimitTop, "$all");
                    if (results == null) continue;
                    var _violations = results as Violation[] ?? results.ToArray();
                    if (_violations.Length == 0) continue;

                    MetricsUtility.ViolationsBookmarksProperties violationsBookmarksProperties =
                        new MetricsUtility.ViolationsBookmarksProperties(_violations, 0, rowData, ruleName, hasPreviousSnapshot, reportData.CurrentSnapshot.DomainId, reportData.CurrentSnapshot.Id.ToString(), _metric);
                    cellidx = MetricsUtility.PopulateViolationsBookmarks(reportData, violationsBookmarksProperties, cellidx, cellProps, displaySourceCode);

                    // Add empty lines for readability
                    for (int i = 1; i < 5; i++)
                    {
                        rowData.Add("");
                        cellidx++;
                    }

                }

                if (rowData.Count <= 1)
                {
                    rowData.Add(Labels.NoViolation);
                }
            }
            else
            {
                rowData.Add(Labels.NoItem);
            }


            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = displayHeader,
                NbRows = rowData.Count,
                NbColumns = 1,
                Data = rowData,
                CellsAttributes = cellProps
            };

            return table;

        }

    }
}
