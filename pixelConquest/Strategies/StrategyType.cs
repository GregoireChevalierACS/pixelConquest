namespace pixelConquest;

// Les comportements de base disponibles. Le "type" fixe la logique générale ;
// les paramètres du StrategyProfile la nuancent ensuite.
enum StrategyType
{
    Random,     // choisit un voisin libre au hasard
    Bfs,        // expansion en largeur, vague régulière
    Greedy,     // priorise les coups qui maximisent l'encerclement
    Aggressive, // cible les pixels proches des adversaires
    Defensive,  // reste compact, consolide sa frontière
}
