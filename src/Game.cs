using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class Game : IDisposable
{
    public bool CloseGame { get; private set; }
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
    private FacilityKind _facilityKind = FacilityKind.Camp;
    // private readonly TileMap _map = new(40, 22);
    private readonly Lighting _lighting = new Lighting(GameSettings.ScreenWidth, GameSettings.ScreenHeight);
    private readonly Hud _hud = new();
    private readonly TransferMenu _menu = new();

    public Game()
    {
        _facility = NewFacility();
        _player.Inventory.Backpack.Add(ItemType.Cloth, 10, 0, 0);
        _player.Inventory.Backpack.Add(ItemType.Scrap, 10, 0, 0);

        Container c = new(8);
        int x = (int)(_player.Position.X/TileMap.TileSize);
        int y = (int)(_player.Position.Y/TileMap.TileSize);
        c.Add(ItemType.Cloth, 5, x, y);
        WorldContainer wc = new(x, y, WorldContainerKind.Crate, c);
        _facility.Containers.Add(wc);
    }

    private Facility NewFacility()
    {
        int seed = Random.Shared.Next();
        if(_facilityKind == FacilityKind.Camp)
        {
            Facility camp = _generator.CreateCamp(MapWidth, MapHeight, seed);
            _player.Position = camp.StartPosition;
            return camp;
        }
        
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
                items += c.Contents.GetSlot(i)?.Amount ?? 0;

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

        WorldContainer? crate = _facility.FindNearestCrate(_player.Position, 48f);
        _hud.Prompt = null;

        if(!_inventoryOpen) _player.Update(dt, _facility.Map, mouseWorld);
        foreach(var l in _facility.LightSources) l.Update(dt, _facility.Map);

        Vector2 cameraTarget = _overview
            ? new Vector2(MapWidth * TileMap.TileSize / 2f, MapHeight * TileMap.TileSize / 2f)
            : _player.Position;
        _camera.Follow(cameraTarget, dt);

        if(crate != null && !_inventoryOpen) _hud.Prompt = "[E] - Open Crate";
        if(_facility.IsAtExtractionPos(_player.Position)) _hud.Prompt = "[E] - Extract";
        //Open crate
        if (RL.IsKeyPressed(KeyboardKey.E) && crate != null)
        {
            if(_inventoryOpen) CloseMenu();
            else OpenMenu(crate.Contents, true);
        }
        //Extract
        if(RL.IsKeyPressed(KeyboardKey.E) && _facility.IsAtExtractionPos(_player.Position))
        {
            HandleExtraction();
            // CloseGame = true;
        }
        //Open inventory
        if (RL.IsKeyPressed(KeyboardKey.Tab))
        {
            if(_inventoryOpen) CloseMenu();
            else OpenMenu(_facility.FindNearPiles(_player.Position, 48f));
        }
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

        if (_inventoryOpen)
        {
            _menu.Update(_player.Inventory.Backpack, _facility);
        }
    }

    public void Draw()
    {
        _camera.Begin();
        _facility.Map.Draw();
        Extraction.Draw(_facility.ExtractionPoint.TileX, _facility.ExtractionPoint.TileY);
        if(_showDepth) DepthDebugView.Draw(_facility);
        LootRenderer.Draw(_facility.Containers);
        LootRenderer.DrawPile(_facility.Loot);
        LightRenderer.Draw(_facility.LightSources);
        _player.Draw();
        _camera.End();

        if (_darknessEnabled)
        {
            _lighting.RenderMask(_player.Flashlight, _facility.LightSources, _camera);
            _lighting.DrawingOverlay();
        }
        _hud.DrawPrompt();
        _hud.Draw(_player.Flashlight, _player.Inventory);
        if (_inventoryOpen)
        {
            _menu.Draw(_player.Inventory.Backpack, RL.GetMousePosition());
        }
        RL.DrawFPS(GameSettings.ScreenWidth - 100, 50);
    }

    public void Dispose()
    {
        _lighting.Dispose();
    }

    private void OpenMenu(Container source, bool isCrate = false)
    {
        int tileX = (int)(_player.Position.X / TileMap.TileSize);
        int tileY = (int)(_player.Position.Y / TileMap.TileSize);
        _menu.Open(source, tileX, tileY, isCrate);
        _inventoryOpen = true;
    }

    private void CloseMenu()
    {
        _menu.Close();
        _inventoryOpen = false;
    }

    private void HandleExtraction()
    {
        if(_facilityKind == FacilityKind.Camp)
        {
            _facilityKind = FacilityKind.Storage;
        }
        else
        {
            _facilityKind = FacilityKind.Camp;
        }
        _facility = NewFacility();
        _camera.SnapTo(_player.Position);
    }
}