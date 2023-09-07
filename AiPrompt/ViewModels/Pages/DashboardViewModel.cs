// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.Helpers;
using AiPrompt.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace AiPrompt.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject, INavigationAware
{
    public static double PromptImageSize { get; } = 190;
    public static double PromptImageSizeSmall { get; } = 180;

    private bool _initialized = false;

    public AppConfig Config { get; set; }
    public GlobalResources Resources { get; set; }
    public PromptTab? CurrentTab { get; set; }

    [AlsoNotifyFor(nameof(PositivePromptText), nameof(PositivePromptText))]
    public bool? WeightMode { get; set; } = false;

    [AlsoNotifyFor(nameof(PositivePromptText))]
    public ObservableCollection<PromptItem> PositivePromptItems { get; set; } = new();
    public string PositivePromptText => GetPromptText(PositivePromptItems);

    [AlsoNotifyFor(nameof(NegativePromptText))]
    public ObservableCollection<PromptItem> NegativePromptItems { get; set; } = new();
    public string NegativePromptText => GetPromptText(NegativePromptItems);

   

    
    public DashboardViewModel(AppConfig config, GlobalResources resources)
    {
        Config = config;
        Resources = resources;
    }

    public async void OnNavigatedTo()
    {
        if (!_initialized || Resources.Tabs.Count == 0)
        {
            _initialized = true;
            await LoadTab();
        }
    }

    public void OnNavigatedFrom() { }


    [RelayCommand]
    public async Task LoadTab()
    {
        NegativePromptItems.Clear();
        PositivePromptItems.Clear();
        OnPropertyChanged(nameof(NegativePromptText));
        OnPropertyChanged(nameof(PositivePromptText));

        List<PromptTab> tabs = new();
        Resources.Tabs.Clear();
        try
        {
            Resources.TabFileMap.Clear();
            foreach (var jsonFile in Directory.GetFiles(Config.TabPromptPath, "*.json"))
            {
                try
                {
                    var content = await File.ReadAllTextAsync(jsonFile);
                    var tab = JsonSerializer.Deserialize<PromptTab>(content);
                    if (tab == null) continue;
                    tab.FilePath = jsonFile;
                    tabs.Add(tab);
                    foreach (var block in tab.Items)
                    {
                        block.Parent = tab;
                        foreach (var item in block.Items)
                        {
                            item.Parent = block;
                        }
                    }
                    Resources.TabFileMap.Add(jsonFile, tab);
                }
                catch (Exception)
                {
                }
            }
        }
        catch (Exception) { }

        foreach (var item in tabs.OrderBy(v => v.Index))
        {
            Resources.Tabs.Add(item);
        }
        CurrentTab = tabs.FirstOrDefault();

        foreach (var tab in Resources.Tabs)
        {
            foreach (var block in tab.Items)
            {
                foreach (var item in block.Items)
                {
                    if (item.IsDefault)
                    {
                        Select(item);
                    }
                }
            }
        }
    }

    private string GetPromptText(IEnumerable<PromptItem> items)
    {
        return string.Join(", ", items.Select(v =>
        {
            return WeightToStringConverter.To(v, WeightMode);
        }));
    }

    [RelayCommand]
    public void SwitchWeightMode()
    {
        WeightMode = WeightMode switch
        {
            false => true,
            true => null,
            null => false,
        };
    }


    public void Select(PromptItem item)
    {
        if (item.Parent?.Parent?.IsNegative == true)
        {
            if (!item.Config.IsSelected)
            {
                item.Config.IsSelected = true;
                NegativePromptItems.Add(item);
            }
            else
            {
                item.Config.IsSelected = false;
                NegativePromptItems.Remove(item);
            }
            OnPropertyChanged(nameof(NegativePromptText));
        }
        else
        {
            if (!item.Config.IsSelected)
            {
                item.Config.IsSelected = true;
                PositivePromptItems.Add(item);
            }
            else
            {
                item.Config.IsSelected = false;
                PositivePromptItems.Remove(item);
            }
            OnPropertyChanged(nameof(PositivePromptText));
        }
    }


    #region ImageClick
    [RelayCommand]
    public void OnPromptImage(PromptItem item)
    {
        if (Keyboard.Modifiers is ModifierKeys.Control)
        {
            OnWeightInc(item);
            return;
        }
        else if (Keyboard.Modifiers is ModifierKeys.Shift)
        {
            OnWeightDec(item);
            return;
        }

        Select(item);
    }

    [RelayCommand]
    public void OnWeightReset(PromptItem item)
    {
        item.Config.Weight = 0;
        OnPropertyChanged(nameof(PositivePromptText));
        OnPropertyChanged(nameof(NegativePromptText));
    }


    [RelayCommand]
    public void OnWeightInc(PromptItem item)
    {
        item.Config.Weight++;
        OnPropertyChanged(nameof(PositivePromptText));
        OnPropertyChanged(nameof(NegativePromptText));
    }


    [RelayCommand]
    public void OnWeightDec(PromptItem item)
    {
        item.Config.Weight--;
        OnPropertyChanged(nameof(PositivePromptText));
        OnPropertyChanged(nameof(NegativePromptText));
    }
    #endregion



    #region 正面提示词
    //[RelayCommand]
    //private void OnButton1()
    //{
    //}

    [RelayCommand]
    private void OnPositiveCopy()
    {
        try
        {
            Clipboard.SetText(PositivePromptText);
        }
        catch { }
    }

    [RelayCommand]
    private void OnPositiveClear()
    {
        foreach (var item in PositivePromptItems)
        {
            item.Config.IsSelected = false;
        }
        PositivePromptItems.Clear();
        OnPropertyChanged(nameof(PositivePromptText));
    }
    #endregion


    #region 负面提示词

    [RelayCommand]
    private void OnNegativeCopy()
    {
        try
        {
            Clipboard.SetText(NegativePromptText);
        }
        catch { }
    }

    //[RelayCommand]
    //private void OnNegativeSave()
    //{
    //}

    [RelayCommand]
    private void OnNegativeDefault()
    {
        
    }

    [RelayCommand]
    private void OnNegativeClear()
    {
        //IsWeightConvert = !IsWeightConvert;
        //OnPropertyChanged(nameof(NegativePromptItems));
        //OnPropertyChanged(nameof(PositivePromptItems));

        foreach (var item in NegativePromptItems)
        {
            item.Config.IsSelected = false;
        }
        NegativePromptItems.Clear();
        OnPropertyChanged(nameof(NegativePromptText));
    }

    #endregion

}
