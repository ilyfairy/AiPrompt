using AiPrompt.Helpers;
using AiPrompt.Models;
using MetadataExtractor;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace AiPrompt.Services;

public partial class TagsService(AppConfigService configService, SerializerService serializerService, ILogger<TagsService> _logger) : ObservableObject
{
    public static double ImageWidth { get; } = 240;

    private bool _isLoaded;
    private readonly AppConfigService configService = configService;

    public ObservableCollection<PromptTab> Tabs { get; set; } = new();
    public Dictionary<PromptTab, string> TabFileMap { get; set; } = new();

    public async Task Load()
    {
        List<PromptTab> tabs = new();
        Tabs.Clear();
        try
        {
            TabFileMap.Clear();
            foreach (var jsonFilePath in System.IO.Directory.GetFiles(configService.Config.TabPromptPath, serializerService.SearchPattern))
            {
                try
                {
                    var content = await File.ReadAllTextAsync(jsonFilePath);
                    PromptTab? tab = serializerService.Deserialize<PromptTab>(content);
                    if (tab == null) continue;

                    tab.FilePath = jsonFilePath;
                    tabs.Add(tab);
                    foreach (var block in tab.Items)
                    {
                        block.Parent = tab;
                        foreach (var item in block.Items)
                        {
                            item.Parent = block;
                        }
                    }
                    TabFileMap.Add(tab, jsonFilePath);
                }
                catch (Exception)
                {
                }
            }
        }
        catch (Exception) { }

        foreach (var item in tabs.OrderBy(v => v.Index))
        {
            Tabs.Add(item);
        }
    }

