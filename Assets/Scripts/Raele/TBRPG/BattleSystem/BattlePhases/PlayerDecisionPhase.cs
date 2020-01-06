using System.Linq;
using System.Collections.Generic;
using Raele.Util;

namespace Raele.TBRPG.BattleSystem.BattlePhases
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerDecisionPhase : BattlePhase<PlayerDecisionPhase>, PlayerTurnHelper
    {
        private struct ActionSet
        {
            public Data.Action action;
            public BattleUnit target;
        }

        public bool AllSet => this.Battle.Allies.All(this.SetActions.Keys.Contains);

        private Dictionary<Ally, ActionSet> SetActions = new Dictionary<Ally, ActionSet>();

        public PlayerDecisionPhase(Battle battle, BattleResolver resolver)
        : base(battle, resolver)
        {}

        public override void Start()
        {
            base.Start();
            this.Resolver.OnPlayerTurn(this.Battle, this);
        }

        public void SetAllyAction(Ally ally, Data.Action action, BattleUnit target)
        {
            this.SetActions[ally] = new ActionSet()
            {
                action = action,
                target = target,
            };
        }

        public void Done()
        {
            this.SetActions.Keys.Where(this.Battle.Allies.Contains)
                .ForEach(ally => this.Battle.PrepareAction(
                    ally,
                    this.SetActions[ally].action,
                    this.SetActions[ally].target
                ));
            this.Battle.Phase = new ActionExecutionPhase(
                this.Battle,
                this.Resolver,
                () => new CleanUpPhase(this.Battle, this.Resolver)
            );
        }
    }
}
