﻿using AiPrompt.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AiPrompt.Services;

public partial class TagsService : ObservableObject
{
    private bool _isLoaded;
    private readonly AppConfig config;

    public ObservableCollection<PromptTab> Tabs { get; set; } = new();
    public Dictionary<PromptTab, string> TabFileMap { get; set; } = new();

    public TagsService(AppConfig config)
    {
        this.config = config;
    }

    public async Task Load()
    {
        List<PromptTab> tabs = new();
        Tabs.Clear();
        try
        {
            TabFileMap.Clear();
            foreach (var jsonFilePath in Directory.GetFiles(config.TabPromptPath, "*.json"))
            {
                try
                {
                    var content = await File.ReadAllTextAsync(jsonFilePath);
                    var tab = JsonSerializer.Deserialize<PromptTab>(content);
                    if (tab == null) continue;
                    tab.FilePath = jsonFilePath;
                    tabs.Add(tab);
                    foreach (var block in tab.Items)
                    {
                        block.Parent = tab;
                        foreach (var item in block.Items)
                        {
                            item.Parent = block;
                        }
                    }
                    TabFileMap.Add(tab, jsonFilePath);
                }
                catch (Exception)
                {
                }
            }
        }
        catch (Exception) { }

        foreach (var item in tabs.OrderBy(v => v.Index))
        {
            Tabs.Add(item);
        }
    }

}
