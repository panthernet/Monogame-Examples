using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    /// <summary>
    /// Particles logic that generates afterburner effect particles
    /// </summary>
    public class AfterburnerParticlesLogic : ParticlesLogic
    {
        //Starting direction to determine actual direction using radian angle
        readonly Vector2 _upDirection = new Vector2(0, -1);

        public AfterburnerParticlesLogic(int logicId, Texture2D texture, bool isEnabled = true)
            : base(logicId, new List<Texture2D> { texture }, isEnabled)
        {
            if (texture == null)
                throw new Exception("AfterburnerParticlesLogic() -> Null texture!");
        }

        /// <summary>
        /// Generates particle object that represents afterburner cloud
        /// </summary>
        /// <param name="startPosition">Cloud start position</param>
        /// <param name="rotation">Host object rotation (to determine cloud shot direction)</param>
        public void GenerateParticle(Vector2 startPosition, float rotation)
        {
            var p = new Particle {Texture = Textures[0], Position = startPosition, Direction = Vector2.Transform(_upDirection, Matrix.CreateRotationZ(rotation))};
            //generate afterburner cloud shot direction
            //set AB shot initial velocity
            p.Velocity = -p.Direction * 6f;
            //set AB cloud initial scale
            p.Scale = Vector2.One * 0.9f;
            //set time to live
            p.Lifetime = 8000;
            p.LogicId = LogicId;
            Particles.Add(p);
            OnParticleCreated(p);
        }

        public override void Update(GameTime gt)
        {
            base.Update(gt); //this will remove all outdated particles

            //update all particles movement
            foreach (var item in Particles)
            {
                //update osition
                item.Position += item.Velocity;
                //slowly fade AB clouds
                var normalizedLifetime = item.TimeSinceStart / item.Lifetime;
                var alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
                item.Color = Color.White * alpha;
                //slowly scale cloud
                item.Scale = item.Scale * 1.02f;
                //slowly stop cloud velocity
                item.Velocity = item.Velocity * 0.8f;
                //if cloud is transparent - dispose
                if (item.Color.A > 0) continue;
                item.Dispose();
                item.Lifetime = 0;
            }
        }
    }
}
