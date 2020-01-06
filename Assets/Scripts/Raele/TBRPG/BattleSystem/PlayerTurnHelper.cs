using Raele.Util;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// 
    /// </summary>
    public interface PlayerTurnHelper
    {
        void SetAllyAction(Ally ally, Data.Action action, BattleUnit target);
        void Done();
    }
}
