using UnityEngine;
using Raele.TBRPG.Localization;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu]
    public class BattleTerms : ScriptableObject
    {
        public LocTerm Attack;
        public LocTerm Defend;
        public LocTerm UseItem;
        public LocTerm CastSpell;
        public LocTerm UseSkill;
        public LocTerm Flee;
    }
}
