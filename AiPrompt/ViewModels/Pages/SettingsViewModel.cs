// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.Models;
using AiPrompt.Services;
using AiPrompt.Views.Windows;
using System.Reflection;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AiPrompt.ViewModels.Pages;

public partial class SettingsViewModel(AppConfigService appConfigService, GlobalResources globalResources) : ObservableObject, INavigationAware
{
    public string AppVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    public GlobalResources GlobalResources { get; private set; } = globalResources;

    [ObservableProperty]
    private AppConfigService configService = appConfigService;

    public void OnNavigatedTo()
    {
    }

    public void OnNavigatedFrom()
    {
        ConfigService.Save();
    }


    [RelayCommand]
    private void OnChangeTheme(string parameter)
    {
        switch (parameter)
        {
            case "theme_light":
                if (ConfigService.Config.Theme == ApplicationTheme.Light)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Light);
                ConfigService.Config.Theme = ApplicationTheme.Light;
                break;

            default:
                if (ConfigService.Config.Theme == ApplicationTheme.Dark)
                    break;

                ApplicationThemeManager.Apply(ApplicationTheme.Dark);
                ConfigService.Config.Theme = ApplicationTheme.Dark;
                break;
        }
        (Application.Current.MainWindow as MainWindow)?.RefreshTitleColor();
    }
}
