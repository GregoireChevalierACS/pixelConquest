using pixelConquest;

// --- Démo : 3 stratégies aléatoires s'affrontent sur la même grille ---

Grid grid = new(30, 15);

List<StrategyBase> strategies = new()
{
    new RandomStrategy(id: 1),
    new RandomStrategy(id: 2),
    new RandomStrategy(id: 3),
};

// Seeds : chaque stratégie démarre d'une zone distincte.
grid.Cells[1, 1] = 1;
grid.Cells[grid.LengthX - 2, 1] = 2;
grid.Cells[grid.LengthX / 2, grid.LengthY - 2] = 3;

Simulation sim = new(grid, strategies);
int ticks = sim.Run();

Console.WriteLine($"Partie terminée en {ticks} ticks.\n");
PrintGrid(grid);

// Décompte final du territoire de chaque stratégie.
Console.WriteLine();
Dictionary<int, int> counts = new();
for (int x = 0; x < grid.LengthX; x++)
{
    for (int y = 0; y < grid.LengthY; y++)
    {
        int owner = grid.Cells[x, y];
        counts[owner] = counts.GetValueOrDefault(owner) + 1;
    }
}

foreach (StrategyBase s in strategies)
{
    Console.WriteLine($"Stratégie {s.Id} : {counts.GetValueOrDefault(s.Id)} pixels");
}
if (counts.GetValueOrDefault(0) > 0)
{
    Console.WriteLine($"Neutres restants : {counts[0]}");
}


void PrintGrid(Grid grid)
{
    // 0 = '.', sinon le chiffre de la stratégie.
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
