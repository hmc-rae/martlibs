using System;
using martgamelib;
using martlib;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace MEngineEditor
{
    public static class Comps
    {
        public class ButtonRenderer : RenderComponent
        {
            private bool pressed = false;

            private Color raised;
            private Color lowered;

            public Vector Bounds;
            public string text;

            private static RectangleShape _shape = new RectangleShape(new Vector2f(1, 1));
            private Text _text;
            private static Font ttf = new Font($"{Directory.GetCurrentDirectory()}\\times.ttf");

            public override void OnCreate()
            {
                base.OnCreate();

                _shape.Origin = new Vector2f(0.5f, 0.5f);
                raised = new Color(59, 204, 74);
                lowered = new Color(16, 56, 20);

                _text = new Text("", ttf);

                Bounds = new Vector(1, 1);

                pressed = false;
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

                Vector PPU = RenderCamera.GetPixelsPerUnit();

                FloatRect b = _text.GetLocalBounds();
                Bounds = new Vector((2 * b.Left) + b.Width, (2 * b.Top) + b.Width) / PPU;

                _text.Origin = new Vector2f(b.Left + (b.Width / 2), b.Top + (b.Height / 2));
                
                _shape.Scale = martgame.ToSFMLVector(Bounds * PPU * Parent.Transform.Scale);

                _text.Scale = martgame.ToSFMLVector(Parent.Transform.Scale);

                _shape.Rotation = (float)(Parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);
                if (pressed)
                    _shape.FillColor = lowered;
                else
                    _shape.FillColor = raised;
                
                _text.Rotation = (float)(Parent.Transform.Rotation.Flip.Degrees + RenderCamera.Parent.Transform.Rotation.Degrees);

                _shape.Position =  martgame.ToSFMLVector(relative);
                _text.Position = martgame.ToSFMLVector(relative);

                RenderCamera.Render(_shape);
                RenderCamera.Render(_text);
            }
        }
    }
}
