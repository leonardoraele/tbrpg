using Raele.Util;

namespace Raele.TBRPG.BattleSystem.BattlePhases
{
    /// <summary>
    /// 
    /// </summary>
    public class BattleStartPhase : BattlePhase<BattleStartPhase>
    {
        public BattleStartPhase(Battle battle, BattleResolver resolver)
        : base(battle, resolver)
        {}

        public override void Start()
        {
            base.Start();
            this.Resolver.OnBattleStart(this.Battle);
            this.Battle.Phase = new EnemyDecisionPhase(this.Battle, this.Resolver);
        }
    }
}
