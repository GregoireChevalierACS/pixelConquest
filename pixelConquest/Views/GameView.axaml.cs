using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace pixelConquest;

public partial class GameView : UserControl
{
    // Levé quand l'utilisateur veut revenir au menu.
    public event Action? BackRequested;

    private readonly GridView _gridView = new();
    private readonly DispatcherTimer _timer = new();

    private SimulationConfig _config = null!;
    private Simulation _simulation = null!;
    private int _tick;

    public GameView()
    {
        AvaloniaXamlLoader.Load(this);

        Panel host = this.FindControl<Panel>("GridHost")!;
        host.Children.Add(_gridView);

        this.FindControl<Button>("BackButton")!.Click += OnBack;

        _timer.Interval = TimeSpan.FromMilliseconds(50);
        _timer.Tick += OnGameTick;
    }

    // Démarre (ou redémarre) une partie avec la config donnée.
    public void Start(SimulationConfig config)
    {
        _config = config;
        StartSimulation();
        _timer.Start();
    }

    private void StartSimulation()
    {
        _simulation = Simulation.FromConfig(_config);
        _tick = 0;
        _gridView.SetSimulation(_simulation, _config);
        UpdateStatus(running: true);
    }

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

    private void OnBack(object? sender, RoutedEventArgs e)
    {
        _timer.Stop();
        BackRequested?.Invoke();
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

        this.FindControl<TextBlock>("StatusText")!.Text = sb.ToString();
    }
}
