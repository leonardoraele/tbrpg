using Raele.Util;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// 
    /// </summary>
    public interface BattleResolver
    {
        void OnBattleStart(Battle battle);
        // void OnRoundStart(Battle battle);
        void OnEnemyPrepared(Battle battle, Enemy enemy, Data.Action action, BattleUnit target);
        void OnPlayerTurn(Battle battle, PlayerTurnHelper helper);
        void OnActionExecuted(Battle battle, ActionExecutionResult result);
        // void OnEventTriggered(Battle battle, BattleEvent event);
        void OnBattleOver(Battle battle, BattleOverResult result);
    }
}
