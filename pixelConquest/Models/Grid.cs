namespace pixelConquest;

class Grid
{
    public Dictionary <int, int> PixelTracker { get; set; } 
    public int LengthX { get; set; }
    public int LengthY { get; set; }

    public Grid(int lengthX, int lengthY)
    {
        PixelTracker = new Dictionary<int, int>();
        LengthX = lengthX;
        LengthY = lengthY;
    }
}