namespace pixelConquest;

// Définition (données) d'une stratégie personnalisée, indépendante de son
// comportement. Sérialisable en JSON pour sauvegarde/chargement.
// Les 4 paramètres sont bornés à [0, 1] et pondèrent le comportement de base.
public record StrategyProfile
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

    // Curseur bipolaire d'attirance : -1 = vise les bords, 0 = neutre, +1 = vise le centre.
    public double CenterEdgeBias { get; init; } = 0.0;

    // Privilégie les coups qui referment de GRANDES zones (encerclements ambitieux).
    public double BigEncirclement { get; init; } = 0.0;

    // --- Règles de déplacement (on/off) ---

    // Étend en priorité depuis le dernier pixel joué (progression "en serpent")
    // plutôt que depuis n'importe quel pixel du territoire.
    public bool FollowLastPixel { get; init; } = false;

    // Tend à continuer dans la même direction que le coup précédent (lignes droites).
    public bool PreferStraightLines { get; init; } = false;

    // Ramène les valeurs dans leurs bornes — garde-fou contre un JSON édité à la main.
    public StrategyProfile Clamped() => this with
    {
        Aggressiveness = Clamp01(Aggressiveness),
        Randomness = Clamp01(Randomness),
        EncirclementPriority = Clamp01(EncirclementPriority),
        Compactness = Clamp01(Compactness),
        CenterEdgeBias = Math.Clamp(CenterEdgeBias, -1.0, 1.0),
        BigEncirclement = Clamp01(BigEncirclement),
    };

    private static double Clamp01(double v) => Math.Clamp(v, 0.0, 1.0);
}
