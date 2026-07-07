namespace pixelConquest;

// Stratégie unique pilotée par un StrategyProfile : le "type" fixe la logique
// générale, les paramètres pondèrent le score de chaque coup candidat.
// Une seule classe couvre donc toute la diversité de comportements.
public class ProfiledStrategy : StrategyBase
{
    public StrategyProfile Profile { get; }

    private readonly Random _random;

    // Mémoire d'état entre les ticks, pour les règles "suivre le dernier pixel"
    // et "lignes droites".
    private Position? _lastMove;
    private (int dx, int dy)? _lastDirection;

    public ProfiledStrategy(int id, StrategyProfile profile, int? seed = null)
        : base(id)
    {
        Profile = profile.Clamped();
        _random = seed is int s ? new Random(s) : new Random();
    }

    public override Position? NextMove(Grid grid)
    {
        List<Position> candidates = CollectCandidates(grid);
        if (candidates.Count == 0)
        {
            return null;
        }

        // Part d'aléatoire : avec proba = Randomness, on ignore le score.
        Position chosen;
        if (_random.NextDouble() < Profile.Randomness)
        {
            chosen = candidates[_random.Next(candidates.Count)];
        }
        else
        {
            chosen = PickBest(candidates, grid);
        }

        // Mémorise le coup et sa direction (par rapport au dernier) pour le prochain tick.
        if (_lastMove is Position prev)
        {
            _lastDirection = (chosen.X - prev.X, chosen.Y - prev.Y);
        }
        _lastMove = chosen;

        return chosen;
    }

    // Cases neutres adjacentes à mon territoire. Si "suivre le dernier pixel" est
    // actif et que le dernier coup a encore des voisins libres, on s'y restreint.
    private List<Position> CollectCandidates(Grid grid)
    {
        if (Profile.FollowLastPixel && _lastMove is Position last)
        {
            List<Position> local = new();
            foreach (Position n in GetNeighbors(last, grid))
            {
                if (grid.Cells[n.X, n.Y] == 0)
                {
                    local.Add(n);
                }
            }
            if (local.Count > 0)
            {
                return local;
            }
            // Sinon (impasse), on retombe sur l'ensemble du territoire ci-dessous.
        }

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

        return candidateSet.ToList();
    }

    private Position PickBest(List<Position> candidates, Grid grid)
    {
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

        (double aggr, double encircle, double compact, double spread) = BaseWeights();

        double score = 0.0;
        score += (aggr + Profile.Aggressiveness) * enemyNeighbors;
        score += (encircle + Profile.EncirclementPriority) * myNeighbors;
        score += (compact + Profile.Compactness) * myNeighbors;
        score += (spread + (1.0 - Profile.Compactness)) * neutralNeighbors;

        // Gros encerclements : bonus supplémentaire fort pour les cases entourées
        // de plein de pixels à moi (elles referment de grandes zones).
        if (Profile.BigEncirclement > 0)
        {
            score += Profile.BigEncirclement * 3.0 * (myNeighbors * myNeighbors);
        }

        // Biais centre/bord : selon la position sur le canvas.
        if (Profile.CenterEdgeBias != 0)
        {
            score += Profile.CenterEdgeBias * 2.0 * CenterAffinity(candidate, grid);
        }

        // Lignes droites : bonus si on continue dans la même direction qu'avant.
        if (Profile.PreferStraightLines && _lastMove is Position prev
            && _lastDirection is (int ldx, int ldy))
        {
            int dx = candidate.X - prev.X;
            int dy = candidate.Y - prev.Y;
            if (dx == ldx && dy == ldy)
            {
                score += 2.0;
            }
        }

        return score;
    }

    // Retourne +1 au centre exact du canvas, -1 dans les coins, ~0 à mi-chemin.
    private static double CenterAffinity(Position p, Grid grid)
    {
        double cx = (grid.LengthX - 1) / 2.0;
        double cy = (grid.LengthY - 1) / 2.0;
        double dx = Math.Abs(p.X - cx) / (cx <= 0 ? 1 : cx);
        double dy = Math.Abs(p.Y - cy) / (cy <= 0 ? 1 : cy);
        double dist = (dx + dy) / 2.0; // 0 au centre, 1 au bord
        return 1.0 - 2.0 * dist;       // +1 centre, -1 bord
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
