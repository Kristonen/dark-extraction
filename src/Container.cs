public class Container
{
    private readonly LootItem?[] _slots;
    public float StackMultiplier { get; }
    public bool IsEmpty => _slots.All(s => s == null);
    public int SlotCount => _slots.Length;

    public Container(int slotCount, float stackMultiplier = 1)
    {
        _slots = new LootItem?[slotCount];
        StackMultiplier = stackMultiplier;
    }

    public LootItem? GetSlot(int index) => _slots[index];

    public int GetMaxStack(ItemType type)
    {
        int baseStack = ItemDataBase.Get(type).MaxStack;
        return (int)MathF.Floor(baseStack * StackMultiplier);
    }

    public int Count(ItemType type)
    {
        int total = 0;
        foreach(LootItem? slot in _slots)
        {
            if(slot != null && slot.Type == type) total += slot.Amount;
        }
        return total;
    }

    public int Add(ItemType type, int amount, int tileX, int tileY)
    {
        if(amount <= 0) return 0;
        int max = GetMaxStack(type);
        if(max <= 0) return amount;
        for(int i = 0; i < _slots.Length; i++)
        {
            LootItem? slot = _slots[i];
            if(slot == null || slot.Type != type || slot.Amount >= max) continue;

            int added = Math.Min(max - slot.Amount, amount);
            slot.Amount += added;
            amount -= added;
            if(amount == 0) return 0;
        }

        for(int i = 0; i < _slots.Length; i++)
        {
            if(_slots[i] != null) continue;
            int put = Math.Min(amount, max);
            _slots[i] = new(type, put, tileX, tileY);
            amount -= put;
            if(amount == 0) return 0;
        }
        return amount;
    }

    public void Add(LootItem item)
    {
        for(int i = 0; i < _slots.Length; i++)
        {
            if(_slots[i] != null) continue;
            _slots[i] = item;
            return;
        }
    }

    public void Move(int from, int to)
    {
        if(from == to) return;
        LootItem? source = _slots[from];
        if(source == null) return;
        LootItem? target = _slots[to];

        if(target == null)
        {
            _slots[to] = source;
            _slots[from] = null;
            return;
        }

        if(target.Type == source.Type)
        {
            int free = Math.Max(0, GetMaxStack(target.Type) - target.Amount);
            int moved = Math.Min(free, source.Amount);
            target.Amount += moved;
            source.Amount -= moved;
            if(source.Amount == 0) _slots[from] = null;
            return;
        }

        (_slots[from], _slots[to]) = (_slots[to], _slots[from]);
    }

    public bool TryRemove(ItemType type, int amount)
    {
        if(Count(type) < amount) return false;

        for(int i = 0; i < _slots.Length && amount > 0; i++)
        {
            LootItem? slot = _slots[i];
            if(slot == null || slot.Type != type) continue;
            int take = Math.Min(slot.Amount, amount);
            slot.Amount -= take;
            amount -= take;

            if(slot.Amount == 0) _slots[i] = null;
        }
        return true;
    }

    public void RemoveFromSlot(int index, int amount)
    {
        LootItem? slot = _slots[index];
        if(slot == null) return;

        slot.Amount -= Math.Min(amount, slot.Amount);
        if(slot.Amount == 0) _slots[index] = null;
    }
}