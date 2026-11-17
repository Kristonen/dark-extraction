using Raylib_cs;
using RL = Raylib_cs.Raylib;

public static class LightRenderer
{
    public static void Draw(List<LightSource> sources)
    {
        foreach(var l in sources)
        {
            RL.DrawCircleV(l.Position, 4, Color.Gold);
        }
    }
}