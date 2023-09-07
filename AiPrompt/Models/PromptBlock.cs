using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace AiPrompt.Models;

public class PromptBlock : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public ObservableCollection<PromptItem> Items { get; set; } = new();

    [JsonIgnore]
    public PromptTab? Parent { get; set; }

}
