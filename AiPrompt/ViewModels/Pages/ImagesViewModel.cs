using AiPrompt.Helpers;
using AiPrompt.Models;
using AiPrompt.Services;
using MetadataExtractor;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Data;
using Wpf.Ui.Controls;
using IO = System.IO;

namespace AiPrompt.ViewModels.Pages;

public partial class ImagesViewModel(AppConfigService configService, GlobalResources globalResources, ILogger<ImagesViewModel> logger, TagsService tagsService) : ObservableObject, INavigationAware
{
    private string? pathCache = null;

    public ObservableCollection<ImageInfo> Images { get; set; } = new();
    public AppConfigService ConfigService { get; private set; } = configService;
    public GlobalResources GlobalResources { get; private set; } = globalResources;

    public bool IsShow { get; set; }
    public ImageInfo? Current { get; set; }

    public async void OnNavigatedTo()
    {
        await InitializeViewModel();
    }

    public void OnNavigatedFrom() { }

    private async Task InitializeViewModel()
    {
        if (pathCache == ConfigService.Config.ImagePath)
        {
            return;
        }
        Images.Clear();
        pathCache = ConfigService.Config.ImagePath;

        BindingOperations.EnableCollectionSynchronization(Images, Images);

        var scale = Utils.GetScale();

        await Task.Run(async () =>
        {
            var path = ConfigService.Config.ImagePath;
            string[] files = [];
            IEnumerable<string> cacheFiles = [];

            //递归遍历文件夹
            void GetFiles(string dir, int level = 3)
            {
                var files = IO.Directory.GetFiles(dir);
                cacheFiles = cacheFiles.Concat(files);

                if (level <= 0) return;
                var dirs = IO.Directory.GetDirectories(dir);
                foreach (var subDir in dirs)
                {
                    GetFiles(subDir, level - 1);
                }
            }

            try
            {
                if (path.Contains('|'))
                {
                    foreach (var rootDir in path.Split('|', StringSplitOptions.RemoveEmptyEntries))
                    {
                        try
                        {
                            GetFiles(rootDir);
                        }
                        catch (Exception) { }
                    }
                }
                else
                {
                    GetFiles(path);
                }

                files = cacheFiles.ToArray();
            }
            catch (Exception)
            {
                return;
            }


            foreach (var item in files)
            {
                string file = Path.GetFullPath(item);

                var imageInfo = tagsService.GetImageInfo(file, scale);

                if (imageInfo is { })
                {
                    Images.Add(imageInfo);
                    await Task.Delay(50);
                }
                //Application.Current.Dispatcher.Invoke(() => Images.Add(image));
            }

            logger.LogInformation("一共{Count}张图片", Images.Count);
        });
    }


    [RelayCommand]
    public void ShowImage(ImageInfo image)
    {
        if (image.IsOfficial || image.IsStableDiffusionText || image.IsStableDiffusionComment)
        {
            Current = image;
            IsShow = true;
        }
    }


    [RelayCommand]
    public void CloseImage(ImageInfo image)
    {
        IsShow = false;
    }
}