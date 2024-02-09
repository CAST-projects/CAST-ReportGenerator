using System;
using Cast.Util;

namespace CastReporting.BLL.Computing
{
    public class ComplexityValuesDTO
    {
        public double? PreviousLowVal { get; set; }
        public double? PreviousAveVal { get; set; }
        public double? PreviousHigVal { get; set; }
        public double? PreviousVhiVal { get; set; }
        public double? SelectedLowVal { get; set; }
        public double? SelectedAveVal { get; set; }
        public double? SelectedHigVal { get; set; }
        public double? SelectedVhiVal { get; set; }
        public double? SelectedTotal { get; set; }

        public void CalculateSelectedTotal()
        {
            if (SelectedLowVal.HasValue && SelectedAveVal.HasValue && SelectedHigVal.HasValue && SelectedVhiVal.HasValue)
                SelectedTotal = SelectedLowVal + SelectedAveVal + SelectedHigVal + SelectedVhiVal;
        }

        public string[] GetComplexityLow(string lowComplexity)
        {
            return new[]
                { lowComplexity //Labels.ComplexityLow
                        , SelectedLowVal?.ToString("N0") ?? FormatHelper.No_Value
                        , PreviousLowVal?.ToString("N0") ?? FormatHelper.No_Value
                        , SelectedLowVal.HasValue && PreviousLowVal.HasValue ? FormatHelper.FormatEvolution((int)(SelectedLowVal.Value - PreviousLowVal.Value)): FormatHelper.No_Value
                        , SelectedLowVal.HasValue && PreviousLowVal.HasValue && Math.Abs(PreviousLowVal.Value) > 0? FormatHelper.FormatPercent((SelectedLowVal - PreviousLowVal) / PreviousLowVal): FormatHelper.No_Value
                        , SelectedLowVal.HasValue && SelectedTotal.HasValue && SelectedTotal.Value>0?FormatHelper.FormatPercent(SelectedLowVal / SelectedTotal, false): FormatHelper.No_Value
                    };
            ;
        }

        public string[] GetComplexityAverage(string averageComplexity)
        {
            return new[]
                { averageComplexity //Labels.ComplexityAverage
                        , SelectedAveVal?.ToString("N0") ?? FormatHelper.No_Value
                        , PreviousAveVal?.ToString("N0") ?? FormatHelper.No_Value
                        , SelectedAveVal.HasValue && PreviousAveVal.HasValue ? FormatHelper.FormatEvolution((int)(SelectedAveVal.Value - PreviousAveVal.Value)) : FormatHelper.No_Value
                        , SelectedAveVal.HasValue && PreviousAveVal.HasValue && Math.Abs(PreviousAveVal.Value) > 0? FormatHelper.FormatPercent((SelectedAveVal - PreviousAveVal) / PreviousAveVal): FormatHelper.No_Value
                        , SelectedAveVal.HasValue && SelectedTotal.HasValue && SelectedTotal.Value>0?FormatHelper.FormatPercent(SelectedAveVal / SelectedTotal, false): FormatHelper.No_Value
                    };
        }

        public string[] GetComplexityHigh(string highComplexity)
        {
            return new[]
                { highComplexity // Labels.ComplexityHigh
                        , SelectedHigVal?.ToString("N0") ?? FormatHelper.No_Value
                        , PreviousHigVal?.ToString("N0") ?? FormatHelper.No_Value
                        , PreviousHigVal.HasValue ? FormatHelper.FormatEvolution((int)(SelectedHigVal.Value - PreviousHigVal.Value)): FormatHelper.No_Value
                        , SelectedHigVal.HasValue && PreviousHigVal.HasValue && Math.Abs(PreviousHigVal.Value) > 0? FormatHelper.FormatPercent((SelectedHigVal - PreviousHigVal) / PreviousHigVal): FormatHelper.No_Value
                        , SelectedHigVal.HasValue && SelectedTotal.HasValue && SelectedTotal.Value>0?FormatHelper.FormatPercent(SelectedHigVal / SelectedTotal, false): FormatHelper.No_Value
                    };
        }

        public string[] GetComplexityExtreme(string extremeComplexity)
        {
            return new[]
                { extremeComplexity //Labels.ComplexityExtreme
                        , SelectedVhiVal?.ToString("N0") ?? FormatHelper.No_Value
                        , PreviousVhiVal?.ToString("N0") ?? FormatHelper.No_Value
                        , SelectedVhiVal.HasValue && PreviousVhiVal.HasValue ? FormatHelper.FormatEvolution((int)(SelectedVhiVal.Value - PreviousVhiVal.Value)): FormatHelper.No_Value
                        , SelectedVhiVal.HasValue && PreviousVhiVal.HasValue && Math.Abs(PreviousVhiVal.Value) > 0? FormatHelper.FormatPercent((SelectedVhiVal - PreviousVhiVal) / PreviousVhiVal): FormatHelper.No_Value
                        , SelectedVhiVal.HasValue && SelectedTotal.HasValue && SelectedTotal.Value>0?FormatHelper.FormatPercent(SelectedVhiVal / SelectedTotal, false): FormatHelper.No_Value
                    };
        }

    }
}
