using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AiPrompt.Models;

public class PromptItem : ObservableObject
{
    //public string[] Prompts { get; set; } = Array.Empty<string>();
    public string PromptCN { get; set; } = string.Empty;
    public string PromptEN { get; set; } = string.Empty;

    /// <summary>
    /// 图片路径, 可以是网络路径,或者本地路径以及相对路径
    /// </summary>
    public string? Image { get; set; }

    /// <summary>
    /// 图片路径是否相对于程序目录
    /// </summary>
    public bool IsImageRelativeTag { get; set; }

    public bool IsDefault { get; set; }

    [JsonIgnore]
    public PromptUIConfig Config { get; set; } = new();

    [JsonIgnore]
    public PromptBlock? Parent { get; set; }

    public class PromptUIConfig : ObservableObject
    {
        public bool IsSelected { get; set; }
        public int Weight { get; set; }
    }
}


