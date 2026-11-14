using Raylib_cs;

public static class ItemVisuals
{
    public static Color GetColor(ItemType type) => type switch
    {
        ItemType.Battery => Color.Yellow,
        ItemType.Scrap => Color.Gray,
        ItemType.Cloth => Color.Brown,
        ItemType.Fuel => Color.Orange,
        ItemType.Circuit => Color.Green,
        ItemType.Crystal => Color.Purple,
        _ => Color.White,
    };
}