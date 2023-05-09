using System;
using SFML.Window;
using SFML.Graphics;
using martlib;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using SFML.System;

namespace martgamelib
{
    public sealed class RenderLayer
    {
        internal RenderTexture target;
        internal Sprite sprite;

        public int LayerID => layerID;
        /// <summary>
        /// The dimensions of the target in pixels.
        /// </summary>
        public Vector PixelSize
        {
            get
            {
                return pixelRadius * 2;
            }
            set
            {
                pixelRadius = value / 2;
            }
        }
        /// <summary>
        /// The radius of the target in pixels.
        /// </summary>
        public Vector PixelRadius
        {
            get
            {
                return pixelRadius;
            }
            set
            {
                pixelRadius = value;
            }
        }
        /// <summary>
        /// The rendering scale of the target to the main window.
        /// </summary>
        public Vector PixelScale
        {
            get
            {
                return pixelScale;
            }
            set
            {
                pixelScale = value;
                sprite.Scale = martgame.ToSFMLVector(pixelScale);
            }
        }
        /// <summary>
        /// The position of the target in the main window.
        /// </summary>
        public Vector MainWindowPosition
        {
            get
            {
                return mainPosition;
            }
            set
            {
                mainPosition = value;
                sprite.Position = martgame.ToSFMLVector(mainPosition);
            }
        }

        public void ScaleToMainWindow()
        {
            Vector2u size = scene.GameWindow.Size;
            pixelScale.X = size.X / PixelSize.X;
            pixelScale.Y = size.Y / PixelSize.Y;
        }

        internal GameScene scene;

        [MonSerializer.MonInclude]
        private int layerID;
        [MonSerializer.MonInclude]
        private Vector pixelRadius;
        [MonSerializer.MonInclude]
        private Vector pixelScale;
        [MonSerializer.MonInclude]
        private Vector mainPosition;

        public bool ClearBackground;

        internal void Create()
        {
            target = new RenderTexture((uint)pixelRadius.X * 2, (uint)pixelRadius.Y * 2);
            sprite = new Sprite(target.Texture);
            sprite.Origin = martgame.ToSFMLVector(pixelRadius);
            target.Display();
        }

        internal void Display(GameWindow window)
        {
            //This might not be necessary. Test to see if it is
            //mTex.Display();

            window.Draw(sprite);

            if (ClearBackground)
                target.Clear(GameWindow.BLANK_COLOR);
        }

        internal void Draw(Drawable drawable)
        {
            target.Draw(drawable);
        }
    }
}
