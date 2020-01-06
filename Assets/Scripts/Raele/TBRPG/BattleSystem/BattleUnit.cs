using Raele.TBRPG.View;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// Base class of all units inside the combat, both enemies and allies.
    /// </summary>
    public interface BattleUnit
    {
        string Name { get; }
        Actor ActorPrototype { get; }
    }
}
