using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Examples.Classes
{
    public interface ISpriteTexture
    {
        /// <summary>
        /// Gets or sets texture visibility
        /// </summary>
        bool Visibility { get; set; }
        /// <summary>
        /// Gets or sets if this texture don't affected by visibility changes
        /// </summary>
        bool IsFixedVisibility { get; set; }
        /// <summary>
        /// Gets or sets actual texture
        /// </summary>
        Texture2D Texture { get; set; }
        /// <summary>
        /// Gets or sets texture drawing depth (-1 - use default sprite depth)
        /// </summary>
        float Depth { get; set; }
        /// <summary>
        /// Gets or sets texture offset while drawing using sprite
        /// </summary>
        Vector2 Offset { get; set; }
        /// <summary>
        /// Gets or sets texture scale
        /// </summary>
        Vector2 Scale { get; set; }

        Vector2 Origin { get; set; }

        ISpriteTexture Clone();
    }
}