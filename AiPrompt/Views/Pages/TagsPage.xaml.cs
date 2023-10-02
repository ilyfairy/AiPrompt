using AiPrompt.Models;
using AiPrompt.ViewModels.Pages;
using Microsoft.Xaml.Behaviors;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AiPrompt.Views.Pages
{
    public partial class TagsPage : Wpf.Ui.Controls.INavigableView<TagsViewModel>
    {
        public TagsViewModel ViewModel { get; }

        public TagsPage(TagsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }

        private async void Button_Drop(object sender, DragEventArgs e)
        {
            if (e.Source is FrameworkElement { DataContext: PromptItem item })
            {
                try
                {
                    var dir = Path.Combine(ViewModel.Config.ImageDropPath, item.Parent?.Parent?.Title ?? "", item.Parent?.Title ?? "");
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

                foreach (var kv in ViewModel.Resources.TabFileMap)
                {
                    if(kv.Value == item?.Parent?.Parent)
                    {
                        var json = JsonSerializer.Serialize(item.Parent.Parent, ViewModel.Resources.JsonOptions);
                        try
                        {
                            await File.WriteAllTextAsync(kv.Key, json);
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

}
