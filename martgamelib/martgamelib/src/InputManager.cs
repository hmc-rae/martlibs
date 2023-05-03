using System;
using martlib;
using SFML.Window;

namespace martgamelib
{
    public class InputManager
    {
        //keyboard
        private bool[] mKeys;
        private int[] mKeyQueue;
        private int mKeyQueueDepth;

        //mouse
        private bool[] mMouse;
        private int[] mMouseQueue;
        private int mMouseQueueDepth;


        private Vector mPos;
        private float mWheelDelta;

        public InputManager()
        {
            //keyboard
            mKeys = new bool[512];
            mKeyQueue = new int[512];
            mKeyQueueDepth = 0;

            //mouse
            mMouse = new bool[16];
            mMouseQueueDepth = 0;
            mMouseQueue = new int[16];

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
        /// call this pre-frame
        /// </summary>
        protected internal void prepoll()
        {
            for (int i = 0; i < mKeyQueueDepth; i++)
                mKeys[mKeyQueue[i] + 1] = mKeys[mKeyQueue[i]];

            for (int i = 0; i < mMouseQueueDepth; i++)
                mMouse[mMouseQueue[i] + 1] = mMouse[mMouseQueue[i]];

            mMouseQueueDepth = mKeyQueueDepth = 0;
            mWheelDelta = 0;
        }

        //keyboard
        internal void OnPress(object? sender, KeyEventArgs e)
        {
            int n = (int)e.Code << 1;
            if (n < 0 || n >= mKeys.Length) return; //safety :thumbsup:
            mKeys[n] = true;
            mKeyQueue[mKeyQueueDepth++] = n;
        }
        internal void OnRelease(object? sender, KeyEventArgs e)
        {
            int n = (int)e.Code << 1;
            if (n < 0 || n >= mKeys.Length) return; //safety :thumbsup:

            mKeys[n] = false;
            mKeyQueue[mKeyQueueDepth++] = n;
        }

        //mouse
        internal void OnMousePress(object? sender, MouseButtonEventArgs e)
        {
            int n = (int)e.Button << 1;
            if (n < 0 || n >= mKeys.Length) return; //safety :thumbsup:
            mMouse[n] = true;
            mMouseQueue[mMouseQueueDepth++] = n;
        }
        internal void OnMouseRelease(object? sender, MouseButtonEventArgs e)
        {
            int n = (int)e.Button << 1;
            if (n < 0 || n >= mKeys.Length) return; //safety :thumbsup:
            mMouse[n] = false;
            mMouseQueue[mMouseQueueDepth++] = n;
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
        public bool KeyPressed(Keyboard.Key key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mKeys.Length) return false;

            return mKeys[n] && !mKeys[n + 1];
        }

        /// <summary>
        /// Returns true if the key has been released over the last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyReleased(Keyboard.Key key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mKeys.Length) return false;

            return !mKeys[n] && mKeys[n + 1];
        }

        /// <summary>
        /// Returns true if the key has been held for more than one frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyHeld(Keyboard.Key key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mKeys.Length) return false;

            return mKeys[n] && mKeys[n + 1];
        }

        /// <summary>
        /// Returns true if the key is pressed down, whether that be held or tapped.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Key(Keyboard.Key key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mKeys.Length) return false;

            return mKeys[n];
        }

        /// <summary>
        /// Returns true if the mouse button has just been pressed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseClicked(Mouse.Button key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mMouse.Length) return false;

            return mMouse[n] && !mMouse[n + 1];
        }
        /// <summary>
        /// Returns true if the mouse button has been released over the last frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseReleased(Mouse.Button key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mMouse.Length) return false;

            return !mMouse[n] && mMouse[n + 1];
        }
        /// <summary>
        /// Returns true if the mouse button has been held for more than one frame.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool MouseHeld(Mouse.Button key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mMouse.Length) return false;

            return mMouse[n] && mMouse[n + 1];
        }
        /// <summary>
        /// Returns true if the mouse button is pressed down, whether it be held or tapped.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Mouse(Mouse.Button key)
        {
            int n = (int)key * 2;
            if (n < 0 || n >= mMouse.Length) return false;

            return mMouse[n];
        }
    }

    public struct KeyRegister
    {
        public readonly char Key;

        internal bool hasPressed;
        internal bool hasReleased;

        private bool pressed;
        private bool held;
        private bool released;

        public KeyRegister(char k)
        {
            Key = k;

            hasPressed = hasReleased = false;
            pressed = held = released = false;
        }
    }
}
