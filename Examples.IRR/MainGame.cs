#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Examples.Classes;
#endregion

// MonoGame Examples Collection
// Example 1. Independent resolution renderer.
// (http://www.panthernet.ru)
//
// Using IR renderer you can effectively scale displayed content to any
// screen resolution maintaining designed screen ratio (commonly 4:3 or
// 16:9). 
// To use it, simply run ResolutionRenderer.Draw() method and provide 
// calculated transformation matrix to SpriteBatch.Begin() method.


namespace Examples.IRR
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _gfx;
        private SpriteBatch _mBatch;
        private ResolutionRenderer _irr;
        private Texture2D _testTexture;
        private Rectangle _testRect;
        private Texture2D _bg;
        private SpriteFont _font;
        private InputHelper _input;
        private const int VIRTUAL_RESOLUTION_WIDTH = 1280;
        private const int VIRTUAL_RESOLUTION_HEIGHT = 720;

        public MainGame()
        {
            _gfx = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            _gfx.IsFullScreen = false;
            IsMouseVisible = true;
            //set real screen resolution
            _gfx.PreferredBackBufferWidth = 1000;
            _gfx.PreferredBackBufferHeight = 400;
            _gfx.PreferMultiSampling = true;
            _gfx.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
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
            _testTexture = Content.Load<Texture2D>("nlo.png");
            _font = Content.Load<SpriteFont>("sf");

            //preload UFO size rectangle using virtual screen coordinates
            _testRect = new Rectangle((int)_irr.VirtualScreenCenter.X - 50, (int)_irr.VirtualScreenCenter.Y - 50, 100, 100);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _testTexture.Dispose();
            _bg.Dispose();
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

            #region Input processing

            if (_input.GamePadState.Buttons.Back == ButtonState.Pressed ||       _input.IsKeyPressed(Keys.Escape))
            {
                Exit();
                return;
            }

            if (_input.IsKeyPressed(Keys.D1))
            {
                _gfx.PreferredBackBufferWidth = 1000;
                _gfx.PreferredBackBufferHeight = 400;
                _irr = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, _gfx.PreferredBackBufferWidth, _gfx.PreferredBackBufferHeight);
                _gfx.ApplyChanges();
                return;
            }
            if (_input.IsKeyPressed(Keys.D2))
            {
                _gfx.PreferredBackBufferWidth = 400;
                _gfx.PreferredBackBufferHeight = 500;
                _irr = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, _gfx.PreferredBackBufferWidth, _gfx.PreferredBackBufferHeight);
                _gfx.ApplyChanges();
                return;
            }
            if (_input.IsKeyPressed(Keys.D3))
            {
                _gfx.PreferredBackBufferWidth = 800;
                _gfx.PreferredBackBufferHeight = 480;
                _irr = new ResolutionRenderer(this, VIRTUAL_RESOLUTION_WIDTH, VIRTUAL_RESOLUTION_HEIGHT, _gfx.PreferredBackBufferWidth, _gfx.PreferredBackBufferHeight);
                _gfx.ApplyChanges();
                return;
            }

            //get virtal equivalent of mouse coordinaes
            var pos = _irr.ToVirtual(_input.MousePos);
            //check for mouse click in the UFO rectangle area
            if (InputHelper.IsPosInBound(pos, _testRect) && _input.IsMousePressed(MouseButton.Left))
                Exit();

            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Prepare IRR call
            _irr.Draw();

            //begin sprite drawing using resolution transform matrix
            //all independent resolution objects must be drawn using this matrix for proper scaling and sizing
            //all coordinates must be specified in virtual screen resolution size range
            _mBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, _irr.GetTransformationMatrix());
            _mBatch.Draw(_bg, new Rectangle(0, 0, _irr.VirtualWidth, _irr.VirtualHeight), Color.White);
            _mBatch.Draw(_testTexture, _testRect, Color.White);
            _mBatch.End();

            //reset screen viewport back to full size
            //so we can draw text from the TopLeft corner of the real screen
            _irr.SetupFullViewport();
            _mBatch.Begin();
            _mBatch.DrawString(_font, "Example1. Resolution independent renderer", Vector2.Zero, Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
            _mBatch.DrawString(_font, "(http://www.panthernet.ru)", new Vector2(0, 20), Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
            _mBatch.DrawString(_font, "Press 1,2,3 keys to change resolution", new Vector2(0, 40), Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
            _mBatch.DrawString(_font, "Press Escape key or click on the UFO to exit", new Vector2(0, 60), Color.White, 0f, Vector2.Zero, .7f, SpriteEffects.None, 0f);
            _mBatch.End();

            base.Draw(gameTime);
        }
    }
}
