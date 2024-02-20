using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers;

public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
{
    private static readonly BooleanToVisibilityConverter _instance = new();

    public object Convert(object value, Type targetType, object parameter /* 是否取反 */, CultureInfo culture)
    {
        bool con = value is true;
        if (parameter is "true" or "True")
        {
            con ^= true;
        }
        if (con)
        {
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _instance;
    }
}
