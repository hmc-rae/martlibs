using System;
using martgamelib;
using martlib;

using SFML.Graphics;

namespace MEngineEditor
{
    public static class Program
    {
        public const string VERSION = "0.1a";
        public static void Main(string[] args)
        {
            martgame game = new martgame(new martgame.WindowDetails()
            {
                Width = 800,
                Height = 450,
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

            RenderLayer maxlayer = new RenderLayer(new Vector(16, 9) * 25, int.MaxValue);
            game.CurrentScene.RegisterRenderLayer(maxlayer);

            maxlayer.ScaleToMainWindow();
            maxlayer.SetRelativePosition(Vector.ZERO);
            maxlayer.ClearBackground = true;
            Console.WriteLine(maxlayer.MainWindowPosition);

            GameObject obj = game.CurrentScene.Instantiate();
            CameraComponent c = obj.AddBehavior(new CameraComponent(0, int.MaxValue)) as CameraComponent;
            c.MapRegion = Vector.XY * 10;
            c.DetectRegion = Vector.XY * 12;

            obj.AddBehavior(new Comps.ButtonRenderer()
            {
                CameraID = 0,
                text = "BUTTON"
            });

            game.Run();
        }
    }
}