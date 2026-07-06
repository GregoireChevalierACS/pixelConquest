using System.Text;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace pixelConquest;

public partial class MainWindow : Window
{
    private readonly GridView _gridView = new();
    private readonly DispatcherTimer _timer = new();

    private Simulation _simulation = null!;
    private SimulationConfig _config = null!;
    private int _tick;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);

        // Insère le contrôle de rendu dans le Panel hôte défini en XAML.
        Panel host = this.FindControl<Panel>("GridHost")!;
        host.Children.Add(_gridView);

        StartNewSimulation();

        // Game loop : un tick toutes les 50 ms.
        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += OnGameTick;
        _timer.Start();
    }

    private void StartNewSimulation()
    {
        _config = BuildDefaultConfig();
        _simulation = Simulation.FromConfig(_config);
        _tick = 0;
        _gridView.SetSimulation(_simulation, _config);
        UpdateStatus(running: true);
    }

    // Appelé à chaque intervalle du timer : avance la simulation d'un pas,
    // redessine, et arrête le timer quand la partie est finie.
    private void OnGameTick(object? sender, EventArgs e)
    {
        bool moved = _simulation.Tick();
        _tick++;
        _gridView.Redraw();

        if (!moved)
        {
            _timer.Stop();
            UpdateStatus(running: false);
            return;
        }

        UpdateStatus(running: true);
    }

    private void UpdateStatus(bool running)
    {
        Dictionary<int, int> counts = new();
        Grid grid = _simulation.Grid;
        for (int x = 0; x < grid.LengthX; x++)
        {
            for (int y = 0; y < grid.LengthY; y++)
            {
                int owner = grid.Cells[x, y];
                counts[owner] = counts.GetValueOrDefault(owner) + 1;
            }
        }

        StringBuilder sb = new();
        sb.Append(running ? "▶ " : "■ ");
        sb.Append($"tick {_tick}");
        for (int i = 0; i < _config.Strategies.Count; i++)
        {
            int id = i + 1;
            sb.Append($"   {_config.Strategies[i].Name}: {counts.GetValueOrDefault(id)}");
        }
        if (!running)
        {
            sb.Append("   — Terminé");
        }

        TextBlock status = this.FindControl<TextBlock>("StatusText")!;
        status.Text = sb.ToString();
    }

    // Config par défaut : charge le catalogue (l'amorce si absent) et engage
    // ses profils sur un canvas de bonne taille.
    private static SimulationConfig BuildDefaultConfig()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "strategies.json");
        StrategyCatalog catalog = new(path);
        catalog.Load();

        if (catalog.Profiles.Count == 0)
        {
            catalog.AddOrUpdate(new StrategyProfile
            {
                Name = "Rusher", Type = StrategyType.Bfs, Color = "#e63946",
                Randomness = 0.05, Compactness = 0.1,
            });
            catalog.AddOrUpdate(new StrategyProfile
            {
                Name = "Turtle", Type = StrategyType.Defensive, Color = "#457b9d",
                Compactness = 0.95,
            });
            catalog.AddOrUpdate(new StrategyProfile
            {
                Name = "Predator", Type = StrategyType.Aggressive, Color = "#2a9d8f",
                Aggressiveness = 0.8, Randomness = 0.1,
            });
            catalog.AddOrUpdate(new StrategyProfile
            {
                Name = "Chaos", Type = StrategyType.Random, Color = "#f4a261",
                Randomness = 1.0,
            });
            catalog.Save();
        }

        return new SimulationConfig
        {
            Width = 80,
            Height = 50,
            Strategies = catalog.Profiles.Take(4).ToList(),
        };
    }
}
