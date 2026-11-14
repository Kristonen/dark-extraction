public enum WorldContainerKind
{
    Pile, Crate,
}

public class WorldContainer
{
    public int TileX { get; }
    public int TileY { get; }
    public WorldContainerKind Kind { get; }
    public Container Contents { get; }
    public const int PileSlots = 16;

    public WorldContainer(int tileX, int tileY, WorldContainerKind kind, Container contents)
    {
        TileX = tileX;
        TileY = tileY;
        Kind = kind;
        Contents = contents;
    }
}