using System;

namespace Raele.TBRPG.BattleSystem.BattlePhases
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionExecutionPhase : BattlePhase<ActionExecutionPhase>
    {
        private Func<IBattlePhase> NextPhaseProvider;

        public ActionExecutionPhase(Battle battle, BattleResolver resolver, Func<IBattlePhase> nextPhaseProvider)
        : base(battle, resolver)
        {
            this.NextPhaseProvider = nextPhaseProvider;
        }

        public override void Start()
        {
            base.Start();
            this.Battle.ExecutePreparedActions();
            this.Battle.Phase = this.NextPhaseProvider.Invoke();
        }
    }
}
