using System.Numerics;
using RL = Raylib_cs.Raylib;
using Raylib_cs;

public class LightSource
{
    //NEU
    private float _radius;
    private int _rayCount;
    private bool _dirty = true;
    private float[] _distances = Array.Empty<float>();
    public int TileX { get; }
    public int TileY { get; }
    public Vector2 Position { get; }
    public float Radius
    {
        get => _radius;
        set { _radius = value; _dirty = true; }
    }
    public int RayCount
    {
        get => _rayCount;
        set { _rayCount = Math.Max(8, value); _dirty = true; }
    }

    public float Intensity { get; set; }
    public Color Tint { get; set; } = new(255, 245, 220, 255);

    public LightSource(int tileX, int tileY, float radius = 160f, float intensity = 1f, int rayCount = 64)
    {
        TileX = tileX;
        TileY = tileY;
        Position = new Vector2((tileX + 0.5f) * TileMap.TileSize, (tileY + 0.5f) * TileMap.TileSize);

        Radius = radius;
        Intensity = intensity;
        RayCount = rayCount;
    }

    public void Update(float dt, TileMap map)
    {
        if(!_dirty) return;

        _distances = new float[_rayCount + 1];
        LightFan.CastRays(map, Position, _radius, 0f, 360f / _rayCount, _distances);
        _dirty = false;
    }

    public bool IsVisible(Rectangle view)
    {
        return Position.X + _radius >= view.X && Position.X - _radius <= view.X + view.Width
            && Position.Y + _radius >= view.Y && Position.Y - _radius <= view.Y + view.Height;
    }
    public void DrawIntoMask(Rectangle view)
    {
        if(_distances.Length == 0 || !IsVisible(view)) return;
        LightFan.DrawGradient(Position, _distances, 0f, 360f / _rayCount, _radius, Intensity, Tint);
    }
}