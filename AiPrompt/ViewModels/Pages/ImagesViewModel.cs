using AiPrompt.Helpers;
using AiPrompt.Models;
using AiPrompt.Services;
using MetadataExtractor;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Data;
using Wpf.Ui.Controls;
using IO = System.IO;

namespace AiPrompt.ViewModels.Pages;

public partial class ImagesViewModel(AppConfigService configService, GlobalResources globalResources) : ObservableObject, INavigationAware
{
    public static double ImageWidth { get; } = 240;

    private string? pathCache = null;

    public ObservableCollection<ImageItem> Images { get; set; } = new();
    public AppConfigService ConfigService { get; private set; } = configService;
    public GlobalResources GlobalResources { get; private set; } = globalResources;

    public bool IsShow { get; set; }
    public ImageItem? Current { get; set; }

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

        var scale = Utils.GetScale();
        BindingOperations.EnableCollectionSynchronization(Images, Images);

        await Task.Run(async () =>
        {
            var path = ConfigService.Config.ImagePath;
            string[] files = Array.Empty<string>();
            var cacheFiles = Enumerable.Empty<string>();

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

            
            foreach (var file in files)
            {
                ImageItem image = new();
                image.Path = file;

                ImagePrompt? imagePrompt = null;
                Console.WriteLine(file);
                var ds = ImageMetadataReader.ReadMetadata(file);
                foreach (var dire in ds)
                {
                    Console.WriteLine($"Name: {dire.Name}");
                    foreach (var tag in dire.Tags)
                    {
                        Console.WriteLine($"TagName:{tag.Name}    Value:{tag.Description}");
                    }
                    if (dire.Name == "PNG-tEXt")
                    {
                        var textualDataTag = dire.Tags.FirstOrDefault(v => v.Name == "Textual Data");
                        if(textualDataTag != null && textualDataTag.Description != null)
                        {
                            var data = TextualDataParse(textualDataTag.Description);
                            if (data != null)
                            {
                                image.TextualData = data;
                            }
                        }
                    }
                    else if (dire.Name == "Exif SubIFD")
                    {
                        foreach (var tag in dire.Tags)
                        {
                            try
                            {
                                if (tag.Description is not ['{', ..]) continue;
                                imagePrompt = JsonSerializer.Deserialize<ImagePrompt>(tag.Description, new JsonSerializerOptions()
                                {
                                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                                });
                            }
                            catch (Exception) { }
                        }
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                Console.WriteLine();
                image.ImagePrompt = imagePrompt;

                try
                {
                    ImageSizeReader.ImageSizeReaderUtil imageUtil = new();
                    using var fs = File.OpenRead(file);
                    var size = imageUtil.GetDimensions(fs);
                    image.Width = size.Width;
                    image.Height = size.Height;
                }
                catch (Exception)
                {
                    image.Width = (int)ImageWidth;
                    image.Height = (int)ImageWidth;
                    continue;
                }

                image.ZoomWidth = ImageWidth;
                image.ZoomHeight = image.Height * (image.ZoomWidth / image.Width);

                image.DecodeWidth = (int)Math.Min(image.Width, image.ZoomWidth);

                var temp = image.DecodeWidth * scale.XScale;
                if (temp > image.Width)
                {
                    image.RenderWidth = image.Width;
                }
                else
                {
                    image.RenderWidth = (int)temp;
                }

                await Task.Delay(10);
                Images.Add(image);
                //Application.Current.Dispatcher.Invoke(() => Images.Add(image));
            }

            Console.WriteLine($"一共{Images.Count}张图片");
        });
    }

    private ImageTextualData? TextualDataParse(string str)
    {
        /*
         *  解析内容:
         *  parameters: masterpiece,  best quality,  masterpiece, best quality, official art, extremely detailed CG unity 8k wallpaper,  day,  in summer,  bust,  simple_background,  girl,  solo,  loli,  long hair,  black hair,  asymmetrical bangs,  cute face,  laughing,  aqua eyes,  brown eyes,  small breasts,  whitedress,  arms behind back,  eye_contact
         *  Negative prompt: sketch,  duplicate,  ugly,  huge eyes,  text,  logo,  monochrome,  worst face,  (bad and mutated hands:1.3),  (worst quality:2.0),  (low quality:2.0),  (blurry:2.0),  horror,  geometry,  bad_prompt,  (bad hands),  (missing fingers),  multiple limbs,  bad anatomy,  (interlocked fingers:1.2),  Ugly Fingers,  (extra digit and hands and fingers and legs and arms:1.4),  ((2girl)),  (deformed fingers:1.2),  (long fingers:1.2), (bad-artist-anime),  bad-artist,  bad hand,  extra legs
         *  Steps: 30, Sampler: DPM++ 2M SDE Karras, CFG scale: 6.0, Seed: 2845123291, Size: 540x960, Model: EMS-30212-EMS, Denoising strength: 0, Clip skip: 2, TI hashes: "bad-artist-anime, bad-artist", Version: v1.5.2.0-6-g0599be9, TaskID: 632275029136035676, Used Embeddings: "bad-artist-anime, bad-artist"
         */

        const string line1Start = "parameters:";
        const string line2Start = "Negative prompt:";
        if (str.StartsWith(line1Start) != true) return null;

        var split = str[line1Start.Length..].Split('\n');
        if (split?.Length != 3) return null;

        var prompt = split[0].Trim();
        var negativePrompt = split[1][line2Start.Length..].Trim();
        var line3Info = split[2].Trim();

        List<KeyValuePair<string, string>> others = new();

        StringBuilder k = new();
        StringBuilder v = new();
        bool isQuotes = false;
        bool isValue = false;
        foreach (var c in line3Info)
        {
            switch (c)
            {
                case ':' when !isQuotes:
                    isValue = true;
                    break;
                case '"':
                    if (isQuotes)
                    {
                        isQuotes = false;
                        break;
                    }
                    isQuotes = true;
                    break;
                case ',' when !isQuotes:
                    others.Add(new(k.ToString(), v.ToString()));
                    k.Clear();
                    v.Clear();
                    isValue = false;
                    break;
                default:
                    if (isValue)
                    {
                        if (!isQuotes && char.IsWhiteSpace(c) && v.Length == 0) break;
                        v.Append(c);
                    }
                    else
                    {
                        if (!isQuotes && char.IsWhiteSpace(c) && k.Length == 0) break;
                        k.Append(c);
                    }
                    break;
            }
        }
        if (isValue)
        {
            others.Add(new(k.ToString(), v.ToString()));
        }

        //var otherInfo = line3Info.Split(',').Select(obj =>
        //{
        //    var kv = obj.Split(':');
        //    try
        //    {
        //        return new KeyValuePair<string, string>?(new(kv[0], kv[1]));
        //    }
        //    catch (Exception e)
        //    {
        //        return null;
        //    }
        //}).Where(v => v != null).Select(v => v!.Value).ToArray();

        ImageTextualData data = new();
        data.Prompt = prompt;
        data.NegativePrompt = negativePrompt;

        foreach (var item in others)
        {
            switch (item.Key)
            {
                case "Steps":
                    if (int.TryParse(item.Value, out var setps))
                        data.Setps = setps;
                    break;
                case "Sampler":
                    data.Sampler = item.Value;
                    break;
                case "CFG scale":
                    if (double.TryParse(item.Value, out var CFGScale))
                        data.CfgScale = CFGScale;
                    break;
                case "Seed":
                    if (long.TryParse(item.Value, out var seed))
                        data.Seed = seed;
                    break;
                case "Size":
                    data.Size = item.Value;
                    break;
                case "Model hash":
                    data.ModelHash = item.Value;
                    break;
                case "Model":
                    data.Model = item.Value;
                    break;
                case "Clip skip":
                    if (int.TryParse(item.Value, out var clipSkip))
                        data.ClipSkip = clipSkip;
                    break;
                case "ENSD":
                    if (int.TryParse(item.Value, out var ENSD))
                        data.ENSD = ENSD;
                    break;
                case "Denoising strength":
                    if (double.TryParse(item.Value, out var denoisingStrength))
                        data.DenoisingStrength = denoisingStrength;
                    break;
                case "Version":
                    data.Version = item.Value;
                    break;
                case "Hires resize":
                    data.HiresResize = item.Value;
                    break;
                case "Hires steps":
                    if (int.TryParse(item.Value, out var hiresSteps))
                        data.HiresSteps = hiresSteps;
                    break;
                case "Hires upscaler":
                    data.HiresUpscaler = item.Value;
                    break;
                default:
                    break;
            }
        }

        return data;
    }


    [RelayCommand]
    public void ShowImage(ImageItem image)
    {
        if (image.TextualData == null) return;
        Current = image;
        IsShow = true;
    }


    [RelayCommand]
    public void CloseImage(ImageItem image)
    {
        IsShow = false;
    }
}