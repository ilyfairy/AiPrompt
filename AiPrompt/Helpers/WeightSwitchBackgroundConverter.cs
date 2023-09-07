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

public class WeightSwitchBackgroundConverter : IValueConverter
{
    private readonly LinearGradientBrush gradientBrush;

    public WeightSwitchBackgroundConverter()
    {
        gradientBrush = new LinearGradientBrush();

        gradientBrush.StartPoint = new Point(0, 0.5);
        gradientBrush.EndPoint = new Point(1, 0.5);

        //gradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.0));       // 红色, 在渐变的0%处
        //gradientBrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.25));  // 橙色, 在渐变的25%处
        //gradientBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.5));   // 黄色, 在渐变的50%处
        //gradientBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.75));   // 绿色, 在渐变的75%处
        //gradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 1.0));     // 蓝色, 在渐变的100%处

        gradientBrush.GradientStops.Add(new GradientStop(Colors.Gray, 0.0));
    }


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool)
        {
            return GlobalResources.Instance.PrimaryColor;
        }
        else
        {
            return gradientBrush;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
