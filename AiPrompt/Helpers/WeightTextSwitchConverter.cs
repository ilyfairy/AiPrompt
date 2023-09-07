using System.Globalization;
using System.Windows.Data;

namespace AiPrompt.Helpers;

public class WeightTextSwitchConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool v)
        {
            return v ? "( )" : "{ }";
        }
        return "(:1)";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
