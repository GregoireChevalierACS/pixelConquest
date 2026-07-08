using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace pixelConquest;

public partial class MainWindow : Window
{
    private ContentControl _host = null!;

    public MainWindow()
    {
        AvaloniaXamlLoader.Load(this);
        _host = this.FindControl<ContentControl>("ViewHost")!;

        ShowSplash();
    }

    // Remplace la vue affichée par la fenêtre.
    private void Navigate(Control view) => _host.Content = view;

    // --- Écran d'accueil ---
    private void ShowSplash()
    {
        SplashView splash = new();
        splash.Finished += ShowMenu;
        Navigate(splash);
    }

    // --- Écran de menu ---
    private void ShowMenu()
    {
        MenuView menu = new();
        menu.CreateStrategyRequested += ShowStrategyCreator;
        menu.EditStrategyRequested += ShowStrategyEditor;
        menu.LaunchRequested += ShowGame;
        Navigate(menu);
    }

    // --- Écran de création de stratégie ---
    private void ShowStrategyCreator()
    {
        StrategyCreatorView creator = new();
        // Retour au menu après sauvegarde ou annulation.
        creator.Done += ShowMenu;
        Navigate(creator);
    }

    // --- Écran de création en mode édition d'une stratégie existante ---
    private void ShowStrategyEditor(StrategyProfile profile)
    {
        StrategyCreatorView creator = new();
        creator.Done += ShowMenu;
        creator.LoadForEdit(profile);
        Navigate(creator);
    }

    // --- Écran de jeu ---
    private void ShowGame(SimulationConfig config)
    {
        GameView game = new();
        game.BackRequested += ShowMenu;
        Navigate(game);
        game.Start(config);
    }
}
