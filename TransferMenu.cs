using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using Raylib_cs;
using RL = Raylib_cs.Raylib;
public static class TransferMenu
{
    const int InventorySize = 36;
    const int InventoryColumns = 6;
    const int InventoryPadding = 16;
    const int SlotSize = 64;
    const int SlotGap = 8;
    const int SlotFontSize = 20;
    private static int _hoveredSlot = -1;
    private static Container? _hoveredContainer;
    private static ItemStack? _draggedItem;
    private static int _dragged = -1;
    private static Rectangle _draggingFromPanel;
    private static Container? _fromContainer;

    public static void Update(Container backpack, WorldContainer nearItems,
        int tileX, int tileY, Facility facility)
    {
        _hoveredSlot = -1;
        Vector2 mousePos = RL.GetMousePosition();
        bool isInventorySide = mousePos.X < RL.GetScreenWidth()/2;
        _hoveredContainer = isInventorySide ? backpack : nearItems.Contents;
        Rectangle panel = isInventorySide ? GetPanel() : GetPanel(RL.GetScreenWidth()/2 + 50);
        for(int i = 0; i < _hoveredContainer.SlotCount; i++)
        {
            Rectangle slotRec = GetSlotRec(i, panel);

            if(RL.CheckCollisionPointRec(mousePos, slotRec)) _hoveredSlot = i;
            
        }

        //Remove item from inventory
        if(RL.IsMouseButtonReleased(MouseButton.Right) && _hoveredSlot != -1 && _hoveredContainer == backpack)
        {
            ItemTransfer.DropGround(_hoveredContainer, _hoveredSlot, int.MaxValue, tileX, tileY, facility);
        }

        //DraggingBegin
        if(RL.IsMouseButtonPressed(MouseButton.Left) && _hoveredSlot != -1)
        {
            _dragged = _hoveredSlot;
            _draggingFromPanel = panel;
            _fromContainer = isInventorySide ? backpack : nearItems.Contents;
        }

        //DraggingEnd
        if(RL.IsMouseButtonReleased(MouseButton.Left) && _dragged != -1 && _fromContainer != null)
        {
            ItemStack? item = _fromContainer.GetSlot(_dragged);

            //Check if dragging form backpack to world
            if(_fromContainer == backpack && _hoveredContainer == nearItems.Contents && item != null)
            {
                ItemTransfer.DropGround(_fromContainer, _dragged, item.Count, tileX, tileY, facility);
            }

            if(_fromContainer == backpack && _hoveredContainer == backpack && item != null)
            {
                ItemTransfer.ChangeSlot(backpack, _dragged, _hoveredSlot);
            }

            //Check if dragging from world to backpack
            if(_fromContainer == nearItems.Contents && _hoveredContainer == backpack && item != null)
            {
                ItemTransfer.TakeFromWorld(nearItems, _dragged, item.Count, backpack, facility);
            }

            _dragged = -1;
            _fromContainer = null;
        }
    }

    public static Rectangle GetPanel(int x = 50)
    {
        int rows = (InventorySize + InventoryColumns - 1) / InventoryColumns;
        float width = InventoryColumns * (SlotSize + SlotGap) - SlotGap + InventoryPadding * 2;
        float height = rows * (SlotSize + SlotGap) - SlotGap + InventoryPadding * 2;
        
        return new(x, 50, width, height);
    }
    public static Rectangle GetSlotRec(int index, Rectangle panel)
    {
        int column = index % InventoryColumns;
        int row = index / InventoryColumns;
        float x = panel.X + (SlotSize + SlotGap) * column + InventoryPadding;
        float y = panel.Y + (SlotSize + SlotGap) * row + InventoryPadding;
        return new(x, y, SlotSize, SlotSize);
    }

    public static void Draw(Container backpack, Container nearItems, Vector2 mousePos)
    {
        Rectangle panel = GetPanel();
        RL.DrawLineEx(new(RL.GetScreenWidth()/2, 0), new(RL.GetScreenWidth()/2, RL.GetScreenHeight()), 10, Color.Black);
        DrawSingleContainer(panel, backpack, mousePos.X < RL.GetScreenWidth()/2);

        if (!nearItems.IsEmpty)
        {
            panel = GetPanel(RL.GetScreenWidth()/2 + 50);
            DrawSingleContainer(panel, nearItems, mousePos.X > RL.GetScreenWidth()/2);
        }
        
        
        if(_dragged == -1) return;
        if(_fromContainer == null) return;

        Rectangle slotRec = GetSlotRec(_dragged, _draggingFromPanel);
        ItemStack? item = _fromContainer.GetSlot(_dragged);
        if(item == null) return;
        slotRec.X = mousePos.X;
        slotRec.Y = mousePos.Y;
        RL.DrawRectangleRec(slotRec, ItemVisuals.GetColor(item.Type));
    }

    public static void DrawSingleContainer(Rectangle panel, Container container, bool mouseOnSide)
    {
        RL.DrawRectangleRec(panel, new(30, 30, 30, 230));
        RL.DrawRectangleLinesEx(panel, 2, Color.White);
        for(int i = 0; i < container.SlotCount; i++)
        {
            Rectangle slotRec = GetSlotRec(i, panel);
            RL.DrawRectangleRec(slotRec, Color.DarkGray);

            if(i >= container.SlotCount) continue;
            ItemStack? item = container.GetSlot(i);
            if(mouseOnSide && i == _hoveredSlot) RL.DrawRectangleLinesEx(slotRec, 2, Color.White);

            if(item == null) continue;
            Rectangle itemRec = new(slotRec.X + 8, slotRec.Y + 8, slotRec.Width - 16, slotRec.Height - 16);
            if(_fromContainer == container && i == _dragged) continue;
            RL.DrawRectangleRec(itemRec, ItemVisuals.GetColor(item.Type));
            string count = $"x{item.Count}";
            int width = RL.MeasureText(count, SlotFontSize);
            RL.DrawText(count, (int)(slotRec.X + slotRec.Width - width - 5), (int)(slotRec.Y + slotRec.Height - 20),
                SlotFontSize, Color.White);
            
        }
    }
}