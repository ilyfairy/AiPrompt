using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AiPrompt.Models;



public class ImageInfo
{
    public int Width { get; set; }

    public int Height { get; set; }

    public double ZoomWidth { get; set; }

    public double ZoomHeight { get; set; }

    public int DecodeWidth { get; set; }

    public int RenderWidth { get; set; }

    public string? Path { get; set; }

    /// <summary>
    /// 用户输入的, 例如用户输入的种子是-1
    /// </summary>
    public StableDiffusionCommentInfo? StableDiffusionCommentInfo { get; set; }

    /// <summary>
    /// 实际生成的, 例如实际生成的种子是固定值
    /// </summary>
    public StableDiffusionTextInfo? StableDiffusionTextInfo { get; set; }

    public OfficialImageCommentInfo? OfficialCommentInfo { get; set; }


    //`(*>﹏<*)′=================================== 一条华丽的分割线 ===================================`(*>﹏<*)′//


    [MemberNotNullWhen(true, nameof(OfficialCommentInfo))]
    public bool IsOfficial => OfficialCommentInfo is { };

    [MemberNotNullWhen(true, nameof(StableDiffusionTextInfo))]
    public bool IsStableDiffusionText => StableDiffusionTextInfo is { };

    public bool IsStableDiffusionComment => StableDiffusionCommentInfo is { };

    /// <summary>
    /// 正面提示词
    /// </summary>
    public string? Prompt => StableDiffusionTextInfo?.Prompt ?? StableDiffusionCommentInfo?.Prompt ?? OfficialCommentInfo?.Prompt;
    public string[]? PromptArray => Prompt?.Split(',').Select(v => v.Trim()).ToArray();

    /// <summary>
    /// 负面提示词
    /// </summary>
    public string? NegativePrompt => StableDiffusionTextInfo?.NegativePrompt ?? StableDiffusionCommentInfo?.NegativePrompt ?? OfficialCommentInfo?.Uc;
    public string[]? NegativePromptArray => NegativePrompt?.Split(',').Select(v => v.Trim()).ToArray();

    public string? Model => StableDiffusionTextInfo?.Model ?? StableDiffusionCommentInfo?.BaseModel?.ModelFileName;
    public int? ClipSkip => StableDiffusionCommentInfo?.ClipSkip ?? StableDiffusionCommentInfo?.ClipSkip;

    /// <summary>
    /// 采样算法
    /// </summary>
    public string? Sampler => StableDiffusionCommentInfo?.SamplerName ?? StableDiffusionCommentInfo?.SamplerName ?? OfficialCommentInfo?.Sampler;

    /// <summary>
    /// 迭代步数
    /// </summary>
    public int? Setps => StableDiffusionCommentInfo?.Steps?? StableDiffusionCommentInfo?.Steps ?? OfficialCommentInfo?.Steps;

    /// <summary>
    /// 种子
    /// </summary>
    public long? Seed => StableDiffusionTextInfo?.Seed ?? StableDiffusionCommentInfo?.Seed ?? OfficialCommentInfo?.Seed;

    /// <summary>
    /// 提示词相关性(CFG Scale)
    /// </summary>
    public double? CfgScale => StableDiffusionCommentInfo?.CfgScale ?? StableDiffusionCommentInfo?.CfgScale ?? OfficialCommentInfo?.Scale; // TODO: verify

    public string? Vae => StableDiffusionTextInfo?.Vae ?? StableDiffusionCommentInfo?.SdVae;

    /// <summary>
    /// 修复方式
    /// </summary>
    public string? HiresUpscaler => StableDiffusionTextInfo?.HiresUpscaler ?? StableDiffusionCommentInfo?.HrUpscaler;

    /// <summary>
    /// 高分辨率步数
    /// </summary>
    public int? HiresSteps => StableDiffusionTextInfo?.HiresSteps;

    /// <summary>
    /// 重绘幅度
    /// </summary>
    public double? DenoisingStrength => StableDiffusionTextInfo?.DenoisingStrength ?? StableDiffusionCommentInfo?.DenoisingStrength;

