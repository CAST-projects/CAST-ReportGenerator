using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("ACTION_PLAN_BOOKMARKS")]
    public class ActionPlanViolationsBookmarks : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {

            int nbLimitTop = options.GetOption("COUNT") == "ALL" ? -1 : options.GetIntOption("COUNT", 10);
            string filter = options.GetOption("FILTER", "ALL").ToUpper();
            bool tag = options.GetBoolOption("TAG", true);

            List<string> rowData = new List<string>();
            // cellProps will contains the properties of the cell (background color) linked to the data by position in the list stored with cellidx.
            List<CellAttributes> cellProps = new List<CellAttributes>();
            int cellidx = 0;

            rowData.Add(Labels.ViolationsInActionPlan);
            cellidx++;

            IEnumerable<Violation> results = reportData.SnapshotExplorer.GetViolationsInActionPlan(reportData.CurrentSnapshot.Href, -1);
            if (results != null)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases  nothing to do in default case
                switch (filter)
                {
                    case "ADDED":
                        results = results.Where(_ => _.RemedialAction.Status.Equals("added"));
                        break;
                    case "PENDING":
                        results = results.Where(_ => _.RemedialAction.Status.Equals("pending"));
                        break;
                    case "SOLVED":
                        results = results.Where(_ => _.RemedialAction.Status.Equals("solved"));
                        break;
                }
                if (nbLimitTop != -1)
                {
                    results = results.Take(nbLimitTop);
                }

                var _violations = results as Violation[] ?? results.ToArray();
                if (_violations.Length != 0)
                {
                    MetricsUtility.ViolationsBookmarksProperties violationsBookmarksProperties =
                        new MetricsUtility.ViolationsBookmarksProperties(_violations, 0, rowData, tag ? "actionPlan" : "actionPlanPriority", false, reportData.CurrentSnapshot.DomainId,
                        reportData.CurrentSnapshot.Id.ToString(), tag ? "actionPlan" : "actionPlanPriority");
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
                for (int i = 1; i < 6; i++)
                {
                    rowData.Add(string.Empty);
                }
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
