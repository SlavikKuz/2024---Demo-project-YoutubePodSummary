using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace YoutubePodSmart.WinForms;

internal static class Program
{
    private static Mutex _mutex = null!;

    [STAThread]
    private static void Main()
    {
        try
        {
            if (!EnsureSingleInstance("AkhApp"))
            {
                MessageBox.Show("An instance of the application is already running.",
                    "Single Instance",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var configuration = BuildConfiguration();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            var host = CreateHostBuilder(configuration).Build();

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = host.Services.GetRequiredService<MainForm>();
            Application.Run(mainForm);
        }
        catch (Exception ex)
        {
            Log.Logger?.Fatal(ex, "An unhandled exception occurred during application execution.");
            MessageBox.Show("And somehow we crushed :(. Please check the log file for details.",
                "Fatal Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static bool EnsureSingleInstance(string appName)
    {
        _mutex = new Mutex(true, appName, out var createdNew);
        return createdNew;
    }

    private static IHostBuilder CreateHostBuilder(IConfiguration configuration) =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((context, config) => config.AddConfiguration(configuration))
            .ConfigureServices((context, services) => ConfigureServices(services, configuration))
            .UseSerilog();

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(_ => new MainForm(configuration));

        //services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        //services.AddSingleton(Log.Logger);
    }

    public static IConfiguration BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

#if DEBUG
        builder.AddJsonFile("appsettings.dev.json", optional: true, reloadOnChange: true);
#endif

        return builder.Build();
    }
}