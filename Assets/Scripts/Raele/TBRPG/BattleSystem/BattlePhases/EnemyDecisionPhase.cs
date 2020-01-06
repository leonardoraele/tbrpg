using Raele.Util;

namespace Raele.TBRPG.BattleSystem.BattlePhases
{
    /// <summary>
    /// 
    /// </summary>
    public class EnemyDecisionPhase : BattlePhase<EnemyDecisionPhase>
    {
        public EnemyDecisionPhase(Battle battle, BattleResolver resolver)
        : base(battle, resolver)
        {}

        public override void Start()
        {
            base.Start();
            this.Battle.Enemies.ForEach(enemy => {
                if (enemy.SampleAction(this.Battle, out Data.Action action, out BattleUnit target))
                {
                    this.Battle.PrepareAction(enemy, action, target);
                    this.Resolver.OnEnemyPrepared(this.Battle, enemy, action, target);
                }
            });
            this.Battle.Phase = new ActionExecutionPhase(
                this.Battle,
                this.Resolver,
                () => new PlayerDecisionPhase(this.Battle, this.Resolver)
            );
        }
    }
}
