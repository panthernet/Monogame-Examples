using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Examples.Classes
{
    /// <summary>
    /// Decal texture is rendered by Vector2 point that is calculated by [destination Rectangle + Offset].
    /// It always uses own Origin property for flexible tuning.
    /// It always rendered relative to destination Rectangle specified in Sprite::Draw() method
    /// It will be scaled and rotated automaticaly based on Sprite::Scale and Sprite::Rotate properties
    /// </summary>
    public class DecalTexture : ISpriteTexture, IDisposable
    {
        /// <summary>
        /// Gets or sets texture visibility
        /// </summary>
        public bool Visibility { get; set; }
        /// <summary>
        /// Gets or sets if this texture don't affected by visibility changes
        /// </summary>
        public bool IsFixedVisibility { get; set; }
        /// <summary>
        /// Gets or sets actual texture
        /// </summary>
        public Texture2D Texture { get; set; }
        /// <summary>
        /// Gets or sets texture drawing depth (-1 - use default sprite depth)
        /// </summary>
        public float Depth { get; set; }
        /// <summary>
        /// Gets or sets texture offset while drawing using sprite
        /// </summary>
        public Vector2 Offset { get; set; }
        /// <summary>
        /// Gets or sets texture scale
        /// </summary>
        public Vector2 Scale { get; set; }

        public Vector2 Origin { get; set; }

        public DecalTexture(Texture2D texture, bool visibility = false, float depth = -1f)
        {
            Texture = texture;
            Visibility = visibility;
            Depth = depth;
            Scale = Vector2.One;
        }

        public void Dispose()
        {
            Texture = null;
        }

        public DecalTexture()
        {
        }

        public ISpriteTexture Clone()
        {
            return new DecalTexture(Texture, Visibility, Depth) { Offset = Offset, IsFixedVisibility = IsFixedVisibility, Scale = Vector2.One, Origin = Origin };
        }
    }
}