using AiPrompt.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AiPrompt.Helpers;

public class WeightToStringConverter : IMultiValueConverter
{
    //false: {{}} [[]]
    //true: (()) [[]]
    //null: (1.2) (0.5)

    public static string To(PromptItem item, bool? weightMode = false)
    {
        return To(item.PromptEN, item.Config.Weight, weightMode);
    }

    public static string To(string prompt, int weight, bool? isWeightConvert = false)
    {
        if (weight < 0)
        {
            var count = Math.Abs(weight);
            return $"{new string('[', count)}{prompt}{new string(']', count)}";
        }

        if (isWeightConvert == null && weight > 0)
        {
            return $"({prompt}:{1 + (double)weight / 10})";
        }
        else
        {
            var left = isWeightConvert == true ? '(' : '{';
            var right = isWeightConvert == true ? ')' : '}';
            var count = Math.Abs(weight);
            return $"{new string(left, count)}{prompt}{new string(right, count)}";
        }
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return Binding.DoNothing;

        if (values[0] is not string prompt) return Binding.DoNothing;
        if (values[1] is not int weight) return Binding.DoNothing;
        if (values.Length < 3 || values[2] is not bool isWeightConvert) isWeightConvert = false;

        return To(prompt, weight, isWeightConvert);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
