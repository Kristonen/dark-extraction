using Raylib_cs;
using RL = Raylib_cs.Raylib;

public static class LootRenderer
{
    public static void Draw(List<WorldContainer> containers)
    {
        foreach(WorldContainer c in containers)
        {
            int cx = (int)((c.TileX + 0.5f) * TileMap.TileSize);
            int cy = (int)((c.TileY + 0.5f) * TileMap.TileSize);

            if(c.Kind == WorldContainerKind.Pile)
            {
                DrawPile(c, cx, cy);
            }
            else
            {
                DrawCrate(c, cx, cy);
            }
        }
    }

    public static void DrawCrate(WorldContainer c, int cx, int cy)
    {
        Color fill = c.Contents.IsEmpty ? new Color(70, 50, 35, 255) : new Color(150, 100, 55, 255);
        RL.DrawRectangle(cx - 12, cy - 12, 24, 24, Color.Black);
        RL.DrawRectangle(cx - 10, cy - 10, 20, 20, fill);
    }

    public static void DrawPile(WorldContainer c, int cx, int cy)
    {
        int shown = 0;
        for(int i = 0; i < c.Contents.SlotCount && shown < 3; i++)
        {
            ItemStack? stack = c.Contents.GetSlot(i);
            if(stack == null) continue;

            int x = cx - 14 + shown * 10;
            RL.DrawRectangle(x - 1, cy - 5, 10, 10, Color.Black);
            RL.DrawRectangle(x, cy - 4, 8, 8, ItemVisuals.GetColor(stack.Type));
            shown++;
        }
    }
}