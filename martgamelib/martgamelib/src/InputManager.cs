using System;
using martlib;
using SFML.Window;

namespace martgamelib
{
    public class InputManager
    {
        //keyboard
        private const int KEYCOUNT = 128;
        private KeyRegister[] mKeyRegister;


        //mouse
        private const int MOUSECOUNT = 256;
        private KeyRegister[] mMouseRegister;


        private Vector mPos;
        private float mWheelDelta;

        public InputManager()
        {
            //keyboard
            mKeyRegister = new KeyRegister[KEYCOUNT];
            for (int i = 0; i < KEYCOUNT; i++)
            {
                mKeyRegister[i] = new KeyRegister();
            }


            //mouse
            mMouseRegister = new KeyRegister[MOUSECOUNT];
            for (int i = 0; i < MOUSECOUNT; i++)
            {
                mMouseRegister[i] = new KeyRegister();
            }

            //misc
            mPos = new Vector();
            mWheelDelta = 0;
        }

        //housekeeping things
        protected internal void Bind(GameWindow window)
        {
            window.KeyPressed += OnPress;
            window.KeyReleased += OnRelease;

            window.MouseButtonPressed += OnMousePress;
            window.MouseButtonReleased += OnMouseRelease;

            window.MouseWheelScrolled += OnMouseScroll;

            window.Closed += OnClose;
        }

        /// <summary>
        /// call this pre-tick
        /// </summary>
        protected internal void prepoll()
        {
            for (int i = 0; i < KEYCOUNT; i++)
            {
                mKeyRegister[i].patch();
            }

            for (int i = 0; i < MOUSECOUNT; i++)
            {
                mMouseRegister[i].patch();
            }
            mWheelDelta = 0;
        }

        //keyboard
        internal void OnPress(object? sender, KeyEventArgs e)
        {
            int n = (int)e.Code;
            if (n < 0 || n >= KEYCOUNT) return; //safety :thumbsup:

            mKeyRegister[n].hasPressed = true;
            mKeyRegister[n].hasHeld = true;
        }
        internal void OnRelease(object? sender, KeyEventArgs e)
        {
            int n = (int)e.Code;
            if (n < 0 || n >= KEYCOUNT) return; //safety :thumbsup:

            mKeyRegister[n].hasReleased = true;
            mKeyRegister[n].hasHeld = false;
        }

        //mouse
        internal void OnMousePress(object? sender, MouseButtonEventArgs e)
        {
            int n = (int)e.Button;
            if (n < 0 || n >= MOUSECOUNT) return; //safety :thumbsup:

            mMouseRegister[n].hasPressed = true;
            mMouseRegister[n].hasHeld = true;
        }
        internal void OnMouseRelease(object? sender, MouseButtonEventArgs e)
        {
            int n = (int)e.Button;
            if (n < 0 || n >= MOUSECOUNT) return; //safety :thumbsup:

            mMouseRegister[n].hasReleased = true;
            mMouseRegister[n].hasHeld = false;
        }

        //mouse scroll
        internal void OnMouseScroll(object? sender, MouseWheelScrollEventArgs e)
        {
            mWheelDelta = e.Delta;
        }

        //mouse move
        internal void OnMouseMove(object? sender, MouseMoveEventArgs e)
        {
            mPos.Set(e.X, e.Y);
        }

        //close
        internal void OnClose(object? sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        //publicly visible stuff
        /// <summary>
        /// The current position of the mouse cursor.
        /// </summary>
        public Vector MousePosition
        {
            get { return mPos; }
        }
        /// <summary>
        /// How much the mouse wheel has been scrolled.
        /// </summary>
        public float MouseWheelDelta
        {
            get { return mWheelDelta; }
        }



        /// <summary>
        /// Returns true if the key has just been pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyPressedBool(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return false;

            return mKeyRegister[n].pressed;
        }
        /// <summary>
        /// Returns 1 if the key has just been pressed, 0 otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int KeyPressed(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return 0;

