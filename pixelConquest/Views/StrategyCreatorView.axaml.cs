using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace pixelConquest;

public partial class StrategyCreatorView : UserControl
{
    // Levé quand l'utilisateur a fini (sauvegarde ou annulation) → retour menu.
    public event Action? Done;

    public StrategyCreatorView()
    {
        AvaloniaXamlLoader.Load(this);
        this.FindControl<Button>("BackButton")!.Click += (_, _) => Done?.Invoke();
    }
}
