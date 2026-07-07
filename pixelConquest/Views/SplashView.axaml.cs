using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace pixelConquest;

public partial class SplashView : UserControl
{
    // Levé quand l'écran d'accueil a fini (après ~3s).
    public event Action? Finished;

    private readonly DispatcherTimer _revealTimer = new();
    private readonly DispatcherTimer _holdTimer = new();
    private PixelLogo _logo = null!;
    private double _reveal;

    public SplashView()
    {
        AvaloniaXamlLoader.Load(this);
        _logo = this.FindControl<PixelLogo>("Logo")!;

        // Le logo apparaît pixel par pixel, puis on tient l'écran ~3s au total.
        _logo.Reveal = 0.0;

        _revealTimer.Interval = TimeSpan.FromMilliseconds(25);
        _revealTimer.Tick += (_, _) =>
        {
            _reveal += 0.04;
            _logo.Reveal = Math.Min(_reveal, 1.0);
            if (_reveal >= 1.0)
            {
                _revealTimer.Stop();
            }
        };
        _revealTimer.Start();

        _holdTimer.Interval = TimeSpan.FromSeconds(3);
        _holdTimer.Tick += (_, _) =>
        {
            _holdTimer.Stop();
            Finished?.Invoke();
        };
        _holdTimer.Start();
    }
}
