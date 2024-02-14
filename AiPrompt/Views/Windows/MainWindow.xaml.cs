using AiPrompt.Models;
using AiPrompt.Services;
using AiPrompt.ViewModels.Windows;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AiPrompt.Views.Windows;

public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    private readonly AppConfigService configService;

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        IServiceProvider serviceProvider,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        AppConfigService configService
    )
    {
        SystemThemeWatcher.Watch(this);
        

        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();

        navigationService.SetNavigationControl(NavigationView);
        //snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        contentDialogService.SetContentPresenter(RootContentDialog);

        NavigationView.SetServiceProvider(serviceProvider);

        //NavigationView.Margin = new(0, TitleBar.Height, 0, 0);
        //_ = NavigationView.ItemTemplate;

        this.configService = configService;

        Width = configService.Config.WindowWidth ?? Width;
        Height = configService.Config.WindowHeight ?? Height;
    }

    public void RefreshTitleColor()
    {
        if (configService.Config.Theme is ApplicationTheme.Dark)
        {
            GlobalResources.Instance.TestString = "Dark";
            TitleBar.Background = new SolidColorBrush(Color.FromRgb(0x21, 0x1f, 0x1f));
        }
        else
        {
            GlobalResources.Instance.TestString = "Light";
            TitleBar.Background = null;
        }
    }

    private void FluentWindow_Loaded(object sender, RoutedEventArgs e)
    {
        ApplicationThemeManager.Apply(configService.Config.Theme);
        RefreshTitleColor();
    }

    private void FluentWindow_Closed(object sender, EventArgs e)
    {
        configService.Config.WindowHeight = (int)Height;
        configService.Config.WindowWidth = (int)Width;
    }
}
