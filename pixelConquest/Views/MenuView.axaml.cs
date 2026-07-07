using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace pixelConquest;

public partial class MenuView : UserControl
{
    public event Action? CreateStrategyRequested;
    public event Action<SimulationConfig>? LaunchRequested;

    public MenuView()
    {
        AvaloniaXamlLoader.Load(this);

        this.FindControl<Button>("CreateButton")!.Click += (_, _) =>
            CreateStrategyRequested?.Invoke();

        this.FindControl<Button>("LaunchButton")!.Click += OnLaunch;
    }

    // Stub temporaire : lance une config par défaut. Sera remplacé à l'étape "Menu".
    private void OnLaunch(object? sender, RoutedEventArgs e)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "strategies.json");
        StrategyCatalog catalog = new(path);
        catalog.Load();

        SimulationConfig config = new()
        {
            Width = 80,
            Height = 50,
            Strategies = catalog.Profiles.Take(4).ToList(),
        };

        LaunchRequested?.Invoke(config);
    }
}
