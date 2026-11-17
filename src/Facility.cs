using System.Numerics;

public enum FacilityKind
{
    Camp, Storage
}
public class Facility
{
    public TileMap Map { get; }
    public List<PlacedRoom> Rooms { get; }
    public DepthMap Depth { get; }
    public List<LootItem> Loot { get; } = [];
    public List<WorldContainer> Containers { get; } = [];
    public List<LightSource> LightSources { get; } = [];
    public ExtractionPoint ExtractionPoint { get; set; }

    public (int x, int y) StartTile
    {
        get
        {
            Room start = Rooms[0].Bounds;
            return(start.CenterX, start.CenterY);
        }
    }

    public Facility(TileMap map, List<PlacedRoom> rooms)
    {
        Map = map;
        Rooms = rooms;
        Depth = DepthMapBuilder.Build(map, StartTile);
    }

    public bool IsAtExtractionPos(Vector2 pos)
    {
        int x = ExtractionPoint.TileX * TileMap.TileSize + 16;
        int y = ExtractionPoint.TileY * TileMap.TileSize + 16;
        if(DistanceTo(pos, new(x, y)) <= 16f)
        {
            return true;
        }
        return false;
    }

    public WorldContainer? FindNearestCrate(Vector2 worldPos, float range)
    {
        return Containers
            .Where(c => c.Kind == WorldContainerKind.Crate && DistanceTo(c, worldPos) <= range)
            .OrderBy(c => DistanceTo(c, worldPos))
            .FirstOrDefault();
    }

    public Container FindNearPiles(Vector2 worldPosition, float radius)
    {
        List<LootItem> nearestLoot = Loot
            .Where(l => DistanceTo(l, worldPosition) <= radius)
            .ToList();
        if(nearestLoot.Count == 0) return new(8);
        Container c = new(nearestLoot.Count);
        foreach(LootItem item in nearestLoot)
        {
            c.Add(item);
        }
        
        return c;
    }

    private static float DistanceTo(WorldContainer c, Vector2 pos)
    {
        Vector2 center = new((c.TileX + 0.5f) * TileMap.TileSize,
                        (c.TileY + 0.5f) * TileMap.TileSize);
        return Vector2.Distance(pos, center);
    }

    private static float DistanceTo(LootItem l, Vector2 pos)
    {
        Vector2 center = new((l.TileX + 0.5f) * TileMap.TileSize,
                            (l.TileY + 0.5f) * TileMap.TileSize);
        return Vector2.Distance(pos, center);
    }

    private static float DistanceTo(Vector2 pos, Vector2 otherPos)
    {
        return Vector2.Distance(pos, otherPos);
    }

    public Vector2 StartPosition
    {
        get
        {
            Room start = Rooms[0].Bounds;
            return new(
                (start.CenterX + 0.5f) * TileMap.TileSize,
                (start.CenterY + 0.5f) * TileMap.TileSize
            );
        }
    }
}