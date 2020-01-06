using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Raele.TBRPG.View;
using Raele.Util;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// Represents one of the enemies the player is fighting against during a battle.
    /// </summary>
    public class Enemy : BattleUnit
    {
        public Data.Enemy Template { get; private set; }
        public string Name { get; private set; }
        public Actor ActorPrototype => this.Template.ActorPrototype;

        private SortedDictionary<int, Data.Enemy.EnemyAction> ActionWeightMap
            = new SortedDictionary<int, Data.Enemy.EnemyAction>();
        private int TotalActionWeight = 0;
        private List<int> ActionWeightMapKeyList = new List<int>(0);

        public Enemy(Data.Enemy template, string name = null)
        {
            this.Template = template;
            this.Name = name ?? this.Template.Name.Text; // TODO This will not produce the actual name but the LocTerm code
            this.SetActions(this.Template.Actions);
        }

        public void SetActions(IEnumerable<Data.Enemy.EnemyAction> actions)
        {
            this.ActionWeightMap.Clear();
            this.TotalActionWeight = 0;
            actions.ForEach(action => this.ActionWeightMap[this.TotalActionWeight += action.DecisionWeight] = action);
            this.ActionWeightMapKeyList = this.ActionWeightMap.Keys.ToList();
            this.TotalActionWeight.Otherwise(() => Debug.LogWarning($"Setting actions of Enemy {this.Template.Name}, but the total action weight equals to zero. This means this Enemy will never have an action to execute."));
        }

        public bool SampleAction(Battle battle, out Data.Action action, out BattleUnit target)
        {
            int randomizedWeightListIndex = RandomUtils.NextInt(this.TotalActionWeight) + 1;
            int fixedWeightListIndex = this.ActionWeightMapKeyList.BinarySearch(randomizedWeightListIndex)
                .ThenIf(i => i < 0, i => ~i);
            int actionWeightMapIndex = this.ActionWeightMapKeyList[fixedWeightListIndex];

            if (this.ActionWeightMap.TryGetValue(actionWeightMapIndex, out Data.Enemy.EnemyAction actionFound))
            {
                action = actionFound.Action;
                target = null; // TODO Define a target
                return true;
            }
            else
            {
                action = null;
                target = null;
                return false;
            }
        }

        public override string ToString()
            => this.Name;
    }
}
