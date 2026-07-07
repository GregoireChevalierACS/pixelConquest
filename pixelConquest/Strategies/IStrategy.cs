namespace pixelConquest;

public interface IStrategy
{
    Position? NextMove(Grid grid);
    List<Position> GetNeighbors(Position position, Grid grid);
    List<Position> GetEncircledPixels(Grid grid);
}
