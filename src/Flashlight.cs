// === Datei: Flashlight.cs ===
using System.Numerics;
using Raylib_cs;

public class Flashlight
{
    public Vector2 Position;
    public float AngleDeg;

    // --- Einstellbare Werte (nichts mehr fest im Zeichencode) ---
    public float Range = 120f;             // Reichweite des Kegels bei voller Batterie
    public float ConeHalfAngleDeg = 28f;   // halber Öffnungswinkel
    public float ConeIntensity = 1f;       // Helligkeit des Kegels bei voller Batterie, 0.0 bis 1.0
    public float GlowRadius = 50f;         // Radius des kleinen Lichtkreises um den Spieler
    public float GlowIntensity = 0.5f;     // Helligkeit dieses Kreises
    public Color Tint = new(255, 245, 220, 255);   // Lichtfarbe

    // --- Zustand ---
    public bool IsOn = true;
    public float Charge = 1f;
    public float BatteryLifeSeconds = 60f;

    public bool IsLit => IsOn && Charge > 0f;
    public bool CanInsertBattery => Charge < 1f;

    // 1.0 bis 25 % Ladung, darunter fällt er auf 0
    private float PowerFactor => MathF.Min(1f, Charge / 0.25f);

    // Schwache Batterie: Der Kegel wird kürzer UND dunkler
    private float CurrentRange => Range * (0.4f + 0.6f * PowerFactor);
    private float CurrentIntensity => ConeIntensity * (0.4f + 0.6f * PowerFactor);

    // Der Winkelbereich des Kegels. Update und Zeichnen benutzen beide diese zwei Werte,
    // damit die Strahlen und das Gezeichnete immer genau zusammenpassen.
    private float ConeStart => AngleDeg - ConeHalfAngleDeg;
    private float ConeStep => 2f * ConeHalfAngleDeg / (ConeRays - 1);

    private const int ConeRays = 100;
    private const int GlowRays = 120;
    private readonly float[] _coneDist = new float[ConeRays];
    private readonly float[] _glowDist = new float[GlowRays + 1];   // +1: letzter Strahl = 360 Grad

    public void Toggle() => IsOn = !IsOn;
    public void InsertBattery() => Charge = 1f;
    public void Update(float dt, Vector2 playerPos, Vector2 aimTarget, TileMap map)
    {
        if (IsLit) Charge = MathF.Max(0f, Charge - dt / BatteryLifeSeconds);

        Position = playerPos;

        Vector2 toTarget = aimTarget - Position;
        AngleDeg = MathF.Atan2(toTarget.Y, toTarget.X) * (180f / MathF.PI);

        // Der kleine Lichtkreis gilt IMMER, auch bei leerer Batterie oder ausgeschalteter
        // Lampe. Seine Strahlen müssen darum auch immer aktuell sein, sonst scheint er
        // mit alten Strahlen durch Wände. Reichweite: nur so weit, wie der Kreis groß ist.
        LightFan.CastRays(map, Position, GlowRadius, 0f, 360f / GlowRays, _glowDist);

        // Ohne Licht braucht der Kegel keine Strahlen
        if (!IsLit) return;

        // Strahlen bis CurrentRange: genau der Radius, mit dem der Kegel gezeichnet wird
        LightFan.CastRays(map, Position, CurrentRange, ConeStart, ConeStep, _coneDist);
    }

    public void DrawIntoMask()
    {
        // 1) Lichtkreis um den Spieler (immer)
        LightFan.DrawGradient(Position, _glowDist, 0f, 360f / GlowRays,
                              GlowRadius, GlowIntensity, Tint);

        if (!IsLit) return;

        // 2) Kegel: ein einziger Fächer mit stufenlosem Verlauf statt 8 Schichten
        LightFan.DrawGradient(Position, _coneDist, ConeStart, ConeStep,
                              CurrentRange, CurrentIntensity, Tint);
    }
}