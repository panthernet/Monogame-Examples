using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    /// <summary>
    /// Core particles logic class
    /// </summary>
    public abstract class ParticlesLogic: IParticlesLogic
    {
        #region Particle properties
        public List<Texture2D> Textures { get; set; }

        // high initial speed with lots of variance.  make the values closer
        // together to have more consistently circular explosions.
        public float MinInitialSpeed = 40;
        public float MaxInitialSpeed = 80;

        // doesn't matter what these values are set to, acceleration is tweaked in
        // the override of InitializeParticle.
        public float MinAcceleration = 0;
        public float MaxAcceleration = 0;

        // explosions should be relatively short lived
        public float MinLifetime = .5f;
        public float MaxLifetime = 1.0f;

        public float MinScale = .3f;
        public float MaxScale = 1.0f;

        // we need to reduce the number of particles on Windows Phone in order to keep
        // a good framerate
#if WINDOWS_PHONE
        public float MinNumParticles = 10;
        public float MaxNumParticles = 12;
#else
        public int MinNumParticles = 15;
        public int MaxNumParticles = 20;
#endif

        public float MinRotationSpeed = -MathHelper.PiOver4;
        public float MaxRotationSpeed = MathHelper.PiOver4;

        #endregion

        /// <summary>
        /// Event that fired when new particle is created
        /// </summary>
        public event CreateParticleEHandler ParticleCreated;
        /// <summary>
        /// Gets or sets is logic enabled
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Gets if logic is marked for dispose
        /// </summary>
        public bool ReadyToDispose { get; private set; }
        /// <summary>
        /// Logics unique id for PariclesManager
        /// </summary>
        public int LogicId { get; set; }
        /// <summary>
        /// Gets particles list
        /// </summary>
        public List<Particle> Particles { get; private set; }

        public BlendState BlendState { get; set; }

        protected Random Random = new Random();

        protected ParticlesLogic(int logicId, List<Texture2D> textures, bool isEnabled = true)
        {
            LogicId = logicId;
            Particles = new List<Particle>();
            IsEnabled = isEnabled;
            BlendState = BlendState.NonPremultiplied;
            Textures = textures;
        }

        internal void OnParticleCreated(Particle p)
        {
            if (ParticleCreated != null)
                ParticleCreated(p);
        }

        public virtual void Update(GameTime gt)
        {
            foreach (var item in Particles)
            {
                //decrease particle ttl
                item.TimeSinceStart += (float)gt.ElapsedGameTime.TotalSeconds;
                //item.TimeToLive -= gt.ElapsedGameTime.TotalMilliseconds;
                if (item.CanBeDeleted)
                {
                    item.Dispose();
                    continue;
                }
                item.Update(gt);
            }
            //remove all marked particles
            Particles.RemoveAll(a => a.CanBeDeleted);
        }

        /// <summary>
        /// Reset logic (clear all particles)
        /// </summary>
        public virtual void Reset()
        {
            foreach (var item in Particles)
                item.Dispose();
            Particles.Clear();
        }

        public virtual void Dispose() { }

        protected virtual Vector2 PickRandomDirection()
        {
            float angle = RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        protected float RandomBetween(float min, float max)
        {
            return min + (float)Random.NextDouble() * (max - min);
        }
    }
}
