public class RoomTemplate
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Tile[,] Tiles { get; set; }
    public List<Connector> Connectors { get; set; }
    public List<LootSpot> LootSpots { get; }
    public ExtractionPoint ExtractionPoint { get; }

    public RoomTemplate(string name, int width, int height, Tile[,] tiles, List<Connector> connectors, List<LootSpot> lootSpots)
    {
        Name = name;
        Width = width;
        Height = height;
        Tiles = tiles;
        Connectors = connectors;
        LootSpots = lootSpots;
    }

    public RoomTemplate(string name, int width, int height, Tile[,] tiles,
                        List<Connector> connectors, List<LootSpot> lootSpots, ExtractionPoint point)
    {
        Name = name;
        Width = width;
        Height = height;
        Tiles = tiles;
        Connectors = connectors;
        LootSpots = lootSpots;
        ExtractionPoint = point;
    }
}