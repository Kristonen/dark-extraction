public class ItemStack
{
    public ItemType Type { get; }
    public int Count { get; set; }

    public ItemStack(ItemType type, int count)
    {
        Type = type;
        Count = count;
    }
}