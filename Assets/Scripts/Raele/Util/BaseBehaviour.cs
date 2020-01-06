using System;
using System.Collections.Generic;
using UnityEngine;

namespace Raele.Util
{
    public class BaseBehaviour : MonoBehaviour
    {
        private List<Action> afterAwakeActions;
        
        public void AfterAwake(Action a)
            => (this.afterAwakeActions = this.afterAwakeActions ?? new List<Action>()).Add(a);
        
        public void Start()
            => this.afterAwakeActions?.ForEach(action => action());
    }
}
