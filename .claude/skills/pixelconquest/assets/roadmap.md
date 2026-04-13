# Learning Roadmap — pixelConquest

## Phase 1 — Setup & Hello World
- [ ] Install .NET 8 SDK
- [ ] Install VS Code + C# Dev Kit extension
- [ ] `dotnet new console` — first project
- [ ] Understand .csproj, Program.cs, namespaces

## Phase 2 — C# Core (domain logic)
- [ ] Classes & constructors (Grid, Pixel)
- [ ] Records (immutable value types)
- [ ] Interfaces (IStrategy)
- [ ] Enums (Direction)
- [ ] Collections — List, Queue, Dictionary
- [ ] LINQ basics

## Phase 3 — Strategy implementations
- [ ] Abstract base class for strategies
- [ ] BFS strategy (breadth-first expansion)
- [ ] Random walk strategy
- [ ] Greedy strategy
- [ ] Unit tests with xUnit

## Phase 4 — Visual rendering
- [ ] Create Avalonia project
- [ ] SkiaSharp canvas integration
- [ ] Render Grid as bitmap
- [ ] Game loop with timer

## Phase 5 — Encirclement mechanic
- [ ] Flood-fill algorithm to detect enclosed regions
- [ ] Trigger on each pixel capture
- [ ] Performance considerations

## Phase 6 — Polish
- [ ] Multiple strategies running concurrently
- [ ] Speed control UI
- [ ] Strategy selection UI
- [ ] Stats panel (% territory per strategy)
