using System.Linq;
using UnityEngine;
using Raele.TBRPG.Campaign;
using Raele.Util;

namespace Raele.TBRPG.Data
{
    /// <summary>
    /// This is a pre-generated player party configuration. This has two uses:
    /// 1) to create pre-configured states for the player party to be loaded at
    /// certain points during the game, e.g. starting party or "flash-back-like"
    /// gameplay with another party; and 2) to have a ready-to-play party for
    /// combat testing.
    /// </summary>
    [CreateAssetMenu]
    public class PregenParty : ScriptableObject
    {
        public Character[] PartyMembers;

        public Party Create()
        {
            Party result = new Party();
            result.PartyMembers.AddRange(this.PartyMembers.Select(c => new PartyMember(c)).ToList());
            return result;
        }
    }
}
