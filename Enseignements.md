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