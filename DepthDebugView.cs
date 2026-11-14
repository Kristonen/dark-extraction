using Raylib_cs;
using RL = Raylib_cs.Raylib;

public static class DepthDebugView
{
    public static void Draw(Facility facility)
    {
        TileMap map = facility.Map;
        DepthMap depth = facility.Depth;
        int max = Math.Max(1, depth.MaxDistance);

        for (int x = 0; x < map.Width; x++)
        {
            for (int y = 0; y < map.Height; y++)
            {
                int d = depth.GetDistance(x, y);
                if(d < 0) continue;
                float t = d / (float)max;
                var color = new Color((int)(255 * t), (int)(255 * (1f * t)), 60, 150);
                RL.DrawRectangle(x * TileMap.TileSize, y * TileMap.TileSize,
                TileMap.TileSize, TileMap.TileSize, color);
            }
        }
    }
}