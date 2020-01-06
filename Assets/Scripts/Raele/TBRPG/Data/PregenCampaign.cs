using UnityEngine;
using Raele.TBRPG.Campaign;
using Raele.Util;

namespace Raele.TBRPG.Data
{
    /// <summary>
    /// This is a pre-generated campaign configuration. This is supposed to be
    /// used to setup the starting campaign configuration. It is not a singleton
    /// scriptable object (or a .yml fire or whatever) so that we can create
    /// different campaign starting points wither for test purposes; or if we
    /// ever have extra campaigns. (e.g. secondary play modes, etc.)
    /// </summary>
    [CreateAssetMenu]
    public class PregenCampaign : ScriptableObject
    {
        public PregenParty Party;

        public void OnEnable()
        {
            this.Party.AssertNotDefault();
        }

        public CampaignData Create()
            => new CampaignData(this.Party);
    }
}
