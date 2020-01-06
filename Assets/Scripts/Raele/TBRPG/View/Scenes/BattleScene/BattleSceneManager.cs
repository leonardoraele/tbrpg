using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Raele.TBRPG.BattleSystem;
using Raele.TBRPG.Data;
using Raele.Util;

namespace Raele.TBRPG.View.Scenes.BattleScene
{
    /// <summary>
    /// This is the main controller of the battle scene. Use LoadBattleScene to go to this scene and start a battle.
    /// This script will NOT start the battle automatically if you just swap to the battle scene (or start the game
    /// there) and this script happens to be there; you have to call LoadBattleScene in order for the battle to actually
    /// start. Add <class>Raele.TBRPG.View.Scenes.BattleScene.TestBattleSetup</class> behaviour to your scene to start a
    /// pre-configured battle automatically when the scene starts.
    /// </summary>
    public class BattleSceneManager : SingletonMonoBehaviour<BattleSceneManager>, BattleResolver
    {
        public const string SCENE_NAME = "Battle";

        public static void LoadBattleScene(GameSession gameSession, BattleConditions conditions, System.Action<BattleOverResult> onBattleOver = null)
        {
            BattleSceneManager.IsLoadingBattleScene = true;
            SceneManager.LoadSceneAsync(BattleSceneManager.SCENE_NAME, LoadSceneMode.Single)
                .OnFinish(() => SceneManager.LoadSceneAsync(conditions.ScenarioScene, LoadSceneMode.Additive))
                .OnFinish(() => {
                    BattleSceneManager.Instance.StartBattle(gameSession, conditions, onBattleOver);
                    BattleSceneManager.IsLoadingBattleScene = false;
                });
        }

        private static bool IsLoadingBattleScene = false;

        public bool IsOngoing => BattleSceneManager.IsLoadingBattleScene || (this.Battle?.CurrentState ?? Battle.State.NOT_STARTED) == Battle.State.ONGOING;
        public string ScenarioSceneName { get; private set; }

        private Battle Battle;
        private System.Action<BattleOverResult> OnBattleOverCallback;
        private Dictionary<BattleUnit, Actor> ActorMap = new Dictionary<BattleUnit, Actor>();
        private SynchronizedCoroutineQueue EventQueue;
        private BattleScenarioManager ScenarioManager;

        public void StartBattle(GameSession gameSession, BattleConditions conditions, System.Action<BattleOverResult> onBattleOver)
        {
            this.EventQueue = new SynchronizedCoroutineQueue();
            this.ScenarioManager = BattleScenarioManager.Instance.AssertNotDefault("Tried to start a battle, but no BattleScenarioManager was found.");
            this.ScenarioSceneName = conditions.ScenarioScene;
            this.OnBattleOverCallback = onBattleOver;
            this.Battle = new Battle(gameSession, conditions, this);
            this.Battle.Start();
        }

        public void Update()
            => this.Battle?.Update();

        public void OnBattleStart(Battle battle)
        {
            this.DeployAllies(battle.Allies);
            this.DeployEnemies(battle.Enemies);
            this.EventQueue.Enqueue(() => new WaitForSeconds(1));
        }

        public void DeployAllies(IEnumerable<Ally> allies)
            => this.DeployUnits(
                allies,
                this.ScenarioManager.AllyDeployPositions
                    .Select(t => t.position)
                    .ToArray()
            );

        public void DeployEnemies(IEnumerable<BattleSystem.Enemy> enemies)
            => this.DeployUnits(
                enemies,
                this.ScenarioManager.EnemyDeployPositions
                    .Select(t => t.position)
                    .ToArray()
            );

        public void DeployUnits(IEnumerable<BattleUnit> units, Vector3[] deployPositions)
        {
            var unitList = units.ToList(); // Avoid multiple enumeration
            
            if (unitList.Count > deployPositions.Length)
            {
                Debug.LogWarning($"Not enough deploy positions in scenario {this.ScenarioSceneName} to put all units in battle. Create more deploy positions.");
            }

            unitList.ForEach((unit, i) =>
                    GameObject.Instantiate(
                            unit.ActorPrototype.gameObject,
                            i < deployPositions.Length ? deployPositions[i] : Vector3.right * (i - deployPositions.Length),
                            Quaternion.identity
                        )
                        .GetComponent<Actor>()
                        .OtherwiseThrow("wat")
                        .Then(actor => this.ActorMap[unit] = actor)
                );
        }

        public void OnEnemyPrepared(Battle battle, BattleSystem.Enemy enemy, Data.Action action, BattleUnit target)
            => this.EventQueue.Enqueue(this.ActorMap[enemy].PlayPrepareAction);

        public void OnPlayerTurn(Battle battle, PlayerTurnHelper helper)
        {
            Debug.Log("It's player's turn.");
        }

        public void OnActionExecuted(Battle battle, ActionExecutionResult result)
        {
            result.AssertNotDefault();
            this.EventQueue.Enqueue(() =>
                {
                    if (result.PreparedAction.Target != null)
                    {
                        this.ActorMap[result.PreparedAction.Target].PlayGetHit();
                    }
                    return this.ActorMap[result.PreparedAction.Actor].PlayAttack();
                })
                .Enqueue(() =>
                {
                    if (result.PreparedAction.Target != null)
                    {
                        this.ActorMap[result.PreparedAction.Target].PlayIdle();
                    }
                    return this.ActorMap[result.PreparedAction.Actor].PlayIdle();
                });
        }

        public void OnBattleOver(Battle battle, BattleOverResult result)
            => this.EventQueue.Enqueue(() => this.OnBattleOverCallback?.Invoke(result));
    }
}
