using CastReporting.Reporting.ReportingModel;
using System.Collections.Generic;
using System.Drawing;

namespace CastReporting.Reporting.Helper
{
    public static class FormatTableHelper
    {

        public static Color FormatColor(string myColor)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases nothing to do in default case
            switch (myColor)
            {
                case "Gainsboro":
                    return Color.Gainsboro;
                case "White":
                    return Color.White;
                case "Lavender":
                    return Color.Lavender;
                case "LightYellow":
                    return Color.LightYellow;
                case "Beige":
                    return Color.Beige;
                case "Gray":
                    return Color.Gray;
                case "LightGrey":
                    return Color.LightGray;
                case "MintCream":
                    return Color.MintCream;
                case "BlanchedAlmond":
                    return Color.BlanchedAlmond;
            }
            return Color.White;
        }

        public static void AddGrayOrBold(bool detail, List<CellAttributes> cellProps, int cellidx, int? nbViolations)
        {
            const string colorBeige = "Beige";
            const string colorLightGray = "LightGrey";
            if (detail)
            {
                cellProps.Add(new CellAttributes(cellidx, colorLightGray, "bold"));
            }
            else if (nbViolations > 0)
            {
                cellProps.Add(new CellAttributes(cellidx, colorBeige));
            }
        }

        public static void AddGrayAndBold(List<CellAttributes> cellProps, int cellidx)
        {
            AddGrayOrBold(true, cellProps, cellidx, 0);
        }

        public static void AddColorsIfCondition(bool condition, List<CellAttributes> cellProps, int cellidx, string colorTrue, string colorFalse)
        {
            cellProps.Add(condition ? new CellAttributes(cellidx, colorTrue) : new CellAttributes(cellidx, colorFalse));
        }

        public static bool limitReached(int dataCount, int headersCount, int limit)
        {
            int currentRows = dataCount / headersCount;
            return limit != -1 && currentRows >= limit;

        }
    }
}
