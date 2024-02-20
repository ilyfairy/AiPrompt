using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers;

public class StringIfElseConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not string str)
            return Binding.DoNothing;

        Span<Range> span = [default, default];
        var count = str.AsSpan().Split(span, '|');
        if(count < 2 ) return Binding.DoNothing;

        if (value is null or false)
            return str.AsSpan(span[1]).ToString();
        else 
            return str.AsSpan(span[0]).ToString();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
