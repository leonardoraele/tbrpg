using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Raele.TBRPG.BattleSystem.BattlePhases;
using Raele.TBRPG.Data;
using Raele.Util;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// Controls the flow of an ongoing battle.
    /// </summary>
    public class Battle
    {
        public enum State { NOT_STARTED, ONGOING, FINISHED }

        public State CurrentState =>
            this.BattleOverResult != null   ? State.FINISHED    :
            this.Phase == null              ? State.NOT_STARTED :
                                              State.ONGOING;
                                 
        public IBattlePhase Phase
        {
            get => this.m_phase;
            set
            {
                this.m_phase?.Finish();
                this.m_phase = value;
            }
        }

        public List<Ally> Allies { get; private set; }
        public List<Enemy> Enemies {get; private set; }
        public bool PlayerTurn { get; set; }
        public GameSession GameSession { get; private set; }
        public bool Started { get; private set; }
        public IEnumerable<PreparedAction> PreparedActions => this.m_preparedActions.Values;
        public BattleOverResult? BattleOverResult { get; private set; }

        private BattleResolver Resolver;
        private IBattlePhase m_phase;
        private Dictionary<BattleUnit, PreparedAction> m_preparedActions = new Dictionary<BattleUnit, PreparedAction>();

        public Battle(GameSession session, BattleConditions conditions, BattleResolver resolver)
        {
            this.Allies = session.CampaignData.Party.PartyMembers.Select(e => new Ally(e)).ToList();
            this.Enemies = conditions.Troop.Enemies.Select(e => new Enemy(e)).ToList();
            this.GameSession = session;
            this.Resolver = resolver;
        }

        public void Start()
        {
            this.Started.ThenThrow("Trying to start an already started battle.");
            this.Phase = new BattleStartPhase(this, this.Resolver);
        }

        public void Update()
            => this.Phase?.HasStarted
                .Then(this.Phase.Update)
                .Otherwise(this.Phase.Start);

        public void PrepareAction(BattleUnit actor, Data.Action action, BattleUnit target)
        {
            Debug.Log($"Battle: Actor {actor} prepares {action} against {target}.");
            this.m_preparedActions[actor] = new PreparedAction()
            {
                Actor = actor,
                Action = action,
                Target = target,
            };
        }

        public void ExecutePreparedActions()
        {
            this.PreparedActions.Select(this.ExecutePreparedAction)
                .ForEach(result => this.Resolver.OnActionExecuted(this, result));
            this.m_preparedActions.Clear();
        }

        public void Finish()
        {
            // this.BattleOverResult = new BattleOverResult() { ... };
            // this.Resolver.OnBattleOver(this, this.BattleOverResult);
        }

        private ActionExecutionResult ExecutePreparedAction(PreparedAction prepAction)
        {
            Debug.Log($"Battle: {prepAction.Actor} executes '{prepAction.Action}' on {prepAction.Target}");
            return new ActionExecutionResult()
            {
                PreparedAction = prepAction,
            };
        }
    }
}
