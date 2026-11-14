public static class LootPlacer
{
    private const double SpotChance = 0.75;
    // public static void Place(Facility facility, Random rng)
    // {
    //     for (int i = 1; i < facility.Rooms.Count; i++)
    //     {
    //         PlacedRoom room = facility.Rooms[i];
    //         if(room.Template.Name.StartsWith("corridor")) continue;

    //         Room bounds = room.Bounds;
    //         float roomDepth = facility.Depth.GetDepthOne(bounds.CenterX, bounds.CenterY);
    //         if(roomDepth < 0) continue;

    //         int count = 1 + rng.Next(0, 1 + (int)(roomDepth * 2));
    //         for (int n = 0; n < count; n++)
    //         {
    //             TryPlaceOne(facility, bounds, rng);
    //         }
    //     }
    // }

    public static void Place(Facility facility, Random rng)
    {
        for (int i = 1; i < facility.Rooms.Count; i++)
        {
            Room bounds = facility.Rooms[i].Bounds;
            foreach(LootSpot spot in facility.Rooms[i].GetMapLootSpots())
            {
                if(rng.NextDouble() > SpotChance) continue;
                float depth = facility.Depth.GetDepthOne(spot.X, spot.Y);
                if(depth < 0) continue;

                WorldContainer? container = CreateContainer(spot, facility.Rooms[i], depth, rng);
                if(container != null) facility.Containers.Add(container);
            }
        }
    }

    private static WorldContainer? CreateContainer(LootSpot spot, PlacedRoom room, float depth, Random rng)
    {
        bool isCrate = spot.Kind == LootSpotKind.Container;
        Container contents = isCrate ? new(8) : new(WorldContainer.PileSlots);
        int count = isCrate 
            ? 1 + rng.Next(0, 2 + (int)(depth * 3)) //Crate
            : rng.Next(1, 3); //Pile
        
        for(int n = 0; n < count; n++)
        {
            ItemType? type = LootTable.Roll(depth, rng);
            if(type == null) continue;

            int amount = ItemDataBase.Get(type.Value).Category == ItemCategory.Resource
                ? rng.Next(1, 4)
                : 1;
            
            contents.Add(type.Value, amount);
        }
        if(contents.IsEmpty) return null;

        var kind = isCrate ? WorldContainerKind.Crate : WorldContainerKind.Pile;
        return new(spot.X, spot.Y, kind, contents);
    }

    private static void TryPlaceOne(Facility facility, Room bounds, Random rng)
    {
        const int maxTries = 20;
        for(int attempt = 0; attempt < maxTries; attempt++)
        {
            int x = rng.Next(bounds.X + 1, bounds.X + bounds.Width - 1);
            int y = rng.Next(bounds.Y + 1, bounds.Y + bounds.Height - 1);

            if(facility.Map.IsWall(x, y)) continue;
            if(facility.Loot.Any(l => l.TileX == x && l.TileY == y)) continue;

            float tileDepth = facility.Depth.GetDepthOne(x, y);
            ItemType? item = LootTable.Roll(tileDepth, rng);
            if(item != null) facility.Loot.Add(new LootItem(item.Value, x, y));
            return;
        }
    }
}