using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AiPrompt.Models;



public class ImageItem
{
    public int Width { get; set; }
    public int Height { get; set; }
    public double ZoomWidth { get; set; }
    public double ZoomHeight { get; set; }
    public int DecodeWidth { get; set; }
    public int RenderWidth { get; set; }

    public string Path { get; set; } = string.Empty;
    public ImagePrompt? ImagePrompt { get; set; }
    public ImageTextualData? TextualData { get; set; }
}

public class ImagePrompt
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("negativePrompt")]
    public string NegativePrompt { get; set; } = string.Empty;

    [JsonPropertyName("width")]
    public long Width { get; set; }

    [JsonPropertyName("height")]
    public long Height { get; set; }

    [JsonPropertyName("imageCount")]
    public long ImageCount { get; set; }

    [JsonPropertyName("samplerName")]
    public string SamplerName { get; set; } = string.Empty;

    [JsonPropertyName("steps")]
    public long Steps { get; set; }

    [JsonPropertyName("cfgScale")]
    public long CfgScale { get; set; }

    [JsonPropertyName("seed")]
    public long Seed { get; set; }

    [JsonPropertyName("clipSkip")]
    public long ClipSkip { get; set; }

    [JsonPropertyName("baseModel")]
    public ImagePromptBaseModel? BaseModel { get; set; }

    [JsonPropertyName("enableHr")]
    public bool EnableHr { get; set; }

    [JsonPropertyName("hrUpscaler")]
    public string HrUpscaler { get; set; } = string.Empty;

    [JsonPropertyName("hrSecondPassSteps")]
    public long HrSecondPassSteps { get; set; }

    [JsonPropertyName("hrResizeX")]
    public long HrResizeX { get; set; }

    [JsonPropertyName("hrResizeY")]
    public long HrResizeY { get; set; }

    [JsonPropertyName("denoisingStrength")]
    public double DenoisingStrength { get; set; }

    [JsonPropertyName("sdVae")]
    public string SdVae { get; set; } = string.Empty;

    [JsonPropertyName("sdxl")]
    public object Sdxl { get; set; } = string.Empty;
}

public class ImagePromptBaseModel
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("modelId")]
    public string ModelId { get; set; } = string.Empty;

    [JsonPropertyName("modelFileId")]
    public string ModelFileId { get; set; } = string.Empty;

    [JsonPropertyName("modelFileName")]
    public string ModelFileName { get; set; } = string.Empty;

    [JsonPropertyName("baseModel")]
    public string BaseModelBaseModel { get; set; } = string.Empty;

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;
}

/// <summary>
/// 图片文件中Textual的提示词
/// </summary>
public class ImageTextualData
{
    public string Prompt { get; set; } = string.Empty;
    public string NegativePrompt { get; set; } = string.Empty;
    public int Setps { get; set; }
    public string Sampler { get; set; } = string.Empty;
    public double CfgScale { get; set; }
    public long Seed { get; set; }
    public string Size { get; set; } = "0x0";
    public string? ModelHash { get; set; }
    public string Model { get; set; } = string.Empty;
    public int ClipSkip { get; set; }
    public int ENSD { get; set; }
    public double DenoisingStrength { get; set; }
    public string? Version { get; set; }
    public string? HiresResize { get; set; }
    public int? HiresSteps { get; set; }
    public string? HiresUpscaler { get; set; }
}