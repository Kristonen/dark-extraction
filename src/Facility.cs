using System.Numerics;

public class Facility
{
    public TileMap Map { get; }
    public List<PlacedRoom> Rooms { get; }
    public DepthMap Depth { get; }
    public List<LootItem> Loot { get; } = new();
    public List<WorldContainer> Containers { get; } = new();

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

    //Only takes the next one, but for the future take every one in range
    public WorldContainer? FindNearestContainer(Vector2 worldPos, float range)
    {
        WorldContainer? best = null;
        float bestDistance = range;

        foreach(WorldContainer c in Containers)
        {
            var pos = new Vector2(
                (c.TileX + 0.5f) * TileMap.TileSize,
                (c.TileY + 0.5f) * TileMap.TileSize);
            
            float d = Vector2.Distance(worldPos, pos);
            if(d <= bestDistance)
            {
                best = c;
                bestDistance = d;
            }
        }
        return best;
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

    public WorldContainer GetOrCreatePile(int tileX, int tileY)
    {
        foreach(WorldContainer wc in Containers)
        {
            if(wc.Kind == WorldContainerKind.Pile && wc.TileX == tileX && wc.TileY == tileY)
            {
                return wc;
            }
        }

        var pile = new WorldContainer(tileX, tileY, WorldContainerKind.Pile,
            new Container(WorldContainer.PileSlots));

        Containers.Add(pile);
        return pile;
    }

    public void RemoveIfEmptyPile(Container container)
    {
        // if(container.IsEmpty)
        // {
        //     Containers.Remove(container);
        // }
    }
}