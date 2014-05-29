using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;


namespace Examples.Classes
{
    /// <summary>
    /// Particles manager class for particle logics handling
    /// </summary>
    public static class ParticlesManager
    {
        /// <summary>
        /// Particle logics list
        /// </summary>
        private static Dictionary<int, IParticlesLogic> Logics { get; set; }

        static ParticlesManager()
        {
            Logics = new Dictionary<int, IParticlesLogic>();
        }

        static SpriteBatch _mBatch;
        static Camera2D _camera;

        /// <summary>
        /// Load particle manager content
        /// </summary>
        /// <param name="game">Game object</param>
        /// <param name="camera">Camera2D object</param>
        public static void LoadContent(Game game, Camera2D camera = null)
        {
            _mBatch = new SpriteBatch(game.GraphicsDevice);
            _camera = camera;
        }

        public static void Update(GameTime gt)
        {
            //update all logics
            foreach (var item in Logics.Values)
                if(item.IsEnabled)
                    item.Update(gt);
            //remove all logics that are marked for dispose
            foreach(var item in Logics.Where(a => a.Value.ReadyToDispose))
            {
                item.Value.Dispose();
                Logics.Remove(item.Key);
            }
        }
        /// <summary>
        /// Draw all particles in logics
        /// </summary>
        /// <param name="gt">Game time</param>
        /// <param name="iBatch">Optional sprite batch for rendering</param>
        /// <param name="useNewDrawCycle">Use own new SpriteBatch::Begin() / End() cycle if True</param>
        public static void Draw(GameTime gt, SpriteBatch iBatch = null, bool useNewDrawCycle = true)
        {
            //select batch renderer, specified in params or default
            var batch = iBatch ?? _mBatch;

            foreach (var logic in Logics.Values)
            {
                if (useNewDrawCycle && _camera != null)
                    batch.BeginCamera(_camera, logic.BlendState);
                foreach (var item in logic.Particles)
                {
                    if (item.CanBeDeleted || item.Color.A == 0 || item.Scale == Vector2.Zero) continue;
                    batch.Draw(item.Texture, item.Position, null, item.Color, item.Rotation, item.Origin, item.Scale, SpriteEffects.None, item.Depth);
                }
                if (useNewDrawCycle && _camera != null) batch.End();
            }
        }

        /// <summary>
        /// Reset logic by Id or all logics at once
        /// </summary>
        /// <param name="id">Optional logic Id</param>
        public static void Reset(int id = -1)
        {
            if (id == -1)
            {
                foreach (var item in Logics.Values)
                    item.Reset();
            }
            else
            {
                if (Logics.ContainsKey(id))
                    Logics[id].Reset();
            }
        }

        /// <summary>
        /// Dispose and clear all logics
        /// </summary>
        public static void Dispose()
        {
            foreach (var item in Logics.Values)
                item.Dispose();
            Logics.Clear();
        }

        /// <summary>
        /// Add particle logic
        /// </summary>
        /// <param name="logic">Particle logic object</param>
        public static void AddLogic(IParticlesLogic logic)
        {
            Logics.Add(logic.LogicId, logic);
        }

        /// <summary>
        /// Add particle logic
        /// </summary>
        /// <param name="logicId">Logic Id</param>
        /// <param name="logic">Particle logic object</param>
        public static void AddLogic(int logicId, IParticlesLogic logic)
        {
            logic.LogicId = logicId;
            Logics.Add(logic.LogicId, logic);
        }

        /// <summary>
        /// Remove particle logic
        /// </summary>
        /// <param name="logicId">Logic Id</param>
        public static void RemoveLogic(int logicId)
        {
            if (Logics.ContainsKey(logicId))
            {
                Logics[logicId].Dispose();
                Logics.Remove(logicId);
            }
        }

        /// <summary>
        /// Returns logic object or null
        /// </summary>
        /// <param name="logicId">Logic Id</param>
        public static IParticlesLogic GetLogic(int logicId)
        {
            return Logics.ContainsKey(logicId) ? Logics[logicId] : null;
        }

        /// <summary>
        /// Returns logic object of specified type
        /// </summary>
        /// <typeparam name="T">Logic type</typeparam>
        /// <param name="logicId">Logic Id</param>
        public static T GetLogic<T>(int logicId)
        {
            return Logics.ContainsKey(logicId) ? (T) Logics[logicId] : (T) (object) null;
        }
    }
}
