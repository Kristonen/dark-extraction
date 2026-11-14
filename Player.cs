using System.Numerics;
using RL = Raylib_cs.Raylib;
using Raylib_cs;

public class Player
{
    public Vector2 Position = new(640, 360);
    private const float Speed = 200f;
    private const float Radius = 8f;
    public Flashlight Flashlight = new();
    public Inventory Inventory = new();
    public void Update(float dt, TileMap map, Vector2 aimTarget)
    {
        var dir = Vector2.Zero;

        if(RL.IsKeyDown(KeyboardKey.W)) dir.Y -= 1;
        if(RL.IsKeyDown(KeyboardKey.A)) dir.X -= 1;
        if(RL.IsKeyDown(KeyboardKey.S)) dir.Y += 1;
        if(RL.IsKeyDown(KeyboardKey.D)) dir.X += 1;
        HandleFlashlightInput();
        Flashlight.Update(dt, Position, aimTarget, map);

        if(dir == Vector2.Zero) return;
        
        dir = Vector2.Normalize(dir);
        Vector2 move = dir * Speed * dt;
        TryMove(move, map);
    }

    public void Draw()
    {
        RL.DrawCircleV(Position, Radius, Color.White);
    }

    private void TryMove(Vector2 delta, TileMap map)
    {
        Vector2 newPos = Position + delta;

        Rectangle hitBox = new(newPos.X - Radius, newPos.Y - Radius, Radius * 2, Radius * 2);

        if(!map.IsBlocked(hitBox)) Position = newPos;
    }

    private void HandleFlashlightInput()
    {
        if(RL.IsKeyPressed(KeyboardKey.F)) Flashlight.Toggle();
        if(RL.IsKeyPressed(KeyboardKey.R) && Flashlight.CanInsertBattery)
        {
            Flashlight.InsertBattery();
            // if (Inventory.TryRemove(ItemType.Battery))
            // {
            //     Flashlight.InsertBattery();
            // }
        }
    }
}