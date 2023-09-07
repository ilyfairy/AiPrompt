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
    public const string ConfigFilePath = "config.json";

    public ThemeType Theme { get; set; } = ThemeType.Light;
    public string TabPromptPath { get; set; } = string.Empty;
    public string ImageDropPath { get; set; } = "TagImages";
    public string ImagePath { get; set; } = "Images";
    public int? WindowWidth { get; set; }
    public int? WindowHeight { get; set; }

    public static string AppDirectory { get; private set; } = AppContext.BaseDirectory;

    ///// <summary>
    ///// false: 显示 ((prompt)), 或 [[[prompt]]]<br/>
    ///// true: 显示 (prompt:1.2) 或 (prompt:0.7)
    ///// </summary>
    //public bool IsSwitchWeight { get; set; }

    private static readonly JsonSerializerOptions options = new();

    static AppConfig()
    {
        options.Converters.Add(new JsonStringEnumConverter());
    }

    public void Save()
    {
		try
		{
            File.WriteAllText(Path.Combine(AppDirectory, ConfigFilePath), JsonSerializer.Serialize(this, options));
        }
		catch (Exception)
		{

		}
    }

    public static AppConfig LoadOrCreate()
    {
        AppConfig? config;
        try
        {
            var location = Path.Combine(AppDirectory, ConfigFilePath);
            Console.WriteLine($"ConfigLocation: {location}");
            config = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(location), options) ?? new();
        }
        catch (Exception)
        {
            config = new();
        }
        return config;

    }
}
