using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Examples.Classes
{
    public class Sprite : IDisposable, ISprite
    {
        /// <summary>
        /// Stores solid non-animated texture (if any)
        /// </summary>
        public Dictionary<int, ISpriteTexture> Textures { get; set; }

        /// <summary>
        /// Gets or sets if sprite can be composed into single solid image for fast rendering
        /// </summary>
        public bool CanBeComposed { get; set; }

        private Vector2 _scale = Vector2.One;
        /// <summary>
        /// Sprite scale
        /// </summary>
        public Vector2 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
            }
        }

        /// <summary>
        /// Sprite rotation
        /// </summary>
        public float Rotation { get; set; }

        private int _transparency = 255;
        /// <summary>
        /// Sprite transparency
        /// </summary>
        public int Transparency
        {
            get
            {
                return _transparency;
            }
            set
            {
                _transparency = value;
            }
        }

        /// <summary>
        /// Default sprite drawing depth if not overriden by current sprite texture settings
        /// </summary>
        public float Depth { get; set; }

        /// <summary>
        /// Default sprite drawing origin
        /// </summary>
        public Vector2 Origin { get; set; }

        public void Dispose()
        {
            if (Textures != null)
                Textures.Clear();
        }

        /// <summary>
        /// Show specified texture by ID and hide all others if their IsFixedVisibility prop isn't set to True
        /// </summary>
        /// <param name="id">texture id</param>
        public void ShowExclusiveTexture(int id)
        {
            if (Textures.ContainsKey(id))
            {
                foreach (var tex in Textures)
                {
                    if (tex.Value.IsFixedVisibility) continue;
                    tex.Value.Visibility = tex.Key == id;
                }
            }
        }

        public Sprite()
        {
            Rotation = 0f;
            Transparency = 255;
            Textures = new Dictionary<int, ISpriteTexture>();
            CanBeComposed = true;
        }

        public Sprite(Dictionary<int, ISpriteTexture> tex)
            : this()
        {
            Textures = tex;
        }

        public Sprite(int textureId, SpriteTexture tex)
            : this()
        {
            Textures.Add(textureId, tex);
        }

        public Sprite(int textureId, Texture2D tex):this()
        {
            Textures.Add(textureId, new SpriteTexture(tex, true));
            Origin = Vector2.Zero;
        }

        public Sprite(Texture2D tex, float depth = -1f)
            : this()
        {
            Textures.Add(0, new SpriteTexture(tex, true, depth));
        }

        /// <summary>
        /// Gets source texture rectangle to render
        /// </summary>
        private Rectangle GetFr(int textureKey = 0)
        {
            return new Rectangle(0, 0, Textures[textureKey].Texture.Width, Textures[textureKey].Texture.Height);                
        }

        public void Draw(GameTime gt, SpriteBatch mbatch, Vector2 destPos, SpriteEffects effect = SpriteEffects.None)
        {
            Draw(gt, mbatch, new Rectangle((int)destPos.X, (int)destPos.Y, 0, 0), effect);
        }

        public void Draw(GameTime gt, SpriteBatch mbatch, Rectangle destRectangle, SpriteEffects effect = SpriteEffects.None)
        {
            var innerOrigin = Origin;
            bool ownSizes = destRectangle.Width == 0;
            //iterate through all available textures
            foreach (var item in Textures)
            {
                //if texture isn't visible - continue
                if (item.Value.Visibility == false) continue;
                var key = item.Key;
                var sz = GetTextureSize(key);
                //if we use vector as position then rectangle width and height we will get from texture
                if (ownSizes)
                {
                    destRectangle.Width = (int)sz.X;
                    destRectangle.Height = (int)sz.Y;
                }
                if (item.Value is SpriteTexture)
                {
                    if (Rotation != 0 && Origin == Vector2.Zero)
                    {
                        //for centered sprite rotation we always set Origin to texture center
                        innerOrigin = new Vector2(sz.X * .5f, sz.Y * .5f);
                    }
                    //now we take an optional offset, anyway if none specified we use offrec later
                    var offrec = new Rectangle(destRectangle.Left + (int)item.Value.Offset.X, destRectangle.Top + (int)item.Value.Offset.Y, destRectangle.Width, destRectangle.Height);

                    //get scale value relative to global Sprite::Scale
                    var texScale = item.Value.Scale != Vector2.One ? Scale - (Scale - item.Value.Scale) : Scale;
                    if (texScale != Vector2.One)
                    {
                        //if we specified some scaling we use custom rectangle inflating to scale sprite and specify rectangle bounds at the same time
                        //(handy when texture is already scaled by destination rectangle and we need to scale it again relatively)
                        var inflateX = texScale.X == 1f ? 0 : (int)(offrec.Width * texScale.X - offrec.Width);
                        var inflateY = texScale.Y == 1f ? 0 : (int)(offrec.Height * texScale.Y - offrec.Height);

                        var hInflX = inflateX * .5f;
                        var hInflY = inflateY * .5f;
                        //If origin is Zero then we inflate rectangle in all directions
                        if (Origin == Vector2.Zero)
                        {
                            offrec.X -= (int)hInflX;
                            offrec.Y -= (int)hInflY;
                            offrec.Width += (int)hInflX;
                            offrec.Height += (int)hInflY;
                        }
                        else //else we just change width and height
                        {
                            offrec.Width += inflateX;
                            offrec.Height += inflateY;
                        }
                    }

                    if (texScale != Vector2.Zero)
                        mbatch.Draw(Textures[key].Texture, offrec, GetFr(key), Color.FromNonPremultiplied(255, 255, 255, Transparency), Rotation, innerOrigin, effect, item.Value.Depth == -1 ? Depth - key * 0.01f : item.Value.Depth);
                }
                else
                {
                    innerOrigin = item.Value.Origin;

                    //get scale value relative to global Sprite::Scale
                    var texScale = Scale * item.Value.Scale;
                    if (texScale.X < 0) texScale.X = 0;
                    if (texScale.Y < 0) texScale.Y = 0;
                    //scale offset by the main Sprite::Scale factor
                    var texOffset = item.Value.Offset * Scale;
                    //calculate new position based on Rotation value
                    //new position will be relative to specified destRectangle
                    var posA = new Vector2(destRectangle.X + texOffset.X, destRectangle.Y + texOffset.Y);
                    var posB = new Vector2(destRectangle.X, destRectangle.Y);
                    var offvec = (Vector2.Transform(posA - posB, Matrix.CreateRotationZ(Rotation)) + posB);

                    if (texScale != Vector2.Zero)
                        mbatch.Draw(Textures[key].Texture, offvec, GetFr(key), Color.FromNonPremultiplied(255, 255, 255, Transparency), Rotation, innerOrigin, texScale, effect, item.Value.Depth == -1 ? Depth - key * 0.01f : item.Value.Depth);
                }
            }
        }

        /// <summary>
        /// Returns texture size by ID
        /// </summary>
        /// <param name="textureId">Texture ID</param>
        public Vector2 GetTextureSize(int textureId)
        {
            if (Textures == null || Textures.Count == 0) return Vector2.Zero;
            return new Vector2(Textures[textureId].Texture.Width, Textures[textureId].Texture.Height);
        }

        /// <summary>
        /// Merges specified sprite into the current one
        /// </summary>
        /// <param name="sprite">Second sprite</param>
        public void MergeSprite(Sprite sprite)
        {
            if (sprite == null) { Debug.WriteLine("MergeSprite() -> Got null sprite!"); return; }
            int keyTexture = 0;
            if(Textures.Keys.Count > 0)
                keyTexture = Textures.Keys.Last() + 1;

            foreach (var item in sprite.Textures)
            {
                int texId = keyTexture;
                if (!Textures.ContainsKey(item.Key))
                {
                    texId = item.Key;
                    Textures.Add(texId, item.Value.Clone());
                }
                else
                {
                    Textures.Add(texId, item.Value.Clone());
                    keyTexture++;
                }
            }
        }

    }
}