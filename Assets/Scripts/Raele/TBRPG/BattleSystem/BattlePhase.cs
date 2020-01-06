using Raele.Util;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BattlePhase<T> : IBattlePhase where T : BattlePhase<T>
    {
        public bool HasStarted { get; private set; }
        public bool HasFinished { get; private set; }

        protected Battle Battle { get; private set; }
        protected BattleResolver Resolver { get; private set; }

        public BattlePhase(Battle battle, BattleResolver resolver)
        {
            this.Battle = battle;
            this.Resolver = resolver;
            this.HasStarted = false;
            this.HasFinished = false;
        }

        public virtual void Start()
        {
            this.HasFinished.ThenThrow("Trying to restart an already finished BattlePhase.");
            this.HasStarted.ThenThrow("Trying to restart an already started BattlePhase.");
            this.HasStarted = true;
        }

        public virtual void Update()
        {
            this.HasStarted.OtherwiseThrow("Trying to update a BattlePhase that didn't started.");
            this.HasFinished.ThenThrow("Trying to update an already finished BattlePhase.");
        }

        public virtual void Finish()
        {
            this.HasStarted.OtherwiseThrow("Trying to finish a BattlePhase that didn't started.");
            this.HasFinished.ThenThrow("Trying to finish an already finished BattlePhase.");
            this.HasFinished = true;
        }
    }
}
