using AiPrompt.Models;
using AiPrompt.Services;
using AiPrompt.ViewModels.Pages;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace AiPrompt.Views.Pages;

public partial class TagsPage : Wpf.Ui.Controls.INavigableView<TagsViewModel>
{
    private readonly TagsService tagsService;

    public TagsViewModel ViewModel { get; }

    public TagsPage(TagsViewModel viewModel, TagsService tagsService)
    {
        ViewModel = viewModel;
        this.tagsService = tagsService;
        DataContext = this;

        InitializeComponent();
    }

    private async void Button_Drop(object sender, DragEventArgs e)
    {
        if (e.Source is FrameworkElement { DataContext: PromptItem item })
        {
            try
            {
                var dir = Path.Combine(ViewModel.ConfigService.Config.ImageDropPath, item.Parent?.Parent?.Title ?? "", item.Parent?.Title ?? "");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (e.Data.GetDataPresent(DataFormats.Bitmap))
                {
                    if (e.Data.GetData(DataFormats.Bitmap) is Bitmap bitmap)
                    {
                        var path = Path.Combine(dir, $"{item.PromptEN.Replace(" ", "")}.png");
                        bitmap.Save(path);
                        item.IsImageRelativeTag = false;
                        item.Image = null;
                        item.Image = path;
                    }
                }
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
                    {
                        var file = files[0];
                        var path = Path.Combine(dir, $"{item.PromptEN.Replace(" ", "")}{Path.GetExtension(file)}");
                        File.Copy(file, path, true);
                        item.IsImageRelativeTag = false;
                        item.Image = null;
                        item.Image = path;
                    }
                }
            }
            catch { }

            foreach (var (tab, filePath) in tagsService.TabFileMap)
            {
                if (tab == item?.Parent?.Parent)
                {
                    var json = JsonSerializer.Serialize(item.Parent.Parent, ViewModel.Resources.JsonOptions);
                    try
                    {
                        await File.WriteAllTextAsync(filePath, json);
                    }
                    catch { }
                    break;
                }
            }
        }
    }

    private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        ViewModel.WeightMode = ViewModel.WeightMode switch
        {
            false => true,
            true => null,
            null => false,
        };
    }

    private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
    }
}
