using AiPrompt.Models;
using AiPrompt.ViewModels.Windows;
using System.Windows.Media;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace AiPrompt.Views.Windows
{
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow(
            MainWindowViewModel viewModel,
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            ISnackbarService snackbarService,
            IContentDialogService contentDialogService
        )
        {
            Watcher.Watch(this);

            ViewModel = viewModel;
            DataContext = this;
            InitializeComponent();

            navigationService.SetNavigationControl(NavigationView);
            //snackbarService.SetSnackbarPresenter(SnackbarPresenter);
            contentDialogService.SetContentPresenter(RootContentDialog);

            NavigationView.SetServiceProvider(serviceProvider);

            //NavigationView.Margin = new(0, TitleBar.Height, 0, 0);
            //_ = NavigationView.ItemTemplate;

            Width = ViewModel.Config.WindowWidth ?? Width;
            Height = ViewModel.Config.WindowHeight ?? Height;
        }

        public void RefreshTitleColor()
        {
            if (ViewModel.Config.Theme is ThemeType.Dark)
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
            Theme.Apply(ViewModel.Config.Theme);
            RefreshTitleColor();
        }

        private void FluentWindow_Closed(object sender, EventArgs e)
        {
            ViewModel.Config.WindowHeight = (int)Height;
            ViewModel.Config.WindowWidth = (int)Width;
        }
    }
}
