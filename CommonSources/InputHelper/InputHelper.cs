using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    public class InputHelper
    {
        #region Variables
        public GamePadState VirtualState { get; private set; }
        public GamePadState GamePadState { get; private set; }
        public GamePadState LastGamePadState { get; private set; }
        public GamePadState LastVirtualState { get; private set; }
        public MouseState LastMouseState { get; private set; }
        private int _prevScrollWheelValue;
        public MouseState MouseState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }
        public KeyboardState LastKeyboardState { get; private set; }
        public Vector2 MousePos { get; private set; }
        public Vector2 LastMousePos { get; private set; }

        /// <summary>
        /// Gets mouse position change value
        /// </summary>
        public Vector2 MousePosChange
        {
            get { return LastMousePos - MousePos; }
        }

        /// <summary>
        /// Gets mouse move vector
        /// </summary>
        public Vector2 MouseMoveVector
        {
            get { var vec = MousePosChange; vec.Normalize(); return vec; }
        }

        /// <summary>
        /// Gets mouse drag change
        /// </summary>
        public Vector2 MouseDragChange
        {
            get { return _endDragPos - _startDragPos; }
        }

        private Vector2 _startDragPos;
        private Vector2 _endDragPos;

#if WINDOWS_PHONE
        private VirtualStick _phoneStick;
        private VirtualButton _phoneA;
        private VirtualButton _phoneB;
#endif
        private Viewport _viewport;
        private readonly Game _manager;

        private static bool _handleVirtualStick;
        public static bool EnableVirtualStick
        {
            get { return _handleVirtualStick; }
            set { _handleVirtualStick = value; }
        }
        public static Vector2 Cursor { get; private set; }
        public static List<GestureSample> Gestures = new List<GestureSample>();
        public static List<TouchLocation> TouchStates = new List<TouchLocation>();
        public bool IsCursorMoved { get; private set; }
        public bool IsCursorValid { get; private set; }
        #endregion


        public InputHelper(Game manager)
        {
            KeyboardState = new KeyboardState();
            GamePadState = new GamePadState();
            MouseState = new MouseState();
            VirtualState = new GamePadState();

            LastKeyboardState = new KeyboardState();
            LastGamePadState = new GamePadState();
            LastMouseState = new MouseState();
            LastVirtualState = new GamePadState();
            _manager = manager;
        }

        /// <summary>
        /// Load input helper content and initialization
        /// </summary>
        public void LoadContent()
        {
            _viewport = _manager.GraphicsDevice.Viewport;
        }

        /// <summary>
        /// Returns true if specified point is in specified rectangle area
        /// </summary>
        /// <param name="pos">Vector position</param>
        /// <param name="zone">Rectangle zone</param>
        public static bool IsPosInBound(Vector2 pos, Rectangle zone)
        {
            if (zone.Contains((int)pos.X, (int)pos.Y))
                return true;
            return false;
        }

        /// <summary>
        /// Returns true if specified point is in specified rectangle area
        /// </summary>
        /// <param name="pos">Point position</param>
        /// <param name="zone">Rectangle zone</param>
        public static bool IsPosInBound(Point pos, Rectangle zone)
        {
            if (zone.Contains(pos))
                return true;
            return false;
        }

#if WINDOWS

        /// <summary>
        /// Returns true if mouse has been moved campared to prev state
        /// </summary>
        public bool IsMouseMoved()
        {
            return MousePosChange != Vector2.Zero;
        }

        /// <summary>
        /// Returns true if mouse left button is pressed and mouse is being dragged
        /// </summary>
        public bool IsMouseDragged()
        {
            return _startDragPos != Vector2.Zero;
        }

        /// <summary>
        /// Returns mouse movement vector
        /// </summary>
        /// <param name="limit">Movement limit</param>
        public Vector2 GetMouseDragVector(float limit = 35f)
        {
            var src = MouseDragChange;
            if (src == Vector2.Zero) return Vector2.Zero;
            var absx = Math.Abs(src.X);
            var absy = Math.Abs(src.Y);
            if (absx > absy)
            {
                return absx > limit ? new Vector2(src.X, 0) : Vector2.Zero;
            }
            return absy > limit ? new Vector2(0, src.Y) : Vector2.Zero;
        }

        /// <summary>
        /// Returns True if mouse button is in specified state (Default state: click)
        /// </summary>
        /// <param name="button">Mouse button</param>
        /// <param name="bState">Mouse button state</param>
        public bool IsMousePressed(MouseButton button, MouseButtonState bState = MouseButtonState.Click)
        {
            switch(button)
            {
                case MouseButton.Right:
                    return bState == MouseButtonState.Click ? LastMouseState.RightButton == ButtonState.Released && MouseState.RightButton == ButtonState.Pressed : MouseState.RightButton == ButtonState.Pressed;
                case MouseButton.Middle:
                    return bState == MouseButtonState.Click ? LastMouseState.MiddleButton == ButtonState.Released && MouseState.MiddleButton == ButtonState.Pressed : MouseState.MiddleButton == ButtonState.Pressed;
                default:
                    return bState == MouseButtonState.Click ? LastMouseState.LeftButton == ButtonState.Released && MouseState.LeftButton == ButtonState.Pressed : MouseState.LeftButton == ButtonState.Pressed;
            }
        }

        /// <summary>
        /// Returns True if mouse wheel has been scrolled in specified direction
        /// </summary>
        /// <param name="direction">Scroll direction</param>
        public bool IsMouseWheelScrolled(ScrollDirection direction)
        {
            if (LastMouseState.ScrollWheelValue - MouseState.ScrollWheelValue > 0 && direction == ScrollDirection.Up) return true;
            if (LastMouseState.ScrollWheelValue - MouseState.ScrollWheelValue < 0 && direction == ScrollDirection.Down) return true;
            return false;
        }

        /// <summary>
        /// Returns True if key is in specified state (By default: pressed)
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="keyState">Key state to check</param>
        public bool IsKeyPressed(Keys key, KeyState keyState = KeyState.Pressed)
        {
            switch (keyState)
            {
                case KeyState.Pressed:
                    return KeyboardState.IsKeyDown(key) && !LastKeyboardState.IsKeyDown(key);
                case KeyState.Released:
                    return !KeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyDown(key);
                case KeyState.Holding:
                    return KeyboardState.IsKeyDown(key) && LastKeyboardState.IsKeyDown(key);
                default: return false;
            }
        }
       
        /// <summary>
        /// Returns mouse scroll value difference between current and previous states
        /// </summary>
        internal int GetScrollDiff()
        {
            return MouseState.ScrollWheelValue - _prevScrollWheelValue;
        }

