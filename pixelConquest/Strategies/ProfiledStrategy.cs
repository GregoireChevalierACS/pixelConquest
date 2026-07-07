namespace pixelConquest;

// Stratégie unique pilotée par un StrategyProfile : le "type" fixe la logique
// générale, les paramètres (0..1) pondèrent le score de chaque coup candidat.
// Une seule classe couvre donc toute la diversité de comportements.
public class ProfiledStrategy : StrategyBase
{
    public StrategyProfile Profile { get; }

    private readonly Random _random;

    public ProfiledStrategy(int id, StrategyProfile profile, int? seed = null)
        : base(id)
    {
        Profile = profile.Clamped();
        _random = seed is int s ? new Random(s) : new Random();
    }

    public override Position? NextMove(Grid grid)
    {
        // 1. Collecter les cases neutres adjacentes à mon territoire (candidats).
        //    On déduplique via un HashSet (une case peut border plusieurs des miennes).
        HashSet<Position> candidateSet = new();
        for (int x = 0; x < grid.LengthX; x++)
        {
            for (int y = 0; y < grid.LengthY; y++)
            {
                if (grid.Cells[x, y] != Id)
                {
                    continue;
                }

                foreach (Position n in GetNeighbors(new Position(x, y), grid))
                {
                    if (grid.Cells[n.X, n.Y] == 0)
                    {
                        candidateSet.Add(n);
                    }
                }
            }
        }

        if (candidateSet.Count == 0)
        {
            return null;
        }

        List<Position> candidates = candidateSet.ToList();

        // 2. Part d'aléatoire : avec proba = Randomness, on ignore le score.
        if (_random.NextDouble() < Profile.Randomness)
        {
            return candidates[_random.Next(candidates.Count)];
        }

        // 3. Scorer chaque candidat, garder le meilleur (bruit léger pour départager).
        Position best = candidates[0];
        double bestScore = double.NegativeInfinity;

        foreach (Position c in candidates)
        {
            double score = ScoreCandidate(c, grid) + _random.NextDouble() * 0.01;
            if (score > bestScore)
            {
                bestScore = score;
                best = c;
            }
        }

        return best;
    }

    // Score composite d'un coup candidat, pondéré par le type et les paramètres.
    private double ScoreCandidate(Position candidate, Grid grid)
    {
        int myNeighbors = 0;      // voisins m'appartenant → compacité / fermeture
        int enemyNeighbors = 0;   // voisins ennemis → proximité de conflit
        int neutralNeighbors = 0; // voisins libres → potentiel d'expansion

        foreach (Position n in GetNeighbors(candidate, grid))
        {
            int owner = grid.Cells[n.X, n.Y];
            if (owner == Id)
            {
                myNeighbors++;
            }
            else if (owner == 0)
            {
                neutralNeighbors++;
            }
            else
            {
                enemyNeighbors++;
            }
        }

        // Poids de base selon le type de comportement.
        (double aggr, double encircle, double compact, double spread) = BaseWeights();

        // On combine les poids de type avec les paramètres du profil (0..1).
        double score = 0.0;
        score += (aggr + Profile.Aggressiveness) * enemyNeighbors;
        score += (encircle + Profile.EncirclementPriority) * myNeighbors;
        score += (compact + Profile.Compactness) * myNeighbors;
        score += (spread + (1.0 - Profile.Compactness)) * neutralNeighbors;

        return score;
    }

    // Chaque type de base incline le score dans une direction dominante.
    private (double aggr, double encircle, double compact, double spread) BaseWeights()
        => Profile.Type switch
        {
            StrategyType.Random     => (0.0, 0.0, 0.0, 0.0),
            StrategyType.Bfs        => (0.0, 0.0, 0.0, 1.0), // s'étale régulièrement
            StrategyType.Greedy     => (0.0, 1.5, 0.0, 0.0), // maximise la fermeture
            StrategyType.Aggressive => (1.5, 0.0, 0.0, 0.0), // fonce sur l'ennemi
            StrategyType.Defensive  => (0.0, 0.0, 1.5, 0.0), // reste compact
            _                       => (0.0, 0.0, 0.0, 0.0),
        };
}
