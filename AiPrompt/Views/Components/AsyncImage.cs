using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AiPrompt.Helpers;
using AiPrompt.Models;
using AiPrompt.ViewModels.Pages;
using Microsoft.Extensions.Logging;

namespace AiPrompt.Views.Components;

public class AsyncImage : Image
{
    private static readonly BitmapImage defaultImage; // 默认图片

    static AsyncImage()
    {
        try
        {
            defaultImage = new BitmapImage(new Uri("pack://application:,,,/Assets/暂无图片.png"));
        }
        catch (Exception)
        {
            defaultImage = new();
        }
        defaultImage.Freeze();
    }

    public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register("Default", typeof(ImageSource), typeof(AsyncImage), new PropertyMetadata(null));
    public static readonly DependencyProperty PromptItemProperty = DependencyProperty.Register("PromptItem", typeof(PromptItem), typeof(AsyncImage), new PropertyMetadata(null, OnPromptItemChanged));

    public ImageSource? Default
    {
        get { return (ImageSource)GetValue(DefaultProperty); }
        set { SetValue(DefaultProperty, value); }
    }

    public PromptItem? PromptItem
    {
        get => (PromptItem?)GetValue(PromptItemProperty);
        set => SetValue(PromptItemProperty, value);
    }


    public AsyncImage()
    {
        Stretch = Stretch.UniformToFill;
    }


    private static readonly SemaphoreSlim _lock = new(1);

    public static async void OnPromptItemChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is null)
            return;

        var item = (PromptItem)args.NewValue;
        var path = item.Image;
        var control = (AsyncImage)dp;

        if (string.IsNullOrWhiteSpace(path))
        {
            control.Source = defaultImage;
            return;
        }

        int size = (int)(TagsViewModel.PromptImageSize * Utils.GetScale().XScale);

        if (!File.Exists(path))
        {
            control.Source = defaultImage;
            return;
        }

        await _lock.WaitAsync().ConfigureAwait(false);
        await Task.Delay(50);

        try
        {
            var id = Environment.CurrentManagedThreadId;

            BitmapImage bitmapImage = null!;
            bitmapImage = new BitmapImage();

            var file = File.OpenRead(path);

            ImageSizeReader.ImageSizeReaderUtil imageUtil = new();
            var imageSize = imageUtil.GetDimensions(file);
            var newSize = Math.Min(size, imageSize.Width);
            file.Position = 0;

            var logger = App.GetService<ILogger<AsyncImage>>()!;
            logger.LogInformation("TagImage解码大小: {Size}", newSize);

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new AutoDisposeStream(file);
            bitmapImage.DecodePixelWidth = newSize;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            control.Dispatcher.Invoke(() =>
            {
                control.Source = bitmapImage ?? control.Default ?? defaultImage;
            });
        }
        catch (Exception)
        {
        }
        finally
        {
            _lock.Release();
        }
    }



}
