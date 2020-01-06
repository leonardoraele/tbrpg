using System.Collections.Generic;
using Raele.Util;

namespace Raele.TBRPG.Campaign
{
    /// <summary>
    /// 
    /// </summary>
    public class Party
    {
        public Inventory Inventory { get; private set; } = new Inventory();
        public List<PartyMember> PartyMembers { get; private set; } = new List<PartyMember>();
    }
}
