using System;
using martgamelib;
using martlib;

using SFML.Graphics;

namespace MEngineEditor
{
    public static class Program
    {
        public const string VERSION = "0.1a";
        private static uint renderscale = 125;
        public static void Main(string[] args)
        {
            martgame game = new martgame(new martgame.WindowDetails()
            {
                Width = 16*renderscale,
                Height = 9*renderscale,
                Title = $"MEngineEditor v{VERSION}"
            },
            new martgame.LogisticDetails()
            {
                FrameRate = 60,
                TickRate = 60,
                DecoupledRender = false,
                ObjectPoolSize = 4096,
                WorkerThreadCount = 8
            });

            RenderLayer maxlayer = new RenderLayer(new Vector(16, 9)*85, int.MaxValue);
            game.CurrentScene.RegisterRenderLayer(maxlayer);

            maxlayer.ScaleToMainWindow();
            maxlayer.SetRelativePosition(Vector.ZERO);
            maxlayer.ClearBackground = true;

            GameObject obj = game.CurrentScene.Instantiate();
            CameraComponent? c = obj.AddBehavior(new CameraComponent(0, int.MaxValue)) as CameraComponent;

            c.EnqueueFunction((BehaviorComponent obj) =>
            {
                c.PixelsPerUnit = Vector.XY * 16;
                return 0;
            });

            obj = game.CurrentScene.Instantiate();

            obj.AddBehavior(new FramerateCounter()
            {
                CameraID = 0,
                Color = Color.White,
                FontPath = $"{Directory.GetCurrentDirectory()}\\times.ttf"
            });

            game.Run();
        }
    }
}