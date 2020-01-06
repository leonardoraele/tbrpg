using System;
using Raele.TBRPG.Campaign;
using Raele.TBRPG.Data;
using Raele.Util;

namespace Raele.TBRPG
{
    /// <summary>
    /// This holds information of the current game session. It should be able to produce and load from a
    /// Persistance.CampaignData object.
    /// </summary>
    public class GameSession
    {
        public CampaignData CampaignData { get; private set; }

        public PlayerPreferences Preferences { get; private set; }

        public GameSession()
            => this.LoadFromLastGameSession()
                .Otherwise(this.LoadFromDefaults);

        public bool LoadFromLastGameSession()
            => false; // TODO This should load a file with the last played CampaignData and the PlayerPreferences from the last play session

        public void LoadFromDefaults()
        {
            this.CampaignData = null;
            this.Preferences = new PlayerPreferences();
        }

        public void StartNewCampaign(PregenCampaign newCampaign)
            => this.CampaignData = newCampaign.Create();

        public void LoadCampaignData(string filename)
            => throw new Exception($"${typeof(GameSession)}.LoadCampaignData not implemented yet.");

        public void Finish()
        {
            // TODO Should save the player preferences and a reference to the current campaign so that we can reload
            // them at LoadFromLastGameSession in the next play session
        }
    }
}
