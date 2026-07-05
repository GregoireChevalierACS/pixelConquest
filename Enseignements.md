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