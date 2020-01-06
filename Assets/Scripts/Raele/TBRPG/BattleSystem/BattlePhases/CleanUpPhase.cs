using UnityEngine;
using Raele.Util;

namespace Raele.TBRPG.BattleSystem.BattlePhases
{
    /// <summary>
    /// 
    /// </summary>
    public class CleanUpPhase : BattlePhase<CleanUpPhase>
    {
        public CleanUpPhase(Battle battle, BattleResolver resolver)
        : base(battle, resolver)
        {}

        public override void Start()
        {
            base.Start();
            Debug.Log("Cleaning up...");
            Debug.Log("Starting new round...");
            this.Battle.Phase = new EnemyDecisionPhase(this.Battle, this.Resolver);
        }
    }
}