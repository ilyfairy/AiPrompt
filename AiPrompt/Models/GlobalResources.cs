using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media;
using Wpf.Ui.Appearance;

namespace AiPrompt.Models;

public class GlobalResources : ObservableObject
{
    public static GlobalResources Instance { get; } = new();

    public string TestString { get; set; } = string.Empty;

    public string ProgramDirectory { get; } = Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!;

    public SolidColorBrush PrimaryColor { get; set; }
    public SolidColorBrush ThemeColor { get; set; } = new SolidColorBrush(Colors.White); // 最深色1
    public SolidColorBrush ThemeColor2 { get; set; } = new SolidColorBrush(Colors.White); // 稍微深色2
    public SolidColorBrush ThemeColor3 { get; set; } = new SolidColorBrush(Colors.White); // 稍微深色2

    public JsonSerializerOptions JsonOptions { get; set; } = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public GlobalResources()
    {
        Theme.Changed += Theme_Changed;
        PrimaryColor = new SolidColorBrush(Colors.White);
    }

    private void Theme_Changed(ThemeType currentTheme, Color systemAccent)
    {
        SetThemeColor(currentTheme == ThemeType.Light);
        PrimaryColor = new SolidColorBrush(Accent.PrimaryAccent);
    }

    public void SetThemeColor(bool isLight)
    {
        if (isLight)
        {
            ThemeColor = new SolidColorBrush(Color.FromRgb(0xfc, 0xfb, 0xfa));
            ThemeColor2 = new SolidColorBrush(Color.FromRgb(0xfc, 0xfb, 0xfa));
            ThemeColor3 = new SolidColorBrush(Color.FromRgb(0xfc, 0xfb, 0xfa));
        }
        else
        {
            ThemeColor = new SolidColorBrush(Color.FromRgb(0x21, 0x1f, 0x1f));
            ThemeColor2 = new SolidColorBrush(Color.FromRgb(0x2d, 0x2d, 0x2d));
            ThemeColor3 = new SolidColorBrush(Color.FromRgb(47, 44, 43));
        }
    }
}
