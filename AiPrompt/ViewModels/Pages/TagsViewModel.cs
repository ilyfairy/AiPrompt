// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.Helpers;
using AiPrompt.Messages;
using AiPrompt.Models;
using AiPrompt.Services;
using CommunityToolkit.Mvvm.Messaging;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace AiPrompt.ViewModels.Pages;

public partial class TagsViewModel : ObservableObject, INavigationAware, IRecipient<SelectPromptItemMessage>, IRecipient<PromptTextUpdateMessage>
{
    public static double PromptImageSize { get; } = 190;
    public static double PromptImageSizeSmall { get; } = 180;


    private bool _initialized = false;

    public TagsViewModel(AppConfigService configService, GlobalResources resources, TagsService tagsService,IMessenger messenger)
    {
        ConfigService = configService;
        Resources = resources;
        TagsService = tagsService;

        messenger.Register<SelectPromptItemMessage>(this);
        messenger.Register<PromptTextUpdateMessage>(this);
    }

    public AppConfigService ConfigService { get; set; }
    public GlobalResources Resources { get; set; }
    public TagsService TagsService { get; set; }
    public PromptTab? CurrentTab { get; set; }

    [AlsoNotifyFor(nameof(PositivePromptText), nameof(PositivePromptText))]
    public bool? WeightMode { get; set; } = false;


    [AlsoNotifyFor(nameof(PositivePromptText))]
    public ObservableCollection<PromptItem> PositivePromptItems { get; set; } = new();
    public string PositivePromptText => GetPromptText(PositivePromptItems);

    [AlsoNotifyFor(nameof(NegativePromptText))]
    public ObservableCollection<PromptItem> NegativePromptItems { get; set; } = new();
    public string NegativePromptText => GetPromptText(NegativePromptItems);

    public async void OnNavigatedTo()
    {
        if (!_initialized || TagsService.Tabs.Count == 0)
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

        await TagsService.Load();
        // TODO: xxx

        CurrentTab = TagsService.Tabs.FirstOrDefault();

        foreach (var tab in TagsService.Tabs)
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


    /// <summary>
    /// 选中或取消
    /// </summary>
    /// <param name="item"></param>
    /// <param name="forceOn">强制选中</param>
    public void Select(PromptItem item, bool forceOn = false)
    {
        //负面tag
        if (item.Parent?.Parent?.IsNegative == true)
        {
            if (!item.Config.IsSelected || forceOn)
            {
                item.Config.IsSelected = true;
                if (!NegativePromptItems.Contains(item))
                {
                    NegativePromptItems.Add(item);
                }
            }
            else
            {
                item.Config.IsSelected = false;
                NegativePromptItems.Remove(item);
            }
            OnPropertyChanged(nameof(NegativePromptText));
        }
        //正面tag
        else
        {
            if (!item.Config.IsSelected || forceOn)
            {
                item.Config.IsSelected = true;
                if (!PositivePromptItems.Contains(item))
                {
                    PositivePromptItems.Add(item);
                }
            }
            else
            {
                item.Config.IsSelected = false;
                PositivePromptItems.Remove(item);
            }
            OnPropertyChanged(nameof(PositivePromptText));
        }
    }

    //接收到TagsTabComponent的消息
    public void Receive(SelectPromptItemMessage message)
    {
        var item = message.PromptItem;
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

    #region 鼠标右键菜单

    [RelayCommand]
    public void OnWeightReset(PromptItem item)
    {
        item.Config.Weight = 0;
        Receive(PromptTextUpdateMessage.Instance);
    }

    [RelayCommand]
    public void OnWeightInc(PromptItem item)
    {
        item.Config.Weight++;
        Receive(PromptTextUpdateMessage.Instance);
    }

    [RelayCommand]
    public void OnWeightDec(PromptItem item)
    {
        item.Config.Weight--;
        Receive(PromptTextUpdateMessage.Instance);
    }

    public void Receive(PromptTextUpdateMessage message)
    {
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
        foreach (var tab in TagsService.Tabs)
        {
            foreach (var block in tab.Items)
            {
                foreach (var item in block.Items)
                {
                    if (item.IsDefault)
                    {
                        Select(item, true);
                    }
                }
            }
        }
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
