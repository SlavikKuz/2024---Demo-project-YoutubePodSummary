using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YoutubePodSmart.Maui.ViewModels;

namespace YoutubePodSmart.Maui;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        // Assuming you have a method to get the configuration and logger instances
        IConfiguration configuration = GetConfiguration();
        ILogger<MainViewModel> logger = GetLogger<MainViewModel>();

        try
        {
            _viewModel = new MainViewModel(configuration, logger);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        // Set the BindingContext to the ViewModel
        BindingContext = _viewModel;

        AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                logger.LogError(ex, "An unhandled exception occurred.");
            }
        };
    }

    private IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(FileSystem.AppDataDirectory))
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

#if DEBUG
        builder.AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true);
#endif

        return builder.Build();
    }

    private ILogger<T> GetLogger<T>()
    {
        // Implement this method to retrieve your ILogger instance
        using var loggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });

        return loggerFactory.CreateLogger<T>();
    }
}