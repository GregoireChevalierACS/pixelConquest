using Avalonia;

namespace pixelConquest;

class Program
{
    // Point d'entrée. [STAThread] est requis par Avalonia sur Windows.
    [STAThread]
    public static void Main(string[] args) =>
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Configure l'application Avalonia (plateforme, rendu, thème).
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
