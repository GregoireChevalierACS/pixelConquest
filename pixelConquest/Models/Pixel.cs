namespace pixelConquest;

class Pixel
{
    public Position Coords { get; set; }
    public int ClaimedBy { get; set; } = 0;


    public Pixel(int x, int y)
    {
    Coords = new Position(x,y);
    }
    
}