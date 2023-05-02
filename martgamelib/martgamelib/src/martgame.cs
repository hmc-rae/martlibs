using communistOverhaul;
using martlib;
using SFML.System;

/* TODO:
 * Implement prefab generation into GameObject.cs
 * Scene loaders
 * RenderComponents need to be made
 * 
 */

namespace martgamelib
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
        private Runtimer fTime, tTime;
        private TickRunner tickRunner;
        private PrefabLibrary library;
        private bool DecoupleRender = false;

        public GameScene CurrentScene => scene;
        public InputManager Input => input;
        public Runtimer FrameTime => fTime;    //Timer that controls rendering
        public Runtimer TickTime => tTime;     //Timer that controls background ticks
        public GameWindow Window => window;
        public PrefabLibrary PrefabLib => library;

        public martgame()
        {
            generate(new WindowDetails(), new LogisticDetails(), new PathingDetails());
        }
        public martgame(WindowDetails w, LogisticDetails l)
        {
            generate(w, l, new PathingDetails());
        }
        public martgame(WindowDetails w, LogisticDetails l, PathingDetails d)
        {
            generate(w, l, d);
        }

        private void generate(WindowDetails w, LogisticDetails l, PathingDetails d)
        {
            SpriteHandler.Initialize(d.texPath, d.entPath);

            //Initialize component reader to default directory
            //It'll be Assets\\Scripts, read all .dll in there for valid
            ComponentManager.Initialize(d.libsPath);

            //Initialize prefab reader
            PrefabLib.LoadPrefabs(d.prefabPath);

            DecoupleRender = l.DecoupledRender;
            //If true, then create a tick runner and run it separately.
            //If false, then set both timers to be the same, and run on framerate exclusively.
            //Still utilizes a distributor for objects

            input = new InputManager();
            window = new GameWindow(w.Width, w.Height, w.Title, input, w.Fullscreen ? GameWindow.FULLSCREEN_STYLE : GameWindow.DEFAULT_STYLE);


            fTime = new Runtimer(1000 / l.FrameRate);

            if (DecoupleRender)
            {
                //If the game logic runs on a separate thread, give the logic thread its own timer
                tTime = new Runtimer(1000 / l.TickRate);
            }
            else
            {
                //Else, match the two timers to the same reference
                tTime = fTime;
            }

            scene = new GameScene(l.ObjectPoolSize, l.WorkerThreadCount, this);

            tickRunner = new TickRunner(scene, this);
        }

        internal bool ChangedScene = false;

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void Run()
        {
            fTime.Start();

            if (DecoupleRender)
                tickRunner.Start();

            while (window.IsOpen)
            {
                window.StartFrame();

                //If not decoupled, do all the game logic here.
                if (!DecoupleRender)
                {
                    scene.StartFrame();

                    scene.Synchronize();

                    scene.EndFrame();

                    if (scene.ChangeScene)
                    {
                        tickRunner.scene = scene.nextScene;
                        ChangedScene = true;
                    }
                }

                //Go through each item and run it's render logic
                scene.Render();

                window.EndFrame();

                //Do some EoF things like check to see if the game is trying to change scenes: start loading a new scene and reset.
                if (ChangedScene)
                {
                    ChangedScene = false;
                    scene = tickRunner.scene;
                    window.ChangeScene(scene);
                }

                fTime.Wait();
            }
            tickRunner.ContinueRunning = false;
        }

        public struct WindowDetails
        {
            public string Title;
            public uint Width, Height;
            public Vector DotsPerUnit;
            public bool Fullscreen;

            public WindowDetails()
            {
                Title = "Untitled Game";
                Width = 1024;
                Height = 1024;
                DotsPerUnit = new Vector(32, 32);
                Fullscreen = false;
            }
        }
        public struct LogisticDetails
        {
            public uint ObjectPoolSize, WorkerThreadCount;
            public long FrameRate;
            public long TickRate;
            public string DefaultScene;
            public bool DecoupledRender;

            public LogisticDetails()
            {
                ObjectPoolSize = 2048;
                WorkerThreadCount = 4;
                FrameRate = 60;
                TickRate = 20;
                DefaultScene = "";
                DecoupledRender = true;
            }
        }
        public struct PathingDetails
        {
            public string texPath, entPath, libsPath, prefabPath;
            public PathingDetails()
            {
                //The two files for reading where animations are located.
                texPath = $"{Directory.GetCurrentDirectory()}\\Assets\\YellowPages\\TextureDirectory.bin";
                entPath = $"{Directory.GetCurrentDirectory()}\\Assets\\YellowPages\\EntityAnimations.bin";

                //Library pointing to all the user-defined code
                libsPath = $"{Directory.GetCurrentDirectory()}\\Assets\\Scripts";

                //Location of all prefabs to be utilized in-game.
                prefabPath = $"{Directory.GetCurrentDirectory()}\\Assets\\Prefabs";
            }
        }
    }
}
