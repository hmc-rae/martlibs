using System;
using martlib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace martgamelib
{
    public class GameWindow : RenderWindow
    {
        internal readonly static Color BLANK_COLOR = new Color(0, 0, 0, 0);
        public const Styles DEFAULT_STYLE = Styles.Titlebar | Styles.Close;
        public const Styles FULLSCREEN_STYLE = Styles.Fullscreen;

        /// <summary>
        /// Builds the GameWindow correctly. <br></br>
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="title"></param>
        /// <param name="dotsPerUnit"></param>
        /// <param name="style"></param>
        public GameWindow(uint w, uint h, string title, InputManager manager, Styles style, Color backdrop = default) : base(new VideoMode(w, h), title, style)
        {
            if (backdrop == default)
            {
                backdrop = BLANK_COLOR;
            }
            clearColor = backdrop;

            mConvertVector = new Vector(w/2, h/2); //need this for a couple transformations applied later on.

            manager.Bind(this);
            boundManager = manager;

            renderLayers = new List<RenderLayer>();

            //new RenderTarget(w, h, new Vector(1, 1), new Vector(0, 0), 0, true, this);
        }

        internal Vector mConvertVector, mHalfVector;
        internal InputManager boundManager;
        internal List<RenderLayer> renderLayers;

        public Color clearColor;

        internal void StartFrame()
        {
            Clear(clearColor);
            DispatchEvents();
        }
        internal void EndFrame()
        {
            for (int i = 0; i < renderLayers.Count; i++)
            {
                renderLayers[i].Display(this);
            }
            Display();
        }

        internal void ChangeScene(GameScene newScene)
        {
            renderLayers = newScene.renderlayers;
        }
    }
}
