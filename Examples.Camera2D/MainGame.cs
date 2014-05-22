#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Examples.Classes;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

// MonoGame Examples Collection
// Example 3. Advanced Camera2D
// (http://www.panthernet.ru)
//
// Using Camera2D you can effectively perform zoom, pan and rotate operations
// along with the ability to transition to and follow coordinates.
// Simple usage:
// 1. Create camera
// 2. Call Camera2D.Update() method in your update method
// 3. Provide Camera2D.GetViewTransformationMatrix() to all nedded SpriteBatch.Begin() calls to draw using camera
// 4. Setup camera input that deals with such props as Zoom and Position

namespace Examples.C2D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _gfx;
        private SpriteBatch _mBatch;
        private ResolutionRenderer _irr;
        private InputHelper _input;
        private const int VIRTUAL_RESOLUTION_WIDTH = 1280;
        private const int VIRTUAL_RESOLUTION_HEIGHT = 720;
        bool _pinching;
        private int _randomIndex;
        private int _selectedIndex = -1;
        private bool _isFollowEnabled;

        readonly Rectangle[] _rectangles = new Rectangle[3];
        private Texture2D _bg;
        private Texture2D _ufoTexture;
        private Texture2D _ufoTexture2;
        private Texture2D _ufoTexture3;
        private SpriteFont _font;
        private Camera2D _camera;

        public MainGame()
        {
            _gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            IsMouseVisible = true;
            _gfx.IsFullScreen = false;
            //set real screen resolution
            _gfx.PreferredBackBufferWidth = 800;
            _gfx.PreferredBackBufferHeight = 400;
            _gfx.PreferMultiSampling = true;
            _gfx.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            TouchPanel.EnabledGestures = GestureType.Pinch | GestureType.PinchComplete;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //set virtual screen resolution
            _irr = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, _gfx.PreferredBackBufferWidth, _gfx.PreferredBackBufferHeight);
            _input = new InputHelper(this);

            _camera = new Camera2D(_irr) { MaxZoom = 10f, MinZoom = .4f, Zoom = 1f };
            _camera.SetPosition(Vector2.Zero);
            _camera.RecalculateTransformationMatrices();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _mBatch = new SpriteBatch(GraphicsDevice);
            _input.LoadContent();

            _bg = Content.Load<Texture2D>("star_bg.jpg");
            _ufoTexture = Content.Load<Texture2D>("nlo.png");
            _ufoTexture2 = Content.Load<Texture2D>("nlo2.png");
            _ufoTexture3 = Content.Load<Texture2D>("nlo3.png");
            _font = Content.Load<SpriteFont>("sf");
            _rectangles[0] = new Rectangle(0, 0, _ufoTexture.Width, _ufoTexture.Height);
            _rectangles[1] = new Rectangle(300, 0, _ufoTexture2.Width, _ufoTexture2.Height);
            _rectangles[2] = new Rectangle(0, 300, _ufoTexture3.Width, _ufoTexture3.Height);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _ufoTexture.Dispose();
            _ufoTexture2.Dispose();
            _ufoTexture3.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //update input helper data
            _input.Update(gameTime);
            _camera.Update(gameTime);

            //update following logic
            if (_isFollowEnabled)
            {
                //if we are already transitioning then just update target coordinates
                if (_camera.IsTransitionActive)
                    _camera.UpdateTransitionTarget(_rectangles[0].Location);
                else _camera.StartTransition(_rectangles[0].Location);
            }

            //move ufo1 in circle
            _rectangles[0].Location = CircleMovement(0, 0, 200);

            if (_input.GamePadState.Buttons.Back == ButtonState.Pressed || _input.IsKeyPressed(Keys.Escape))
            {
                Exit();
                return;
            }

            #region Pinch zoom
            //PINCH ZOOM
            foreach (var gesture in InputHelper.Gestures)
            {
                if (gesture.GestureType == GestureType.Pinch)
                {
                    // current positions
                    var a = gesture.Position;
                    var b = gesture.Position2;
                    var dist = Vector2.Distance(a, b);

                    // prior positions
                    var aOld = gesture.Position - gesture.Delta;
                    var bOld = gesture.Position2 - gesture.Delta2;
                    var distOld = Vector2.Distance(aOld, bOld);

                    if (!_pinching)
                    {
                        // start of pinch, record original distance
                        _pinching = true;
                    }

                    // work out zoom amount based on pinch distance...
                    var scale = ((distOld - dist) * 0.05f) / 10f;
                    _camera.Zoom -= scale;
                }
                else if (gesture.GestureType == GestureType.PinchComplete)
                {
                    // end of pinch
                    _pinching = false;
                    return;
                }
            }

            //if pinch is in progress
            if (_pinching) return;
            #endregion

            foreach (var touch in InputHelper.TouchStates)
            {
                if (_camera.IsTransitionActive) continue;

                #region Camera movement

                //CAMERA MOVEMENT
                if (touch.State == TouchLocationState.Moved)
                {
                    TouchLocation ploc;
                    if (touch.TryGetPreviousLocation(out ploc))
                        CameraMovement(ploc.Position - touch.Position);
                }
                #endregion
            }

            //camera movement by mouse
            if (_input.IsMousePressed(MouseButton.Right, MouseButtonState.PressedOnly))
                CameraMovement(_input.MousePosChange);

            if (_input.IsMousePressed(MouseButton.Left))
            {
                //get virtual coordinates, take a note that TRUE param is specified due we have used IRR and we need aditional viewport adjustment
                var coords = _camera.ToVirtual(_input.MousePos);
                for(var i =0; i < _rectangles.Length; i++)
                {
                    //now we just check if any UFO is in specified coordinates and select it
                    if (_rectangles[i].Contains((int)coords.X, (int)coords.Y))
                    {
                        _selectedIndex = i; break;
                    }
                }
            }

            //camera zoom by mouse wheel
            var scrlDiff = _input.GetScrollDiff();
            if (scrlDiff != 0)
                _camera.Zoom += scrlDiff > 0 ? (float)Math.Log10(scrlDiff / 50) : -(float)Math.Log10(Math.Abs(scrlDiff / 50));

            //center on target rectangle (zoom and pos changed)
            if (_input.IsKeyPressed(Keys.C))
            {
                if (_isFollowEnabled) { _isFollowEnabled = false; _camera.StopTransition(); }
                var rec = _rectangles[_randomIndex].NewInflate(100, 100);
                _camera.CenterOnTarget(rec);
                _randomIndex++; if (_randomIndex > 2) _randomIndex = 0;
            }
            //transition to target coordinates (smooth pos change, no zoom changed)
            if (_input.IsKeyPressed(Keys.T))
            {
                if (_isFollowEnabled) { _isFollowEnabled = false; _camera.StopTransition(); }
                _camera.StartTransition(_rectangles[_randomIndex].Location);
                _randomIndex++; if (_randomIndex > 2) _randomIndex = 0;
            }
            //follow the ufo1 using constantly updated transition logic
            if (_input.IsKeyPressed(Keys.F))
            {
                _isFollowEnabled = !_isFollowEnabled;
                if (!_isFollowEnabled)
                    _camera.StopTransition();
            }

            //rotation controls
            if (_input.IsKeyPressed(Keys.Q, Classes.KeyState.Holding))
                _camera.Rotation -= 0.01f;
            if (_input.IsKeyPressed(Keys.E, Classes.KeyState.Holding))
                _camera.Rotation += 0.01f;
            //reset all the stuff
            if (_input.IsKeyPressed(Keys.R))
            {
                _camera.Rotation = 0;
                _isFollowEnabled = false;
                _camera.StopTransition();
                _camera.SetPosition(Vector2.Zero);
                _camera.Zoom = 1f;
                _selectedIndex = -1;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Prepare IRR call
            _irr.Draw();

            //  UI DRAWING using ResolutionRenderer
            //all independent resolution objects must be drawn using this matrix for proper scaling and sizing
            //all coordinates must be specified in virtual screen resolution size range
            _mBatch.BeginResolution(_irr);
            _mBatch.Draw(_bg, new Rectangle(0,0, _irr.VirtualWidth, _irr.VirtualHeight), Color.White);
            _mBatch.End();


            // GAME DRAWING using Camera2D
            _mBatch.BeginCamera(_camera);
            _mBatch.Draw(_ufoTexture, _rectangles[0], _selectedIndex == 0 ? Color.Red : Color.White);
            _mBatch.Draw(_ufoTexture2, _rectangles[1], _selectedIndex == 1 ? Color.Red : Color.White);
            _mBatch.Draw(_ufoTexture3, _rectangles[2], _selectedIndex == 2 ? Color.Red : Color.White);
            _mBatch.End();

            //reset screen viewport back to full size
            //so we can draw text from the TopLeft corner of the real screen
            _irr.SetupFullViewport();
            _mBatch.Begin();
            _mBatch.DrawString(_font, "Example 3. Advanced Camera2D", new Vector2(0, 0), Color.White, .6f);
            _mBatch.DrawString(_font, "(http://www.panthernet.ru)", new Vector2(0, 20), Color.White, .6f);
            _mBatch.DrawString(_font, string.Format("Camera pos: {0:0}/{1:0}  Zoom: {2:0.00} Follow: {3}", _camera.Position.X, _camera.Position.Y, _camera.Zoom, _isFollowEnabled ? "Yes" : "No"), new Vector2(0, 40), Color.White, .6f);
            _mBatch.DrawString(_font, "Use RMB/MWheel/Q/E to control camera\nUse LMB to select ufos\nPress C or T to center/transition camera on UFOs\nPress F to follow the ufo1\nPress R to reset or Escape to exit", new Vector2(0, 60), Color.White, .6f);
            _mBatch.End();
            _irr.SetupVirtualScreenViewport();

            base.Draw(gameTime);
        }

        private int _cmstep;
        /// <summary>
        /// Simple circle movement in line
        /// </summary>
        private Point CircleMovement(float centerX, float centerY, float radius)
        {
            _cmstep++;
            if (_cmstep > 360) _cmstep = 0;
            return new Point((int)(radius * Math.Cos(_cmstep * 3.1415926f / 180.0f) + centerX),
                (int)(radius * Math.Cos(_cmstep * 3.1415926f / 180.0f) + centerY));
            
        }

        private void CameraMovement(Vector2 shift)
        {
            //if we have big movement
            if (shift.LengthSquared() > 10f)
            {
                //divide on camera zoom value to get smoother position change
                var shiftValue = shift / _camera.Zoom;
                _camera.Move(shiftValue);
            }
        }
    }
}
