***13 Avril 2026***

 ```dotnet new console -n nomduprojet```  **créée un projet et autogénère la structure de base**
fichier .csproj ~équivalent package.json
```namespace pixelConquest``` "module" qui regroupe les classes. L'équivalent d'un module ES6. Toutes les classes du projet auront ce même namespace, elles se verront automatiquement sans import
```class Pixel``` déclaration de la classe, comme en JS
En C#, les propriétés se déclarent comme ça :
```public int X { get; set; }```
convention : en C#, les propriétés publiques s'écrivent en PascalCase
```{ get; set; }``` est la syntaxe C# pour dire "on peut lire et modifier cette valeur". L'équivalent JS serait simplement des champs sur un objet.
Le constructeur s'appelle exactement comme la classe, et il est exécuté quand on crée une instance. Les paramètres x et y (minuscules) sont locaux au constructeur, et assignés aux propriétés X et Y (majuscules) de l'instance.

Pour créer un pixel :
```
var pixel = new Pixel(3, 7);
```
Ce qui donnera un pixel aux coordonnées X=3, Y=7.
Champ : public int ClaimedBy = 0;
Propriété : public int ClaimedBy { get; set; } = 0;
Le constructeur ne prend pas le Dictionary en paramètre, c'est la Grid qui le crée elle-même en interne avec new Dictionary<int, int>()
**Convention** : 
Les paramètres du constructeur sont en camelCase (lengthX), les propriétés en PascalCase (LengthX)
En C#, les propriétés avec { get; set; } se terminent par }. L'accolade fermante fait déjà office de fin de déclaration. Le ; supplémentaire est une erreur de syntaxe.
Les ; terminent les instructions (assignations, appels de méthode...), pas les déclarations de membres.

Une interface n'a pas de constructeur elle définit seulement un contrat (les méthodes que les classes qui l'implémentent devront avoir). C'est comme une signature TypeScript
En C#, pas de mot-clé function juste le type de retour directement
Dans une interface, pas de corps { } juste la signature suivie d'un ;

***5 Juillet 2026***

**Interface `IStrategy`**
Convention C# : les interfaces sont préfixées d'un `I` majuscule (`IStrategy`, `IEnumerable`, `IDisposable`). C'est purement conventionnel mais universel dans l'écosystème .NET — l'équivalent JS n'existe pas, on nommait juste `Strategy` en TS.
```csharp
interface IStrategy
{
    Position NextMove(Grid grid);
    List<Position> GetNeighbors(Position position, Grid grid);
    List<Pixel> GetEncircledPixels(Grid grid);
}
```
Une méthode se déclare `TypeDeRetour NomMethode(TypeParam nomParam)`. Ici `Position NextMove(Grid grid)` = "une méthode `NextMove` qui prend une `Grid` et retourne une `Position`". L'ordre est inversé par rapport à TS où on écrirait `nextMove(grid: Grid): Position`.

