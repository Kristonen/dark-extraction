public class RoomTemplate
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Tile[,] Tiles { get; set; } = new Tile[0,0];
    public List<Connector> Connectors { get; set; } = [];
    public List<LootSpot> LootSpots { get; set; } = [];
    public List<LightSource> LightSources { get; set; } = [];
    public ExtractionPoint ExtractionPoint { get; set; }

    public RoomTemplate(string name, int width, int height)
    {
        Name = name;
        Width = width;
        Height = height;
    }
}