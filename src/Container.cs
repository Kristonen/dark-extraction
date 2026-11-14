public class Container
{
    private readonly ItemStack?[] _slots;
    public float StackMultiplier { get; }
    public bool IsEmpty => _slots.All(s => s == null);
    public int SlotCount => _slots.Length;

    public Container(int slotCount, float stackMultiplier = 1)
    {
        _slots = new ItemStack?[slotCount];
        StackMultiplier = stackMultiplier;
    }

    public ItemStack? GetSlot(int index) => _slots[index];

    public int GetMaxStack(ItemType type)
    {
        int baseStack = ItemDataBase.Get(type).MaxStack;
        return (int)MathF.Floor(baseStack * StackMultiplier);
    }

    public int Count(ItemType type)
    {
        int total = 0;
        foreach(ItemStack? slot in _slots)
        {
            if(slot != null && slot.Type == type) total += slot.Count;
        }
        return total;
    }

    public int Add(ItemType type, int amount)
    {
        if(amount <= 0) return 0;
        int max = GetMaxStack(type);
        if(max <= 0) return amount;
        for(int i = 0; i < _slots.Length; i++)
        {
            ItemStack? slot = _slots[i];
            if(slot == null || slot.Type != type || slot.Count >= max) continue;

            int added = Math.Min(max - slot.Count, amount);
            slot.Count += added;
            amount -= added;
            if(amount == 0) return 0;
        }

        for(int i = 0; i < _slots.Length; i++)
        {
            if(_slots[i] != null) continue;
            int put = Math.Min(amount, max);
            _slots[i] = new ItemStack(type, put);
            amount -= put;
            if(amount == 0) return 0;
        }
        return amount;
    }

    public void Move(int from, int to)
    {
        if(from == to) return;
        ItemStack? source = _slots[from];
        if(source == null) return;
        ItemStack? target = _slots[to];

        if(target == null)
        {
            _slots[to] = source;
            _slots[from] = null;
            return;
        }

        if(target.Type == source.Type)
        {
            int free = Math.Max(0, GetMaxStack(target.Type) - target.Count);
            int moved = Math.Min(free, source.Count);
            target.Count += moved;
            source.Count -= moved;
            if(source.Count == 0) _slots[from] = null;
            return;
        }

        (_slots[from], _slots[to]) = (_slots[to], _slots[from]);
    }

    public bool TryRemove(ItemType type, int amount)
    {
        if(Count(type) < amount) return false;

        for(int i = 0; i < _slots.Length && amount > 0; i++)
        {
            ItemStack? slot = _slots[i];
            if(slot == null || slot.Type != type) continue;
            int take = Math.Min(slot.Count, amount);
            slot.Count -= take;
            amount -= take;

            if(slot.Count == 0) _slots[i] = null;
        }
        return true;
    }

    public void RemoveFromSlot(int index, int amount)
    {
        ItemStack? slot = _slots[index];
        if(slot == null) return;

        slot.Count -= Math.Min(amount, slot.Count);
        if(slot.Count == 0) _slots[index] = null;
    }
}