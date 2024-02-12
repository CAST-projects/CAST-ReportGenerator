using Cast.Util;
using CastReporting.BLL.Computing;
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
    [Block("VIOLATIONS_LIST")]
    public class ViolationsListByBC : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            List<string> rowData = new List<string>();

            int[] bcIds = options.GetOption("BCID") != null ? options.GetOption("BCID").Trim().Split('|').Select(int.Parse).ToArray() : new[] { (int)BusinessCriteria.Security }; // by default, security
            int nbLimitTop = options.GetOption("COUNT") == "ALL" ? -1 : options.GetIntOption("COUNT", 10);
            bool shortName = options.GetOption("NAME", "FULL").Equals("SHORT");
            string[] filter = options.GetOption("FILTER", "ALL").Trim().Split('|');
            bool critical = options.GetOption("VIOLATIONS", "CRITICAL").Equals("CRITICAL");
            string module = options.GetOption("MODULE");
            string[] technos = options.GetOption("TECHNOLOGIES") != null && !options.GetOption("TECHNOLOGIES").Equals("ALL") ? options.GetOption("TECHNOLOGIES").Trim().Split('|') : new[] { "$all" };

            bool hasPri = bcIds.Contains((int)BusinessCriteria.Robustness)
                            || bcIds.Contains((int)BusinessCriteria.Performance)
                            || bcIds.Contains((int)BusinessCriteria.Security);

            rowData.Add(Labels.ViolationStatus);
            if (hasPri) rowData.Add(Labels.PRI);
            rowData.Add(Labels.ExclusionStatus);
            rowData.Add(Labels.ActionStatus);
            rowData.Add(Labels.RuleName);
            rowData.Add(Labels.BusinessCriterionName);
            rowData.Add(Labels.ObjectName);
            rowData.Add(Labels.ObjectStatus);
            int nbCols = hasPri ? 8 : 7;
            int nbRows;

            List<Violation> results = new List<Violation>();


            foreach (int bcId in bcIds)
            {
                Module mod = module != null ? reportData.CurrentSnapshot.Modules.FirstOrDefault(m => m.Name.Equals(module)) : null;
                string href = mod == null ? reportData.CurrentSnapshot.Href : mod.Href;

                string technologies = technos.Aggregate(string.Empty, (current, techno) => current.Equals(string.Empty) ? techno : current + "," + techno);

                IEnumerable<Violation> bcresults = reportData.SnapshotExplorer.GetViolationsListIDbyBC(href,
                    critical ? "(critical-rules)" : $"(nc:{bcId},cc:{bcId})",
                    bcId, -1, $"({technologies})").ToList();

                List<Violation> filterResults = new List<Violation>();
                if (!bcresults.Any()) continue;
                foreach (string _filter in filter)
                {
                    switch (_filter)
                    {
                        case "ADDED":
                            filterResults.AddRange(bcresults.Where(_ => _.Diagnosis.Status.Equals("added")));
                            break;
                        case "UNCHANGED":
                            filterResults.AddRange(bcresults.Where(_ => _.Diagnosis.Status.Equals("unchanged")));
                            break;
                        case "UPDATED":
                            filterResults.AddRange(bcresults.Where(_ => _.Diagnosis.Status.Equals("updated")));
                            break;
                        default:
                            filterResults.AddRange(bcresults);
                            break;
                    }
                }
                var _violations = filterResults.ToList();
                foreach (Violation _bcresult in _violations)
                {
                    _bcresult.Component.PriBusinessCriterion = reportData.CurrentSnapshot.GetMetricName(bcId);
                }
                results.AddRange(_violations);
            }

            results = nbLimitTop != -1 ? results.Take(nbLimitTop).ToList() : results;

            if (results.Count != 0)
            {
                foreach (Violation _violation in results)
                {
                    rowData.Add(_violation.Diagnosis?.Status ?? FormatHelper.No_Value);
                    if (hasPri) rowData.Add(_violation.Component?.PropagationRiskIndex.ToString("N0"));
                    rowData.Add(_violation.ExclusionRequest?.Status ?? FormatHelper.No_Value);
                    rowData.Add(_violation.RemedialAction?.Status ?? FormatHelper.No_Value);
                    rowData.Add(_violation.RulePattern?.Name ?? FormatHelper.No_Value);
                    rowData.Add(_violation.Component?.PriBusinessCriterion ?? FormatHelper.No_Value);
                    rowData.Add(shortName ? _violation.Component?.ShortName : _violation.Component?.Name ?? FormatHelper.No_Value);
                    rowData.Add(_violation.Component?.Status ?? FormatHelper.No_Value);
                }
                nbRows = results.Count + 1;
            }
            else
            {
                rowData.Add(Labels.NoItem);
                for (int i = 1; i < nbCols; i++)
                {
                    rowData.Add(string.Empty);
                }
                nbRows = 2;
            }

            var table = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = nbCols,
                Data = rowData
            };

            return table;

        }
    }
}
