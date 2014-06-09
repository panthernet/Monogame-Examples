using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Examples.Classes
{
    /// <summary>
    /// UI Designer Component is made to simplify the proccess of getting screen coordinates
    /// for a bunch of UI textures. Generaly, you can easily arrange UI textures as you like and get all the coords for code.
    /// As we can use IRR we always can have the same virtual resolution so received coordinates will always be actual.
    /// It is not bound to InputHelper for portablity purposes and ResolutionRenderer can be easily cut off if you want.
    /// Made by PantheR (http://www.panthernet.ru)
    /// </summary>
    public class UIDesignerComponent : DrawableGameComponent
    {
        /// <summary>
        /// Textures collection that we want to place on the screen
        /// </summary>
        public List<MyTuple<Texture2D, Rectangle, string>> Textures { get; set; }
        /// <summary>
        /// Currently selected texture
        /// </summary>
        public Texture2D SelectedTexture { get; private set; }
        /// <summary>
        /// Texture bounds (screen coordinates of the selected texture)
        /// </summary>
        public Rectangle TextureBounds;
        /// <summary>
        /// Currently selected texture index
        /// </summary>
        private int _textureIndex;
        /// <summary>
        /// Reserve copy of initial bounds
        /// </summary>
        private Rectangle _savedInitialRect;
        private SpriteBatch _mBatch;
        /// <summary>
        /// Stored IRR from constructor params
        /// </summary>
        private readonly ResolutionRenderer _irr;
        /// <summary>
        /// Storage for previously pressed keys
        /// </summary>
        private Keys[] _prevKeys = { };
        private MouseState _prevMouseState;
        private bool _isDragging;
        private bool _isResizing;
        private readonly bool _showTutorialInfo;
#if WINDOWS
        private readonly UIDesigner _wfDesigner;
#endif
        /// <summary>
        /// Internal trigger for onscreen text visibility
        /// </summary>
        private bool _textVisible = true;
        /// <summary>
        /// Font that will be used to draw text in this component
        /// </summary>
        public SpriteFont Font { get; set; }
        /// <summary>
        /// Optional font scale
        /// </summary>
        public float FontScale { get; set; }
        /// <summary>
        /// Value that is used to incr/decr texture position and sizes
        /// </summary>
        public int ShiftSize { get; set; }
        /// <summary>
        /// Gets or sets selected texture distinction using red tint
        /// </summary>
        public bool UseRedTintOnSelectedTexture { get; set; }
        private readonly Game _game;

        /// <summary>
        /// UI Designer component constructor
        /// </summary>
        /// <param name="game">Current Game instance</param>
        /// <param name="contentPath">Path under default ContentManager to load images from (eg. if default CM path is 'Content' then we will be looking for images in 'Content/contentPath')</param>
        /// <param name="font">Font that will be used for text drawing</param>
        /// <param name="texture">Texture that will be displayed</param>
        /// <param name="initialRect">Initial texture bounds rectangle</param>
        /// <param name="displayName">Optional texture display name</param>
        /// <param name="resol">Optional IRR instance</param>
        public UIDesignerComponent(Game game, string contentPath, SpriteFont font, Texture2D texture, Rectangle initialRect, string displayName, ResolutionRenderer resol = null)
            : this(game, contentPath, font, new List<MyTuple<Texture2D, Rectangle, string>> { new MyTuple<Texture2D, Rectangle, string>(texture, initialRect, displayName) }, resol)
        {
        }


        /// <summary>
        /// UI Designer component constructor
        /// </summary>
        /// <param name="game">Current Game instance</param>
        /// <param name="contentPath">Path under default ContentManager to load images from (eg. if default CM path is 'Content' then we will be looking for images in 'Content/contentPath')</param>
        /// <param name="font">Font that will be used for text drawing</param>
        /// <param name="list">Texture+Bounds+Name data list</param>
        /// <param name="resol">Optional IRR instance</param>
        /// <param name="showTutorialInfo"></param>
        public UIDesignerComponent(Game game, string contentPath, SpriteFont font, List<MyTuple<Texture2D, Rectangle, string>> list, ResolutionRenderer resol = null, bool showTutorialInfo = false)
            : base(game)
        {
            _game = game;
            if (list == null) Textures = new List<MyTuple<Texture2D, Rectangle, string>>();
            else
            {
                Textures = list;
                SelectedTexture = Textures[_textureIndex].Item1;
                TextureBounds = _savedInitialRect = Textures[_textureIndex].Item2;
                _tmpWidth = TextureBounds.Width;
                _tmpHeight = TextureBounds.Height;
            }
            _irr = resol;
            _showTutorialInfo = showTutorialInfo;
            Font = font;
            DrawOrder = 102; //set high draw order to be visible among the other components
            FontScale = 1f;
            ShiftSize = 3;
#if WINDOWS
            _wfDesigner = new UIDesigner(Game.Content,contentPath, list);
#endif
        }

        protected override void LoadContent()
        {
            if (_mBatch == null)
                _mBatch = new SpriteBatch(Game.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            if (_mBatch == null)
                _mBatch = new SpriteBatch(Game.GraphicsDevice);
            //Begin sprite batch and use IRR if available
            if (_irr != null)
                _mBatch.BeginResolution(_irr, BlendState.AlphaBlend);
            else _mBatch.Begin();
            //draw texture if we have one
            for (int i = 0; i < Textures.Count; i++)
            {
                if (Textures[i].Item1 != null && Textures[i].Item2 != Rectangle.Empty)
                    _mBatch.Draw(Textures[i].Item1, Textures[i].Item2, i == _textureIndex && UseRedTintOnSelectedTexture ? Color.Red : Color.White);
            }
            //draw text if visibility allows
            _mBatch.End();

            if (_textVisible)
            {
                _mBatch.Begin();
                _mBatch.DrawString(Font, string.Format("UIDesigner active: {0} ({1})", SelectedTexture == null ? "" : System.IO.Path.GetFileName(SelectedTexture.Name), SelectedTexture == null ? "" : Textures[_textureIndex].Item3), new Vector2(0, 0), Color.White, FontScale);
                _mBatch.DrawString(Font, string.Format("Shift size: {0}  Bounds: [{1},{2}] [{3},{4}]", ShiftSize, TextureBounds.X, TextureBounds.Y, TextureBounds.Width, TextureBounds.Height), new Vector2(0, 20), Color.White, FontScale);
                if (_showTutorialInfo)
                    _mBatch.DrawString(Font, string.Format("Press TAB or LMB click to switch textures\nPress D to open control window\nPress C to copy coordinates to clipboard\nPress R to reset bounds\nPress V to toggle screen text\nPress ARROWS or Drag by mouse to move texture\nPress ARROWS or RMB(+shift to save aspect) to resize txture)\nPress +/- to change shift size"), new Vector2(0, 40), Color.White, FontScale);
                _mBatch.End();
            }
        }


        public override void Update(GameTime gameTime)
        {
            if(!_game.IsActive) return;
            //process key presses
            var state = Keyboard.GetState();
            var pkeys = state.GetPressedKeys();
            var mState = Mouse.GetState();
#if WINDOWS
            if (pkeys.Contains(Keys.C))
            {
                //Copy texture rectangle bounds into the clipboard for easy pasting
                System.Windows.Forms.Clipboard.SetText(string.Format("{0},{1},{2},{3}", TextureBounds.X, TextureBounds.Y, TextureBounds.Width, TextureBounds.Height));
            }
#endif

            if (pkeys.Contains(Keys.Tab) && !_prevKeys.Contains(Keys.Tab))
            {
                _textureIndex++;
                if (_textureIndex >= Textures.Count) _textureIndex = 0;
                SelectedTexture = Textures[_textureIndex].Item1;
                TextureBounds = _savedInitialRect = Textures[_textureIndex].Item2;
                _tmpWidth = TextureBounds.Width;
                _tmpHeight = TextureBounds.Height;
            }

            //control the shift size value
            if (pkeys.Contains(Keys.OemMinus) && !_prevKeys.Contains(Keys.OemMinus))
                ShiftSize -= 1;
            if (pkeys.Contains(Keys.OemPlus) && !_prevKeys.Contains(Keys.OemPlus))
                ShiftSize += 1;
            //control text visibility
            if (pkeys.Contains(Keys.V) && !_prevKeys.Contains(Keys.V))
                _textVisible = !_textVisible;
            //control bounds reset
            if (pkeys.Contains(Keys.R) && !_prevKeys.Contains(Keys.R))
            {
                TextureBounds = _savedInitialRect;
                _tmpWidth = TextureBounds.Width;
                _tmpHeight = TextureBounds.Height;
            }
            //control bounds position and size
            if (pkeys.Contains(Keys.Right))
                if (pkeys.Contains(Keys.LeftShift)) TextureBounds.Width += ShiftSize;
                else TextureBounds.X += ShiftSize;
            if (pkeys.Contains(Keys.Left))
                if (pkeys.Contains(Keys.LeftShift)) TextureBounds.Width -= ShiftSize;
                else TextureBounds.X -= ShiftSize;
            if (pkeys.Contains(Keys.Up))
                if (pkeys.Contains(Keys.LeftShift)) TextureBounds.Height -= ShiftSize;
                else TextureBounds.Y -= ShiftSize;
            if (pkeys.Contains(Keys.Down))
                if (pkeys.Contains(Keys.LeftShift)) TextureBounds.Height += ShiftSize;
                else TextureBounds.Y += ShiftSize;
#if WINDOWS
            if (pkeys.Contains(Keys.D) && !_prevKeys.Contains(Keys.D))
            {
                _wfDesigner.UpdateData(Textures);
                if (_wfDesigner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    Textures = _wfDesigner.ResultList;
                _textureIndex = 0;
                if (Textures.Count > 0)
                {
                    SelectedTexture = Textures[_textureIndex].Item1;
                    TextureBounds = _savedInitialRect = Textures[_textureIndex].Item2;
                    _tmpWidth = TextureBounds.Width;
                    _tmpHeight = TextureBounds.Height;
                }
            }
#endif
            #region Mouse resize
            if (mState.RightButton == ButtonState.Pressed && _prevMouseState.RightButton == ButtonState.Released)
                _isResizing = true;
            if (_isResizing)
            {
                var shiftX = _prevMouseState.X - mState.X;
                var shiftY = _prevMouseState.Y - mState.Y;
                if (shiftX != 0)
                {
                    if (pkeys.Contains(Keys.LeftShift))
                    {
                        var tWidth = _tmpWidth - shiftX;
                        if (tWidth >= .001)
                        {
                            // Figure out the ratio
                            var ratioX = tWidth / _tmpWidth;
                            // now we can get the new height and width
                            _tmpHeight = (_tmpHeight * ratioX);
                            _tmpWidth = (_tmpWidth * ratioX);
                            TextureBounds.Height = (int)_tmpHeight;
                            TextureBounds.Width = (int)_tmpWidth;
                        }
                    }
                    else
                    {
                        _tmpWidth -= shiftX;
                        _tmpHeight -= shiftY;
                        TextureBounds.Width = (int)_tmpWidth;
                        TextureBounds.Height = (int)_tmpHeight;
                    }
                }
            }

            if (mState.RightButton == ButtonState.Released && _isResizing)
                _isResizing = false;
            #endregion


            if (Textures.Count > 0)
            {
                //correct bounds if we are about to go off screen
                if (_tmpWidth < .001f) _tmpWidth = .001f;
                if (_tmpHeight < .001f) _tmpHeight = .001f;
                if (TextureBounds.X < 0) TextureBounds.X = 0;
                if (TextureBounds.Y < 0) TextureBounds.Y = 0;
                if (TextureBounds.X > _irr.VirtualWidth) TextureBounds.X = _irr.VirtualWidth;
                if (TextureBounds.Y > _irr.VirtualHeight) TextureBounds.Y = _irr.VirtualHeight;
                TextureBounds.Width = (int)_tmpWidth;
                TextureBounds.Height = (int)_tmpHeight;
                Textures[_textureIndex].Item2 = TextureBounds;
            }
            _prevKeys = pkeys;

            #region Mouse dragging
            //start drag
            if (mState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                //select texture under the mouse if any
                var coords = _irr == null ? new Point(mState.X, mState.Y) : _irr.ToVirtual(new Point(mState.X, mState.Y));
                for (int i = 0; i < Textures.Count; i++)
                    if (Textures[i].Item2.Contains(coords.X, coords.Y))
                    {
                        if (_textureIndex != i)
                        {
                            _textureIndex = i;
                            SelectedTexture = Textures[_textureIndex].Item1;
                            TextureBounds = _savedInitialRect = Textures[_textureIndex].Item2;
                            _tmpHeight = TextureBounds.Height;
                            _tmpWidth = TextureBounds.Width;
                        }
                        //start drag selected texture
                        _isDragging = true;
                        break;
                    }
            }
            if (_isDragging)
            {
                var shiftX = _prevMouseState.X - mState.X;
                var shiftY = _prevMouseState.Y - mState.Y;
                TextureBounds.X -= shiftX;
                TextureBounds.Y -= shiftY;
            }

            if (mState.LeftButton == ButtonState.Released && _isDragging)
                _isDragging = false;
            #endregion



            _prevMouseState = mState;
        }

        private float _tmpHeight;
        private float _tmpWidth;

        /// <summary>
        /// My own implementation of Tuple using fields instead of properties.
        /// This allows the values to be changed right inside this class.
        /// </summary>
        /// <typeparam name="T1">Type 1</typeparam>
        /// <typeparam name="T2">Type 2</typeparam>
        /// <typeparam name="T3">Type 3</typeparam>
        public class MyTuple<T1, T2, T3>
        {
            public T1 Item1;
            public T2 Item2;
            public T3 Item3;

            public MyTuple(T1 i1, T2 i2, T3 i3)
            {
                Item1 = i1; Item2 = i2; Item3 = i3;
            }
        }
    }
}