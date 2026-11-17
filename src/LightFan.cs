using System.Numerics;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public static class LightFan
{
    public static void CastRays(TileMap map, Vector2 origin, float radius,
                                float startAngleDeg, float angleStepDeg, float[] distances)
    {
        for(int i = 0; i < distances.Length; i++)
        {
            Vector2 dir = DirFromAngle(startAngleDeg + i * angleStepDeg);
            distances[i] = map.Raycast(origin, dir, radius);
        }
    }

    public static void DrawGradient(Vector2 origin, float[] distances, float startAngleDeg,
                                    float angleStepDeg, float radius, float intensity, Color tint)
    {
        int triangleCount = distances.Length - 1;
        if(triangleCount <= 0 || radius <= 0f) return;

        Rlgl.CheckRenderBatchLimit(triangleCount * 3);
        Rlgl.Begin(DrawMode.Triangles);

        byte centerAlpha = ToAlpha(intensity);

        for(int i = 0; i < triangleCount; i++)
        {
            float d0 = MathF.Min(distances[i], radius);
            float d1 = MathF.Min(distances[i + 1], radius);

            Vector2 p0 = origin + DirFromAngle(startAngleDeg + i * angleStepDeg) * distances[i];
            Vector2 p1 = origin + DirFromAngle(startAngleDeg + (i + 1) * angleStepDeg) * distances[i + 1];

            byte alpha0 = ToAlpha(intensity * (1f - d0 / radius));
            byte alpha1 = ToAlpha(intensity * (1f - d1 / radius));

            Rlgl.Color4ub(tint.R, tint.G, tint.B, centerAlpha);
            Rlgl.Vertex2f(origin.X, origin.Y);

            Rlgl.Color4ub(tint.R, tint.G, tint.B, alpha1);
            Rlgl.Vertex2f(p1.X, p1.Y);

            Rlgl.Color4ub(tint.R, tint.G, tint.B, alpha0);
            Rlgl.Vertex2f(p0.X, p0.Y);
        }

        Rlgl.End();
    }

    public static byte ToAlpha(float value)
    {
        return (byte)(Math.Clamp(value, 0, 1f) * 255 + 0.5f);
    }

    public static Vector2 DirFromAngle(float angleDeg)
    {
        float rad = angleDeg * (MathF.PI / 180f);
        return new Vector2(MathF.Cos(rad), MathF.Sin(rad));
    }
}