using Cast.Util.Log;
using Cast.Util.Version;
using CastReporting.Domain.Imaging;
using CastReporting.Reporting.Atrributes;
using CastReporting.Reporting.Builder.BlockProcessing;
using CastReporting.Reporting.Core.Languages;
using CastReporting.Reporting.ReportingModel;
using CastReporting.Reporting.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Cast.Util;

namespace CastReporting.Reporting.Block.Table
{
    [Block("AETP_LIST")]
    public class AETPList : TableBlock
    {
        public override TableDefinition Content(ReportData reportData, Dictionary<string, string> options)
        {
            string _metricFormat = options.GetOption("FORMAT", "N2");
            int nbLimitTop = options.GetIntOption("COUNT", 10);

            List<string> rowData = new List<string>();
            rowData.AddRange(new[] { Labels.ObjectName, Labels.ObjectFullName, Labels.IFPUG_ObjectType, Labels.Status, Labels.EffortComplexity, Labels.EquivalenceRatio, Labels.AEP });
            int nbRows = 1;

            if (!VersionUtil.Is19Compatible(reportData.ServerVersion))
            {
                LogHelper.LogError("Bad version of RestAPI. Should be 1.9 at least for component AETP_LIST");
                rowData.Add(Labels.NoData);
                for (int i = 0; i < 6; i++)
                {
                    rowData.Add(string.Empty);
                }
                nbRows++;
                return new TableDefinition
                {
                    HasRowHeaders = false,
                    HasColumnHeaders = true,
                    NbRows = nbRows,
                    NbColumns = 7,
                    Data = rowData
                };
            }

            IEnumerable<OmgFunctionTechnical> functions = reportData.SnapshotExplorer.GetOmgFunctionsTechnical(reportData.CurrentSnapshot.Href, nbLimitTop)?.ToList();
            if (functions?.Count() > 0)
            {
                foreach (OmgFunctionTechnical omgFunction in functions)
                {
                    List<string> row = new List<string>();
                    try
                    {
                        row.Add(string.IsNullOrEmpty(omgFunction.ObjectName) ? FormatHelper.No_Data : omgFunction.ObjectName);
                        row.Add(string.IsNullOrEmpty(omgFunction.ObjectFullName) ? FormatHelper.No_Data : omgFunction.ObjectFullName);
                        row.Add(string.IsNullOrEmpty(omgFunction.ObjectType) ? FormatHelper.No_Data : omgFunction.ObjectType);
                        row.Add(string.IsNullOrEmpty(omgFunction.ObjectStatus) ? FormatHelper.No_Data : omgFunction.ObjectStatus);
                        row.Add(string.IsNullOrEmpty(omgFunction.EffortComplexity) ? FormatHelper.No_Data : FormatDouble(omgFunction.EffortComplexity, _metricFormat));
                        row.Add(string.IsNullOrEmpty(omgFunction.EquivalenceRatio) ? FormatHelper.No_Data : FormatDouble(omgFunction.EquivalenceRatio, _metricFormat));
                        row.Add(string.IsNullOrEmpty(omgFunction.AepCount) ? FormatHelper.No_Data : FormatDouble(omgFunction.AepCount, _metricFormat));
                        rowData.AddRange(row);
                        nbRows += 1;
                    }
                    catch (Exception e) when (e is FormatException)
                    {
                        LogHelper.LogWarn("Invalid data cannot be add in the AETP_LIST table : " + e.Message);
                    }
                }
            }
            else
            {
                rowData.AddRange(new[] { Labels.NoItem, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty });
            }

            var resultTable = new TableDefinition
            {
                HasRowHeaders = false,
                HasColumnHeaders = true,
                NbRows = nbRows,
                NbColumns = 7,
                Data = rowData
            };

            return resultTable;
        }

        private string FormatDouble(string doubleToFormat, string optFormat)
        {
            if (doubleToFormat == null) return FormatHelper.No_Value;
            try
            {
                double var = double.Parse(doubleToFormat, new CultureInfo("en-US"));
                return var.ToString(optFormat);
            }
            catch (FormatException)
            {
                return doubleToFormat;
            }
        }


    }
}
