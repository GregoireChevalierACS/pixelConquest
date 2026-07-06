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