    //PNG-tEXt  ->  Textual Data  ->  parameters:xxxx \n Negative prompt:xxx \n kvs
    public StableDiffusionTextInfo? ParseStableDiffusionText(string textualDataString)
    {
        /*
         *  解析内容:
         *  parameters: masterpiece,  best quality,  masterpiece, best quality, official art, extremely detailed CG unity 8k wallpaper,  day,  in summer,  bust,  simple_background,  girl,  solo,  loli,  long hair,  black hair,  asymmetrical bangs,  cute face,  laughing,  aqua eyes,  brown eyes,  small breasts,  whitedress,  arms behind back,  eye_contact
         *  Negative prompt: sketch,  duplicate,  ugly,  huge eyes,  text,  logo,  monochrome,  worst face,  (bad and mutated hands:1.3),  (worst quality:2.0),  (low quality:2.0),  (blurry:2.0),  horror,  geometry,  bad_prompt,  (bad hands),  (missing fingers),  multiple limbs,  bad anatomy,  (interlocked fingers:1.2),  Ugly Fingers,  (extra digit and hands and fingers and legs and arms:1.4),  ((2girl)),  (deformed fingers:1.2),  (long fingers:1.2), (bad-artist-anime),  bad-artist,  bad hand,  extra legs
         *  Steps: 30, Sampler: DPM++ 2M SDE Karras, CFG scale: 6.0, Seed: 2845123291, Size: 540x960, Model: EMS-30212-EMS, Denoising strength: 0, Clip skip: 2, TI hashes: "bad-artist-anime, bad-artist", Version: v1.5.2.0-6-g0599be9, TaskID: 632275029136035676, Used Embeddings: "bad-artist-anime, bad-artist"
         */


        const string line1Start = "parameters:";
        const string line2Start = "Negative prompt:";
        if (textualDataString.StartsWith(line1Start) != true) return null;

        var split = textualDataString[line1Start.Length..].Split('\n');
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

        StableDiffusionTextInfo data = new();
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
                case "VAE":
                    data.Vae = item.Value;
                    break;
                case "TaskID":
                    data.TaskID = item.Value;
                    break;
                default:
                    break;
            }
        }

        return data;
    }

    // PNG-tEXt  ->  Textual Data  -> Comment:{json}
    public OfficialImageCommentInfo? ParseOfficialComment(string textualDataString)
    {
        string startWith = "Comment: ";
        if (!textualDataString.StartsWith(startWith))
            return null;

        textualDataString = textualDataString[startWith.Length..];

        var result = JsonSerializer.Deserialize<OfficialImageCommentInfo>(textualDataString);

        return result;
    }


    public ImageInfo? GetImageInfo(string file, (double XScale, double YScale) scale)
    {
        ImageInfo imageInfo = new();
        imageInfo.Path = file;

        IReadOnlyList<MetadataExtractor.Directory> ds;
        try
        {
            ds = ImageMetadataReader.ReadMetadata(file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "图片加载失败 {File}", file);
            return null;
        }

        _logger.LogInformation("File: {File}", file);

        foreach (var dire in ds)
        {
            if (dire.Tags.FirstOrDefault(v => v.Name == "Textual Data") is { Description: { } textualDataTag })
            {
                if (ParseStableDiffusionText(textualDataTag) is { } sd) // sd
                {
                    imageInfo.StableDiffusionTextInfo = sd;
                }
                else if (ParseOfficialComment(textualDataTag) is { } official) // official
                {
                    imageInfo.OfficialCommentInfo = official;
                }
                else if (textualDataTag.StartsWith("prompt: ")) // new1 json
                {
                    // json
                }
            }

            //`(*>﹏<*)′=================================== 一条华丽的分割线 ===================================`(*>﹏<*)′//

            if (dire.Tags.FirstOrDefault(v => v.Name is "User Comment") is { Description: { } userComment }) // sd
            {
                try
                {
                    // User Comment
                    imageInfo.StableDiffusionCommentInfo = JsonSerializer.Deserialize<StableDiffusionCommentInfo>(userComment, new JsonSerializerOptions()
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "User Comment 解析失败 File:{File}  UserComment:{UserComment}", file, userComment);
                }
            }

            //`(*>﹏<*)′=================================== 一条华丽的分割线 ===================================`(*>﹏<*)′//

            if (dire.Tags.FirstOrDefault(v => v.Name is "Textual Data") is { Description: { } new1 }) // new1 json
            {
                string startWith = "generation_data: ";
                if (new1.StartsWith(startWith))
                {
                    try
                    {
                        // User Comment
                        imageInfo.StableDiffusionCommentInfo = JsonSerializer.Deserialize<StableDiffusionCommentInfo>(new1[startWith.Length..], new JsonSerializerOptions()
                        {
                            NumberHandling = JsonNumberHandling.AllowReadingFromString
                        });
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "generation_data 解析失败 File:{File}  UserComment:{UserComment}", file, new1);
                    }
                }
                if (ParseStableDiffusionText(new1) is { } sd) // sd
                {
                    imageInfo.StableDiffusionTextInfo = sd;
                }
            }

            //if (dire.Name == "PNG-tEXt")
            //{
            //}
            //else if (dire.Name == "Exif SubIFD")
            //{
            //}
            //else if (dire.Name == "PNG-iTXt")
            //{
            //}
        }

        //解析图片大小
        try
        {
            ImageSizeReader.ImageSizeReaderUtil imageUtil = new();
            using var fs = File.OpenRead(file);
            var size = imageUtil.GetDimensions(fs);
            imageInfo.Width = size.Width;
            imageInfo.Height = size.Height;
        }
        catch (Exception)
        {
            imageInfo.Width = (int)ImageWidth;
            imageInfo.Height = (int)ImageWidth;
            return null;
        }

        imageInfo.ZoomWidth = ImageWidth;
        imageInfo.ZoomHeight = imageInfo.Height * (imageInfo.ZoomWidth / imageInfo.Width);

        imageInfo.DecodeWidth = (int)Math.Min(imageInfo.Width, imageInfo.ZoomWidth);

        var temp = imageInfo.DecodeWidth * scale.XScale;
        if (temp > imageInfo.Width)
        {
            imageInfo.RenderWidth = imageInfo.Width;
        }
        else
        {
            imageInfo.RenderWidth = (int)temp;
        }

        return imageInfo;
    }
}
