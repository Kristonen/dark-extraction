using Raylib_cs;
using RL = Raylib_cs.Raylib;
public readonly record struct ExtractionPoint(int TileX, int TileY);

public static class Extraction
{
    public static void Draw(int tileX, int tileY)
    {
        int x = tileX * TileMap.TileSize + 16;
        int y = tileY * TileMap.TileSize + 16;

        RL.DrawCircle(x, y, 8, Color.Blue);
    }
}