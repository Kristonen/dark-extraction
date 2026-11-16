using System.Numerics;
using System.Runtime.InteropServices.Marshalling;
using Raylib_cs;
using RL = Raylib_cs.Raylib;
public class TransferMenu
{
    const int InventorySize = 36;
    const int InventoryColumns = 6;
    const int InventoryPadding = 16;
    const int SlotSize = 64;
    const int SlotGap = 8;
    const int SlotFontSize = 20;
    private Container? _nearItems;
    private int _hoveredSlot = -1;
    private Container? _hoveredContainer;
    private int _dragged = -1;
    private Rectangle _draggingFromPanel;
    private Container? _fromContainer;
    private int _tileX;
    private int _tileY;
    private bool _isCrate;

    public void Update(Container backpack, Facility facility)
    {
        if(_nearItems == null) return;
        _hoveredSlot = -1;
        Vector2 mousePos = RL.GetMousePosition();
        bool isInventorySide = mousePos.X < RL.GetScreenWidth()/2;
        Rectangle panel = isInventorySide ? GetPanel() : GetPanel(RL.GetScreenWidth()/2 + 50);
        _hoveredContainer = isInventorySide ? backpack : _nearItems;
        for(int i = 0; i < _hoveredContainer.SlotCount; i++)
        {
            Rectangle slotRec = GetSlotRec(i, panel);
            if(RL.CheckCollisionPointRec(mousePos, slotRec)) _hoveredSlot = i;
        }

        //Remove item from inventory
        if(RL.IsMouseButtonReleased(MouseButton.Right) && _hoveredSlot != -1 && isInventorySide)
        {
            ItemTransfer.DropGround(backpack, _hoveredSlot, _tileX, _tileY, facility);
        }

        //DraggingBegin
        if(RL.IsMouseButtonPressed(MouseButton.Left) && _hoveredSlot != -1)
        {
            Console.WriteLine($"Dragged: {_hoveredSlot}");
            _dragged = _hoveredSlot;
            _draggingFromPanel = panel;
            _fromContainer = _hoveredContainer;
        }

        //DraggingEnd
        if(RL.IsMouseButtonReleased(MouseButton.Left) && _dragged != -1)
        {
            //Create new container for empty ground
            if(_hoveredContainer == null && _nearItems.SlotCount == 0) _hoveredContainer = new(8);

            // //Check if dragging from backpack to world
            if(_fromContainer == backpack && _hoveredContainer != backpack)
            {
                if (_isCrate)
                {
                    ItemTransfer.PutInCrate(backpack, _dragged, _nearItems);
                }
                else
                {
                    ItemTransfer.DropGround(backpack, _dragged, _tileX, _tileY, facility);
                    RefreshContainer(facility);
                }
            }

            //Change slots in backpack
            if(_fromContainer == backpack && _hoveredContainer == backpack)
            {
                ItemTransfer.ChangeSlot(backpack, _dragged, _hoveredSlot);
            }

            //Check if dragging from world to backpack
            if(_fromContainer != backpack && _hoveredContainer == backpack)
            {
                if (_isCrate)
                {
                    ItemTransfer.TakeFromCrate(_nearItems, _dragged, backpack);
                }
                else
                {
                    ItemTransfer.TakeFromWorld(_nearItems, _dragged, backpack, facility);
                    RefreshContainer(facility);
                }
            }

            _dragged = -1;
            _fromContainer = null;
        }
    }

    public Rectangle GetPanel(int x = 50)
    {
        int rows = (InventorySize + InventoryColumns - 1) / InventoryColumns;
        float width = InventoryColumns * (SlotSize + SlotGap) - SlotGap + InventoryPadding * 2;
        float height = rows * (SlotSize + SlotGap) - SlotGap + InventoryPadding * 2;
        
        return new(x, 50, width, height);
    }
    public Rectangle GetSlotRec(int index, Rectangle panel)
    {
        int column = index % InventoryColumns;
        int row = index / InventoryColumns;
        float x = panel.X + (SlotSize + SlotGap) * column + InventoryPadding;
        float y = panel.Y + (SlotSize + SlotGap) * row + InventoryPadding;
        return new(x, y, SlotSize, SlotSize);
    }

    public void Open(Container nearItems, int tileX, int tileY, bool isCrate = false)
    {
        _isCrate = isCrate;
        _tileX = tileX;
        _tileY = tileY;
        _nearItems = nearItems;
        ResetInteraction();
    }

    public void Close()
    {
        _nearItems = null;
        ResetInteraction();
    }

    private void ResetInteraction()
    {
        _hoveredSlot = -1;
        _hoveredContainer = null;
        _dragged = -1;
        _fromContainer = null;
    }

    private void RefreshContainer(Facility facility)
    {
        int x = _tileX * TileMap.TileSize;
        int y = _tileY * TileMap.TileSize;
        _nearItems = facility.FindNearPiles(new(x, y), 48f);
    }

    public void Draw(Container backpack, Vector2 mousePos)
    {
        bool mouseOnInventory = mousePos.X < RL.GetScreenWidth()/2;

        Rectangle panel = GetPanel();
        RL.DrawLineEx(new(RL.GetScreenWidth()/2, 0), new(RL.GetScreenWidth()/2, RL.GetScreenHeight()), 10, Color.Black);
        DrawSingleContainer(panel, backpack, mouseOnInventory);

        panel = GetPanel(GameSettings.ScreenWidth/2 + 50);
        if(_nearItems != null) DrawSingleContainer(panel, _nearItems, !mouseOnInventory);
        
        
        if(_dragged == -1) return;
        if(_fromContainer == null) return;

        Rectangle draggingRec = GetSlotRec(_dragged, _draggingFromPanel);
        LootItem? draggingItem = _fromContainer.GetSlot(_dragged);
        if(draggingItem == null) return;
        draggingRec.X = mousePos.X;
        draggingRec.Y = mousePos.Y;
        RL.DrawRectangleRec(draggingRec, ItemVisuals.GetColor(draggingItem.Type));
    }

    public void DrawSingleContainer(Rectangle panel, Container container, bool mouseOnSide)
    {
        RL.DrawRectangleRec(panel, new(30, 30, 30, 230));
        RL.DrawRectangleLinesEx(panel, 2, Color.White);
        int count = container == _nearItems ? 36 : container.SlotCount;
        for(int i = 0; i < count; i++)
        {
            Rectangle slotRec = GetSlotRec(i, panel);
            RL.DrawRectangleRec(slotRec, Color.DarkGray);

            if(i >= container.SlotCount) continue;
            LootItem? item = container.GetSlot(i);
            if(mouseOnSide && i == _hoveredSlot) RL.DrawRectangleLinesEx(slotRec, 2, Color.White);

            if(item == null) continue;
            Rectangle itemRec = new(slotRec.X + 8, slotRec.Y + 8, slotRec.Width - 16, slotRec.Height - 16);
            if(_fromContainer == container && i == _dragged) continue;
            RL.DrawRectangleRec(itemRec, ItemVisuals.GetColor(item.Type));
            string itemCount = $"x{item.Amount}";
            int width = RL.MeasureText(itemCount, SlotFontSize);
            RL.DrawText(itemCount, (int)(slotRec.X + slotRec.Width - width - 5), (int)(slotRec.Y + slotRec.Height - 20),
                SlotFontSize, Color.White);
            
        }
    }
}