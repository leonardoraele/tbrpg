using UnityEngine;
using UnityEngine.Serialization;
using Raele.TBRPG.Localization;
using Raele.TBRPG.View;
using Raele.Util;

namespace Raele.TBRPG.Data
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Character : ScriptableObject
    {
        public LocTerm Name;
        [FormerlySerializedAsAttribute("Prefab")]
        public Actor ActorPrototype;

        public void OnEnable()
        {
            this.Name.AssertNotDefault();
            this.ActorPrototype.AssertNotDefault();
        }
    }
}
