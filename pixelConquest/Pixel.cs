namespace pixelConquest;

class Pixel
{
    public int X { get; set; }
    public int Y { get; set; }
    public int ClaimedBy { get; set; } = 0;


    public Pixel(int x, int y)
    {
    X = x;
    Y = y;   
    }
    
}