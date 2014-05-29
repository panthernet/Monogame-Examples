using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    /// <summary>
    /// Under development
    /// </summary>
    public class ExplosionFireParticlesLogic: ParticlesLogic
    {
        public ExplosionFireParticlesLogic(int logicId, List<Texture2D> textures, bool isEnabled)
            : base(logicId, textures, isEnabled)
        {
            BlendState = BlendState.Additive;
        }

        public void GenerateExplosion(Vector2 center, float radius)
        {
            if (Textures.Count == 0)
                throw new Exception("ExplosionParticlesLogic() -> Textures not set!");

            var firesCount = Random.Next(MinNumParticles, MaxNumParticles);
            for (var i = 0; i < firesCount; i++)
            {
                var direction = PickRandomDirection();
                var scale = RandomBetween(MinScale, MaxScale);
                var p = new Particle
                {
                    Texture = Textures[Random.Next(0, Textures.Count - 1)],
                    Position = center,
                    Lifetime = RandomBetween(MinLifetime, MaxLifetime),
                    Velocity = RandomBetween(MinInitialSpeed, MaxInitialSpeed) * direction,
                    InitialScale = new Vector2(scale, scale),
                    Direction = direction,
                    Acceleration = RandomBetween(MinAcceleration, MaxAcceleration) * direction,
                    RotationSpeed = RandomBetween(MinRotationSpeed, MaxRotationSpeed)
                };
                p.Acceleration = -p.Velocity / p.Lifetime;

                Particles.Add(p);
            }


        }

        public override void Update(GameTime gt)
        {
            base.Update(gt); //this will remove all outdated particles

            var dt = (float)gt.ElapsedGameTime.TotalSeconds;
            //update all particles movement
            foreach (var item in Particles)
            {
                if (item.CanBeDeleted) continue;

                // normalized lifetime is a value from 0 to 1 and represents how far
                // a particle is through its life. 0 means it just started, .5 is half
                // way through, and 1.0 means it's just about to be finished.
                // this value will be used to calculate alpha and scale, to avoid 
                // having particles suddenly appear or disappear.
                var normalizedLifetime = item.TimeSinceStart / item.Lifetime;

                // we want particles to fade in and fade out, so we'll calculate alpha
                // to be (normalizedLifetime) * (1-normalizedLifetime). this way, when
                // normalizedLifetime is 0 or 1, alpha is 0. the maximum value is at
                // normalizedLifetime = .5, and is
                // (normalizedLifetime) * (1-normalizedLifetime)
                // (.5)                 * (1-.5)
                // .25
                // since we want the maximum alpha to be 1, not .25, we'll scale the 
                // entire equation by 4.
                var alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                item.Color = Color.White * alpha;

                // make particles grow as they age. they'll start at 75% of their size,
                // and increase to 100% once they're finished.
                item.Scale = item.InitialScale * (.75f + .25f * normalizedLifetime);

                item.Position += item.Velocity * dt; // move
                item.Rotation += item.RotationSpeed * dt; //rotate
            }
        }
    }
}
