using AiPrompt.Models;
using AiPrompt.ViewModels.Pages;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AiPrompt.Helpers;

// Control.Background = 文件路径 to ImageBrush
public class PathAsyncToControlBackgroundImageBrushConverter : MarkupExtension, IMultiValueConverter
{
    public static PathAsyncToControlBackgroundImageBrushConverter Instance { get; } = new();

    private static readonly Brush defaultBrush; // 默认图片

    static PathAsyncToControlBackgroundImageBrushConverter()
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
        defaultBrush.Freeze();
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3) return Binding.DoNothing;
        if (parameter is not Stretch stretch)
        {
            stretch = Stretch.UniformToFill;
        }

        int size = (int)(TagsViewModel.PromptImageSize * Utils.GetScale().XScale);
        var item = (PromptItem)values[0];
        var path = values[1]?.ToString();
        bool isRelTag = item.IsImageRelativeTag;
        var control = (Control)values[2];

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
            Task.Run(() =>
            {
                BitmapImage bitmapImage = null!;
                bitmapImage = new BitmapImage();
                var file = File.OpenRead(filePath);

                ImageSizeReader.ImageSizeReaderUtil imageUtil = new();
                var imageSize = imageUtil.GetDimensions(file);
                var newSize = Math.Min(size, imageSize.Width);
                //newSize = 100;
                file.Position = 0;

                var logger = App.GetService<ILogger<PathAsyncToControlBackgroundImageBrushConverter>>()!;
                logger.LogInformation("TagImage解码大小: {Size}", newSize);

                bitmapImage.BeginInit();
                //bitmapImage.CreateOptions = BitmapCreateOptions.DelayCreation;
                //bitmapImage.StreamSource = file;
                //bitmapImage.UriSource = new Uri($"file://{Path.GetFullPath(filePath)}");
                //bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = new AutoDisposeStream(file);
                bitmapImage.DecodePixelWidth = newSize;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                
                control.Dispatcher.Invoke(() =>
                {
                    control.Background = new ImageBrush(bitmapImage) { Stretch = stretch };
                });
            });

            return new ImageBrush();
        }
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

public class AutoDisposeStream(Stream baseStream) : Stream
{
    public Stream BaseStream { get; } = baseStream;

    public override bool CanRead => BaseStream.CanRead;

    public override bool CanSeek => BaseStream.CanSeek;

    public override bool CanWrite => BaseStream.CanWrite;

    public override long Length => BaseStream.Length;

    public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

    public override void Flush() => BaseStream.Flush();

    public override int Read(byte[] buffer, int offset, int count)
    {
        var result = BaseStream.Read(buffer, offset, count);
        if (Position == Length)
        {
            Dispose();
        }
        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        
        return BaseStream.Seek(offset, origin);
    }

    public override void SetLength(long value) => BaseStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => BaseStream.Write(buffer, offset, count);
}