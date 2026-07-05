namespace pixelConquest;

// Expansion aléatoire : choisit au hasard un pixel neutre adjacent au territoire
// déjà conquis par cette stratégie.
class RandomStrategy : StrategyBase
{
    private readonly Random _random = new();

    public RandomStrategy(int id) : base(id)
    {
    }

    public override Position? NextMove(Grid grid)
    {
        // 1. Collecter tous les pixels neutres (valeur 0) adjacents à un pixel
        //    appartenant à cette stratégie (valeur == Id).
        List<Position> candidates = new();

        for (int x = 0; x < grid.LengthX; x++)
        {
            for (int y = 0; y < grid.LengthY; y++)
            {
                if (grid.Cells[x, y] != Id)
                {
                    continue;
                }

                foreach (Position neighbor in GetNeighbors(new Position(x, y), grid))
                {
                    if (grid.Cells[neighbor.X, neighbor.Y] == 0)
                    {
                        candidates.Add(neighbor);
                    }
                }
            }
        }

        // 2. Aucun coup possible (territoire enclavé ou grille pleine).
        if (candidates.Count == 0)
        {
            return null;
        }

        // 3. Tirer un candidat au hasard.
        return candidates[_random.Next(candidates.Count)];
    }
}
