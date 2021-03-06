﻿using CastReporting.UI.WPF.Core.Resources.Languages;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CastReporting.UI.WPF.Core.Converter
{
    public class MessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter != null ? Messages.ResourceManager.GetString(parameter.ToString()) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
