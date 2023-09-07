using AiPrompt.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AiPrompt.Helpers;

public class WeightCountToBrush : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is PromptItem item)
        {
            if (item.Config.Weight is 0) return Binding.DoNothing;
            return new SolidColorBrush((item.Config.Weight % 3) switch
            {
                1 => Colors.Pink,
                _ => Colors.White,
            });
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
