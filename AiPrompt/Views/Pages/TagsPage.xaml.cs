using AiPrompt.Models;
using AiPrompt.Services;
using AiPrompt.ViewModels.Pages;
using Microsoft.Extensions.Logging;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace AiPrompt.Views.Pages;

public partial class TagsPage : Wpf.Ui.Controls.INavigableView<TagsViewModel>
{
    private readonly TagsService _tagsService;
    private readonly ILogger<TagsPage> _logger;
    private readonly SerializerService _serializerService;

    public TagsViewModel ViewModel { get; }

    public TagsPage(TagsViewModel viewModel, TagsService tagsService, ILogger<TagsPage> logger, SerializerService serializerService)
    {
        ViewModel = viewModel;
        _tagsService = tagsService;
        _logger = logger;
        _serializerService = serializerService;
        DataContext = this;

        InitializeComponent();
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
