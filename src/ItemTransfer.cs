using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Cryptography;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public static class ItemTransfer
{
    // public static int MoveStack(Container from, int fromSlot, Container to, int amount)
    // {
    //     if(ReferenceEquals(from, to)) return 0;
    //     LootItem? slot = from.GetSlot(fromSlot);
    //     if(slot == null) return 0;
    //     ItemType type = slot.Type;
    //     int wanted = Math.Min(amount, slot.Amount);
    //     int rest = to.Add(type, wanted, slot.TileX, slot.TileY);
    //     int moved = wanted - rest;
    //     from.RemoveFromSlot(fromSlot, moved);
    //     return moved;
    // }

    public static int ChangeSlot(Container source, int from, int to)
    {
        LootItem? fromItem = source.GetSlot(from);
        if(fromItem == null) return 0;
        if(to == -1) return 0;
        source.Move(from, to);
        return 0;
    }

    public static void DropGround(Container from, int slot, int tileX, int tileY, Facility facility)
    {
        LootItem? item = from.GetSlot(slot);
        if(item == null) return;
        facility.Loot.Add(new(item.Type, item.Amount, tileX, tileY));
        from.RemoveFromSlot(slot, item.Amount);
    }

    public static void PutInCrate(Container from, int slot, Container to)
    {
        LootItem? item = from.GetSlot(slot);
        if(item == null) return;
        to.Add(item.Type, item.Amount, 0, 0);
        from.RemoveFromSlot(slot, item.Amount);
    }

    public static void TakeFromCrate(Container crate, int slot, Container to)
    {
        LootItem? item = crate.GetSlot(slot);
        if(item == null) return;
        int wanted = item.Amount;
        int rest = to.Add(item.Type, item.Amount, 0, 0);
        int moved = wanted - rest;
        crate.RemoveFromSlot(slot, moved);
    }

    public static void TakeFromWorld(Container source, int slot, int amount, Container to, Facility facility)
    {
        LootItem? item = source.GetSlot(slot);
        if(item == null) return;
        int rest = to.Add(item.Type, item.Amount, item.TileX, item.TileY);
        Console.WriteLine(rest);
        if(rest == 0)
        {
            facility.Loot.Remove(item);
            return;
        }
    }
}