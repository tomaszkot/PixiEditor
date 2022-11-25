﻿using System.Globalization;

namespace PixiEditor.Helpers.Converters;

internal class DebugConverter
    : SingleInstanceConverter<DebugConverter>
{
    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
