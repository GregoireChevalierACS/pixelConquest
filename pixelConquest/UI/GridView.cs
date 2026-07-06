using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace pixelConquest;

// Contrôle Avalonia qui dessine l'état d'une grille : une case colorée par pixel.
// La couleur d'un pixel = couleur du profil de la stratégie qui le possède
// (0 = neutre). Le rendu s'adapte à la taille disponible du contrôle.
class GridView : Control
{
    private Simulation? _simulation;

    // ID de stratégie → pinceau de couleur (converti depuis le hex du profil).
    private readonly Dictionary<int, IBrush> _brushes = new();

    // Couleur des pixels neutres.
    private static readonly IBrush NeutralBrush = new SolidColorBrush(Color.FromRgb(30, 30, 34));

    public void SetSimulation(Simulation simulation, SimulationConfig config)
    {
        _simulation = simulation;

        _brushes.Clear();
        for (int i = 0; i < config.Strategies.Count; i++)
        {
            int id = i + 1;
            Color color = ParseHex(config.Strategies[i].Color);
            _brushes[id] = new SolidColorBrush(color);
        }

        InvalidateVisual();
    }

    // Demande à Avalonia de redessiner (appelé après chaque tick).
    public void Redraw() => InvalidateVisual();

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (_simulation is null)
        {
            return;
        }

        Grid grid = _simulation.Grid;

        // Taille d'une case pour remplir la zone du contrôle en gardant les carrés.
        double cell = Math.Min(Bounds.Width / grid.LengthX, Bounds.Height / grid.LengthY);
        if (cell <= 0)
        {
            return;
        }

        // Centre la grille dans le contrôle.
        double offsetX = (Bounds.Width - cell * grid.LengthX) / 2;
        double offsetY = (Bounds.Height - cell * grid.LengthY) / 2;

        for (int x = 0; x < grid.LengthX; x++)
        {
            for (int y = 0; y < grid.LengthY; y++)
            {
                int owner = grid.Cells[x, y];
                IBrush brush = owner == 0
                    ? NeutralBrush
                    : _brushes.GetValueOrDefault(owner, NeutralBrush);

                Rect rect = new(offsetX + x * cell, offsetY + y * cell, cell, cell);
                context.FillRectangle(brush, rect);
            }
        }
    }

    // "#RRGGBB" → Color. Tolère l'absence de '#'. Gris par défaut si invalide.
    private static Color ParseHex(string hex)
    {
        if (Color.TryParse(hex, out Color color))
        {
            return color;
        }
        return Color.FromRgb(136, 136, 136);
    }
}
