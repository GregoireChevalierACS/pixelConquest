using pixelConquest;

// === Démo du système de configuration et de profils personnalisés ===

// 1. Catalogue de profils, persisté en JSON à côté de l'exécutable.
string catalogPath = Path.Combine(AppContext.BaseDirectory, "strategies.json");
StrategyCatalog catalog = new(catalogPath);
catalog.Load();

// Si le catalogue est vide (1er lancement), on l'amorce avec des profils variés.
if (catalog.Profiles.Count == 0)
{
    catalog.AddOrUpdate(new StrategyProfile
    {
        Name = "Rusher", Type = StrategyType.Bfs, Color = "#e63946",
        Randomness = 0.05, Compactness = 0.1,
    });
    catalog.AddOrUpdate(new StrategyProfile
    {
        Name = "Turtle", Type = StrategyType.Defensive, Color = "#457b9d",
        Compactness = 0.95,
    });
    catalog.AddOrUpdate(new StrategyProfile
    {
        Name = "Predator", Type = StrategyType.Aggressive, Color = "#2a9d8f",
        Aggressiveness = 0.8, Randomness = 0.1,
    });
    catalog.AddOrUpdate(new StrategyProfile
    {
        Name = "Chaos", Type = StrategyType.Random, Color = "#f4a261",
        Randomness = 1.0,
    });
    catalog.Save();
    Console.WriteLine($"Catalogue amorcé et sauvegardé → {catalogPath}\n");
}
else
{
    Console.WriteLine($"Catalogue chargé ({catalog.Profiles.Count} profils) ← {catalogPath}\n");
}

// 2. Construire une configuration de partie à partir de profils du catalogue.
SimulationConfig config = new()
{
    Width = 40,
    Height = 20,
    Strategies = new()
    {
        catalog.FindByName("Rusher")!,
        catalog.FindByName("Turtle")!,
        catalog.FindByName("Predator")!,
        catalog.FindByName("Chaos")!,
    },
};

// 3. Lancer la simulation.
Simulation sim = Simulation.FromConfig(config);
int ticks = sim.Run();

Console.WriteLine($"Partie : {config.Width}x{config.Height}, "
                + $"{config.Strategies.Count} stratégies, terminée en {ticks} ticks.\n");
PrintGrid(sim.Grid);

// 4. Résultats par stratégie.
Console.WriteLine("\n--- Résultats ---");
Dictionary<int, int> counts = CountTerritory(sim.Grid);
for (int i = 0; i < config.Strategies.Count; i++)
{
    int id = i + 1;
    StrategyProfile p = config.Strategies[i];
    Console.WriteLine($"{id}. {p.Name,-10} [{p.Type,-10}] : {counts.GetValueOrDefault(id),4} pixels");
}


void PrintGrid(Grid grid)
{
    for (int y = 0; y < grid.LengthY; y++)
    {
        for (int x = 0; x < grid.LengthX; x++)
        {
            int owner = grid.Cells[x, y];
            Console.Write(owner == 0 ? '.' : (char)('0' + owner));
        }
        Console.WriteLine();
    }
}

Dictionary<int, int> CountTerritory(Grid grid)
{
    Dictionary<int, int> counts = new();
    for (int x = 0; x < grid.LengthX; x++)
    {
        for (int y = 0; y < grid.LengthY; y++)
        {
            int owner = grid.Cells[x, y];
            counts[owner] = counts.GetValueOrDefault(owner) + 1;
        }
    }
    return counts;
}
