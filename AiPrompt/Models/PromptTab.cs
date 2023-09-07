using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AiPrompt.Models;

public class PromptTab : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public int Index { get; set; }
    public bool IsNegative { get; set; }
    public ObservableCollection<PromptBlock> Items { get; set; } = new();

    [JsonIgnore]
    public string? FilePath { get; set; }
}
