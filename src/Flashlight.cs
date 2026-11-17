using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class Flashlight
{
    public Vector2 Position;
    public float AngleDeg;
    public float Range = 320f;
    public float ConeHalfAngleDeg = 28f;

    public bool IsOn = true;
    public float Charge = 1f;
    public float BaterryLifeSeconds = 60f;

    public bool IsLit => IsOn && Charge > 0f;
    public bool CanInsertBattery => Charge < 1f;
    private float PowerFactor => MathF.Min(1f, Charge / 0.25f);
    private float CurrentRange => Range * (0.4f + 0.6f * PowerFactor);
    private const int ConeRays = 100;
    private readonly float[] _coneDist = new float[ConeRays];
    private const int GlowRays = 120;
    private const float GlowRadius = 50f;
    private readonly float[] _glowDist = new float[GlowRays + 1];
    public void Toggle() => IsOn = !IsOn;
    public void InsertBattery() => Charge = 1f;

    public void Update(float dt, Vector2 playerPos, Vector2 aimTarget, TileMap map)
    {
        if(IsLit) Charge = MathF.Max(0f, Charge - dt / BaterryLifeSeconds);

        Position = playerPos;

        Vector2 toTarget = aimTarget - Position;
        AngleDeg = MathF.Atan2(toTarget.Y, toTarget.X) * (180f/MathF.PI);

        float coneStart = AngleDeg - ConeHalfAngleDeg;
        float coneStep = 2f * ConeHalfAngleDeg / (ConeRays - 1);
        for(int i = 0; i < ConeRays; i++)
        {
            Vector2 dir = DirFromAngle(coneStart + i * coneStep);
            _coneDist[i] = map.Raycast(Position, dir, Range);
        }

        if(!IsLit) return;

        float glowStep = 360f / GlowRays;
        for(int i = 0; i <= GlowRays; i++)
        {
            Vector2 dir = DirFromAngle(i * glowStep);
            _glowDist[i] = map.Raycast(Position, dir, Range);
        }
    }

    public void DrawIntoMask()
    {
        // 1) Lichtkreis um den Spieler: 6 Schichten, von groß/schwach bis klein/stark
        for (int layer = 0; layer < 6; layer++)
        {
            float radius = GlowRadius * (1f - layer / 6f);
            DrawFan(_glowDist, 0f, 360f / GlowRays, 0, GlowRays,
                    radius, new Color(255, 245, 220, 20));
        }

        if(!IsLit) return;

        // 2) Lichtkegel: 8 Schichten, wie vorher.
        //    Jede Schicht ist kürzer UND schmaler als die davor.
        float range = CurrentRange;
        float coneStart = AngleDeg - ConeHalfAngleDeg;
        float coneStep = 2f * ConeHalfAngleDeg / (ConeRays - 1);
        float middle = (ConeRays - 1) / 2f; // Index des mittleren Strahls

        const int layers = 8;
        for (int layer = 0; layer < layers; layer++)
        {
            float t = layer / (float)(layers - 1);   // 0.0 bis 1.0
            float radius = range * (1f - 0.55f * t); // wird kürzer
            float keep = 1f - 0.45f * t;             // Anteil des Kegels, der übrig bleibt

            // Nur die Strahlen nehmen, die um die Mitte herum liegen
            int from = (int)MathF.Ceiling(middle - middle * keep);
            int to = (int)MathF.Floor(middle + middle * keep);

            DrawFan(_coneDist, coneStart, coneStep, from, to,
                    radius, new Color(255, 245, 220, 38));
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