using System.Drawing;
using System.IO;
using System.Windows.Controls;
using AiPrompt.Models;
using AiPrompt.Services;
using AiPrompt.ViewModels.Components;
using Microsoft.Extensions.Logging;

namespace AiPrompt.Views.Components;

/// <summary>
/// Interaction logic for TagsBlock.xaml
/// </summary>
public partial class TagsTabComponent : ItemsControl
{
    private readonly AppConfigService _configService;
    private readonly TagsService _tagsService;
    private readonly ILogger<TagsTabComponent> _logger;
    private readonly SerializerService _serializerService;

    public TagsTabComponentViewModel ViewModel { get; }

    public TagsTabComponent()
    {
        ViewModel = new();
        DataContext = this;

        _configService = App.GetService<AppConfigService>()!;
        _tagsService = App.GetService<TagsService>()!;
        _logger = App.GetService<ILogger<TagsTabComponent>>()!;
        _serializerService = App.GetService<SerializerService>()!;
        InitializeComponent();

        _logger.LogInformation("创建TagsTabComponent实例");
    }


    //拖拽图片过来
    private async void Button_Drop(object sender, DragEventArgs e)
    {
        if (e.Source is FrameworkElement { DataContext: PromptItem item })
        {
            try
            {
                //dir 需要存放图片的文件夹
                var dir = Path.Combine(_configService.Config.ImageDropPath, item.Parent?.Parent?.Title ?? "", item.Parent?.Title ?? "");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                //如果是图片
                if (e.Data.GetDataPresent(DataFormats.Bitmap))
                {
                    if (e.Data.GetData(DataFormats.Bitmap) is Bitmap bitmap)
                    {
                        var path = Path.Combine(dir, $"{item.PromptCN.Replace(" ", "")}.png");
                        bitmap.Save(path);
                        item.IsImageRelativeTag = false;
                        item.Image = null;
                        item.Image = path;
                    }
                }
                //如果是文件路径
                else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    if (e.Data.GetData(DataFormats.FileDrop) is string[] and [var file])
                    {
                        var path = Path.Combine(dir, $"{item.PromptCN.Replace(" ", "")}{Path.GetExtension(file)}");
                        File.Copy(file, path, true);
                        item.IsImageRelativeTag = false;
                        item.Image = null;
                        item.Image = path;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "拖拽异常");
                return;
            }

            foreach (var (tab, filePath) in _tagsService.TabFileMap)
            {
                if (tab == item?.Parent?.Parent)
                {
                    //var json = JsonSerializer.Serialize(item.Parent.Parent, ViewModel.Resources.JsonOptions);
                    try
                    {
                        var content = _serializerService.Serialize(item.Parent.Parent);
                        await File.WriteAllTextAsync(filePath, content);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "拖拽保存配置文件失败");
                    }
                    break;
                }
            }
        }
    }




    //public PromptTab? PromptTab
    //{
    //    get { return (PromptTab?)GetValue(PromptTabProperty); }
    //    set { SetValue(PromptTabProperty, value); }
    //}

    //public static readonly DependencyProperty PromptTabProperty = DependencyProperty.Register("PromptTab", typeof(PromptTab), typeof(TagsTabComponent), new PropertyMetadata(null, (dp, args) =>
    //{
    //    var sv = App.GetService<TagsTabCacheService>()!;
    //    var content = sv.Get((PromptTab)args.NewValue);
    //    (dp as TagsTabComponent)!.Content = content;
    //    Console.WriteLine("变更...");
    //}));


}
