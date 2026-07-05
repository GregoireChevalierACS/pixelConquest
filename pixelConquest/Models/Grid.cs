namespace pixelConquest;

class Grid
{
    public int[,] Cells { get; set; }
    public int LengthX { get; set; }
    public int LengthY { get; set; }

    public Grid(int lengthX, int lengthY)
    {
        LengthX = lengthX;
        LengthY = lengthY;
        Cells = new int[lengthX, lengthY];
    }
}
