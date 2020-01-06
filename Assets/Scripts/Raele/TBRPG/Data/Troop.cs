using UnityEngine;

namespace Raele.TBRPG.Data
{
    /// <summary>
    /// This is a template for a group of enemies a player can battle against.
    /// </summary>
    [CreateAssetMenu]
    public class Troop : ScriptableObject
    {
        public string Description;
        public Enemy[] Enemies;
    }
}
