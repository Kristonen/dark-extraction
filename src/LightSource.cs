using System.Numerics;
using RL = Raylib_cs.Raylib;
using Raylib_cs;

public class LightSource
{
    public int MaxLayer { get; }
    public float GlowRadius { get; }
    public int GlowRays { get; }
    public float Range { get; } = 320f;
    public int TileX { get; }
    public int TileY { get; }
    public Vector2 Position { get; }
    private readonly float[] _glowDist;

    public LightSource(int maxLayer, float glowRadius, int glowRays, int tileX, int tileY)
    {
        MaxLayer = maxLayer;
        GlowRadius = glowRadius;
        GlowRays = glowRays;
        TileX = tileX;
        TileY = tileY;
        Position = new(tileX * TileMap.TileSize + 16, tileY * TileMap.TileSize + 16);
        _glowDist = new float[glowRays + 1];
    }

    public void Update(float dt, TileMap map)
    {
        float glowStep = 360f / GlowRays;
        for(int i = 0; i <= GlowRays; i++)
        {
            Vector2 dir = DirFromAngle(i * glowStep);
            _glowDist[i] = map.Raycast(Position, dir, Range);
        }
    }

    public void DrawIntoMask()
    {
        for(int layer = 0; layer < MaxLayer; layer++)
        {
            float radius = GlowRadius * (1f - layer / 6f);
            DrawFan(_glowDist, 0f, 360f / GlowRays, 0, GlowRays, radius, new(255, 245, 220, 20));
        }
    }

    private void DrawFan(float[] dist, float startAngle, float angleStep, int from, int to, float maxLen, Color color)
    {
        for(int i = from; i < to; i++)
        {
            float a0 = startAngle + i * angleStep;
            float a1 = startAngle + (i + 1) * angleStep;

            Vector2 p0 = Position + DirFromAngle(a0) * MathF.Min(dist[i], maxLen);
            Vector2 p1 = Position + DirFromAngle(a1) * MathF.Min(dist[i + 1], maxLen);

            RL.DrawTriangle(Position, p1, p0, color);
        }
    }

    private Vector2 DirFromAngle(float angleDeg)
    {
        float rad = angleDeg * (MathF.PI / 180f);
        return new(MathF.Cos(rad), MathF.Sin(rad));
    }
}