using System;
using UnityEngine;

namespace Raele.TBRPG.Data
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu]
    public class Enemy : Character
    {
        [Serializable]
        public struct EnemyAction
        {
            public Data.Action Action;
            public int DecisionWeight;
        }

        public EnemyAction[] Actions;
    }
}
