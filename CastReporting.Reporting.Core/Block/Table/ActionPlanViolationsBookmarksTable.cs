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
    [Block("ACTION_PLAN_BOOKMARKS_TABLE")]
    public class ActionPlanViolationsBookmarksTable : TableBlock<ImagingData>
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {

            int nbLimitTop = options.GetOption("COUNT") == "ALL" ? -1 : options.GetIntOption("COUNT", 10);
            string filter = options.GetOption("FILTER", "ALL").ToUpper();
            bool displayHeader = options.GetBoolOption("HEADER", true);
            bool tag = options.GetBoolOption("TAG", true);

            List<string> rowData = new List<string>();
            var headers = new HeaderDefinition();
            headers.Append(Labels.RuleName);
            headers.Append(Labels.ObjectName);
            headers.Append(Labels.IFPUG_ObjectType);
            headers.Append(Labels.Status);
            headers.Append(Labels.Priority);
            headers.Append(Labels.AssociatedValue);
            headers.Append(Labels.FilePath);
            headers.Append(Labels.StartLine);
            headers.Append(Labels.EndLine);

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

                var _violations = results as List<Violation> ?? results.ToList();
                int nbViolations = _violations.Count();
                if (nbViolations > 0)
                {
                    rowData.AddRange(MetricsUtility.PopulateViolationsBookmarksRow(reportData, _violations, headers, tag ? "actionPlan" : "actionPlanPriority"));
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

            if (displayHeader) rowData.InsertRange(0, headers.Labels);
            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = displayHeader,
                NbColumns = headers.Count,
                NbRows = rowData.Count / headers.Count,
                Data = rowData,
            };

            return table;

        }
    }
}
