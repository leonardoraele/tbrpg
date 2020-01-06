using UnityEngine;
using Raele.Util;

namespace Raele.TBRPG.View.Scenes.BattleScene
{
    public class BattleScenarioManager : SingletonMonoBehaviour<BattleScenarioManager>
    {
        public Transform[] AllyDeployPositions;
        public Transform[] EnemyDeployPositions;
    }
}
