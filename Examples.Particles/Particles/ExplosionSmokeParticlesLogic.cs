using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    /// <summary>
    /// Under development
    /// </summary>
    public class ExplosionSmokeParticlesLogic: ParticlesLogic
    {
        public int SmokesMinCount { get; set; }
        public int SmokesMaxCount { get; set; }

        public ExplosionSmokeParticlesLogic(int logicId, List<Texture2D> textures, bool isEnabled)
            : base(logicId, textures, isEnabled)
        {
            SmokesMinCount = 4;
            SmokesMaxCount = 6;

        }

        protected void InitializeConstants()
        {
            // less initial speed than the explosion itself
            MinInitialSpeed = 20;
            MaxInitialSpeed = 50;

            // acceleration is negative, so particles will accelerate away from the
            // initial velocity.  this will make them slow down, as if from wind
            // resistance. we want the smoke to linger a bit and feel wispy, though,
            // so we don't stop them completely like we do ExplosionParticleSystem
            // particles.
            MinAcceleration = -5;
            MaxAcceleration = -10;

            // explosion smoke lasts for longer than the explosion itself, but not
            // as long as the plumes do.
            MinLifetime = 1.0f;
            MaxLifetime = 2.0f;

            MinScale = .5f;
            MaxScale = 1.0f;

            // we need to reduce the number of particles on Windows Phone in order to keep
            // a good framerate
#if WINDOWS_PHONE
            MinNumParticles = 5;
            MaxNumParticles = 10;
#else
            MinNumParticles = 10;
            MaxNumParticles = 20;
#endif

            MinRotationSpeed = -MathHelper.PiOver4;
            MaxRotationSpeed = MathHelper.PiOver4;

            BlendState = BlendState.AlphaBlend;

        }

        public void GenerateExplosion(Vector2 center, float radius)
        {
            if (Textures.Count == 0)
                throw new Exception("ExplosionSmokeParticlesLogic() -> Textures not set!");

            InitializeConstants();

            var smokesCount = Random.Next(SmokesMinCount, SmokesMaxCount);
            for (int i = 0; i < smokesCount; i++)
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
                float normalizedLifetime = item.TimeSinceStart / item.Lifetime;

                float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
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
