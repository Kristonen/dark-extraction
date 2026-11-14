using RL = Raylib_cs.Raylib;
using Raylib_cs;

RL.InitWindow(GameSettings.ScreenWidth, GameSettings.ScreenHeight, "Extraction Game");
RL.SetTargetFPS(60);

// Game game = new();
using(Game game = new())
{
    while (!RL.WindowShouldClose())
    {
        float dt = RL.GetFrameTime();

        game.Update(dt);

        RL.BeginDrawing();
        RL.ClearBackground(Color.Black);
        game.Draw();
        RL.EndDrawing();
    }
}
RL.CloseWindow();