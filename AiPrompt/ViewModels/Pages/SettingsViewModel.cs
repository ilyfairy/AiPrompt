// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.Models;
using AiPrompt.Views.Windows;
using System.Reflection;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AiPrompt.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject, INavigationAware
    {
        public string AppVersion { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
        public GlobalResources GlobalResources { get; private set; }

        [ObservableProperty]
        private AppConfig config;

        public SettingsViewModel(AppConfig config, GlobalResources globalResources)
        {
            this.config = config;
            GlobalResources = globalResources;
        }


        public void OnNavigatedTo()
        {

        }

        public void OnNavigatedFrom()
        {
            Config.Save();
        }


        [RelayCommand]
        private void OnChangeTheme(string parameter)
        {
            switch (parameter)
            {
                case "theme_light":
                    if (Config.Theme == ThemeType.Light)
                        break;

                    Theme.Apply(ThemeType.Light);
                    Config.Theme = ThemeType.Light;
                    break;

                default:
                    if (Config.Theme == ThemeType.Dark)
                        break;

                    Theme.Apply(ThemeType.Dark);
                    Config.Theme = ThemeType.Dark;
                    break;
            }
            (Application.Current.MainWindow as MainWindow)?.RefreshTitleColor();
        }
    }
}
