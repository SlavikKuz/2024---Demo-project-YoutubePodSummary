using YoutubePodSmart.WinForms;

namespace YoutubePodSmart;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new MainFrom());
    }
}