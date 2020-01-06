using UnityEngine;
using Raele.TBRPG.Data;
using Raele.TBRPG.View;

namespace Raele.TBRPG.Campaign
{
    /// <summary>
    /// This is an instance of a Data.Character inside the party. It holds instance information about the character in
    /// this current game, that can change between game playthrughs, like current level, stat values, and equiped items.
    /// </summary>
    public class PartyMember
    {
        public const int NUM_OF_EQUIPMENTS = 4;

        public Equipment[] Equipments { get; private set; } = new Equipment[NUM_OF_EQUIPMENTS];
        public Armor Armor { get; private set; }
        public int CurrentLevel { get; private set; }
        public string Name { get; private set; }
        public Actor ActorPrototype => this.Template.ActorPrototype;
        public Character Template { get; private set; }

        public PartyMember(Character template)
        {
            this.Template = template;
        }
    }
}
