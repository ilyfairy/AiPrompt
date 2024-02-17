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
        if (promptTab is null)
            return new TagsTabComponent();

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
                // 使用Dispatcher在UI线程上添加数据到new Control
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    foreach (var block in promptTab.Items)
                    {
                        await Task.Delay(100);
                        newControl.Items.Add(block);
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
