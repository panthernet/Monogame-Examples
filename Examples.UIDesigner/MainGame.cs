using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Examples.Classes;

// MonoGame Examples Collection
// Example 2. UI Designer Component.
// (http://www.panthernet.ru)
//
// Using UI designer component you can effectively position and resize
// specified texture ang get its coordinates and size right into the
// windows clipboard.
// To use it, simply add the component in LoadContent() method. 

namespace Examples.UIDesigner
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

        private Texture2D _bg;
        private SpriteFont _font;

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

            _bg = Content.Load<Texture2D>("wall_bg.jpg");
            var playTexture = Content.Load<Texture2D>("play.png");
            var optTexture = Content.Load<Texture2D>("options.png");
            var quitTexture = Content.Load<Texture2D>("quit.png");
            _font = Content.Load<SpriteFont>("sf");


            Components.Add(new UIDesignerComponent(this,"", _font, new List<UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>>
            {
                new UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>(playTexture,  new Rectangle(0,636,205,51), "Play"),
                new UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>(optTexture,  new Rectangle(1065,631,191,51), "Options"),
                new UIDesignerComponent.MyTuple<Texture2D, Rectangle, string>(quitTexture,  new Rectangle(1025,21,247,65), "Quit"),
            }, _irr, true) { FontScale = 0.5f, UseRedTintOnSelectedTexture = true });
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
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

            if (_input.GamePadState.Buttons.Back == ButtonState.Pressed || _input.IsKeyPressed(Keys.Escape))
            {
                Exit();
                return;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provtides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //IRR draw call to prepare the stuff
            _irr.Draw();

            //begin sprite drawing using resolution transform matrix
            //all independent resolution objects must be drawn using this matrix for proper scaling and sizing
            //all coordinates must be specified in virtual screen resolution size range

            //lets use new Extensions method for simplified code reading
            _mBatch.BeginResolution(_irr);
            _mBatch.Draw(_bg, new Rectangle(0, 0, _irr.VirtualWidth, _irr.VirtualHeight), Color.White);
            _mBatch.End();

            //reset screen viewport back to full size
            //so we can draw text from the TopLeft corner of the real screen
            _irr.SetupFullViewport();
            _mBatch.Begin();
            _mBatch.DrawString(_font, "Example2. UIDesigner (http://www.panthernet.ru)", new Vector2(0, 380), Color.White, 0f, Vector2.Zero, .5f, SpriteEffects.None, 0f);
            _mBatch.End();
            _irr.SetupVirtualScreenViewport();

            base.Draw(gameTime);
        }
    }
}
