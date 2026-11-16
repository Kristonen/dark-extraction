public class LootItem
{
    public ItemType Type { get; set; }
    public int Amount { get; set; }
    public int TileX { get; set; }
    public int TileY { get; set; }

    public LootItem(ItemType type, int amount, int tileX, int tileY)
    {
        Type = type;
        Amount = amount;
        TileX = tileX;
        TileY = tileY;
    }
}