#endif



        internal void Update(GameTime gameTime)
        {
            //return if game core isn't active
            if(!_manager.IsActive) return;

            if (_handleVirtualStick)
            {
#if XBOX
                VirtualState = GamePad.GetState(PlayerIndex.One);
#elif WINDOWS
                VirtualState = GamePad.GetState(PlayerIndex.One).IsConnected ? GamePad.GetState(PlayerIndex.One) : HandleVirtualStickWin();
#elif WINDOWS_PHONE
                VirtualState = HandleVirtualStickWP7();
#endif
            }

#if WINDOWS
            LastGamePadState = GamePadState;
            LastMouseState = MouseState;
            _prevScrollWheelValue = MouseState.ScrollWheelValue;
            MouseState = Mouse.GetState();
            if (MouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released)
                _endDragPos = _startDragPos = new Vector2(MouseState.X, MouseState.Y);
            if (MouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Pressed)
                _endDragPos = new Vector2(MouseState.X, MouseState.Y);
            if (MouseState.LeftButton == ButtonState.Released && LastMouseState.LeftButton == ButtonState.Pressed)
                _startDragPos = _endDragPos = Vector2.Zero;

            LastKeyboardState = KeyboardState;
            KeyboardState = Keyboard.GetState();
            MousePos = new Vector2(MouseState.X, MouseState.Y);
            LastMousePos = new Vector2(LastMouseState.X, LastMouseState.Y);
#endif
            Gestures.Clear();
            while (TouchPanel.IsGestureAvailable)
            {
                Gestures.Add(TouchPanel.ReadGesture());
            }
            TouchStates.Clear();
            foreach (var touch in TouchPanel.GetState())
                TouchStates.Add(touch);

            // Update cursor
            if (GamePadState.IsConnected && GamePadState.ThumbSticks.Left != Vector2.Zero)
            {
                var temp = GamePadState.ThumbSticks.Left;
                Cursor += temp * new Vector2(300f, -300f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Mouse.SetPosition((int)Cursor.X, (int)Cursor.Y);
            }
            else
            {
                Cursor = new Vector2(MouseState.X, MouseState.Y);
            }
            Cursor = new Vector2(MathHelper.Clamp(Cursor.X, 0f, _viewport.Width), MathHelper.Clamp(Cursor.Y, 0f, _viewport.Height));

#if WINDOWS
            IsCursorValid = _viewport.Bounds.Contains(MouseState.X, MouseState.Y);
#elif WINDOWS_PHONE
            IsCursorValid = MouseState.LeftButton == ButtonState.Pressed;
#endif
        }

        public void Draw()
        {
        }

        /// <summary>
        ///   Helper for checking if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (GamePadState.IsButtonDown(button) && LastGamePadState.IsButtonUp(button));
        }

        public bool IsNewButtonRelease(Buttons button)
        {
            return (LastGamePadState.IsButtonDown(button) && GamePadState.IsButtonUp(button));
        }

        private GamePadState HandleVirtualStickWin()
        {
            var leftStick = Vector2.Zero;
            var buttons = new List<Buttons>();

            if (KeyboardState.IsKeyDown(Keys.A))
                leftStick.X -= 1f;
            if (KeyboardState.IsKeyDown(Keys.S))
                leftStick.Y -= 1f;
            if (KeyboardState.IsKeyDown(Keys.D))
                leftStick.X += 1f;
            if (KeyboardState.IsKeyDown(Keys.W))
                leftStick.Y += 1f;
            if (KeyboardState.IsKeyDown(Keys.Space))
                buttons.Add(Buttons.A);
            if (KeyboardState.IsKeyDown(Keys.LeftControl))
                buttons.Add(Buttons.B);
            if (leftStick != Vector2.Zero)
                leftStick.Normalize();

            return new GamePadState(leftStick, Vector2.Zero, 0f, 0f, buttons.ToArray());
        }

        private GamePadState HandleVirtualStickWp7()
        {
            var buttons = new List<Buttons>();
            var stick = Vector2.Zero;
#if WINDOWS_PHONE
            _phoneA.Pressed = false;
            _phoneB.Pressed = false;
            TouchCollection touchLocations = TouchPanel.GetState();
            foreach (TouchLocation touchLocation in touchLocations)
            {
                _phoneA.Update(touchLocation);
                _phoneB.Update(touchLocation);
                _phoneStick.Update(touchLocation);
            }
            if (_phoneA.Pressed)
            {
                buttons.Add(Buttons.A);
            }
            if (_phoneB.Pressed)
            {
                buttons.Add(Buttons.B);
            }
            stick = _phoneStick.StickPosition;
#endif
            return new GamePadState(stick, Vector2.Zero, 0f, 0f, buttons.ToArray());
        }
    }
}
