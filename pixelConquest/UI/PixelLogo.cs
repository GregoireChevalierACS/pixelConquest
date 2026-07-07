using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace pixelConquest;

// Dessine un logo pixel-art "PC" (pixel Conquest) sur une petite grille,
// avec les couleurs des stratégies de démo pour rester dans le thème du jeu.
class PixelLogo : Control
{
    // Motif 13 colonnes x 7 lignes. Chiffres = index de couleur, 0 = vide.
    // Lettres "P" et "C" en pixel-art.
    private static readonly int[,] Pattern =
    {
        // x→ 0  1  2  3  4  5  6  7  8  9 10 11 12
        {  1, 1, 1, 1, 0, 0, 0, 0, 2, 2, 2, 2, 2 }, // y=0
        {  1, 0, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 2 }, // y=1
        {  1, 0, 0, 0, 1, 0, 0, 2, 0, 0, 0, 0, 0 }, // y=2
        {  1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 0, 0, 0 }, // y=3
        {  1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0 }, // y=4
        {  1, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 2 }, // y=5
        {  1, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2 }, // y=6
    };

    private static readonly Color[] Palette =
    {
        Colors.Transparent,
        Color.FromRgb(0xE6, 0x39, 0x46), // rouge
        Color.FromRgb(0x2A, 0x9D, 0x8F), // vert
    };

    // Progression 0..1 pour une petite animation d'apparition.
    private double _reveal = 1.0;
    public double Reveal
    {
        get => _reveal;
        set { _reveal = value; InvalidateVisual(); }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        int cols = Pattern.GetLength(1);
        int rows = Pattern.GetLength(0);

        double cell = Math.Min(Bounds.Width / cols, Bounds.Height / rows);
        if (cell <= 0)
        {
            return;
        }

        double gap = cell * 0.12; // petit espace entre pixels pour l'effet "pixel-art"
        double offsetX = (Bounds.Width - cell * cols) / 2;
        double offsetY = (Bounds.Height - cell * rows) / 2;

        int totalLit = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (Pattern[y, x] != 0)
                {
                    totalLit++;
                }
            }
        }

        int litToShow = (int)(totalLit * _reveal);
        int litSeen = 0;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int c = Pattern[y, x];
                if (c == 0)
                {
                    continue;
                }

                litSeen++;
                if (litSeen > litToShow)
                {
                    continue;
                }

                Rect rect = new(
                    offsetX + x * cell + gap / 2,
                    offsetY + y * cell + gap / 2,
                    cell - gap,
                    cell - gap);
                context.FillRectangle(new SolidColorBrush(Palette[c]), rect);
            }
        }
    }
}
