using AiPrompt.Models;
using AiPrompt.ViewModels.Pages;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AiPrompt.Helpers;

// 文件路径 to ImageBrush
public class PathToImageBrushConverter : IMultiValueConverter
{
    private readonly Brush defaultBrush; // 默认图片

    public PathToImageBrushConverter()
    {
        try
        {
            var image = new BitmapImage(new Uri("pack://application:,,,/Assets/暂无图片.png"));
            defaultBrush = new ImageBrush(image) { Stretch = Stretch.UniformToFill };
        }
        catch (Exception)
        {
            defaultBrush = new SolidColorBrush();
        }
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2) return Binding.DoNothing;
        if (parameter is not Stretch stretch)
        {
            stretch = Stretch.UniformToFill;
        }

        int size = (int)(DashboardViewModel.PromptImageSize * Utils.GetScale().XScale);
        var item = (PromptItem)values[0];
        var path = values[1]?.ToString();
        bool isRelTag = item.IsImageRelativeTag;

        if (string.IsNullOrWhiteSpace(path)) return defaultBrush;
        if (isRelTag && item.Parent?.Parent is { FilePath: { } tagFile } tab)
        {
            var dir = Path.GetDirectoryName(tagFile);
            var targetImageFilePath = Path.Combine(dir ?? "", path);
            if (File.Exists(targetImageFilePath))
            {
                return OpenFromFile(targetImageFilePath);
            }
        }
        if (File.Exists(path))
        {
            return OpenFromFile(path);
        }

        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri && uri.Scheme is "http" or "https")
            {
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = uri;
                bitmapImage.DecodePixelWidth = size;
                bitmapImage.EndInit();
                return new ImageBrush(bitmapImage) { Stretch = stretch };
            }
        }

        return defaultBrush;


        ImageBrush OpenFromFile(string filePath)
        {
            using var file = File.OpenRead(filePath);
            MemoryStream ms = new();
            file.CopyTo(ms);
            ms.Position = 0;
            file.Position = 0;

            var newSize = size;
            ImageSizeReader.ImageSizeReaderUtil imageUtil = new();
            var imageSize = imageUtil.GetDimensions(file);
            if (imageSize.Width < newSize)
                newSize = imageSize.Width;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.DecodePixelWidth = newSize;
            bitmapImage.EndInit();


            return new ImageBrush(bitmapImage) { Stretch = stretch };
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
