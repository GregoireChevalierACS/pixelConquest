namespace pixelConquest;

interface IStrategy
{
    Position NextMove(Grid grid);
    List<Position> GetNeighbors(Position position, Grid grid);
    List<Pixel> GetEncircledPixels(Grid grid);
}
