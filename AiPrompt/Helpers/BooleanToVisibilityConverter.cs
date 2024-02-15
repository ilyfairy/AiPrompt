﻿using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers;

public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
{
    private static readonly BooleanToVisibilityConverter _instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isVisibility && isVisibility)
        {
            return Visibility.Visible;
        }
        return Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
        {
            return true;
        }
        return false;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _instance;
    }
}
