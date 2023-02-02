using martlib;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace martgamelib.src
{
    public class martgame
    {
        internal static Vector2f ToSFMLVector(Vector vector)
        {
            return new Vector2f((float)vector.X, (float)vector.Y);
        }

        private GameScene scene;
        private InputManager input;
        private GameWindow window;
        private Runtimer timer;

        public GameScene CurrentScene => scene;
        public InputManager Input => input;
        public Runtimer Time => timer;  
        public GameWindow Window => window;

        public martgame(WindowDetails w, LogisticDetails l)
        {
            input = new InputManager();
            window = new GameWindow(w.Width, w.Height, w.Title, input, w.Fullscreen ? GameWindow.FULLSCREEN_STYLE : GameWindow.DEFAULT_STYLE);
            timer = new Runtimer(1000 / l.FrameRate);
            scene = new GameScene(l.ObjectPoolSize, l.WorkerThreadCount, this);
        }

        public void Run()
        {
            timer.Start();
            while (window.IsOpen)
            {
                window.StartFrame();
                scene.StartFrame();
                scene.Synchronize();
                window.EndFrame();
                scene.EndFrame();
                timer.Wait();
            }
        }


        public struct WindowDetails
        {
            public string Title;
            public uint Width, Height;
            public Vector DotsPerUnit;
            public bool Fullscreen;
        }
        public struct LogisticDetails
        {
            public uint ObjectPoolSize, WorkerThreadCount;
            public long FrameRate;
        }
    }
}
