using UnityEngine;
using Raele.TBRPG.BattleSystem;
using Raele.TBRPG.Data;
using Raele.Util;

namespace Raele.TBRPG.View.Scenes.BattleScene
{
    /// <summary>
    /// This MonoBehaviour starts a new test battle immediately when the scene is loaded. This script has no other
    /// purpose than to start a test batte and should be disabled if the battle test is not intended.
    /// </summary>
    public class TestBattleSetup : SingletonMonoBehaviour<TestBattleSetup>
    {
        public PregenCampaign PregenCampaign;
        public string ScenarioScene;
        public Troop EnemyTroop;

        private void Start()
        {
            if (BattleSceneManager.Instance?.IsOngoing ?? false)
            {
                Debug.Log($"${typeof(TestBattleSetup)} is enabled, but a battle has already been started, so the test battle will be skipped.");
                return;
            }

            Debug.Log("Starting test battle...");
            GameSession session = GameSessionManager.Instance.GameSession;
            session.StartNewCampaign(this.PregenCampaign);
            BattleConditions conditions = new BattleConditions()
            {
                Troop = this.EnemyTroop,
                ScenarioScene = this.ScenarioScene,
            };
            BattleSceneManager.LoadBattleScene(session, conditions);
        }
    }
}
