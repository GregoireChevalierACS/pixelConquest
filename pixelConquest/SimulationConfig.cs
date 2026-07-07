namespace pixelConquest;

// Paramètres d'une partie à lancer : dimensions du canvas et stratégies engagées.
// Sérialisable pour sauvegarder/rejouer une configuration.
public record SimulationConfig
{
    public int Width { get; init; } = 30;
    public int Height { get; init; } = 15;

    // Les profils qui vont s'affronter. Leur ordre définit leur Id (1, 2, 3…).
    public List<StrategyProfile> Strategies { get; init; } = new();

    // Position de départ (seed) de chaque stratégie, dans le même ordre que Strategies.
    // Si vide ou incomplet, la simulation placera des seeds par défaut.
    public List<Position> Seeds { get; init; } = new();

    // Bornes de sécurité pour éviter des configs absurdes.
    public const int MinSize = 5;
    public const int MaxSize = 500;
    public const int MaxStrategies = 8;

    public bool IsValid(out string? error)
    {
        if (Width < MinSize || Width > MaxSize || Height < MinSize || Height > MaxSize)
        {
            error = $"Dimensions hors bornes [{MinSize}..{MaxSize}] : {Width}x{Height}.";
            return false;
        }

        if (Strategies.Count < 1 || Strategies.Count > MaxStrategies)
        {
            error = $"Nombre de stratégies invalide (1..{MaxStrategies}) : {Strategies.Count}.";
            return false;
        }

        error = null;
        return true;
    }
}
