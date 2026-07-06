# pixelConquest

Simulateur de conquête territoriale pixel par pixel, pour apprendre le C# avec Avalonia UI et SkiaSharp.

## Concept

Plusieurs stratégies s'affrontent sur un canvas. Chaque stratégie conquiert le terrain pixel contigu par pixel contigu. Lorsque des pixels du canvas sont entièrement **encerclés** par les pixels d'une stratégie, ils prennent automatiquement sa couleur.

La simulation se termine quand il n'y a plus aucun pixel neutre.

## Stack technique

| Couche | Technologie |
|--------|-------------|
| Langage | C# (.NET 8) |
| UI | Avalonia UI (cross-platform) |
| Rendu graphique | SkiaSharp (pixel-level canvas) |
| IDE | VS Code + C# Dev Kit |
| Tests | xUnit |

## Structure du projet

```
pixelConquest/
├── src/
│   ├── Core/              # Logique métier (Grid, Pixel, stratégies)
│   ├── Rendering/         # Adaptateur canvas SkiaSharp
│   └── App/               # Point d'entrée Avalonia + vues
├── tests/
│   └── Core.Tests/        # Tests unitaires des stratégies
└── pixelConquest.sln
```

## Fonctionnement

1. Le canvas est une grille de pixels, initialement tous neutres
2. Chaque stratégie part d'une position de départ (seed)
3. À chaque tick, chaque stratégie choisit le prochain pixel à conquérir parmi ses voisins libres
4. Après chaque conquest, l'algorithme d'encerclement vérifie si des pixels neutres sont isolés
5. Les pixels encerclés sont immédiatement absorbés

## Stratégies envisagées

- **Expansion aléatoire** — choisit un voisin libre au hasard
- **Expansion BFS** — conquête en largeur, vague régulière
- **Expansion gloutonne** — priorise les pixels qui maximisent l'encerclement
- **Expansion agressive** — cible en priorité les pixels proches des adversaires
- **Expansion défensive** — construit des frontières avant d'avancer

## Algorithme d'encerclement

Un groupe de pixels neutres est considéré encerclé par la stratégie S si **aucun chemin contigu** (4-connexité ou 8-connexité) ne relie ce groupe au bord du canvas sans traverser un pixel appartenant à S.

Implémentation envisagée : flood fill depuis les bords — tout pixel neutre non atteignable depuis un bord est encerclé.

## Roadmap d'apprentissage

- [x] Setup .NET SDK + VS Code + C# Dev Kit
- [x] Premier projet console :
    - [x] ```dotnet new console -n pixelConquest```
        - [x] Création du dossier projet, sous-dossier obj, Program.cs & .csproj (équivalent package.json)
    - [x] ```dotnet run```
    - [x] Reopen du dossier dotnet via VSCode pour reconnaissance par le devkit
    - [x] Bases de la syntaxe C#
- [ ] Domaine core : 
    - [x] `Grid`, 
    - [x] `Pixel`, 
    - [x] interfaces (`IStrategy`)
        - [x] Définition du besoin :
        ```
        IStrategy doit avoir :
        Position NextMove(Grid grid)
        List<Position> GetNeighbors(Position position, Grid grid)
        List<Position> GetEncircledPixels(Grid grid)
        ```
    - [x] ~~`Color`~~ — abandonné : le domaine n'utilise que des IDs entiers (0 = neutre, 1/2/3… = stratégies). L'association `id → couleur` vivra dans la couche rendu (SkiaSharp fournit déjà `SKColor`). Décision YAGNI.
    - [x] classes (`Grid`, `Pixel`)
    - [x] records (`Position`)
    - **Note archi** : stockage de la grille en `int[,] Cells` (tableau 2D dense) plutôt que `Dictionary` ou `Pixel[,]` — perf mémoire/accès pour le rendu et les flood-fills.
- [x] Pattern Strategy — classe abstraite + implémentations
    - [x] `StrategyBase` (abstraite) : `GetNeighbors` (4-connexité) commun, `NextMove` abstrait, `GetEncircledPixels` (flood-fill)
    - [x] `ProfiledStrategy` (concrète) : comportement unique piloté par un `StrategyProfile` (type + paramètres pondérés)
- [x] Algorithme de détection d'encerclement — flood-fill depuis les bords (`GetEncircledPixels`), validé sur cas anneau
- [x] Stratégies concurrentes multiples — orchestrateur `Simulation` (tour par tour jusqu'à épuisement) + absorption des poches par la stratégie majoritaire sur la frontière
- [x] Personnalisation & configuration des stratégies
    - [x] `StrategyType` (enum) : Random, Bfs, Greedy, Aggressive, Defensive
    - [x] `StrategyProfile` (record) : nom, couleur, + 4 paramètres 0..1 (agressivité, aléatoire, encerclement, compacité)
    - [x] `StrategyCatalog` : sauvegarde/chargement des profils en JSON (`strategies.json`)
    - [x] `SimulationConfig` : taille du canvas + stratégies engagées ; `Simulation.FromConfig(...)`
- [x] Fenêtre Avalonia + game loop — `MainWindow` + `DispatcherTimer` (50 ms/tick), barre d'état live
- [x] Pipeline de rendu SkiaSharp — `GridView : Control` (via `Render`/`DrawingContext`, Skia sous Avalonia), couleurs des profils