    /// <summary>
    /// 放大倍数
    /// </summary>
    public string? HiresResize => StableDiffusionTextInfo?.HiresResize 
        ?? (StableDiffusionCommentInfo?.HrResizeX is { } && StableDiffusionCommentInfo?.HrResizeY is { } 
            ? $"{StableDiffusionCommentInfo.HrResizeX}x{StableDiffusionCommentInfo.HrResizeY}" 
        : null); // 1080x1920
}

public class StableDiffusionCommentInfo
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = string.Empty;

    [JsonPropertyName("negativePrompt")]
    public string NegativePrompt { get; set; } = string.Empty;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("imageCount")]
    public int ImageCount { get; set; }

    [JsonPropertyName("samplerName")]
    public string SamplerName { get; set; } = string.Empty;

    [JsonPropertyName("steps")]
    public int Steps { get; set; }

    [JsonPropertyName("cfgScale")]
    public double CfgScale { get; set; }

    [JsonPropertyName("seed")]
    public long Seed { get; set; }

    [JsonPropertyName("clipSkip")]
    public int ClipSkip { get; set; }

    [JsonPropertyName("baseModel")]
    public StableDiffusionCommentBaseModel? BaseModel { get; set; }

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
    public string? SdVae { get; set; }

    [JsonPropertyName("sdxl")]
    public object? Sdxl { get; set; }

    [JsonPropertyName("workEngine")]
    public string? WorkEngine { get; set; }
}

public class StableDiffusionCommentBaseModel
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
    public string BaseModel { get; set; } = string.Empty;

    [JsonPropertyName("hash")]
    public string Hash { get; set; } = string.Empty;
}

/// <summary>
/// 图片文件中Textual的提示词
/// </summary>
public class StableDiffusionTextInfo
{
    public string Prompt { get; set; } = string.Empty;
    public string NegativePrompt { get; set; } = string.Empty;
    public int Setps { get; set; }
    public string Sampler { get; set; } = string.Empty;
    public double CfgScale { get; set; }
    public long Seed { get; set; }
    public string Size { get; set; } = "0x0"; // width x height
    public string? ModelHash { get; set; }
    public string Model { get; set; } = string.Empty;
    public int ClipSkip { get; set; }
    public int ENSD { get; set; }
    public double DenoisingStrength { get; set; }
    public string? Version { get; set; }
    public string? HiresResize { get; set; }
    public int? HiresSteps { get; set; }
    public string? HiresUpscaler { get; set; }
    public string? Vae { get; set; }
    public string? TaskID { get; set; }
}

public record OfficialImageCommentInfo(
    [property: JsonPropertyName("prompt")] string Prompt,
    [property: JsonPropertyName("steps")] int Steps,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("width")] int Width,
    [property: JsonPropertyName("scale")] double Scale,
    [property: JsonPropertyName("uncond_scale")] double UncondScale,
    [property: JsonPropertyName("cfg_rescale")] double CfgRescale,
    [property: JsonPropertyName("seed")] long Seed,
    [property: JsonPropertyName("n_samples")] int NSamples,
    [property: JsonPropertyName("hide_debug_overlay")] bool HideDebugOverlay,
    [property: JsonPropertyName("noise_schedule")] string NoiseSchedule,
    [property: JsonPropertyName("legacy_v3_extend")] bool LegacyV3Extend,
    [property: JsonPropertyName("sampler")] string Sampler,
    [property: JsonPropertyName("controlnet_strength")] double ControlnetStrength,
    [property: JsonPropertyName("controlnet_model")] object ControlnetModel,
    [property: JsonPropertyName("dynamic_thresholding")] bool DynamicThresholding,
    [property: JsonPropertyName("dynamic_thresholding_percentile")] double DynamicThresholdingPercentile,
    [property: JsonPropertyName("dynamic_thresholding_mimic_scale")] double DynamicThresholdingMimicScale,
    [property: JsonPropertyName("sm")] bool Sm,
    [property: JsonPropertyName("sm_dyn")] bool SmDyn,
    [property: JsonPropertyName("skip_cfg_below_sigma")] double SkipCfgBelowSigma,
    [property: JsonPropertyName("lora_unet_weights")] object LoraUnetWeights,
    [property: JsonPropertyName("lora_clip_weights")] object LoraClipWeights,
    [property: JsonPropertyName("uc")] string Uc, // 负面提示词
    [property: JsonPropertyName("request_type")] string RequestType,
    [property: JsonPropertyName("signed_hash")] string SignedHash
);