            return mKeyRegister[n].pressed ? 1 : 0;
        }



        /// <summary>
        /// Returns true if the key has been released over the last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyReleasedBool(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return false;

            return mKeyRegister[n].released;
        }
        /// <summary>
        /// Returns 1 if the key has been released over the last frame, 0 otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int KeyReleased(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return 0;

            return mKeyRegister[n].released ? 1 : 0;
        }



        /// <summary>
        /// Returns true if the key has been held for more than one frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyHeldBool(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return false;

            return mKeyRegister[n].held;
        }
        /// <summary>
        /// Returns 1 if the key is held down, 0 otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int KeyHeld(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return 0;

            return mKeyRegister[n].held ? 1 : 0;
        }



        /// <summary>
        /// Returns true if the key is pressed down, whether that be held or tapped.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyBool(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return false;

            return mKeyRegister[n].pressed || mKeyRegister[n].held;
        }
        /// <summary>
        /// Returns 1 if the key is pressed down, whether that be held or tapped, 0 otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Key(Keyboard.Key key)
        {
            int n = (int)key;
            if (n < 0 || n >= KEYCOUNT) return 0;

            return (mKeyRegister[n].pressed || mKeyRegister[n].held) ? 1 : 0;
        }



        /// <summary>
        /// Returns 1 if a is held, -1 if b is held, and 0 otherwise.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int KeyDelta(Keyboard.Key a, Keyboard.Key b)
        {
            return Key(a) - Key(b);
        }



        /// <summary>
        /// Returns true if the mouse button has just been pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseClickedBool(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return false;

            return mMouseRegister[n].pressed;
        }
        /// <summary>
        /// Returns 1 if the mouse button has just been pressed, 0 otherwise.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int MouseClicked(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return 0;

            return mMouseRegister[n].pressed ? 1 : 0;
        }


        /// <summary>
        /// Returns true if the mouse button has been released over the last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseReleasedBool(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return false;

            return mMouseRegister[n].released;
        }
        /// <summary>
         /// Returns true if the mouse button has been released over the last frame.
         /// </summary>
         /// <param name="key"></param>
         /// <returns></returns>
        public int MouseReleased(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return 0;

            return mMouseRegister[n].released ? 1 : 0;
        }



        /// <summary>
        /// Returns true if the mouse button has been held for more than one frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseHeldBool(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return false;

            return mMouseRegister[n].held;
        }
        /// <summary>
        /// Returns true if the mouse button has been held for more than one frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int MouseHeld(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return 0;

            return mMouseRegister[n].held ? 1 : 0;
        }



        /// <summary>
        /// Returns true if the mouse button is pressed down, whether it be held or tapped.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseBool(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return false;

            return mMouseRegister[n].pressed || mMouseRegister[n].held;
        }
        /// <summary>
        /// Returns true if the mouse button is pressed down, whether it be held or tapped.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Mouse(Mouse.Button key)
        {
            int n = (int)key;
            if (n < 0 || n >= MOUSECOUNT) return 0;

            return mMouseRegister[n].pressed || mMouseRegister[n].held ? 1 : 0;
        }



        /// <summary>
        /// Returns 1 if a is held, -1 if b is held, and 0 otherwise.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public int MouseDelta(Mouse.Button a, Mouse.Button b)
        {
            return Mouse(a) - Mouse(b);
        }
    }

    internal struct KeyRegister
    {
        internal bool hasPressed;
        internal bool hasHeld;
        internal bool hasReleased;

        internal bool pressed;
        internal bool held;
        internal bool released;

        public KeyRegister()
        {
            hasPressed = hasHeld = hasReleased = false;
            pressed = held = released = false;
        }

        internal void patch()
        {
            pressed = hasPressed;
            held = hasHeld;
            released = hasReleased;

            hasPressed = hasReleased = false;
        }
    }
}
