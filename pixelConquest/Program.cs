using pixelConquest;

// --- Démo console : une stratégie aléatoire conquiert une petite grille ---

Grid grid = new(20, 10);

RandomStrategy strategy = new(id: 1);

// Seed : le pixel de départ de la stratégie (au centre).
grid.Cells[grid.LengthX / 2, grid.LengthY / 2] = strategy.Id;

// Boucle de simulation : on avance tant que la stratégie a un coup à jouer.
int tick = 0;
while (true)
{
    Position? move = strategy.NextMove(grid);
    if (move is null)
    {
        break; // plus aucun pixel neutre atteignable
    }

    grid.Cells[move.X, move.Y] = strategy.Id;
    tick++;
}

Console.WriteLine($"Simulation terminée en {tick} ticks.\n");

// Affichage : '.' = neutre, '#' = conquis par la stratégie 1.
for (int y = 0; y < grid.LengthY; y++)
{
    for (int x = 0; x < grid.LengthX; x++)
    {
        Console.Write(grid.Cells[x, y] == 0 ? '.' : '#');
    }
    Console.WriteLine();
}
