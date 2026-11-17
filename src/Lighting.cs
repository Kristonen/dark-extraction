using System.Numerics;
using Raylib_cs;
using RL = Raylib_cs.Raylib;

public class Lighting : IDisposable
{
    private readonly RenderTexture2D _mask;
    private readonly int _width;
    private readonly int _height;
    private readonly Color _ambient = new(6, 6, 10, 255);

    public Lighting(int width, int height)
    {
        _width = width;
        _height = height;
        _mask = RL.LoadRenderTexture(width, height);
    }

    public void RenderMask(Flashlight flashlight, List<LightSource> lights, GameCamera camera)
    {
        RL.BeginTextureMode(_mask);
        RL.ClearBackground(_ambient);
        camera.Begin();
        RL.BeginBlendMode(BlendMode.Additive);

        Vector2 topLeft = camera.ScreenToWorld(Vector2.Zero);
        Vector2 bottomRight = camera.ScreenToWorld(new Vector2(GameSettings.ScreenWidth, GameSettings.ScreenHeight));
        Rectangle view = new(topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
        foreach(LightSource light in lights)
        {
            light.DrawIntoMask(view);
        }

        flashlight.DrawIntoMask();
        RL.EndBlendMode();
        camera.End();
        RL.EndTextureMode();
    }

    public void DrawingOverlay()
    {
        RL.BeginBlendMode(BlendMode.Multiplied);
        RL.DrawTextureRec(
            _mask.Texture, new(0, 0, _width, -_height),
            Vector2.Zero, Color.White
        );
        RL.EndBlendMode();
    }

    public void Dispose()
    {
        RL.UnloadRenderTexture(_mask);
    }
}