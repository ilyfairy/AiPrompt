﻿// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.Models;
using AiPrompt.Services;
using AiPrompt.ViewModels.Pages;
using AiPrompt.ViewModels.Windows;
using AiPrompt.Views.Pages;
using AiPrompt.Views.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Windows.Media;
using System.Windows.Threading;
using Wpf.Ui;

namespace AiPrompt;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging

    private static readonly IHost _host = Host
        .CreateDefaultBuilder()
        .ConfigureAppConfiguration(c => 
        {
            string dir = AppContext.BaseDirectory;
            c.SetBasePath(dir);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddHostedService<ApplicationHostService>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ISnackbarService, SnackbarService>();
            services.AddSingleton<IContentDialogService, ContentDialogService>();

            services.AddSingleton<TagsPage>();
            services.AddSingleton<TagsViewModel>();

            services.AddSingleton<ImagesPage>();
            services.AddSingleton<ImagesViewModel>();

            services.AddSingleton<SettingsPage>();
            services.AddSingleton<SettingsViewModel>();

            services.AddSingleton(GlobalResources.Instance);
            services.AddSingleton<TagsService>();
            services.AddSingleton<AppConfigService>();
            services.AddSingleton<SerializerService>();
            services.AddSingleton<TagsTabCacheService>();

            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            logging.AddSerilog(Log.Logger);
        })
        .Build();

    /// <summary>
    /// Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null"/>.</returns>
    public static T? GetService<T>() where T : class
    {
        return _host.Services.GetService(typeof(T)) as T;
    }

    /// <summary>
    /// Occurs when the application is loading.
    /// </summary>
    private void OnStartup(object sender, StartupEventArgs e)
    {
        try
        {
            //主题
            var configService = _host.Services.GetRequiredService<AppConfigService>();
            configService.LoadOrCreate();
            GlobalResources.Instance.SetThemeColor(configService.Config.Theme == Wpf.Ui.Appearance.ApplicationTheme.Light);

            //字体
            var fontUri = new Uri("pack://application:,,,/Assets/font.ttf");
            var fonts = Fonts.GetFontFamilies(fontUri);
            var font = fonts.First();

            Current.Resources["GlobalFont"] = font;

            foreach (var item in Current.Resources.MergedDictionaries)
            {
                if (item.Source.ToString() == "pack://application:,,,/Wpf.Ui;component/Styles/Wpf.Ui.xaml")
                {
                    foreach (var xaml in item.MergedDictionaries)
                    {
                        if (xaml.Source.ToString() == "pack://application:,,,/Wpf.Ui;component/Styles/Assets/Fonts.xaml")
                        {
                            xaml["ContentControlThemeFontFamily"] = font;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {
        }


        _host.Start();
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    private async void OnExit(object sender, ExitEventArgs e)
    {
        var config = _host.Services.GetService<AppConfigService>();
        config?.Save();
        await _host.StopAsync();
        _host.Dispose();
    }

    /// <summary>
    /// Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }
}
