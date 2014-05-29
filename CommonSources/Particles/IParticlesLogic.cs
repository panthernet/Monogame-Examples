using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Examples.Classes
{
    /// <summary>
    /// Common particles logic interface
    /// </summary>
    public interface IParticlesLogic : IDisposable
    {
        /// <summary>
        /// Event that fired when new particle is created
        /// </summary>
        event CreateParticleEHandler ParticleCreated;
        /// <summary>
        /// Gets particles list
        /// </summary>
        List<Particle> Particles { get; }
        /// <summary>
        /// Logic update
        /// </summary>
        /// <param name="gt">Game time</param>
        void Update(GameTime gt);
        /// <summary>
        /// Reset logic
        /// </summary>
        void Reset();
        /// <summary>
        /// Gets or sets is logic enabled
        /// </summary>
        bool IsEnabled { get; set; }
        /// <summary>
        /// Gets if logic is marked for dispose
        /// </summary>
        bool ReadyToDispose { get; }
        /// <summary>
        /// Logics unique id for PariclesManager
        /// </summary>
        int LogicId { get;  set; }

        BlendState BlendState { get; }
    }

    public delegate void CreateParticleEHandler(Particle p);
    //public delegate void DestroyAllEHandler(int logic_id);
}
