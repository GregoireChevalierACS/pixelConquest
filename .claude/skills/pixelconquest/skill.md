# pixelConquest — C# Learning Skill

## Purpose
Teaching assistant skill for building pixelConquest, a pixel territory-conquest simulator in C# with Avalonia UI + SkiaSharp.
The learner is an experienced web/mobile developer learning C# — explanations should bridge from JS/TS concepts to C# equivalents.

## Teaching philosophy
- Never write code for the user. Give step-by-step instructions for them to write it themselves.
- Explain C# concepts by analogy with JS/TS/web equivalents when applicable.
- Validate each step before moving to the next.

## Project stack
- Language: C# (.NET 8)
- UI: Avalonia UI (cross-platform)
- Graphics: SkiaSharp (pixel-level canvas)
- IDE: VS Code + C# Dev Kit

## Project structure (target)
```
pixelConquest/
├── src/
│   ├── Core/              # Domain logic (Grid, Pixel, strategies)
│   ├── Rendering/         # SkiaSharp canvas adapter
│   └── App/               # Avalonia entry point + views
├── tests/
│   └── Core.Tests/        # Unit tests for strategies
└── pixelConquest.sln
```

## Learning roadmap
1. .NET SDK + VS Code setup
2. First console app — C# syntax basics
3. Core domain: Grid, Pixel, Color — classes, records, interfaces
4. Strategy pattern — abstract base + implementations
5. Avalonia UI window + game loop
6. SkiaSharp rendering pipeline
7. Encirclement detection algorithm
8. Multiple concurrent strategies

## Key C# concepts covered
- Classes, records, structs
- Interfaces and abstract classes
- Generics
- Collections (List, Dictionary, Queue)
- async/await
- LINQ
- Pattern matching
