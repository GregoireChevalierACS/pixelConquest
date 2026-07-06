using pixelConquest;

// --- Démo 1 : expansion aléatoire sur une grille (moteur de conquête) ---
RunRandomExpansion();

// --- Démo 2 : détection d'encerclement (flood-fill depuis les bords) ---
RunEncirclementDemo();


void RunRandomExpansion()
{
    Grid grid = new(20, 10);
    RandomStrategy strategy = new(id: 1);
    grid.Cells[grid.LengthX / 2, grid.LengthY / 2] = strategy.Id;

    int tick = 0;
    while (strategy.NextMove(grid) is Position move)
    {
        grid.Cells[move.X, move.Y] = strategy.Id;
        tick++;
    }

    Console.WriteLine($"[Démo 1] Expansion aléatoire terminée en {tick} ticks.\n");
    PrintGrid(grid);
    Console.WriteLine();
}

void RunEncirclementDemo()
{
    // Grille 7x7 : la stratégie 1 forme un anneau, laissant une poche
    // neutre encerclée au centre. Un pixel neutre au bord reste libre.
    Grid grid = new(7, 7);
    RandomStrategy strategy = new(id: 1);

    // Dessine un anneau de '1' de (2,2) à (4,4), centre (3,3) laissé neutre.
    for (int x = 2; x <= 4; x++)
    {
        for (int y = 2; y <= 4; y++)
        {
            bool isRingBorder = x == 2 || x == 4 || y == 2 || y == 4;
            if (isRingBorder)
            {
                grid.Cells[x, y] = strategy.Id;
            }
        }
    }

    Console.WriteLine("[Démo 2] Grille avec une poche encerclée au centre :");
    PrintGrid(grid);

    List<Position> encircled = strategy.GetEncircledPixels(grid);
    Console.WriteLine($"\nPixels encerclés détectés : {encircled.Count}");
    foreach (Position p in encircled)
    {
        Console.WriteLine($"  ({p.X}, {p.Y})");
    }
}

void PrintGrid(Grid grid)
{
    for (int y = 0; y < grid.LengthY; y++)
    {
        for (int x = 0; x < grid.LengthX; x++)
        {
            Console.Write(grid.Cells[x, y] == 0 ? '.' : '#');
        }
        Console.WriteLine();
    }
}
