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

        public RenderLayer() 
        { 
            
        }
        public RenderLayer(Vector size, int layerID)
        {
            pixelRadius = size / 2;
            this.layerID = layerID;
            PixelScale = Vector.XY;
        }

        public int LayerID => layerID;
        /// <summary>
        /// The dimensions of the target in pixels.
        /// </summary>
        public Vector PixelSize => pixelRadius * 2;
        /// <summary>
        /// The radius of the target in pixels.
        /// </summary>
        public Vector PixelRadius => pixelRadius;


        /// <summary>
        /// The rendering scale of the target to the main window.
        /// </summary>
        public Vector PixelScale;
        /// <summary>
        /// The position of the target in the main window.
        /// </summary>
        public Vector MainWindowPosition;

        /// <summary>
        /// Updates the scaling of the layer so that it appears flush with the screen.
        /// </summary>
        public void ScaleToMainWindow()
        {
            Vector2u size = scene.GameWindow.Size;
            PixelScale.X = size.X / PixelSize.X;
            PixelScale.Y = size.Y / PixelSize.Y;
        }
        /// <summary>
        /// Sets the position of the layer on the main window as a scale from (-1, -1), the bottom left corner, to the top right corner (1, 1).
        /// </summary>
        /// <param name="relative"></param>
        public void SetRelativePosition(Vector relative)
        {
            Vector size = martgame.FromSFMLVector(scene.GameWindow.Size);
            size /= 2;

            relative = relative.Flip * size;
            relative += size;

            MainWindowPosition = relative;
        }

        internal GameScene scene;

        [MonSerializer.MonInclude]
        private int layerID;
        [MonSerializer.MonInclude]
        private Vector pixelRadius;

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

            sprite.Scale = martgame.ToSFMLVector(PixelScale);
            sprite.Position = martgame.ToSFMLVector(MainWindowPosition);

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
