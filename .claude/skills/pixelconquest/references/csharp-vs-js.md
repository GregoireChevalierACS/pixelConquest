# C# vs JavaScript/TypeScript — Quick Reference

## Variables
| JS/TS | C# |
|---|---|
| `const x = 5` | `const int x = 5` (compile-time) or `int x = 5` |
| `let x = 5` | `int x = 5` |
| `var x = 5` | `var x = 5` (inferred, same idea) |

## Types
| JS/TS | C# |
|---|---|
| `number` | `int`, `float`, `double`, `decimal` |
| `string` | `string` |
| `boolean` | `bool` |
| `undefined` / `null` | `null` (nullable: `int?`) |
| `any` | `object` (avoid) |

## Functions
| JS/TS | C# |
|---|---|
| `function foo(x: number): string` | `string Foo(int x)` |
| Arrow `(x) => x * 2` | Lambda `(x) => x * 2` (same syntax!) |
| `async function` / `await` | `async Task<T>` / `await` |

## Classes
| JS/TS | C# |
|---|---|
| `class Foo { constructor() {} }` | `class Foo { public Foo() {} }` |
| `interface IFoo {}` | `interface IFoo {}` |
| `extends` | `: BaseClass` |
| `implements` | `: IInterface` |
| Getter/setter | `public int X { get; set; }` |

## Collections
| JS/TS | C# |
|---|---|
| `Array<T>` / `T[]` | `T[]` or `List<T>` |
| `Map<K,V>` | `Dictionary<K,V>` |
| `Set<T>` | `HashSet<T>` |
| `.filter()` | `.Where()` (LINQ) |
| `.map()` | `.Select()` (LINQ) |
| `.find()` | `.FirstOrDefault()` (LINQ) |

## Modules
| JS/TS | C# |
|---|---|
| `import { X } from './foo'` | `using MyApp.Core;` |
| `export class Foo` | `public class Foo` (in a namespace) |
| `package.json` | `.csproj` |
| `npm install` | `dotnet add package` |
