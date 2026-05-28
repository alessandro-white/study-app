using Avalonia;
using StudyApp.Services;

namespace StudyApp;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        DatabaseService.Instance.Initialize();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
