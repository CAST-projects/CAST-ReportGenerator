﻿using Cast.Util.Log;
using Cast.Util.Version;
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
    [Block("AEFP_LIST")]
    public class AEFPList : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            int nbLimitTop;
            int nbColumns = 0;
            if (null == options || !options.ContainsKey("COUNT") || !int.TryParse(options["COUNT"], out nbLimitTop))
            {
                nbLimitTop = 10;
            }
            string type = string.Empty;
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
                        type = "Transactional";
                        break;
                    default:
                        type = string.Empty;
                        break;
                }
            }
            string status = string.Empty;
            if (null != options && options.ContainsKey("STATUS"))
            {
                status = options["STATUS"] ?? string.Empty;
                status = status.Trim();
                switch (status.ToUpper())
                {
                    case "ADDED":
                        status = "Added";
                        break;
                    case "DELETED":
                        status = "Deleted";
                        break;
                    case "MODIFIED":
                        status = "Modified";
                        break;
                    default:
                        status = string.Empty;
                        break;
                }
            }
            bool displayPrevious = options.GetBoolOption("PREVIOUS", false);
            bool displayZero = options.GetBoolOption("ZERO", true);

            List<string> rowData = new List<string>();
            if (type.Equals(string.Empty))
            {
                rowData.Add(Labels.IFPUG_ElementType);
                nbColumns++;
            }
            if (displayPrevious)
            {
                rowData.AddRange(new[] { Labels.FunctionName, Labels.IFPUG_ObjectType, Labels.Technology, Labels.ModuleName, Labels.ObjectName, Labels.AEP, Labels.Previous, Labels.Status, Labels.ComplexityFactor, Labels.UpdatedArtifacts });
                nbColumns = nbColumns + 10;
            }
            else
            {
                rowData.AddRange(new[] { Labels.FunctionName, Labels.IFPUG_ObjectType, Labels.Technology, Labels.ModuleName, Labels.ObjectName, Labels.AEP, Labels.Status, Labels.ComplexityFactor, Labels.UpdatedArtifacts });
                nbColumns = nbColumns + 9;
            }
            int nbRows = 1;

            if (!VersionUtil.Is19Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.9 at least for component AEFP_LIST");
                rowData.Add(Labels.NoData);
                int cnt = type.Equals(string.Empty) ? 9 : 8;
                for (int i = 0; i < cnt; i++)
                {
                    rowData.Add(string.Empty);
                }
                nbRows++;
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbRows,
                    NbColumns = type.Equals(string.Empty) ? 10 : 9,
                    Data = rowData
                };
            }

            // return all data because the filter cannot be applied now (no filter in url)
            IEnumerable<OmgFunction> functions = reportData.SnapshotExplorer.GetOmgFunctionsEvolutions(reportData.CurrentSnapshot.Href, -1)?.ToList();
            IEnumerable<OmgFunction> prevFunctions = null;
            bool previous = displayPrevious && reportData.PreviousSnapshot != null;
            if (previous)
            {
                prevFunctions = reportData.SnapshotExplorer.GetOmgFunctionsEvolutions(reportData.PreviousSnapshot.Href, -1)?.ToList();
            }
            if (functions != null && functions.Any())
            {
                var exportedList = functions;
                if (!string.IsNullOrEmpty(type))
                {
                    exportedList = exportedList.Where(f => f.ElementType.Contains(type));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    exportedList = exportedList.Where(f => f.ElementType.Contains(status));
                }
                if (!displayZero)
                {
                    exportedList = exportedList.Where(f => {
                        if (f.NoOfFPs != null) return int.Parse(f.NoOfFPs) > 0;
                        if (f.Aeps != null) return int.Parse(f.Aeps) > 0;
                        return false;
                    });
                }
                if (nbLimitTop > 0)
                {
                    exportedList = exportedList.Take(nbLimitTop);
                }
                foreach (OmgFunction omgFunction in exportedList)
                {
                    if (type.Equals(string.Empty))
                    {
                        rowData.Add(string.IsNullOrEmpty(omgFunction.ElementType) ? Constants.No_Data : GetType(omgFunction.ElementType));
                    }

                    rowData.Add(string.IsNullOrEmpty(omgFunction.FunctionName) ? Constants.No_Data : omgFunction.FunctionName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectType) ? Constants.No_Data : omgFunction.ObjectType);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.Technology) ? Constants.No_Data : omgFunction.Technology);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ModuleName) ? Constants.No_Data : omgFunction.ModuleName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ObjectName) ? Constants.No_Data : omgFunction.ObjectName);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.Aeps) ?
                            string.IsNullOrEmpty(omgFunction.NoOfFPs) ? Constants.No_Data : omgFunction.NoOfFPs
                            : omgFunction.Aeps);
                    if (previous)
                    {
                        OmgFunction prevFunction = prevFunctions.Where(f => f.ObjectName.Equals(omgFunction.ObjectName) && f.ObjectType.Equals(omgFunction.ObjectType)).FirstOrDefault();
                        if (prevFunction != null)
                        {
                            rowData.Add(string.IsNullOrEmpty(prevFunction.Aeps) ?
                                string.IsNullOrEmpty(prevFunction.NoOfFPs) ? Constants.No_Data : prevFunction.NoOfFPs
                                : prevFunction.Aeps);
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
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ElementType) ? Constants.No_Data : GetStatus(omgFunction.ElementType));
                    rowData.Add(string.IsNullOrEmpty(omgFunction.ComplexityFactor) ? Constants.No_Data : omgFunction.ComplexityFactor);
                    rowData.Add(string.IsNullOrEmpty(omgFunction.UpdatedArtifacts) ? Constants.No_Data : omgFunction.UpdatedArtifacts);
                    nbRows += 1;
                }
            }
            else
            {
                rowData.AddRange(new[] { Labels.NoItem, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
                if (previous) rowData.Add(string.Empty);
                if (type.Equals(string.Empty)) rowData.Add(string.Empty);
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = nbColumns,
                Data = rowData
            };

            return resultTable;
        }

        private static string GetStatus(string source)
        {
            if (source.Contains("Added")) return "Added";
            if (source.Contains("Modified")) return "Modified";
            return source.Contains("Deleted") ? "Deleted" : string.Empty;
        }

        private static string GetType(string source)
        {
            if (source.Contains("Data Function")) return "Data Function";
            return source.Contains("Transactional") ? "Transactional" : string.Empty;
        }
    }
}
