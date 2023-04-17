using System;
using martlib;
using SFML.Graphics;
using SFML.Window;
using martgamelib.src;
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

            renderTargets = new List<RenderTarget>();

            //new RenderTarget(w, h, new Vector(1, 1), new Vector(0, 0), 0, true, this);
        }

        internal Vector mConvertVector, mHalfVector;
        internal InputManager boundManager;
        internal List<RenderTarget> renderTargets;

        public Color clearColor;

        internal void StartFrame()
        {
            DispatchEvents();
            Clear(clearColor);
        }
        internal void EndFrame()
        {
            for (int i = 0; i < renderTargets.Count; i++)
            {
                renderTargets[i].Display(this);
            }
            Display();
        }

        /// <summary>
        /// Returns true if successfully bound, false otherwise.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        internal bool BindToLayer(int l, CameraComponent cam)
        {

            for (int i = 0; i < renderTargets.Count; i++)
            {
                if (renderTargets[i].mLayer == l)
                {
                    cam.target = renderTargets[i];
                    return true;
                }
            }

            return false;
        }
        internal void CreateLayer(int l)
        {

        }
        internal void BindTarget(RenderTarget target)
        {
            for (int i = 0; i < renderTargets.Count; i++)
            {
                if (renderTargets[i].mLayer < target.mLayer) continue;
                if (renderTargets[i].mLayer == target.mLayer) throw new LayerException();
                if (renderTargets[i].mLayer > target.mLayer)
                {
                    renderTargets.Insert(i, renderTargets[i]);
                    return;
                }
            }
        }

        internal void ChangeScene(GameScene newScene)
        {
            renderTargets = newScene.renderTargets;
        }
    }

    internal class RenderTarget
    {
        internal Sprite sprite;
        internal RenderTexture mTex;
        internal int mLayer;
        internal bool clear;

        internal Vector position;
        internal Vector2f spriteScale;
        /// <summary>
        /// Generates a new RenderTarget layer. 
        /// </summary>
        /// <param name="w">The width of the layer in pixels</param>
        /// <param name="h">The height of the layer in pixels</param>
        /// <param name="unitRadius">The pixels per unit render ratio</param>
        /// <param name="pos">A vector ranging from <-1.0, -1.0> to <1.0, 1.0></param> 
        /// <param name="l"></param>
        /// <param name="c"></param>
        public RenderTarget(uint w, uint h, Vector scale, Vector pos, int l, bool c, GameWindow parent)
        {
            position = pos;
            spriteScale = martgame.ToSFMLVector(scale);

            mTex = new RenderTexture(w, h);
            sprite = new Sprite(mTex.Texture);
            sprite.Origin = new Vector2f(w / 2, h / 2);
            sprite.Position = martgame.ToSFMLVector((parent.mConvertVector * pos) + parent.mConvertVector);

            mLayer = l;
            clear = c;

            mTex.Display();
        }

        public void Display(GameWindow window)
        {
            //This might not be necessary. Test to see if it is
            //mTex.Display();

            window.Draw(sprite);
            if (clear)
                mTex.Clear(GameWindow.BLANK_COLOR);
        }
    }
    internal class LayerException : Exception
    {

    }
}
