
/*
 *   Copyright (c) 2019 CAST
 *
 */

using Cast.Util;
using CastReporting.BLL.Computing;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Helper;
using CastReporting.Reporting.ReportingModel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections.Generic;

namespace CastReporting.Reporting.Block.Graph
{
    [Block("RADAR_METRIC_ID")]
    public class RadarMetricId : GraphBlock
    {
        public override TableDefinition Content(ImagingData reportData, Dictionary<string, string> options)
        {
            string[] qidList = options.GetOption("ID")?.Split('|');
            string _version = options.GetOption("SNAPSHOT", "BOTH");

            var rowData = new List<string> { null };

            if (reportData?.CurrentSnapshot == null) return null;

            if (_version == "CURRENT" || _version == "BOTH")
            {
                string currSnapshotLabel = reportData.CurrentSnapshot.GetSnapshotVersionNumber();
                rowData.Add(currSnapshotLabel);
            }

            if (reportData.PreviousSnapshot != null && (_version == "PREVIOUS" || _version == "BOTH"))
            {
                string prevSnapshotLabel = reportData.PreviousSnapshot.GetSnapshotVersionNumber();
                rowData.Add(prevSnapshotLabel ?? FormatHelper.No_Value);
            }

            int nbRow = 0;
            if (qidList != null)
            {
                foreach (string qid in qidList)
                {
                    int id = int.Parse(qid.Trim());
                    string qidName;
                    double? curRes;
                    double? prevRes;

                    switch (_version)
                    {
                        case "CURRENT":
                            qidName = reportData.CurrentSnapshot.GetMetricName(id, true);
                            if (string.IsNullOrEmpty(qidName)) continue;
                            rowData.Add(qidName);
                            curRes = reportData.CurrentSnapshot.GetMetricValue(id);
                            rowData.Add(curRes?.ToString() ?? FormatHelper.Zero);
                            nbRow++;
                            break;
                        case "PREVIOUS":
                            if (reportData.PreviousSnapshot != null)
                            {
                                qidName = reportData.PreviousSnapshot.GetMetricName(id, true);
                                if (string.IsNullOrEmpty(qidName)) continue;
                                rowData.Add(qidName);
                                prevRes = reportData.PreviousSnapshot.GetMetricValue(id);
                                rowData.Add(prevRes?.ToString() ?? FormatHelper.Zero);
                                nbRow++;
                            }
                            break;
                        default:
                            qidName = reportData.CurrentSnapshot.GetMetricName(id, true);
                            if (string.IsNullOrEmpty(qidName)) continue;
                            rowData.Add(qidName);
                            curRes = reportData.CurrentSnapshot.GetMetricValue(id);
                            rowData.Add(curRes?.ToString() ?? FormatHelper.Zero);
                            if (reportData.PreviousSnapshot != null)
                            {
                                prevRes = reportData.PreviousSnapshot.GetMetricValue(id);
                                rowData.Add(prevRes?.ToString() ?? FormatHelper.Zero);
                            }
                            nbRow++;
                            break;
                    }
                }
            }

            int nbCol = _version == "CURRENT" || _version == "PREVIOUS" ? 2 : 3;
            if ((_version == "BOTH" || _version == "PREVIOUS") && reportData.PreviousSnapshot == null) nbCol--;

            TableDefinition resultTable = new TableDefinition
            {
                HasRowHeaders = true,
                HasColumnHeaders = true,
                NbRows = nbRow + 1,
                NbColumns = nbCol,
                Data = rowData
            };

            return resultTable;
        }

    }

}
