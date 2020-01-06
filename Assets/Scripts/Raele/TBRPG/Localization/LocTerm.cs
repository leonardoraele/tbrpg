using UnityEngine;

namespace Raele.TBRPG.Localization
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu]
    public class LocTerm : ScriptableObject
    {
        public string Text;

        public override string ToString()
            => this.Text;
    }
}
