using Raele.TBRPG.Data;

namespace Raele.TBRPG.Campaign
{
    /// <summary>
    /// This holds information that should be persisted between game sessions.
    /// </summary>
    public class CampaignData
    {
        public Party Party { get; private set; }
        public CampaignSettings Settings { get; private set; }

        public CampaignData(PregenParty party, CampaignSettings settings = null)
        {
            this.Party = party.Create();
            this.Settings = settings ?? new CampaignSettings();
        }
    }
}