**`List<T>` — les génériques**
`List<Position>` = une liste d'éléments de type `Position`. Le `<T>` (ici `<Position>` ou `<Pixel>`) est un **générique** : le type contenu est vérifié à la compilation. Équivalent TS de `Position[]` ou `Array<Position>`, mais en C# c'est fortement typé et la vérification est stricte (impossible d'y mettre un autre type).
`List<T>` vit dans `System.Collections.Generic`, mais grâce à `<ImplicitUsings>enable</ImplicitUsings>` dans le .csproj, pas besoin de l'importer manuellement — c'est comme si les imports les plus courants étaient déjà faits globalement (équivalent d'un fichier de globals auto-importés).

**Tableau multidimensionnel `int[,]`**
`int[,]` est un vrai **tableau 2D** (rectangulaire) : une seule zone mémoire contiguë de `LengthX * LengthY` entiers. La virgule dans `[,]` indique le nombre de dimensions : `[,]` = 2D, `[,,]` = 3D.
```csharp
Cells = new int[lengthX, lengthY]; // création, toutes les cases initialisées à 0
int owner = Cells[x, y];           // lecture d'une case
Cells[x, y] = 2;                   // écriture
```
À ne pas confondre avec `int[][]` (tableau **de tableaux**, "jagged array") où chaque ligne est un tableau séparé pouvant avoir une longueur différente. Ici on veut une grille régulière, donc `int[,]`.
Différence clé avec JS : en JS un tableau 2D c'est `Array<Array<number>>` (des tableaux imbriqués, redimensionnables). En C#, `int[,]` a une taille **fixe** définie à la création et un accès mémoire direct (bien plus rapide). En C#, un tableau ne se redimensionne pas — si on a besoin de taille dynamique, on utilise `List<T>`.
Bonus : en C# les cases d'un `int[]`/`int[,]` sont **automatiquement initialisées à 0** à la création (pas de `undefined` comme en JS). C'est pratique ici : 0 = pixel neutre par défaut.

**Pourquoi stocker un `int` plutôt qu'un objet `Pixel` ?**
On aurait pu faire `Pixel[,]` (un objet par case). On a choisi `int[,]` où la valeur = l'ID du propriétaire (0 = neutre, 1/2/3… = chaque stratégie). Raisons :
- **Mémoire** : un `int` c'est 4 octets dans le tableau. Un objet `Pixel` c'est une allocation sur le tas (heap) + un pointeur (référence) dans le tableau → des milliers d'objets alloués pour une grille dense.
- **Vitesse** : lire `Cells[x,y]` (un `int`) est un accès mémoire direct. Lire `pixels[x,y].ClaimedBy` demande de suivre le pointeur vers l'objet puis lire son champ (déréférencement) — plus lent, surtout balayé à chaque frame de rendu.
- **Redondance** : la classe `Pixel` portait `Coords` (X,Y) alors que la position dans le tableau donne déjà les coordonnées ; et `ClaimedBy` est un `int` → autant stocker directement l'`int`.
Notion sous-jacente : en C#, `int` est un **type valeur** (struct, stocké directement dans le tableau), alors qu'une classe comme `Pixel` est un **type référence** (le tableau stocke un pointeur vers l'objet ailleurs en mémoire). Pour une grille dense et performante, les types valeur gagnent.

**Pattern Strategy — classe abstraite `abstract`**
Une classe `abstract` ne peut pas être instanciée directement (`new StrategyBase(...)` est interdit). Elle sert de **base commune** dont héritent des classes concrètes. Équivalent conceptuel des classes abstraites TS.
```csharp
abstract class StrategyBase : IStrategy { ... }
```
Le `: IStrategy` signifie "cette classe **implémente** l'interface" (comme `implements` en TS). En C#, la même syntaxe `:` sert pour l'héritage ET l'implémentation d'interface — le contexte fait la différence.

**Méthodes `abstract` vs `virtual` vs normales**
- `public abstract Position? NextMove(Grid grid);` → **pas de corps**, juste la signature. Oblige chaque classe fille à la définir. C'est le point de variation entre stratégies.
- `public virtual List<Position> GetEncircledPixels(...) { ... }` → a un corps par défaut, mais une classe fille **peut** le remplacer (`override`).
- `public List<Position> GetNeighbors(...) { ... }` → méthode normale, héritée telle quelle, non redéfinissable. Logique commune à toutes les stratégies.

**Héritage et `override`**
```csharp
class RandomStrategy : StrategyBase
{
    public RandomStrategy(int id) : base(id) { }      // appelle le constructeur parent
    public override Position? NextMove(Grid grid) { ... } // redéfinit la méthode abstraite
}
```
- `: base(id)` dans le constructeur = appel du constructeur de la classe parente (équivalent `super(id)` en JS/TS).
- `override` est **obligatoire** en C# pour redéfinir une méthode `abstract`/`virtual` (en JS on redéfinit sans mot-clé). C'est une sécurité : le compilateur vérifie qu'on redéfinit bien quelque chose qui existe.
- **Polymorphisme** : c'est tout l'intérêt du pattern. On peut traiter n'importe quelle stratégie via le type `IStrategy` ou `StrategyBase` sans savoir laquelle c'est ; l'appel à `NextMove` exécute la bonne version. Ça permettra de faire tourner N stratégies différentes dans la même boucle.

**Types nullable (`Position?`)**
Avec `<Nullable>enable</Nullable>` dans le .csproj, les types référence sont **non-nullables par défaut** : retourner `null` sur un `Position` déclenche un avertissement. Le `?` (`Position?`) autorise explicitement `null`. Ici `NextMove` retourne `Position?` car il peut ne plus y avoir de coup possible.
Côté appelant, on teste avec `if (move is null)`. Le `is null` est la façon idiomatique C# (plutôt que `== null`).
C'est proche du `strictNullChecks` de TS : le compilateur t'oblige à gérer le cas `null` au lieu de le découvrir en plantant à l'exécution.

**Divers rencontrés ici**
- `new()` (target-typed new) : `Random _random = new();` — le type est déjà connu à gauche, inutile de le répéter à droite (`new Random()`). Sucre syntaxique C# 9+.
- `readonly` : `private readonly Random _random` — le champ ne peut être assigné qu'à la déclaration ou dans le constructeur, jamais réassigné ensuite. Proche du `readonly` TS / d'un `const` de champ.
- Convention : les champs privés sont préfixés d'un `_` (`_random`) — convention .NET très répandue.
- Tuples et déstructuration : `(int dx, int dy)[] directions = { (0,-1), ... };` puis `foreach ((int dx, int dy) in directions)`. Les tuples permettent de grouper des valeurs sans créer de classe, et on peut les déstructurer comme en JS.
- Interpolation de chaîne : `$"... {tick} ..."` — le `$` devant la chaîne active l'interpolation, `{}` insère une expression. Équivalent des template strings JS avec backticks.

**Organisation des fichiers (piège rencontré)**
Le SDK .NET compile automatiquement **tous les `.cs` situés sous le dossier du projet** (là où est le .csproj). Un dossier `Strategies/` placé *à côté* du projet (un niveau au-dessus) n'était **pas compilé** → erreur `CS0246 : type introuvable`. Solution : déplacer `Strategies/` **dans** `pixelConquest/`. Règle à retenir : tout le code source doit vivre sous le dossier du projet.

**Détection d'encerclement — flood-fill depuis les bords**
Astuce algorithmique importante : plutôt que de chercher directement les zones "fermées" (dur), on cherche l'**inverse**. Un pixel neutre est *libre* s'il peut atteindre un bord du canvas en ne passant que par des neutres. Donc :
1. on part de tous les neutres situés **sur un bord**,
2. on propage (flood-fill) à travers les voisins neutres → ça marque tous les *libres*,
3. tout neutre **non marqué** est forcément encerclé.
C'est un classique : plus simple de calculer le complémentaire d'un ensemble que l'ensemble lui-même.

**`Queue<T>` et le parcours BFS**
Le flood-fill utilise une file d'attente `Queue<Position>` (structure FIFO : premier entré, premier sorti).
```csharp
Queue<Position> toVisit = new();
toVisit.Enqueue(p);              // ajoute en fin de file
Position current = toVisit.Dequeue(); // retire et renvoie le premier
while (toVisit.Count > 0) { ... }     // tant qu'il reste des cases à traiter
```
Avec une `Queue`, on obtient un parcours **en largeur** (BFS) : on traite les cases proches avant les lointaines. (Une `Stack<T>` — LIFO — donnerait un parcours en profondeur/DFS ; pour un simple "atteindre tout", les deux marchent, le résultat est identique.)
Le tableau `bool[,] reachable` sert de mémo pour ne pas repasser deux fois sur la même case (sinon boucle infinie). En C#, un `bool[,]` est initialisé à `false` partout automatiquement.

**Pattern `while (... is Position move)`**
Rencontré dans la boucle de simulation :
```csharp
while (strategy.NextMove(grid) is Position move) { ... utilise move ... }
```
C'est du **pattern matching** avec déclaration de variable. `NextMove` retourne un `Position?`. Le motif `is Position move` est vrai seulement si le résultat n'est **pas null**, et dans ce cas il l'assigne à une nouvelle variable `move` (de type `Position` non-nullable, directement utilisable). Ça remplace élégamment le combo "appeler, stocker dans une variable, tester si null, déréférencer". Pas d'équivalent JS direct — c'est une force du système de types C#.

**Records et égalité par valeur (crucial pour HashSet/Dictionary)**
`Position` est un `record`. En C#, un `record` génère automatiquement l'**égalité par valeur** : deux `Position(3,3)` sont considérés égaux (`==` renvoie true) et produisent le **même hash code**. C'est différent d'une `class` classique dont l'égalité est par **référence** (deux objets distincts en mémoire ≠ même si contenu identique).
Conséquence concrète : on peut mettre des `Position` dans un `HashSet<Position>` ou comme clé de `Dictionary`, et `Contains`/lookup marchent sur le **contenu** (x,y), pas sur l'identité de l'objet. Sans record, il aurait fallu écrire `Equals` + `GetHashCode` à la main. Analogie JS : en JS `{x:3,y:3} !== {x:3,y:3}` (comparaison par référence) et un `Set` ne dédoublonnerait pas ; le `record` C# résout ça nativement.

**`HashSet<T>` — appartenance en O(1)**
`HashSet<Position> encircledSet = new(encircled);` construit un ensemble à partir d'une liste. `encircledSet.Contains(p)` teste l'appartenance en temps constant (via hash), là où `List.Contains` serait O(n). Utilisé ici pour tester rapidement "cette case fait-elle partie de la poche encerclée ?". On l'utilise aussi comme mémo `visited` pour ne pas retraiter une case.

**`Dictionary` : `GetValueOrDefault` et déstructuration**
- `borderCount[cell] = borderCount.GetValueOrDefault(cell) + 1;` : `GetValueOrDefault(k)` renvoie la valeur si la clé existe, sinon la valeur par défaut du type (0 pour un `int`) — évite de gérer à part "la clé n'existe pas encore". Pattern classique pour compter des occurrences.
- `foreach ((int id, int count) in borderCount)` : on itère un `Dictionary` en **déstructurant** directement chaque paire clé/valeur en deux variables. Chaque élément d'un `Dictionary` est une `KeyValuePair` déstructurable comme un tuple.

**Architecture : séparer le moteur (`Simulation`) des stratégies**
La classe `Simulation` orchestre la partie (elle détient la `Grid` + la `List<StrategyBase>`, fait les ticks, applique l'absorption). Les stratégies, elles, ne savent que répondre "quel est mon prochain coup ?". Cette séparation (orchestrateur vs acteurs) est la même logique que séparer un "game loop" de la logique des entités. Ça rendra le branchement de l'UI trivial : la fenêtre n'aura qu'à appeler `sim.Tick()` et lire `grid.Cells`.

**Règle métier — absorption des poches encerclées**
Algorithme en 2 temps :
1. `GetEncircledPixels` donne TOUS les pixels encerclés (peu importe par qui). On les regroupe en **poches connexes** (encore un flood-fill, mais restreint à l'ensemble des encerclés).
2. Pour chaque poche, on compte les pixels de **frontière** par stratégie (cases possédées adjacentes à la poche) → la stratégie majoritaire remplit toute la poche. Égalité tranchée par le plus petit ID (déterministe : un algo de simulation doit donner le même résultat à données égales).

***6 Juillet 2026***

**Enum**
```csharp
enum StrategyType { Random, Bfs, Greedy, Aggressive, Defensive }
```
Un `enum` est un type dont les valeurs sont une liste fixe de constantes nommées. En interne c'est un `int` (Random=0, Bfs=1…), mais on manipule les noms. Bien plus sûr que des "magic strings" ou des `int` bruts. Équivalent des enums TS. On peut faire du pattern matching dessus avec `switch`.

**`record` en style "propriétés" + `init`**
`StrategyProfile` est un `record` écrit avec un corps `{ }` (et non la forme courte `record Position(int X, int Y)`). Chaque propriété utilise `{ get; init; }` :
```csharp
public string Name { get; init; } = "Sans nom";
```
`init` (au lieu de `set`) = la propriété peut être assignée **uniquement à la création** de l'objet, puis devient en lecture seule → objet **immuable**. On les initialise avec la syntaxe *object initializer* :
```csharp
new StrategyProfile { Name = "Rusher", Type = StrategyType.Bfs, Randomness = 0.05 };
```
Immuabilité = plus sûr pour des données de config (personne ne peut muter un profil par accident). Proche d'un objet `readonly` en TS.

**Expression `with` (copie non-destructive)**
```csharp
this with { Aggressiveness = Clamp01(Aggressiveness), ... }
```
`with` crée une **copie** d'un record en ne changeant que les champs listés, sans modifier l'original. Indispensable avec l'immuabilité : pour "modifier" un record, on en fabrique une version corrigée. Équivalent JS : `{ ...profile, aggressiveness: clamp(...) }` (le spread), mais typé.

**Sérialisation JSON — `System.Text.Json`**
La lib JSON est **native** en .NET (pas de package à installer), dans `System.Text.Json`.
```csharp
var options = new JsonSerializerOptions {
    WriteIndented = true,                              // JSON lisible/indenté
    Converters = { new JsonStringEnumConverter() },    // enums en texte, pas en nombre
};
string json = JsonSerializer.Serialize(profiles, options);          // objet → texte
var list = JsonSerializer.Deserialize<List<StrategyProfile>>(json, options); // texte → objet
```
- Par défaut un enum se sérialise en **nombre** (`1`). Le `JsonStringEnumConverter` l'écrit en **texte** (`"Bfs"`) → fichier plus lisible et robuste si on réordonne l'enum.
- La désérialisation renvoie un type **nullable** (`List<…>?`) car le JSON pourrait être `null`/invalide → on gère avec `?? new()`.
- Les records avec `init` se désérialisent nativement (System.Text.Json sait remplir les propriétés `init`).

**Fichiers — `System.IO`**
- `File.Exists(path)`, `File.ReadAllText(path)`, `File.WriteAllText(path, content)` : lecture/écriture de fichier en une ligne.
- `Path.Combine(a, b)` : assemble un chemin de façon portable (gère les `/` `\` selon l'OS). Ne jamais concaténer des chemins à la main.
- `AppContext.BaseDirectory` : le dossier où tourne l'exécutable (ici `bin/Debug/...`). Pratique pour poser un fichier à côté de l'app.

**LINQ (premier contact)**
LINQ = des méthodes de requête sur les collections, très proche des méthodes de tableau JS :
- `.Select(p => p.Clamped())` ≈ `.map()`
- `.Where(p => ...)` ≈ `.filter()`
- `.FirstOrDefault(predicate)` ≈ `.find()` (renvoie `null`/défaut si rien trouvé)
- `.FindIndex(predicate)` ≈ `.findIndex()`
- `.ToList()` : matérialise le résultat en `List<T>` (LINQ est "paresseux", il faut parfois forcer l'évaluation).
`string.Equals(a, b, StringComparison.OrdinalIgnoreCase)` : comparaison de chaînes insensible à la casse (façon idiomatique C#, plutôt que `a.ToLower() == b.ToLower()`).

**Conception : données vs comportement (le point clé de cette étape)**
On a séparé :
- `StrategyProfile` = **les données** (nom, couleur, type, paramètres). Sérialisable, sauvegardable, éditable.
- `ProfiledStrategy` = **le comportement** (le code qui, à partir d'un profil, calcule `NextMove`).
Une **seule** classe de comportement pilotée par un profil remplace 5 classes rigides. Le `NextMove` **score** chaque coup candidat en combinant les poids du *type* de base (via un `switch`) et les *paramètres* du profil. La diversité devient continue (des curseurs 0..1) au lieu de discrète.
C'est le même esprit que le pattern Strategy, poussé plus loin : au lieu d'une classe par comportement, on a un comportement **paramétrable par la donnée**. Ça rendra l'UI simple (des sliders qui écrivent dans le profil).

**Organisation Git — `.gitignore`**
Ajouté un `.gitignore` pour exclure `bin/` et `obj/` (sorties de compilation régénérées à chaque build). On ne versionne **jamais** les artefacts de build — seulement le code source. Équivalent d'ignorer `node_modules/` et `dist/` en web.

**Packages NuGet — les dépendances .NET**
NuGet est le gestionnaire de packages de .NET (l'équivalent de npm). On ajoute une dépendance avec :
```
dotnet add <projet> package Avalonia
```
Ça inscrit une `<PackageReference Include="Avalonia" Version="12.0.5" />` dans le `.csproj` (≈ une ligne de `dependencies` dans package.json). Les packages sont mis en cache dans `~/.nuget/packages`. On a ajouté `Avalonia`, `Avalonia.Desktop` (support desktop) et `Avalonia.Themes.Fluent` (thème visuel). Bon à savoir : **Avalonia utilise SkiaSharp en interne** pour dessiner — il a été tiré automatiquement comme dépendance transitive (on le voit dans bin/ : `SkiaSharp.dll`). Donc "SkiaSharp" du README est bien présent, sous le capot d'Avalonia.

**Avalonia — structure d'une app GUI**
Avalonia est un framework UI cross-platform (Windows/Mac/Linux), très inspiré de WPF. Une app minimale se compose de :
- `Program.cs` : le `Main` qui démarre l'app. `AppBuilder.Configure<App>().UsePlatformDetect().StartWithClassicDesktopLifetime(args)`. L'attribut `[STAThread]` sur `Main` est obligatoire sur Windows (modèle de thread des UI Windows).
- `App.axaml` (+ `.axaml.cs`) : la classe application, qui charge le thème et crée la fenêtre principale.
- `MainWindow.axaml` (+ `.axaml.cs`) : la fenêtre.

**AXAML / XAML — décrire l'UI en balisage**
Les fichiers `.axaml` décrivent l'interface en XML (le XAML d'Avalonia). C'est déclaratif, comme du HTML/JSX pour une UI native :
```xml
<DockPanel>
    <Border DockPanel.Dock="Bottom">...</Border>
    <Panel x:Name="GridHost" />
</DockPanel>
```
- Chaque fichier `.axaml` est associé à une classe C# "code-behind" (`MainWindow.axaml.cs`) via `x:Class`. Le mot-clé `partial` sur la classe permet de répartir une même classe sur plusieurs fichiers (le XAML génère une part, le .cs l'autre).
- `x:Name="GridHost"` nomme un élément → on le récupère côté C# avec `this.FindControl<Panel>("GridHost")`.
- Panneaux de disposition : `DockPanel` (ancre des enfants sur les bords), `Panel` (superposition simple), etc. — analogues aux systèmes de layout CSS.

**Dessin custom — `Control` + `Render(DrawingContext)`**
Pour dessiner nous-mêmes (la grille), on hérite de `Control` et on surcharge `Render` :
```csharp
public override void Render(DrawingContext context) {
    context.FillRectangle(brush, new Rect(x, y, w, h));
}
```
- `DrawingContext` = la surface de dessin (≈ le context d'un `<canvas>` HTML).
- `InvalidateVisual()` = "cette zone est périmée, redessine-la" → Avalonia rappellera `Render`. On l'appelle après chaque tick.
- On mappe `id de stratégie → IBrush` (pinceau de couleur) une fois, puis on peint chaque case. `Color.TryParse("#RRGGBB", out var c)` convertit le hex du profil en couleur — c'est là que la couleur stockée en JSON devient un pixel à l'écran.

**Game loop — `DispatcherTimer`**
Pour animer, on utilise un `DispatcherTimer` (timer qui s'exécute sur le **thread UI**, donc on peut redessiner sans risque) :
```csharp
_timer.Interval = TimeSpan.FromMilliseconds(50); // ~20 ticks/seconde
_timer.Tick += OnGameTick;                        // abonnement à l'événement
_timer.Start();
```
- `_timer.Tick += OnGameTick;` : abonnement à un **événement** (les `event` C#, façon idiomatique d'écouter). `+=` ajoute un handler (comme `addEventListener`).
- À chaque `Tick` : `sim.Tick()` (avance le moteur), puis `gridView.Redraw()` (redessine). Quand la partie est finie, `_timer.Stop()`.
C'est LA séparation qu'on avait anticipée : le moteur (`Simulation`) ne connaît rien de l'UI ; l'UI se contente de l'appeler périodiquement et de lire `grid.Cells`.

**`.csproj` pour une app Avalonia**
Réglages ajoutés : `<OutputType>WinExe</OutputType>` (app fenêtrée sans console qui traîne — vs `Exe` pour la console), `<BuiltInComInteropSupport>true` (requis Windows), `<ApplicationManifest>app.manifest` (déclare le DPI-awareness pour un rendu net sur écrans haute résolution).

***7 Juillet 2026***

**Navigation entre écrans (multi-vues)**
Une app à plusieurs écrans (Splash → Menu → Création → Jeu) : la fenêtre contient un `ContentControl` dont on remplace la propriété `Content` par la vue courante.
```csharp
private void Navigate(Control view) => _host.Content = view;
```
Chaque écran est un `UserControl` (un composant réutilisable, ≈ un composant React). Les écrans ne se connaissent pas entre eux : ils **lèvent des événements** que la fenêtre écoute pour décider de la navigation. Ça garde les vues découplées.

**Communication par événements (`event Action`)**
Une vue signale une intention sans savoir qui l'écoute :
```csharp
public event Action? Finished;                       // sans donnée
public event Action<SimulationConfig>? LaunchRequested; // avec une donnée
...
Finished?.Invoke();                 // déclenche l'événement (le ?. gère "aucun abonné")
LaunchRequested?.Invoke(config);
```
Côté fenêtre, on s'abonne avec `+=` :
```csharp
menu.LaunchRequested += ShowGame;   // ShowGame(config) sera appelé
```
`Action` = un délégué (référence de méthode) sans retour ; `Action<T>` en prend un paramètre. C'est le mécanisme idiomatique C# pour du "callback"/pub-sub, l'équivalent d'émettre un event et d'y abonner un handler en JS.

**Accessibilité `public` / `internal` (piège rencontré)**
En C#, une classe sans modificateur est `internal` (visible seulement dans l'assembly). Les vues générées par AXAML sont `public`. Une méthode/événement `public` **ne peut pas exposer un type `internal`** → erreur `CS0051/CS0053 : accessibilité incohérente`. Comme les vues publiques manipulent nos types métier (`SimulationConfig`, `StrategyProfile`, `Simulation`…), il a fallu rendre **tout le domaine `public`**. Règle : la visibilité d'un type doit être ≥ celle de tout ce qui l'expose.

**Construire des contrôles en code (vs XAML)**
On peut créer l'UI en XAML (déclaratif) **ou** en C# (impératif). Pour des listes dynamiques (une ligne par stratégie, une pastille par couleur), le C# est plus pratique :
```csharp
CheckBox box = new() { IsChecked = true };
Border swatch = new() { Width = 16, Height = 16, Background = HexBrush(hex) };
StackPanel row = new() { Orientation = Orientation.Horizontal, Children = { box, swatch, label } };
itemsControl.ItemsSource = items;   // on injecte la liste construite
```
`Thickness` (marges/paddings) et `CornerRadius` vivent dans le namespace racine `Avalonia` (pas `Avalonia.Layout`) — d'où un `using Avalonia;` nécessaire.

**Contrôles de saisie utilisés**
- `TextBox` (champ texte, `PlaceholderText` pour l'invite — `Watermark` est déprécié), `ComboBox` (liste déroulante ; `ItemsSource = Enum.GetValues<StrategyType>()` la remplit avec toutes les valeurs de l'enum), `Slider` (curseur 0..1), `NumericUpDown` (nombre avec flèches), `CheckBox`.
- Écouter un changement de valeur : `slider.PropertyChanged += (_, e) => { if (e.Property == Slider.ValueProperty) ... }`. Les contrôles Avalonia exposent leurs propriétés comme des `AvaloniaProperty` statiques, qu'on compare pour filtrer l'événement.
- `PointerPressed` = événement de clic (souris) sur n'importe quel contrôle → utilisé pour rendre les pastilles de couleur cliquables.

**Colorpicker maison**
Pas besoin du gros widget : une `WrapPanel` de `Border` colorés cliquables (palette) + un `TextBox` hex synchronisé avec un aperçu. `Color.TryParse("#RRGGBB", out var c)` valide et convertit la saisie. Simple, léger, dans le thème.

**Dessin animé conditionnel (logo qui apparaît)**
Le `PixelLogo` dessine un motif `int[,]` (pixel-art) et n'affiche que les N premiers pixels selon une progression `Reveal` (0..1). Un `DispatcherTimer` incrémente `Reveal` → effet d'apparition progressive. Un 2e timer (3s) lève `Finished` pour passer au menu. Rappel utile : le temps de démarrage à froid de `dotnet run` (compilation + lancement) s'ajoute avant l'affichage — pour tester le timing réel du splash, lancer l'`.exe` déjà compilé directement.