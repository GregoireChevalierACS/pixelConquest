using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace pixelConquest;

public partial class StrategyCreatorView : UserControl
{
    // Levé quand l'utilisateur a fini (sauvegarde ou annulation) → retour menu.
    public event Action? Done;

    private readonly StrategyCatalog _catalog;

    // Palette de couleurs proposées (cliquables).
    private static readonly string[] PaletteColors =
    {
        "#e63946", "#f4a261", "#e9c46a", "#2a9d8f",
        "#457b9d", "#8338ec", "#ff006e", "#fb5607",
        "#3a86ff", "#06d6a0", "#ef476f", "#ffd166",
    };

    public StrategyCreatorView()
    {
        AvaloniaXamlLoader.Load(this);

        string path = Path.Combine(AppContext.BaseDirectory, "strategies.json");
        _catalog = new StrategyCatalog(path);
        _catalog.Load();

        SetupTypeCombo();
        SetupSliders();
        SetupPalette();
        SetupColorSync();

        this.FindControl<Button>("BackButton")!.Click += (_, _) => Done?.Invoke();
        this.FindControl<Button>("SaveButton")!.Click += OnSave;
    }

    private void SetupTypeCombo()
    {
        ComboBox combo = this.FindControl<ComboBox>("TypeInput")!;
        combo.ItemsSource = Enum.GetValues<StrategyType>();
        combo.SelectedIndex = 0;
    }

    // Chaque slider met à jour son label numérique en direct.
    private void SetupSliders()
    {
        BindSlider("AggrSlider", "AggrValue");
        BindSlider("RandSlider", "RandValue");
        BindSlider("EncSlider", "EncValue");
        BindSlider("CompSlider", "CompValue");
    }

    private void BindSlider(string sliderName, string labelName)
    {
        Slider slider = this.FindControl<Slider>(sliderName)!;
        TextBlock label = this.FindControl<TextBlock>(labelName)!;
        label.Text = slider.Value.ToString("0.0");
        slider.PropertyChanged += (_, e) =>
        {
            if (e.Property == Slider.ValueProperty)
            {
                label.Text = slider.Value.ToString("0.0");
            }
        };
    }

    // Construit la grille de pastilles de couleur cliquables.
    private void SetupPalette()
    {
        WrapPanel palette = this.FindControl<WrapPanel>("Palette")!;
        foreach (string hex in PaletteColors)
        {
            Border swatch = new()
            {
                Width = 28,
                Height = 28,
                Margin = new Thickness(0, 0, 6, 6),
                CornerRadius = new CornerRadius(4),
                Background = Color.TryParse(hex, out Color c)
                    ? new SolidColorBrush(c)
                    : Brushes.Gray,
                Cursor = new Cursor(StandardCursorType.Hand),
            };
            swatch.PointerPressed += (_, _) => SetColor(hex);
            palette.Children.Add(swatch);
        }
    }

    // Synchronise le champ hex avec l'aperçu quand l'utilisateur le tape à la main.
    private void SetupColorSync()
    {
        TextBox hex = this.FindControl<TextBox>("HexInput")!;
        hex.PropertyChanged += (_, e) =>
        {
            if (e.Property == TextBox.TextProperty)
            {
                UpdatePreview(hex.Text ?? "");
            }
        };
    }

    private void SetColor(string hex)
    {
        this.FindControl<TextBox>("HexInput")!.Text = hex;
        UpdatePreview(hex);
    }

    private void UpdatePreview(string hex)
    {
        Border preview = this.FindControl<Border>("ColorPreview")!;
        if (Color.TryParse(hex, out Color c))
        {
            preview.Background = new SolidColorBrush(c);
        }
    }

    private void OnSave(object? sender, RoutedEventArgs e)
    {
        TextBlock error = this.FindControl<TextBlock>("ErrorText")!;

        string name = (this.FindControl<TextBox>("NameInput")!.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            error.Text = "Donne un nom à ta stratégie.";
            return;
        }

        string hex = (this.FindControl<TextBox>("HexInput")!.Text ?? "").Trim();
        if (!Color.TryParse(hex, out _))
        {
            error.Text = "Couleur invalide (format attendu : #RRGGBB).";
            return;
        }

        // Avertit si le nom existe déjà (on remplacera).
        StrategyType type = (StrategyType)this.FindControl<ComboBox>("TypeInput")!.SelectedItem!;

        StrategyProfile profile = new()
        {
            Name = name,
            Type = type,
            Color = hex,
            Aggressiveness = this.FindControl<Slider>("AggrSlider")!.Value,
            Randomness = this.FindControl<Slider>("RandSlider")!.Value,
            EncirclementPriority = this.FindControl<Slider>("EncSlider")!.Value,
            Compactness = this.FindControl<Slider>("CompSlider")!.Value,
        };

        _catalog.AddOrUpdate(profile);
        _catalog.Save();

        Done?.Invoke();
    }
}
