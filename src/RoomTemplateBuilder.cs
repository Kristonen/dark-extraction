public class RoomTemplateBuilder
{
    private RoomTemplate _template;

    public RoomTemplateBuilder(string name, int width, int height)
    {
        _template = new(name, width, height);
    }
    
    public RoomTemplateBuilder AddLightSource(LightSource light)
    {
        _template.LightSources.Add(light);
        return this;
    }

    public RoomTemplateBuilder AddLightSource(List<LightSource> lights)
    {
        _template.LightSources = lights;
        return this;
    }

    public RoomTemplateBuilder AddLootSpot(LootSpot spot)
    {
        _template.LootSpots.Add(spot);
        return this;
    }

    public RoomTemplateBuilder AddLootSpot(List<LootSpot> spots)
    {
        _template.LootSpots = spots;
        return this;
    }

    public RoomTemplateBuilder AddTiles(Tile[,] tiles)
    {
        _template.Tiles = tiles;
        return this;
    }

    public RoomTemplateBuilder AddConnector(Connector connector)
    {
        _template.Connectors.Add(connector);
        return this;
    }

    public RoomTemplateBuilder AddConnector(List<Connector> connectors)
    {
        _template.Connectors = connectors;
        return this;
    }

    public RoomTemplateBuilder AddExtractionPoint(ExtractionPoint point)
    {
        _template.ExtractionPoint = point;
        return this;
    }

    public RoomTemplate Build()
    {
        return _template;
    }
}