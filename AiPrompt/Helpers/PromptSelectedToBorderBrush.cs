using AiPrompt.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AiPrompt.Helpers;

public class PromptSelectedToBorderBrush : IValueConverter
{
    public Brush NotSelectedBrush { get; set; }

    public PromptSelectedToBorderBrush()
    {
        NotSelectedBrush = new SolidColorBrush(Colors.Transparent);
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //if (value is not PromptItem item)
        //{
        //    return Binding.DoNothing;
        //}
        if (value is bool isSelected)
        {
            if (isSelected)
            {
                return GlobalResources.Instance.PrimaryColor;
            }
            else
            {
                return NotSelectedBrush;
            }
        }

        return Binding.DoNothing;

        //item.Update();
        //if (item.Config.IsSelected)
        //{
        //    return GlobalResources.Instance.PrimaryColor;
        //}
        //else
        //{
        //    return NotSelectedBrush;
        //}
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
