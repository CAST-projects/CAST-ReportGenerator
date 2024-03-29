using CastReporting.UI.WPF.Core.ViewModel;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CastReporting.UI.WPF.Core.Converter
{
    internal class ReportingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? 0 : (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? ReportingMode.Application : (ReportingMode)value;
        }
    }
}
