using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AiPrompt.Helpers;

public class SizeBalanceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter is not int size)
        {
            if (!int.TryParse(parameter?.ToString(), out size))
            {
                size = 200;
            }
        }

        if (value is ImageSource source)
        {
            var v = size / source.Width;
            return new SizeF(size, (float)source.Height * (float)v);
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

