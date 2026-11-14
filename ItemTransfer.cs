using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public static class ItemTransfer
{
    public static int MoveStack(Container from, int fromSlot, Container to, int amount)
    {
        if(ReferenceEquals(from, to)) return 0;
        ItemStack? slot = from.GetSlot(fromSlot);
        if(slot == null) return 0;
        ItemType type = slot.Type;
        int wanted = Math.Min(amount, slot.Count);
        int rest = to.Add(type, wanted);
        int moved = wanted - rest;
        from.RemoveFromSlot(fromSlot, moved);
        return moved;
    }

    public static int ChangeSlot(Container source, int from, int to)
    {
        ItemStack? fromItem = source.GetSlot(from);
        if(fromItem == null) return 0;
        if(to == -1) return 0;
        source.Move(from, to);
        return 0;
    }

    public static int DropGround(Container from, int slot, int amount, int tileX, int tileY, Facility facility)
    {
        WorldContainer pile = facility.GetOrCreatePile(tileX, tileY);
        int moved = MoveStack(from, slot, pile.Contents, amount);

        facility.RemoveIfEmptyPile(pile);
        return moved;
    }

    public static int TakeFromWorld(WorldContainer source, int slot, int amount, Container to, Facility facility)
    {
        int moved = MoveStack(source.Contents, slot, to, amount);
        facility.RemoveIfEmptyPile(source);
        return moved;
    }
}