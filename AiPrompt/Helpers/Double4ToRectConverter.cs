using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers
{
    public class Double4ToRectConverter : MarkupExtension, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (values is [var x and not null, var y and not null, var width and not null, var height and not null, ..])
                {
                    return new Rect(
                        System.Convert.ToDouble(x),
                        System.Convert.ToDouble(y),
                        System.Convert.ToDouble(width),
                        System.Convert.ToDouble(height));
                }
                return Binding.DoNothing;
            }
            catch (Exception)
            {
                return Binding.DoNothing;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return [];
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
