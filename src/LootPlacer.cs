public static class LootPlacer
{
    private const double SpotChance = 0.75;

    public static void Place(Facility facility, Random rng)
    {
        for (int i = 1; i < facility.Rooms.Count; i++)
        {
            foreach(LootSpot spot in facility.Rooms[i].GetMapLootSpots())
            {
                if(rng.NextDouble() > SpotChance) continue;
                float depth = facility.Depth.GetDepthOne(spot.X, spot.Y);
                if(depth < 0) continue;
                if(spot.Kind == LootSpotKind.Loose)
                {
                    ItemType? type = LootTable.Roll(depth, rng);
                    if(type != null)
                    {
                        int amount = rng.Next(1, 4);
                        LootItem item = new(type.Value, amount, spot.X, spot.Y);
                        facility.Loot.Add(item);
                    }
                    
                }
                else
                {
                    WorldContainer? container = CreateContainer(spot, depth, rng);
                    if(container != null) facility.Containers.Add(container);
                }
            }
        }
    }

    private static WorldContainer? CreateContainer(LootSpot spot, float depth, Random rng)
    {
        Container contents = new(8);
        int count = 1 + rng.Next(0, 2 + (int)(depth * 3));
        
        for(int n = 0; n < count; n++)
        {
            ItemType? type = LootTable.Roll(depth, rng);
            if(type == null) continue;

            int amount = ItemDataBase.Get(type.Value).Category == ItemCategory.Resource
                ? rng.Next(1, 4)
                : 1;
            
            contents.Add(type.Value, amount, spot.X, spot.Y);
        }
        if(contents.IsEmpty) return null;
        return new(spot.X, spot.Y, WorldContainerKind.Crate, contents);
    }
}