using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Examples.Classes
{
    /// <summary>
    /// Under development
    /// </summary>
    public class ExplosionSparksParticlesLogic: ParticlesLogic
    {
        public int SparksMinCount { get; set; }
        public int SparksMaxCount { get; set; }

        //Starting direction to determine actual direction using radian angle
        readonly Vector2 _upDirection = new Vector2(0, -1);

        public ExplosionSparksParticlesLogic(int logicId, List<Texture2D> textures, bool isEnabled)
            : base(logicId, textures, isEnabled)
        {
            SparksMinCount = 4;
            SparksMaxCount = 8;
            BlendState = BlendState.Additive;
        }

        public void GenerateExplosion(Vector2 center, float radius)
        {
            if (Textures.Count == 0)
                throw new Exception("ExplosionParticlesLogic() -> Textures not set!");


            var sparksCount = Random.Next(SparksMinCount, SparksMaxCount);
            for (var i = 0; i < sparksCount; i++)
            {
                Particles.Add(new Particle
                {
                    Texture = Textures[Random.Next(0, Textures.Count - 1)],
                    Position = center,
                    Lifetime = .5f,
                    Scale = new Vector2(.6f, .6f),
                    Rotation = Random.Next()
                });
                var p = Particles.Last();
                p.Velocity = Vector2.Transform(_upDirection, Matrix.CreateRotationZ(p.Rotation)) * 6;
            }

        }

        public override void Update(GameTime gt)
        {
            base.Update(gt); //this will remove all outdated particles

            //update all particles movement
            foreach (var item in Particles)
            {
                item.Position += item.Velocity; // move
                item.Rotation += item.RotationSpeed; //rotate
                var normalizedLifetime = item.TimeSinceStart / item.Lifetime;
                var alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                item.Color = Color.Orange * alpha;

                //if cloud is transparent - dispose
                if (item.Color.A <= 0)
                    item.Lifetime = 0;
            }
        }
    }
}
