using System;
using martlib;
using SFML.Graphics;
using SFML.Window;

namespace martgamelib
{
    public class GameWindow : RenderWindow
    {
        public const Styles DEFAULT_STYLE = Styles.Titlebar | Styles.Close;
        public const Styles FULLSCREEN_STYLE = Styles.Fullscreen;

        /// <summary>
        /// Builds the GameWindow correctly.
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="title"></param>
        /// <param name="dotsPerUnit"></param>
        /// <param name="style"></param>
        public GameWindow(uint w, uint h, string title, Vector dotsPerUnit, InputManager manager, Styles style) : base(new VideoMode(w, h), title, style)
        {
            mConvertVector = new Vector(w/2, h/2); //need this for a couple transformations applied later on.

            manager.Bind(this);
            boundManager = manager;


        }

        private Vector mConvertVector;
        private InputManager boundManager;
    }
}
