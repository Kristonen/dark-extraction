public class PlacedRoom
{
    public RoomTemplate Template { get; }
    public int X { get; }
    public int Y { get; }
    public Room Bounds => new Room(X, Y, Template.Width, Template.Height);

    public PlacedRoom(RoomTemplate template, int x, int y)
    {
        Template = template;
        X = x;
        Y = y;
    }

    public IEnumerable<Connector> GetMapConnectors()
    {
        foreach(Connector c in Template.Connectors)
        {
            yield return new(X + c.X, Y + c.Y, c.Direction);
        }
    }

    public IEnumerable<LootSpot> GetMapLootSpots()
    {
        foreach(LootSpot loot in Template.LootSpots)
        {
            yield return new(X + loot.X, Y + loot.Y, loot.Kind);
        }
    }
    
    public IEnumerable<LightSource> GetMapLightSources()
    {
        foreach(LightSource l in Template.LightSources)
        {
            yield return new(
                6, l.GlowRadius, l.GlowRays, X + l.TileX, Y + l.TileY
            );
        }
    }
}