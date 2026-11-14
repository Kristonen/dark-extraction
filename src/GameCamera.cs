using System.Numerics;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class GameCamera
{
    private Camera2D _camera;
    private Vector2 _position;
    public float Zoom
    {
        get => _camera.Zoom;
        set => _camera.Zoom = value;
    }

    public GameCamera()
    {
        _camera = new()
        {
            Offset = new Vector2(GameSettings.ScreenWidth / 2f, GameSettings.ScreenHeight / 2f),
            Zoom = 2f,
        };
    }

    public void Follow(Vector2 target, float dt)
    {
        float t = 1f - MathF.Exp(-10f * dt);
        _position = Vector2.Lerp(_position, target, t);
        ApplyPosition();
    }

    public void SnapTo(Vector2 target)
    {
        _position = target;
        ApplyPosition();
    }

    private void ApplyPosition()
    {
        _camera.Target = new Vector2(MathF.Round(_position.X), MathF.Round(_position.Y));
    }

    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return RL.GetScreenToWorld2D(screenPos, _camera);
    }

    public void Begin() => RL.BeginMode2D(_camera);
    public void End() => RL.EndMode2D();
}