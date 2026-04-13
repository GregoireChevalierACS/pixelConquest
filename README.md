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

- [ ] Setup .NET SDK + VS Code
- [ ] Premier projet console — bases de la syntaxe C#
- [ ] Domaine core : `Grid`, `Pixel`, `Color` — classes, records, interfaces
- [ ] Pattern Strategy — classe abstraite + implémentations
- [ ] Fenêtre Avalonia + game loop
- [ ] Pipeline de rendu SkiaSharp
- [ ] Algorithme de détection d'encerclement
- [ ] Stratégies concurrentes multiples
