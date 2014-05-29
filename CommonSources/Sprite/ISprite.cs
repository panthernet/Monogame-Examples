using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Examples.Classes
{
    public interface ISprite
    {
        void Draw(GameTime gt, SpriteBatch mbatch, Rectangle rec, SpriteEffects effect = SpriteEffects.None);
        float Depth { get; }
        int Transparency { get; }
    }
}
