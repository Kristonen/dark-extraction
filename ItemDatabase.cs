public static class ItemDataBase
{
    public static readonly List<ItemDefinition> All =
    [
        new(ItemType.Battery, "Battery", ItemCategory.Consumable, 4, 0.00f, 10f),
        new(ItemType.Cloth, "Cloth", ItemCategory.Resource, 10, 0.00f, 30f),
        new(ItemType.Scrap, "Scrap", ItemCategory.Resource, 20, 0.00f, 20f),
        new(ItemType.Fuel, "Fuel", ItemCategory.Resource, 5, 0.10f, 8f),
        new(ItemType.Circuit, "Circuit", ItemCategory.Resource, 5, 0.35f, 6f),
        new(ItemType.Crystal, "Crystal", ItemCategory.Resource, 3, 0.70f, 3f),
    ];

    public static ItemDefinition Get(ItemType type)
    {
        return All.First(d => d.Type == type);
    }
}