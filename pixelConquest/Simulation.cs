namespace pixelConquest;

// Orchestrateur du jeu : détient la grille et les stratégies, fait avancer la
// partie tick par tick, et applique l'absorption des poches encerclées.
class Simulation
{
    public Grid Grid { get; }
    public List<StrategyBase> Strategies { get; }

    // Directions 4-connexité, réutilisées pour parcourir les poches et frontières.
    private static readonly (int dx, int dy)[] Directions =
    {
        (0, -1), (0, 1), (-1, 0), (1, 0),
    };

    public Simulation(Grid grid, List<StrategyBase> strategies)
    {
        Grid = grid;
        Strategies = strategies;
    }

    // Construit une simulation prête à jouer depuis une configuration :
    // crée la grille, instancie une ProfiledStrategy par profil (Id = 1,2,3…),
    // et place chaque seed (positions fournies, sinon réparties par défaut).
    public static Simulation FromConfig(SimulationConfig config)
    {
        if (!config.IsValid(out string? error))
        {
            throw new ArgumentException($"Configuration invalide : {error}");
        }

        Grid grid = new(config.Width, config.Height);
        List<StrategyBase> strategies = new();

        for (int i = 0; i < config.Strategies.Count; i++)
        {
            int id = i + 1; // 0 est réservé aux pixels neutres
            strategies.Add(new ProfiledStrategy(id, config.Strategies[i]));

            Position seed = i < config.Seeds.Count
                ? config.Seeds[i]
                : DefaultSeed(i, config);

            grid.Cells[seed.X, seed.Y] = id;
        }

        return new Simulation(grid, strategies);
    }

    // Seed par défaut : répartit les stratégies le long des bords, espacées.
    private static Position DefaultSeed(int index, SimulationConfig config)
    {
        int count = config.Strategies.Count;
        int x = (int)((index + 0.5) / count * config.Width);
        x = Math.Clamp(x, 1, config.Width - 2);
        int y = index % 2 == 0 ? 1 : config.Height - 2;
        return new Position(x, y);
    }

    // Un tick : chaque stratégie joue un coup (si elle peut), puis on absorbe les
    // poches nouvellement encerclées. Retourne false si plus aucune stratégie
    // n'a pu jouer (partie terminée).
    public bool Tick()
    {
        bool anyMove = false;

        foreach (StrategyBase strategy in Strategies)
        {
            if (strategy.NextMove(Grid) is Position move)
            {
                Grid.Cells[move.X, move.Y] = strategy.Id;
                anyMove = true;
            }
        }

        AbsorbEncircledPockets();
        return anyMove;
    }

    // Fait tourner la simulation jusqu'à épuisement de toutes les stratégies.
    // Retourne le nombre de ticks joués.
    public int Run()
    {
        int ticks = 0;
        while (Tick())
        {
            ticks++;
        }
        return ticks;
    }

    // Regroupe les pixels encerclés en poches connexes, puis attribue chaque poche
    // à la stratégie qui possède le plus de pixels de frontière avec elle.
    public void AbsorbEncircledPockets()
    {
        // On réutilise la 1re stratégie pour appeler GetEncircledPixels : la
        // méthode est commune (dans StrategyBase) et indépendante de l'ID.
        if (Strategies.Count == 0)
        {
            return;
        }

        List<Position> encircled = Strategies[0].GetEncircledPixels(Grid);
        if (encircled.Count == 0)
        {
            return;
        }

        // Ensemble des cases encerclées, pour tester l'appartenance en O(1).
        HashSet<Position> encircledSet = new(encircled);
        HashSet<Position> visited = new();

        foreach (Position start in encircled)
        {
            if (visited.Contains(start))
            {
                continue;
            }

            // Flood-fill de la poche connexe à partir de 'start'.
            List<Position> pocket = new();
            Queue<Position> queue = new();
            queue.Enqueue(start);
            visited.Add(start);

            while (queue.Count > 0)
            {
                Position current = queue.Dequeue();
                pocket.Add(current);

                foreach ((int dx, int dy) in Directions)
                {
                    Position next = new(current.X + dx, current.Y + dy);
                    if (encircledSet.Contains(next) && !visited.Contains(next))
                    {
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }

            int owner = DetermineOwner(pocket, encircledSet);
            foreach (Position p in pocket)
            {
                Grid.Cells[p.X, p.Y] = owner;
            }
        }
    }

    // Détermine à quelle stratégie revient une poche : celle qui possède le plus
    // de pixels de frontière (cases non-neutres adjacentes à la poche).
    // Égalité → plus petit ID (déterministe).
    private int DetermineOwner(List<Position> pocket, HashSet<Position> encircledSet)
    {
        Dictionary<int, int> borderCount = new();

        foreach (Position p in pocket)
        {
            foreach ((int dx, int dy) in Directions)
            {
                int nx = p.X + dx;
                int ny = p.Y + dy;

                if (nx < 0 || nx >= Grid.LengthX || ny < 0 || ny >= Grid.LengthY)
                {
                    continue;
                }

                int cell = Grid.Cells[nx, ny];
                // On ne compte que les cases possédées (non neutres) hors de la poche.
                if (cell != 0 && !encircledSet.Contains(new Position(nx, ny)))
                {
                    borderCount[cell] = borderCount.GetValueOrDefault(cell) + 1;
                }
            }
        }

        int bestOwner = 0;
        int bestCount = -1;
        foreach ((int id, int count) in borderCount)
        {
            if (count > bestCount || (count == bestCount && id < bestOwner))
            {
                bestCount = count;
                bestOwner = id;
            }
        }

        return bestOwner;
    }
}
