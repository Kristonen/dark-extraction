public enum Direction
{
    North, East, South, West
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction d) => d switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        _ => Direction.East
    };

    public static (int dx, int dy) Offset(this Direction d) => d switch
    {
        Direction.North => (0, -1),
        Direction.East => (1, 0),
        Direction.South => (0, 1),
        _ => (-1, 0)
    };
}