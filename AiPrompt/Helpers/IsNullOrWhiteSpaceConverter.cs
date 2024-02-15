using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers;

public class IsNullOrWhiteSpaceToStringConverter : MarkupExtension, IValueConverter
{
    private static readonly IsNullOrWhiteSpaceToStringConverter _instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (string.IsNullOrWhiteSpace(value?.ToString()))
            return parameter;
        return value;
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
