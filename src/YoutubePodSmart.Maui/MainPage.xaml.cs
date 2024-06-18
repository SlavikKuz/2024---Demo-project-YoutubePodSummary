using Microsoft.Extensions.Logging;
using YoutubePodSmart.Maui.ViewModels;

namespace YoutubePodSmart.Maui;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel, ILogger<MainPage> logger)
    {
        InitializeComponent();

        NavigationPage.SetHasNavigationBar(this, false);

        _viewModel = viewModel;
        BindingContext = _viewModel;

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred.");
            }
        };
    }
}