public readonly record struct Room(int X, int Y, int Width, int Height)
{
    public int CenterX => X + Width / 2;
    public int CenterY => Y + Height / 2;

    public bool Overlaps(Room other, int margin)
    {
        return X - margin < other.X + other.Width
            && X + Width + margin > other.X
            && Y - margin < other.Y + other.Height
            && Y + Height + margin > other.Y;
    }
}