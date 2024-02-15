using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AiPrompt.Helpers;
using AiPrompt.Messages;
using AiPrompt.Models;
using CommunityToolkit.Mvvm.Messaging;

namespace AiPrompt.ViewModels.Components;

public partial class TagsTabComponentViewModel
{
    private readonly IMessenger _messenger;

    public TagsTabComponentViewModel()
    {
        _messenger = App.GetService<IMessenger>()!;
    }

    [RelayCommand]
    public void OnPromptImage(PromptItem item)
    {
        _messenger.Send(new SelectPromptItemMessage(item));
    }




    #region 图片右键菜单
    [RelayCommand]
    public void CopyCurrentTag(PromptItem item)
    {
        Utils.SetClipboardText(item.PromptEN);
    }

    [RelayCommand]
    public void OnWeightReset(PromptItem item)
    {
        item.Config.Weight = 0;
        _messenger.Send(PromptTextUpdateMessage.Instance);
    }


    [RelayCommand]
    public void OnWeightInc(PromptItem item)
    {
        item.Config.Weight++;
        _messenger.Send(PromptTextUpdateMessage.Instance);
    }


    [RelayCommand]
    public void OnWeightDec(PromptItem item)
    {
        item.Config.Weight--;
        _messenger.Send(PromptTextUpdateMessage.Instance);
    }
    #endregion

}
