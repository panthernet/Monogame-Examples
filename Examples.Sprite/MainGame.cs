#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Examples.Classes;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace Examples.Sprites
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

        private Rectangle _spriteRect;

        private Texture2D _bg;
        private Sprite _asteroidSprite;
        private Sprite _ufoSprite;
        private SpriteFont _font;
        private Camera2D _camera;

        private const int TEX_SHIP = 0;
        private const int TEX_SHIP_TAIL = 1;

        #region Spaceship variables
        private Vector2 _position = Vector2.Zero;
        private Vector2 _direction = new Vector2(0, -1);
        private Vector2 _acceleration = Vector2.Zero;
        private Vector2 _velocity = Vector2.Zero;
        private const int ACCEL_BASE = 10;
        private const int ACCEL_BREAK = 1;
        private readonly Vector2 _velocityStop = new Vector2(0.1f, 0.1f);
        #endregion

        public MainGame()
        {
            _gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            _gfx.IsFullScreen = false;
            IsMouseVisible = true;
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

            //create sprite with 2 textures
            // - SpriteTexture is the main ship texture operated by _spriteRect position
            // - Decaltexture is an engine tail texture that is automaticaly aligned to _spriteRect rectangle using Offset
            //   It uses default Origin and is hidden by default.
            _ufoSprite = new Sprite(new Dictionary<int, ISpriteTexture>
            {
                {TEX_SHIP, new SpriteTexture(Content.Load<Texture2D>("spaceship.png"), true) },
                {TEX_SHIP_TAIL, new DecalTexture(Content.Load<Texture2D>("spaceship_tail.png")) },
            }) 
            { Scale = Vector2.One * .5f }; //sprite is scaled in half
            //here we operate sizes relative to texture size, they will be scaled automaticaly in Sprite::Draw() if needed
            //set tail texture offset to fit the engines of the ship
            _ufoSprite.Textures[TEX_SHIP_TAIL].Offset = new Vector2(-_ufoSprite.Textures[TEX_SHIP].Texture.Width * .5f + 7, _ufoSprite.Textures[TEX_SHIP].Texture.Height * .5f - 20);
            //set sprite origin to center so we can rotate our ship around center of the texture
            _ufoSprite.Origin = _ufoSprite.GetTextureSize(TEX_SHIP) * .5f;

            //set starting coordinates by defining the sprite rectangle
            var size = _ufoSprite.GetTextureSize(TEX_SHIP);
            _spriteRect = new Rectangle(0, 0, (int)(size.X), (int)(size.Y));


            _asteroidSprite = new Sprite(0, Content.Load<Texture2D>("asteroid.png"));

            //load font
            _font = Content.Load<SpriteFont>("sf");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _ufoSprite.Dispose();
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



            if (_input.GamePadState.Buttons.Back == ButtonState.Pressed || _input.IsKeyPressed(Keys.Escape))
            {
                Exit();
                return;
            }

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

            #region Spaceship controls and math

            //rotation controls
            if (_input.IsKeyPressed(Keys.Left, Classes.KeyState.Holding))
            {
                _ufoSprite.Rotation -= 0.02f;//rotate visual sprite
                _direction = Vector2.Transform(_direction, Matrix.CreateRotationZ(-0.02f));//rotate math 
            }
            if (_input.IsKeyPressed(Keys.Right, Classes.KeyState.Holding))
            {
                _ufoSprite.Rotation += 0.02f;//rotate visual sprite
                _direction = Vector2.Transform(_direction, Matrix.CreateRotationZ(0.02f));//rotate math 
            }

            
            //acceleration controls
            if (_input.IsKeyPressed(Keys.Up, Classes.KeyState.Holding))
                _acceleration = _direction * ACCEL_BASE;
            else
                if (_input.IsKeyPressed(Keys.Down, Classes.KeyState.Holding))
                    _acceleration = _direction * -ACCEL_BASE * .3f;
                else _acceleration = ACCEL_BREAK * -_velocity; //fade velocity

            //calculate velocity
            _velocity += _acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //show/hide sprite engine tails
            if (Math.Abs(_velocity.X) > _velocityStop.X || Math.Abs(_velocity.Y) > _velocityStop.Y)
                _ufoSprite.Textures[TEX_SHIP_TAIL].Visibility = true;
            else _ufoSprite.Textures[TEX_SHIP_TAIL].Visibility = false;

            //scale engine tail by velocity
            _ufoSprite.Textures[TEX_SHIP_TAIL].Scale = new Vector2(_ufoSprite.Textures[TEX_SHIP_TAIL].Scale.X, _velocity.LengthSquared() * .05f);

            //slowly rotate asteroid
            _asteroidSprite.Rotation += .002f;
            //calculate asteroid gravity
            var posA = new Vector2(-300, -300);
            var posB = new Vector2(_spriteRect.Center.X, _spriteRect.Center.Y);
            Vector2 directionToPlanet = (posA - posB);
            directionToPlanet.Normalize();
            float distance = Vector2.DistanceSquared(posB, posA);
            //SHIPMASS * MASS / distance * gravitySTR
            float gPull = 50 * 100 / distance * 1;
            _velocity += directionToPlanet * gPull;  

            //calculate new position
            _position += _velocity;
            _spriteRect.X = (int)_position.X;
            _spriteRect.Y = (int)_position.Y;

            //follow spaceship using camera
            if (_camera.IsTransitionActive)
                _camera.UpdateTransitionTarget(new Vector2(_spriteRect.X, _spriteRect.Y));
            else _camera.StartTransition(new Vector2(_spriteRect.X, _spriteRect.Y));

            //check velocity constraints
            _velocity.X = Math.Min(_velocity.X,  10f);
            _velocity.X = Math.Max(_velocity.X, -10f);
            _velocity.Y = Math.Min(_velocity.Y,  10f);
            _velocity.Y = Math.Max(_velocity.Y, -10f);
            #endregion
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
            _mBatch.Draw(_bg, new Rectangle(0, 0, _irr.VirtualWidth, _irr.VirtualHeight), Color.White);
            _mBatch.End();


            // GAME DRAWING using Camera2D
            _mBatch.BeginCamera(_camera, BlendState.NonPremultiplied);
            _asteroidSprite.Draw(gameTime, _mBatch, new Vector2(-300, -300));
            _ufoSprite.Draw(gameTime, _mBatch, _spriteRect);
            _mBatch.End();

            //reset screen viewport back to full size
            //so we can draw text from the TopLeft corner of the real screen
            _irr.SetupFullViewport();
            _mBatch.Begin();
            _mBatch.DrawString(_font, "Example 4. Sprite", new Vector2(0, 0), Color.White, .6f);
            _mBatch.DrawString(_font, "(http://www.panthernet.ru)", new Vector2(0, 20), Color.White, .6f);
            _mBatch.DrawString(_font, "Use Arrows to control space ship\nPress Escape to exit", new Vector2(0, 40), Color.White, .6f);
            _mBatch.End();
            _irr.SetupVirtualScreenViewport();

            base.Draw(gameTime);
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
