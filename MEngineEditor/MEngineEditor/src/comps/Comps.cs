using System;
using martgamelib;
using martlib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace MEngineEditor
{
    public class ButtonRenderer : RenderComponent
    {
        private bool pressed = false;

        private Color raised;
        private Color lowered;

        public Vector Bounds;
        public string text;
        public uint characterSize;

        private static RectangleShape _shape = new RectangleShape(new Vector2f(1, 1));
        private Text _text;
        private static Font ttf = new Font($"{Directory.GetCurrentDirectory()}\\times.ttf");

        private Vector compressScale;
        public void CalculateBounds()
        {
            compressScale = Vector.XY;
            if (!CanRender()) return;

            FloatRect rect = _text.GetLocalBounds();
            Vector size = new Vector(rect.Left + rect.Width, rect.Top + rect.Height);
            Vector BoundSize = Bounds * RenderCamera.PixelsPerUnit;

            compressScale = BoundSize / size;
        }
        public void SetNaturalBounds()
        {
            compressScale = Vector.XY;
            if (!CanRender()) return;

            FloatRect rect = _text.GetLocalBounds();
            Vector size = new Vector(rect.Left + rect.Width, rect.Top + rect.Height);

            Bounds = size / RenderCamera.PixelsPerUnit;
        }
        public override void OnCreate()
        {
            base.OnCreate();

            _shape.Origin = new Vector2f(0.5f, 0.5f);
            raised = new Color(44, 148, 55);
            lowered = new Color(16, 56, 20);

            _text = new Text(text, ttf);
            _text.CharacterSize = characterSize;
            pressed = false;

            CalculateBounds();
        }
        public override void OnTick()
        {
            if (!CanRender()) return;

            Vector mpos = RenderCamera.GetRealMousePosition(Input.MousePosition);
            Vector dif = mpos - Parent.Transform.Position;
            dif = dif.Absolute;

            if (pressed)
            {
                if (!Input.MouseHeldBool(Mouse.Button.Left) || dif >= Parent.Transform.Scale * Bounds)
                {
                    pressed = false;
                }
            }
            else if (!pressed)
            {
                if (Input.MouseClickedBool(Mouse.Button.Left) && dif < Parent.Transform.Scale * Bounds)
                {
                    pressed = true;
                }
            }
        }
        public override void Render()
        {
            base.Render();

            if (!CanRender()) return;

            Vector relative = RenderCamera.GetRelativePosition(Parent);
            if (!RenderCamera.IsVisible(relative)) return;

            relative = RenderCamera.GetMappedPosition(relative);

            _text.DisplayedString = text;
            _text.CharacterSize = characterSize;
            _text.FillColor = Color.White;

            CalculateBounds();

            FloatRect b = _text.GetLocalBounds();

            _shape.Scale = martgame.ToSFMLVector(Bounds * RenderCamera.PixelsPerUnit * Parent.Transform.Scale);
            _shape.Position = martgame.ToSFMLVector(relative);
            _shape.Rotation = (float)(Parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);
            if (pressed)
                _shape.FillColor = lowered;
            else
                _shape.FillColor = raised;

            _text.Origin = new Vector2f(b.Left + (b.Width / 2), b.Top + (b.Height / 2));
            _text.Scale = martgame.ToSFMLVector(Parent.Transform.Scale * compressScale);
            _text.Rotation = (float)(Parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);
            _text.Position = martgame.ToSFMLVector(relative);

            RenderCamera.Render(_shape);
            RenderCamera.Render(_text);
        }
    }
    public class FramerateCounter : TextRenderer
    {
        public override void OnCreate()
        {
            base.OnCreate();

            DisplayedString = "FPS: 0";

            UnitSize = 1.25;
            Parent.Transform.Position = RenderCamera.MapRegion * new Vector(-1, 1);
        }

        int counter = 0;
        double dt = 0;
        public override void OnTick()
        {
            base.OnFrame();
            dt += TickTime.DeltaTime;
            counter += 1;
            if (dt >= 1)
            {
                DisplayedString = $"FPS: {counter}";
                dt = 0;
                counter = 0;
            }
        }
        public override void Render()
        {
            base.Render();
        }
    }
}
