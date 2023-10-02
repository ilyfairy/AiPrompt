using System.Globalization;
using System.Windows.Data;

namespace AiPrompt.Helpers;

public class BooleanToVisibilityConverter : IValueConverter
{
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
}
