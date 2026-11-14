public static class LootTable
{
    public static ItemType? Roll(float depthZone, Random rng)
    {
        var available = ItemDataBase.All.Where(i => i.MinDepth <= depthZone).ToList();
        if(available.Count == 0) return null;

        float totalWeight = available.Sum(i => i.Weight);
        float roll = (float)(rng.NextDouble() * totalWeight);

        float running = 0;
        foreach(ItemDefinition item in available)
        {
            running += item.Weight;
            if(roll < running) return item.Type;
        }
        return available[^1].Type;
    }
}