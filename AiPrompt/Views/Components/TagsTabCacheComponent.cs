using System.Windows.Controls;
using AiPrompt.Models;
using AiPrompt.Services;

namespace AiPrompt.Views.Components;

public class TagsTabCacheComponent : UserControl
{
    private readonly TagsTabCacheService _tagsTabCacheService;

    public TagsTabCacheComponent()
    {
        _tagsTabCacheService = App.GetService<TagsTabCacheService>()!;
    }

    public PromptTab? PromptTab
    {
        get { return (PromptTab?)GetValue(PromptTabProperty); }
        set { SetValue(PromptTabProperty, value); }
    }

    public static readonly DependencyProperty PromptTabProperty = DependencyProperty.Register("PromptTab", typeof(PromptTab), typeof(TagsTabCacheComponent), new PropertyMetadata(null, (dp, args) =>
    {
        var sv = App.GetService<TagsTabCacheService>()!;
        var content = sv.Get((PromptTab)args.NewValue);
        (dp as TagsTabCacheComponent)!.Content = content;
    }));
}
