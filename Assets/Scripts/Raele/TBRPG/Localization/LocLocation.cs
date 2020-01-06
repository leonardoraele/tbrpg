using UnityEngine;

namespace Raele.TBRPG.Localization
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu]
    public class LocLocation : ScriptableObject
    {
        public string Location;

        public override string ToString()
            => this.Location;
    }
}
