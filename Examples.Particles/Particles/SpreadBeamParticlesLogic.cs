using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    /// <summary>
    /// Particles logic that generates laser beams slightly spread by the direction
    /// </summary>
    public class SpreadBeamParticlesLogic : ParticlesLogic
    {
        private readonly Random _rnd = new Random();
        //Starting direction to determine actual direction using radian angle
        readonly Vector2 _upDirection = new Vector2(0, -1);

        public SpreadBeamParticlesLogic(int logicId, Texture2D texture, bool isEnabled = true)
            : base(logicId, new List<Texture2D> { texture }, isEnabled)
        {
            if (texture == null)
                throw new Exception("AfterburnerParticlesLogic() -> Null texture!");
            BlendState = BlendState.Additive;
        }

        /// <summary>
        /// Generates particle object that  represents laser beam
        /// </summary>
        /// <param name="startPosition">Beam start position</param>
        /// <param name="rotation">Host object rotation (to determine beam direction)</param>
        public void GenerateBeam(Vector2 startPosition, float rotation)
        {
            var p = new Particle { Texture = Textures[0], Position = startPosition };
            //modifier for different beam spreading
            var mod = _rnd.Next(1) == 0 ? -1 : 1;
            //determine slightly spreaded rotation
            p.Rotation = rotation + (float)_rnd.NextDouble() * .2f * mod;
            //get direction from rotation radian angle
            p.Direction = Vector2.Transform(_upDirection, Matrix.CreateRotationZ(p.Rotation));
            //set final velocity
            p.Velocity = p.Direction * 15f;
            //set time to live
            p.Lifetime = 8000;
            p.LogicId = LogicId;
            Particles.Add(p);
            OnParticleCreated(p);
        }

        /// <summary>
        /// Update logic
        /// </summary>
        /// <param name="gt">Game time</param>
        public override void Update(GameTime gt)
        {
            base.Update(gt); //this will remove all outdated particles

            //update all particles movement
            foreach (var item in Particles)
            {
                item.Position += item.Velocity;
            }
        }
    }
}
