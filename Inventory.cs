using System.Runtime;

public class Inventory
{
    private readonly Dictionary<ItemType, int> _items = new();
    public Container Backpack { get; } = new Container(6, 1);
    public int Count(ItemType type) => Backpack.Count(type);
    public int Add(ItemType type, int amount = 1) => Backpack.Add(type, amount);
    public bool TryRemove(ItemType type, int amount = 1) => Backpack.TryRemove(type, amount);

    // public int Count(ItemType type)
    // {
    //     return _items.TryGetValue(type, out int n) ? n : 0;
    // }

    // public void Add(ItemType type, int amount = 1)
    // {
    //     _items[type] = Count(type) + amount;
    // }

    // public bool TryRemove(ItemType type, int amount = 1)
    // {
    //     if(Count(type) < amount) return false;
    //     _items[type] = Count(type) - amount;
    //     return true;
    // }
}