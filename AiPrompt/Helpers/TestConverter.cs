using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers;

public class TestConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
