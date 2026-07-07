using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace pixelConquest;

public partial class MenuView : UserControl
{
    public event Action? CreateStrategyRequested;
    public event Action<SimulationConfig>? LaunchRequested;

    private readonly StrategyCatalog _catalog;

    // Associe chaque profil affiché à sa checkbox, pour relire les cochés au lancement.
    private readonly List<(StrategyProfile Profile, CheckBox Box)> _rows = new();

    public MenuView()
    {
        AvaloniaXamlLoader.Load(this);

        string path = Path.Combine(AppContext.BaseDirectory, "strategies.json");
        _catalog = new StrategyCatalog(path);
        _catalog.Load();

        this.FindControl<Button>("CreateButton")!.Click += (_, _) =>
            CreateStrategyRequested?.Invoke();
        this.FindControl<Button>("LaunchButton")!.Click += OnLaunch;

        BuildStrategyList();
    }

    // Construit une ligne par profil : [checkbox] [pastille couleur] Nom [type].
    private void BuildStrategyList()
    {
        _rows.Clear();
        ItemsControl list = this.FindControl<ItemsControl>("StrategyList")!;
        List<Control> items = new();

        if (_catalog.Profiles.Count == 0)
        {
            items.Add(new TextBlock
            {
                Text = "Aucune stratégie. Crée-en une pour commencer.",
                Foreground = Brush.Parse("#8d99ae"),
                Margin = new Thickness(8),
            });
        }

        // On coche les 4 premières par défaut (pratique pour lancer vite).
        for (int i = 0; i < _catalog.Profiles.Count; i++)
        {
            StrategyProfile profile = _catalog.Profiles[i];

            CheckBox box = new() { IsChecked = i < 4, VerticalAlignment = VerticalAlignment.Center };

            Border swatch = new()
            {
                Width = 16,
                Height = 16,
                CornerRadius = new CornerRadius(3),
                Background = HexBrush(profile.Color),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(8, 0, 8, 0),
            };

            TextBlock label = new()
            {
                Text = $"{profile.Name}",
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
            };

            TextBlock type = new()
            {
                Text = $"  [{profile.Type}]",
                Foreground = Brush.Parse("#8d99ae"),
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center,
            };

            StackPanel row = new()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(6, 4, 6, 4),
                Children = { box, swatch, label, type },
            };

            items.Add(row);
            _rows.Add((profile, box));
        }

        list.ItemsSource = items;
    }

    private void OnLaunch(object? sender, RoutedEventArgs e)
    {
        List<StrategyProfile> selected = _rows
            .Where(r => r.Box.IsChecked == true)
            .Select(r => r.Profile)
            .ToList();

        TextBlock hint = this.FindControl<TextBlock>("HintText")!;

        if (selected.Count < 2)
        {
            hint.Text = "Sélectionne au moins 2 stratégies pour lancer une partie.";
            return;
        }
        if (selected.Count > SimulationConfig.MaxStrategies)
        {
            hint.Text = $"Maximum {SimulationConfig.MaxStrategies} stratégies par partie.";
            return;
        }

        int width = (int)(this.FindControl<NumericUpDown>("WidthInput")!.Value ?? 80);
        int height = (int)(this.FindControl<NumericUpDown>("HeightInput")!.Value ?? 50);

        SimulationConfig config = new()
        {
            Width = width,
            Height = height,
            Strategies = selected,
        };

        if (!config.IsValid(out string? error))
        {
            hint.Text = error;
            return;
        }

        hint.Text = "";
        LaunchRequested?.Invoke(config);
    }

    private static IBrush HexBrush(string hex) =>
        Color.TryParse(hex, out Color c) ? new SolidColorBrush(c) : Brushes.Gray;
}
