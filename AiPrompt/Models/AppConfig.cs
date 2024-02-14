// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wpf.Ui.Appearance;

namespace AiPrompt.Models;

public class AppConfig : ObservableObject
{
    public ApplicationTheme Theme { get; set; } = ApplicationTheme.Light;
    public string TabPromptPath { get; set; } = string.Empty;
    public string ImageDropPath { get; set; } = "TagImages";
    public string ImagePath { get; set; } = "Images";
    public int? WindowWidth { get; set; }
    public int? WindowHeight { get; set; }

    ///// <summary>
    ///// false: 显示 ((prompt)), 或 [[[prompt]]]<br/>
    ///// true: 显示 (prompt:1.2) 或 (prompt:0.7)
    ///// </summary>
    //public bool IsSwitchWeight { get; set; }
}
