using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Examples.Classes
{
    /// <summary>
    /// Particle object
    /// </summary>
    public class Particle : IDisposable
    {
        #region Modifiers
        /// <summary>
        /// Gets or sets unique rotation speed for that particle
        /// </summary>
        public float RotationSpeed { get; set; }
        #endregion

        /// <summary>
        /// Optional particle ID or particle type ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets particle direction
        /// </summary>
        public Vector2 Direction { get; set; }
        /// <summary>
        /// Gets or sets particle position
        /// </summary>
        public Vector2 Position { get; set; }
        /// <summary>
        /// Gets or sets particle velocity
        /// </summary>
        public Vector2 Velocity { get; set; }
        /// <summary>
        /// Gets or sets particle overlay color
        /// </summary>
        public Color Color { get; set; }

        public Vector2 Acceleration { get; set; }

        /// <summary>
        /// Gets or sets particle scale
        /// </summary>
        public Vector2 Scale { get; set; }

        public Vector2 InitialScale { get; set; }

        private Vector2 _origin;
        /// <summary>
        /// Gets particle origin (always centered and refreshed automaticaly)
        /// </summary>
        public Vector2 Origin { get { return _origin; } }
        /// <summary>
        /// Gets or sets particle rotation
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// Gets or sets particle rendering depth
        /// </summary>
        public float Depth { get; set; }
        /// <summary>
        /// Gets or sets particle time to live
        /// </summary>
        internal float Lifetime { get; set; }
        /// <summary>
        /// Gets or sets if this particle can be deleted from list (shoul be fone automaticaly from ParticleLogic class)
        /// </summary>
        internal bool CanBeDeleted { get { return TimeSinceStart > Lifetime; } }
        /// <summary>
        /// Gets or sets particle parent logic Id
        /// </summary>
        internal int LogicId { get; set; }

        private Texture2D _texture;
        /// <summary>
        /// Gets or sets particle texture
        /// </summary>
        public Texture2D Texture { get { return _texture; } set
        {
            _texture = value;
            _origin = _texture == null ? Vector2.Zero : new Vector2(_texture.Width * .5f, _texture.Height * .5f);
        }
        }

        public float TimeSinceStart { get; set; }

        public Particle()
        {
            Scale = Vector2.One;
            Color = Color.White;
            TimeSinceStart = 0f;
        }

        public void Dispose()
        {
            _texture = null;
        }
        public void Update(GameTime gt)
        {
        }
    }
}
