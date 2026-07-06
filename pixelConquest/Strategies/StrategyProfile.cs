namespace pixelConquest;

// Définition (données) d'une stratégie personnalisée, indépendante de son
// comportement. Sérialisable en JSON pour sauvegarde/chargement.
// Les 4 paramètres sont bornés à [0, 1] et pondèrent le comportement de base.
record StrategyProfile
{
    public string Name { get; init; } = "Sans nom";

    // Type de comportement de base.
    public StrategyType Type { get; init; } = StrategyType.Random;

    // Couleur d'affichage au format hex "#RRGGBB" (parsée par le renderer plus tard).
    public string Color { get; init; } = "#888888";

    // --- Paramètres de comportement (0 = min, 1 = max) ---

    // Cible les pixels proches des adversaires plutôt que le terrain neutre.
    public double Aggressiveness { get; init; } = 0.0;

    // Part d'imprévisibilité : 0 = suit strictement sa règle, 1 = choix au hasard.
    public double Randomness { get; init; } = 0.0;

    // Privilégie les coups qui referment des poches (pour absorber du territoire).
    public double EncirclementPriority { get; init; } = 0.0;

    // Rester groupé/compact (défensif) vs s'étaler loin (offensif territorial).
    public double Compactness { get; init; } = 0.5;

    // Ramène toutes les valeurs dans [0, 1] — garde-fou contre un JSON édité à la main.
    public StrategyProfile Clamped() => this with
    {
        Aggressiveness = Clamp01(Aggressiveness),
        Randomness = Clamp01(Randomness),
        EncirclementPriority = Clamp01(EncirclementPriority),
        Compactness = Clamp01(Compactness),
    };

    private static double Clamp01(double v) => Math.Clamp(v, 0.0, 1.0);
}
