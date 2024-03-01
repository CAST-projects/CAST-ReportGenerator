using CastReporting.Domain;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Linq;

namespace CastReporting.Reporting.Block.Table
{
    [Block("IFPUG_FUNCTIONS")]
    public class IfpugFunctions : TableBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = -1;
            }
            string type = null;
            if (null != options && options.ContainsKey("TYPE"))
            {
                type = options["TYPE"] ?? string.Empty;
                type = type.Trim();
                switch (type.ToUpper())
                {
                    case "DF":
                        type = "Data Function";
                        break;
                    case "TF":
                        type = "Transaction";
                        break;
                    default:
                        type = string.Empty;
                        break;
                }
            }
            bool displayHeader = options == null || !options.ContainsKey("HEADER") || "NO" != options["HEADER"];
            bool displayPrevious = options.GetBoolOption("PREVIOUS", false);
            bool displayZero = options.GetBoolOption("ZERO", true);

            IEnumerable<IfpugFunction> functions = reportData.SnapshotExplorer.GetIfpugFunctions(reportData.CurrentSnapshot.Href, string.IsNullOrEmpty(type) || displayZero ? nbLimitTop : -1)?.ToList();
            if (!displayZero && functions != null)
            {
                functions = functions.Where(f =>
                {
                    if (f.NbOfFPs != null) return int.Parse(f.NbOfFPs) > 0;
                    if (f.NoOfFPs != null) return int.Parse(f.NoOfFPs) > 0;
                    if (f.Afps != null) return int.Parse(f.Afps) > 0;
                    return false;
                });
            }
            bool previous = displayPrevious && reportData.PreviousSnapshot != null;

            List<string> rowData = new List<string>();
            IEnumerable<IfpugFunction> prevFunctions = null;
            if (previous)
            {
                prevFunctions = reportData.SnapshotExplorer.GetIfpugFunctions(reportData.PreviousSnapshot.Href, string.IsNullOrEmpty(type) ? nbLimitTop : -1)?.ToList();
            }


            if (displayHeader)
            {
                if (displayPrevious)
                {
                    rowData.AddRange(new[] { Labels.IFPUG_ElementType, Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.Previous, Labels.IFPUG_FPDetails, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });
                }
                else
                {
                    rowData.AddRange(new[] { Labels.IFPUG_ElementType, Labels.ObjectName, Labels.IFPUG_NoOfFPs, Labels.IFPUG_FPDetails, Labels.IFPUG_ObjectType, Labels.ModuleName, Labels.Technology });
                }

            }

            int nbRows = 0;

            if (functions != null && functions.Any())
            {
                var exportedList = functions;
                if (!string.IsNullOrEmpty(type))
                {
                    exportedList = exportedList.Where(f => f.ElementType == type);
                }
                if (nbLimitTop > 0)
                {
                    exportedList = exportedList.Take(nbLimitTop);
                }
                foreach (var ifpugFunction in exportedList)
                {
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ElementType) ? " " : ifpugFunction.ElementType);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ObjectName) ? " " : ifpugFunction.ObjectName);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.NbOfFPs) ?
                        string.IsNullOrEmpty(ifpugFunction.NoOfFPs) ?
                        string.IsNullOrEmpty(ifpugFunction.Afps) ? " " : ifpugFunction.Afps
                        : ifpugFunction.NoOfFPs
                        : ifpugFunction.NbOfFPs);
                    if (previous)
                    {
                        var prevFunction = prevFunctions.Where(f => f.ObjectName.Equals(ifpugFunction.ObjectName) && f.ObjectType.Equals(ifpugFunction.ObjectType) && f.ElementType.Equals(ifpugFunction.ElementType)).FirstOrDefault();
                        if (prevFunction != null)
                        {
                            rowData.Add(string.IsNullOrEmpty(prevFunction.NbOfFPs) ?
                                string.IsNullOrEmpty(prevFunction.NoOfFPs) ?
                                string.IsNullOrEmpty(prevFunction.Afps) ? " " : prevFunction.Afps
                                : prevFunction.NoOfFPs
                                : prevFunction.NbOfFPs);
                        }
                        else
                        {
                            rowData.Add(" ");
                        }
                    }
                    else if (displayPrevious)
                    {
                        rowData.Add(" ");
                    }
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.FPDetails) ? " " : ifpugFunction.FPDetails);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ObjectType) ? " " : ifpugFunction.ObjectType);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.ModuleName) ? " " : ifpugFunction.ModuleName);
                    rowData.Add(string.IsNullOrEmpty(ifpugFunction.Technology) ? " " : ifpugFunction.Technology);
                    nbRows += 1;
                }
            }
            else
            {
                rowData.AddRange(new[] { Labels.NoItem, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = displayHeader,
                NbRows = nbRows + (displayHeader ? 1 : 0),
                NbColumns = displayPrevious ? 8 : 7,
                Data = rowData
            };

            return resultTable;
        }
    }
}
