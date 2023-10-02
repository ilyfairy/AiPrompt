using System.Globalization;
using System.Windows.Data;

namespace AiPrompt.Helpers
{
    public class Double4ToThicknessConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values is [var x, var y, var width, var height, ..])
            {
                return new Thickness(
                    System.Convert.ToDouble(x),
                    System.Convert.ToDouble(y),
                    System.Convert.ToDouble(width), 
                    System.Convert.ToDouble(height));
            }
            return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
