namespace pixelConquest;

abstract class StrategyBase : IStrategy
{
    // ID de la stratégie (1, 2, 3…). C'est la valeur écrite dans Grid.Cells
    // pour marquer les pixels conquis par cette stratégie.
    public int Id { get; }

    protected StrategyBase(int id)
    {
        Id = id;
    }

    // Chaque stratégie décide différemment de son prochain coup → abstrait.
    public abstract Position? NextMove(Grid grid);

    // Les voisins d'une case ne dépendent pas de la stratégie : logique commune.
    // 4-connexité (haut, bas, gauche, droite), bornée aux limites de la grille.
    public List<Position> GetNeighbors(Position position, Grid grid)
    {
        List<Position> neighbors = new();

        (int dx, int dy)[] directions =
        {
            (0, -1), // haut
            (0, 1),  // bas
            (-1, 0), // gauche
            (1, 0),  // droite
        };

        foreach ((int dx, int dy) in directions)
        {
            int nx = position.X + dx;
            int ny = position.Y + dy;

            if (nx >= 0 && nx < grid.LengthX && ny >= 0 && ny < grid.LengthY)
            {
                neighbors.Add(new Position(nx, ny));
            }
        }

        return neighbors;
    }

    // Stub : la détection d'encerclement (flood-fill) est une étape dédiée
    // de la roadmap. Pour l'instant on retourne vide pour rester exécutable.
    public virtual List<Position> GetEncircledPixels(Grid grid)
    {
        return new List<Position>();
    }
}
