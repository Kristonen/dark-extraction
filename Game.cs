using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class Game : IDisposable
{
    private readonly Player _player = new();
    private readonly GameCamera _camera = new();
    private const int MapWidth = 300;
    private const int MapHeight = 160;
    private bool _darknessEnabled;
    private bool _overview = false;
    private bool _showDepth = false;
    private bool _inventoryOpen = false;
    private readonly FacilityGenerator _generator = new(RoomLibary.CreateAll());
    private Facility _facility;
    // private readonly TileMap _map = new(40, 22);
    private readonly Lighting _lighting = new Lighting(GameSettings.ScreenWidth, GameSettings.ScreenHeight);
    private readonly Hud _hud = new();
    private readonly WorldContainer emptyContainer;

    public Game()
    {
        _facility = NewFacility();
        emptyContainer = new(0, 0, WorldContainerKind.Pile, new(20));
        

        var a = new Container(2);
        a.Add(ItemType.Cloth, 15);
        var b = new Container(1);

        Console.WriteLine(ItemTransfer.MoveStack(a, 0, b, 10));
        Console.WriteLine(ItemTransfer.MoveStack(a, 1, b, 5));
        var c2 = new Container(1, 1.5f);
        Console.WriteLine(ItemTransfer.MoveStack(a, 1, c2, 5));
        Console.WriteLine(a.Count(ItemType.Cloth));

        var src = new Container(1); src.Add(ItemType.Cloth, 10);
        var dst = new Container(1); dst.Add(ItemType.Cloth, 6);
        Console.WriteLine(ItemTransfer.MoveStack(src, 0, dst, 10));
        Console.WriteLine(src.Count(ItemType.Cloth));

        int cloth = _player.Inventory.Backpack.Add(ItemType.Cloth, 10);
        Console.WriteLine($"Cloth: {cloth}");
    }

    private Facility NewFacility()
    {
        int seed = Random.Shared.Next();
        Facility facility = _generator.Generate(MapWidth, MapHeight, seed);
        _player.Position = facility.StartPosition;
        _camera.SnapTo(facility.StartPosition);
        int[] perThird = new int[3];
        int totalItems = 0;
        foreach (WorldContainer c in facility.Containers)
        {
            float d = facility.Depth.GetDepthOne(c.TileX, c.TileY);
            if (d < 0) continue;

            int items = 0;
            for (int i = 0; i < c.Contents.SlotCount; i++)
                items += c.Contents.GetSlot(i)?.Count ?? 0;

            perThird[Math.Min(2, (int)(d * 3))] += items;
            totalItems += items;
        }
        Console.WriteLine($"Behälter: {facility.Containers.Count}, Gegenstände: {totalItems} " +
                        $"(Tiefe 0-33%: {perThird[0]}, 33-66%: {perThird[1]}, 66-100%: {perThird[2]})");
        return facility;
    }

    public void Update(float dt)
    {
        Vector2 mouseWorld = _camera.ScreenToWorld(RL.GetMousePosition());
        if(!_inventoryOpen) _player.Update(dt, _facility.Map, mouseWorld);
        Vector2 cameraTarget = _overview
            ? new Vector2(MapWidth * TileMap.TileSize / 2f, MapHeight * TileMap.TileSize / 2f)
            : _player.Position;
        _camera.Follow(cameraTarget, dt);

        if(RL.IsKeyPressed(KeyboardKey.E)) HandleInteract();
        if(RL.IsKeyPressed(KeyboardKey.Tab)) _inventoryOpen = !_inventoryOpen;
        //DEBUG
        if (RL.IsKeyPressed(KeyboardKey.G))
        {
            _facility = NewFacility();
        }
        if (RL.IsKeyPressed(KeyboardKey.F1))
        {
            _darknessEnabled = !_darknessEnabled;
        }
        if (RL.IsKeyPressed(KeyboardKey.F2))
        {
            _overview = !_overview;
            _camera.Zoom = _overview ? 0.12f : 2f;
        }
        if (RL.IsKeyPressed(KeyboardKey.F3))
        {
            _showDepth = !_showDepth;
        }

        if(RL.IsKeyPressed(KeyboardKey.T)) DebugTakeAll();
        if(RL.IsKeyPressed(KeyboardKey.Q)) DebugDropFirst();

        if (_inventoryOpen)
        {
            WorldContainer? worldContainer = _facility.FindNearestContainer(_player.Position, 48f);
            worldContainer = worldContainer == null ? emptyContainer : worldContainer;
            int tileX = (int)(_player.Position.X / TileMap.TileSize);
            int tileY = (int)(_player.Position.Y / TileMap.TileSize);
            TransferMenu.Update(_player.Inventory.Backpack, worldContainer,
            tileX, tileY, _facility);
        }
    }

    public void Draw()
    {
        _camera.Begin();
        _facility.Map.Draw();
        if(_showDepth) DepthDebugView.Draw(_facility);
        LootRenderer.Draw(_facility.Containers);
        _player.Draw();
        _camera.End();

        if (_darknessEnabled)
        {
            _lighting.RenderMask(_player.Flashlight, _camera);
            _lighting.DrawingOverlay();
        }
        _hud.Draw(_player.Flashlight, _player.Inventory);
        if (_inventoryOpen)
        {
            WorldContainer? worldContainer = _facility.FindNearestContainer(_player.Position, 48f);
            worldContainer = worldContainer == null ? emptyContainer : worldContainer;
            TransferMenu.Draw(_player.Inventory.Backpack, worldContainer.Contents, RL.GetMousePosition());
        }
        RL.DrawFPS(GameSettings.ScreenWidth - 100, 50);
    }

    public void Dispose()
    {
        _lighting.Dispose();
    }

    private void HandleInteract()
    {
        WorldContainer? target = _facility.FindNearestContainer(_player.Position, 48f);
        if(target == null)
        {
            Console.WriteLine("Nothing near");
            return;
        }
        string kind = target.Kind == WorldContainerKind.Pile ? "Pile" : "Crate";
        DebugPrint(kind, target.Contents);
    }

    private void DebugTakeAll()
    {
        WorldContainer? target = _facility.FindNearestContainer(_player.Position, 48f);
        if(target == null)
        {
            Console.WriteLine("Nothing in the near");
            return;
        }

        for(int i = 0; i < target.Contents.SlotCount; i++)
        {
            ItemTransfer.TakeFromWorld(target, i, int.MaxValue, _player.Inventory.Backpack, _facility);
        }
        DebugPrint("Backpack", _player.Inventory.Backpack);
    }

    private void DebugDropFirst()
    {
        Container backpack = _player.Inventory.Backpack;
        for(int i = 0; i < backpack.SlotCount; i++)
        {
            if(backpack.GetSlot(i) == null) continue;
            int tileX = (int)(_player.Position.X / TileMap.TileSize);
            int tileY = (int)(_player.Position.Y / TileMap.TileSize);

            int moved = ItemTransfer.DropGround(backpack, i, int.MaxValue, tileX, tileY, _facility);
            Console.WriteLine($"{moved} being dropped");
            DebugPrint("BackPack", backpack);
            return;
        }
        Console.WriteLine("Backpack is empty");
    }

    private void DebugPrint(string title, Container container)
    {
        Console.WriteLine($"--- {title} ---");

        bool any = false;
        for(int i = 0; i < container.SlotCount; i++)
        {
            ItemStack? stack = container.GetSlot(i);
            if(stack == null) continue;
            Console.WriteLine($"    Place {i}: {stack.Count}x {ItemDataBase.Get(stack.Type).Name}");
            any = true;
        }
        if(!any) Console.WriteLine("    (empty)");
    }
}