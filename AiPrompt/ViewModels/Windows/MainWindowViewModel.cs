// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.Models;
using System.Collections.ObjectModel;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace AiPrompt.ViewModels.Windows
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private AppConfig config;

        public string ApplicationTitle { get; set; } = "Ai Prompt";

        public ObservableCollection<object> MenuItems { get; set; } = new()
        {
            new NavigationViewItem()
            {
                Content = "首页",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.TagsPage)
            },
            new NavigationViewItem()
            {
                Content = "图片",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Image24 },
                TargetPageType = typeof(Views.Pages.ImagesPage)
            }
        };

        public ObservableCollection<object> FooterMenuItems { get; set; } = new()
        {
            new NavigationViewItem()
            {
                Content = "设置",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

        public MainWindowViewModel(AppConfig config)
        {
            this.config = config;
        }

        //public ObservableCollection<MenuItem> TrayMenuItems { get; set; } = new()
        //{
        //    new MenuItem { Header = "Home", Tag = "tray_home" }
        //};
    }
}
