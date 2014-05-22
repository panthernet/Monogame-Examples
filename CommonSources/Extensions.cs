using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Examples.Classes
{
    public static class Extensions
    {
        public static Rectangle NewInflate(this Rectangle rect, int width, int height)
        {
            var r = new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
            r.Inflate(width, height);
            return r;
        }

        public static void DrawString(this SpriteBatch mBatch, SpriteFont font, string text, Vector2 pos, Color color, float scale = 1f)
        {
            mBatch.DrawString(font, text, pos, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }

        public static void Draw(this SpriteBatch mBatch, Texture2D tex, Rectangle rec)
        {
            mBatch.Draw(tex, rec, Color.White);
        }

        public static void Draw(this SpriteBatch mBatch, Texture2D tex, Vector2 pos)
        {
            mBatch.Draw(tex, pos, Color.White);
        }

        public static void BeginResolution(this SpriteBatch mBatch, ResolutionRenderer renderer)
        {
            mBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, renderer.GetTransformationMatrix());
        }

        public static void BeginResolution(this SpriteBatch mBatch, ResolutionRenderer renderer, BlendState bstate)
        {
            mBatch.Begin(SpriteSortMode.Deferred, bstate, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, renderer.GetTransformationMatrix());
        }
    }
}