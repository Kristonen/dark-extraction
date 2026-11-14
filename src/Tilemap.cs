using System.Numerics;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public enum Tile
{
    Floor, Wall,
}

public class TileMap
{
    public const int TileSize = 32;

    public int Width { get; }
    public int Height { get; }

    private readonly Tile[,] _tiles;

    public TileMap(int width, int height)
    {
        Width = width;
        Height = height;
        _tiles = new Tile[Width, Height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                _tiles[x, y] = Tile.Wall;
            }
        }
    }

    public void SetTile(int x, int y, Tile tile)
    {
        if(x < 0 || y < 0 || x >= Width || y >= Height) return;
        _tiles[x, y] = tile;
    }

    public bool IsWall(int tx, int ty)
    {
        if(tx < 0 || ty < 0 || tx >= Width || ty >= Height) return true;
        return _tiles[tx, ty] == Tile.Wall;
    }

    public bool IsBlocked(Rectangle area)
    {
        int left = (int)MathF.Floor(area.X / TileSize);
        int top = (int)MathF.Floor(area.Y / TileSize);

        int right = (int)MathF.Floor((area.X + area.Width - 0.01f) / TileSize);
        int bottom = (int)MathF.Floor((area.Y + area.Height - 0.01f) / TileSize);

        for(int x = left; x <= right; x++)
        {
            for(int y = top; y <= bottom; y++)
            {
                if(IsWall(x,y)) return true;
            }
        }
        return false;
    }

    public void Draw()
    {
        for(int x = 0; x < Width; x++)
        {
            for(int y = 0; y < Height; y++)
            {
                int px = x * TileSize;
                int py = y * TileSize;

                Color color = _tiles[x, y] == Tile.Wall
                    ? new(90, 90, 100, 255)
                    : new(30, 30, 35, 255);
                
                RL.DrawRectangle(px, py, TileSize, TileSize, color);
            }
        }
    }

    public float Raycast(Vector2 origin, Vector2 dir, float maxDistance)
    {
        const float step = 2f;
        for(float d = 0; d < maxDistance; d += step)
        {
            Vector2 p = origin + dir * d;
            int tx = (int)MathF.Floor(p.X / TileSize);
            int ty = (int)MathF.Floor(p.Y / TileSize);

            if(IsWall(tx, ty)) return d;
        }

        return maxDistance;
    }
}