namespace pixelConquest;

public abstract class StrategyBase : IStrategy
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

    // Détection d'encerclement par flood-fill depuis les bords.
    // Principe : un pixel neutre (0) est "libre" s'il existe un chemin de pixels
    // neutres contigus jusqu'à un bord du canvas. Tout pixel neutre qui n'est PAS
    // atteignable depuis un bord est, par définition, encerclé.
    // On calcule donc les neutres libres (flood-fill depuis les bords), puis on
    // retourne tous les autres neutres.
    public virtual List<Position> GetEncircledPixels(Grid grid)
    {
        // Marque les pixels neutres atteignables depuis un bord ("libres").
        bool[,] reachable = new bool[grid.LengthX, grid.LengthY];
        Queue<Position> toVisit = new();

        // 1. Amorcer la file avec tous les pixels neutres situés sur un bord.
        for (int x = 0; x < grid.LengthX; x++)
        {
            for (int y = 0; y < grid.LengthY; y++)
            {
                bool isBorder = x == 0 || x == grid.LengthX - 1
                             || y == 0 || y == grid.LengthY - 1;

                if (isBorder && grid.Cells[x, y] == 0 && !reachable[x, y])
                {
                    reachable[x, y] = true;
                    toVisit.Enqueue(new Position(x, y));
                }
            }
        }

        // 2. Flood-fill : propager à travers les voisins neutres non encore atteints.
        while (toVisit.Count > 0)
        {
            Position current = toVisit.Dequeue();

            foreach (Position neighbor in GetNeighbors(current, grid))
            {
                if (grid.Cells[neighbor.X, neighbor.Y] == 0 && !reachable[neighbor.X, neighbor.Y])
                {
                    reachable[neighbor.X, neighbor.Y] = true;
                    toVisit.Enqueue(neighbor);
                }
            }
        }

        // 3. Tout pixel neutre non atteint depuis un bord est encerclé.
        List<Position> encircled = new();
        for (int x = 0; x < grid.LengthX; x++)
        {
            for (int y = 0; y < grid.LengthY; y++)
            {
                if (grid.Cells[x, y] == 0 && !reachable[x, y])
                {
                    encircled.Add(new Position(x, y));
                }
            }
        }

        return encircled;
    }
}
