using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AiPrompt.Helpers;
using AiPrompt.Models;
using AiPrompt.ViewModels.Pages;
using Microsoft.Extensions.Logging;

namespace AiPrompt.Views.Components;

public class AsyncButton : Wpf.Ui.Controls.Button
{
    private static readonly BitmapImage defaultImageSource; // 默认图片

    static AsyncButton()
    {
        try
        {
            defaultImageSource = new BitmapImage(new Uri("pack://application:,,,/Assets/暂无图片.png"));
        }
        catch (Exception)
        {
            defaultImageSource = new();
        }
        defaultImageSource.Freeze();
    }

    public static readonly DependencyProperty DefaultProperty = DependencyProperty.Register("Default", typeof(ImageSource), typeof(AsyncButton), new PropertyMetadata(null));
    public static readonly DependencyProperty PromptItemProperty = DependencyProperty.Register("PromptItem", typeof(PromptItem), typeof(AsyncButton), new PropertyMetadata(null, OnPromptItemChanged));

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


    public AsyncButton()
    {
        Background = Brushes.Transparent;
    }

    public void SetImage(ImageSource imageSource)
    {
        Background = new ImageBrush(imageSource)
        {
            Stretch = Stretch.UniformToFill,
        };
    }

    private static readonly SemaphoreSlim _lock = new(1);

    public static async void OnPromptItemChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        if (args.NewValue is null)
            return;

        var item = (PromptItem)args.NewValue;
        var path = item.Image;
        var control = (AsyncButton)dp;

        if (string.IsNullOrWhiteSpace(path))
        {
            control.SetImage(defaultImageSource);
            return;
        }

        int size = (int)(TagsViewModel.PromptImageSize * Utils.GetScale().XScale);

        if (!File.Exists(path))
        {
            control.SetImage(defaultImageSource);
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

            var logger = App.GetService<ILogger<AsyncButton>>()!;
            logger.LogInformation("TagImage解码大小: {Size}", newSize);

            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new AutoDisposeStream(file);
            bitmapImage.DecodePixelWidth = newSize;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            control.Dispatcher.Invoke(() =>
            {
                control.SetImage(bitmapImage ?? control.Default ?? defaultImageSource);
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
