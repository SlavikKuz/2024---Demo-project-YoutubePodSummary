using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;
using YoutubePodSmart.Maui.ViewModels;

namespace YoutubePodSmart.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Load appsettings.json from embedded resource
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("YoutubePodSmart.Maui.appsettings.json");

            if (stream == null)
            {
                throw new FileNotFoundException("appsettings.json not found in the assembly resources.");
            }

            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(stream);

#if DEBUG
            // Load appsettings.dev.json from embedded resource in debug mode
            using var devStream = assembly.GetManifestResourceStream("YoutubePodSmart.Maui.appsettings.dev.json");

            if (devStream != null)
            {
                configBuilder.AddJsonStream(devStream);
            }
#endif

            var config = configBuilder.Build();

            // Add the configuration to the builder
            builder.Configuration.AddConfiguration(config);

            // Register configuration and logging services
            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(config.GetSection("Logging"));
                loggingBuilder.AddConsole();
#if DEBUG
                loggingBuilder.AddDebug();
#endif
            });

            // Register the MainPage and its ViewModel
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<MainViewModel>();

            var app = builder.Build();

            return app;
        }
        catch (Exception ex)
        {
            var c = ex.Message;
            throw;
        }
    }
}