using UnityEngine;
using Raele.TBRPG.Localization;

namespace Raele.TBRPG.Data
{
    /// <summary>
    /// This is a possible action an unit can take during battle.
    /// </summary>
    [CreateAssetMenu]
    public class Action : ScriptableObject
    {
        public LocTerm Name;
        public LocTerm Description;

        public override string ToString()
            => this.Name.Text; // TODO This will produce the LocTerm code instead of an actual name
    }
}
