using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using AiPrompt.Models;
using AiPrompt.Views.Components;

namespace AiPrompt.Services;

public class TagsTabCacheService
{
    private readonly Dictionary<PromptTab, TagsTabComponent> cache = [];

    public TagsTabCacheService()
    {
        
    }

    public TagsTabComponent Get(PromptTab promptTab)
    {
        if (cache.TryGetValue(promptTab, out var control))
        {
            return control;
        }
        else
        {
            var newControl = new TagsTabComponent();
            //newControl.ItemsSource = promptTab.Items;

            Task.Run(() =>
            {
                // 使用Dispatcher在UI线程上添加数据到newControl
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    foreach (var item in promptTab.Items)
                    {
                        await Task.Delay(50);
                        newControl.Items.Add(item);
                    }
                });
            });

            cache.Add(promptTab, newControl);
            return newControl;
        }
    }

    public void Clear()
    {
        foreach (var item in cache)
        {
        }
        cache.Clear();
    }

}
