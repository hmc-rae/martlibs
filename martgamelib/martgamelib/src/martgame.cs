using communistOverhaul;
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
        private Runtimer timerA, timerB;
        private TickRunner tickRunner;

        public GameScene CurrentScene => scene;
        public InputManager Input => input;
        public Runtimer FrameTime => timerA;
        public Runtimer TickTime => timerB;
        public GameWindow Window => window;

        public martgame(WindowDetails w, LogisticDetails l)
        {
            SpriteHandler.Initialize($"{Directory.GetCurrentDirectory()}\\Assets\\YellowPages\\TextureDirectory.path", $"{Directory.GetCurrentDirectory()}\\Assets\\YellowPages\\EntityAnimations.path");

            //Initialize component reader to default directory
            //It'll be Assets\\Scripts, read all files in there for valid

            input = new InputManager();
            window = new GameWindow(w.Width, w.Height, w.Title, input, w.Fullscreen ? GameWindow.FULLSCREEN_STYLE : GameWindow.DEFAULT_STYLE);
            timerA = new Runtimer(1000 / l.FrameRate);
            timerB = new Runtimer(1000 / l.TickRate);
            scene = new GameScene(l.ObjectPoolSize, l.WorkerThreadCount, this);

            tickRunner = new TickRunner(scene, this);
        }
        public martgame(WindowDetails w, LogisticDetails l, string directoryPath, string entityPath, string libsPath)
        {
            SpriteHandler.Initialize(directoryPath, entityPath);

            //Initialize component reader to set directory

            input = new InputManager();
            window = new GameWindow(w.Width, w.Height, w.Title, input, w.Fullscreen ? GameWindow.FULLSCREEN_STYLE : GameWindow.DEFAULT_STYLE);
            timerA = new Runtimer(1000 / l.FrameRate);
            timerB = new Runtimer(1000 / l.TickRate);
            scene = new GameScene(l.ObjectPoolSize, l.WorkerThreadCount, this);
            window.ChangeScene(scene);

            tickRunner = new TickRunner(scene, this);
        }

        internal bool ChangedScene = false;
        public void Run()
        {
            timerA.Start();
            while (window.IsOpen)
            {
                window.StartFrame();
                window.EndFrame();

                //Do some EoF things like check to see if the game is trying to change scenes: start loading a new scene and reset.
                if (ChangedScene)
                {
                    ChangedScene = false;
                    scene = tickRunner.scene;
                    window.ChangeScene(scene);
                }

                timerA.Wait();
            }
            tickRunner.ContinueRunning = false;
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
            public long TickRate;
        }
    }
}
