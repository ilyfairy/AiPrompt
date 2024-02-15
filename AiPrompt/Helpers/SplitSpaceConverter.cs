using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace AiPrompt.Helpers;

/* 
 * "1,  2 ,  3 , 4" -> "1, 2, 3, 4"
 */
public class SplitSpaceConverter : MarkupExtension, IValueConverter
{
    private static readonly SplitSpaceConverter _instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        char split;
        if (parameter is string and [var splitChar])
            split = splitChar;
        else if(parameter is char splitChar2)
            split = splitChar2;
        else
            split = ',';

        var str = value?.ToString() ?? "";
        StringBuilder s = new(str.Length);

        int startIndex = 0;
        while (true)
        {
            int index = str.IndexOf(split, startIndex);
            if (index != -1)
            {
                try
                {
                    var temp = str.AsSpan()[startIndex..index].Trim();
                    if (temp.Length != 0)
                    {
                        s.Append(temp);
                        s.Append(split);
                        s.Append(' ');
                    }
                }
                catch { }
            }
            else
            {
                var temp = str.AsSpan(startIndex).Trim();
                if(temp.Length != 0)
                    s.Append(temp);
                break;
            }
            startIndex = index + 1;
        }

        return s.ToString();
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
