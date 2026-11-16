using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class Hud
{
    public string? Prompt { get; set; }
    public void DrawPrompt()
    {
        if(Prompt == null) return;

        int width = RL.MeasureText(Prompt, 22);
        RL.DrawText(Prompt, (GameSettings.ScreenWidth - width) / 2, GameSettings.ScreenHeight - 140,
                    22, Color.White);
    }
    public void Draw(Flashlight flashlight, Inventory inventory)
    {
        const int x = 20, y = 20, width = 160, height = 14;

        RL.DrawRectangle(x, y, width, height, new(40, 40, 40, 255));
        Color fill = flashlight.Charge > 0.25f
            ? new(240, 220, 220, 255)
            : new(220, 70, 60, 255);
        RL.DrawRectangle(x, y, (int)(width * flashlight.Charge), height, fill);
        RL.DrawRectangleLines(x, y, width, height, Color.Gray);
        int batteries = inventory.Count(ItemType.Battery);
        RL.DrawText($"Batteries: {batteries} [R] Change [F] Light", x, y + 22, 18, Color.LightGray);
    }
}