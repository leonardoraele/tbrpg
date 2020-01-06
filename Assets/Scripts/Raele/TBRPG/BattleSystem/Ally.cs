using System.Collections.Generic;
using Raele.TBRPG.Campaign;
using Raele.TBRPG.View;

namespace Raele.TBRPG.BattleSystem
{
    /// <summary>
    /// An ally is one of the characters controlled by the player during combat.
    /// It holds the combat transitent states of a PartyMember.
    /// </summary>
    public class Ally : BattleUnit
    {
        public PartyMember Template;
        public IEnumerable<Data.Action> Actions { get; private set; } = new Data.Action[0];
        public string Name => this.Template.Name;
        public Actor ActorPrototype => this.Template.ActorPrototype;

        public Ally(PartyMember partyMember)
        {
            this.Template = partyMember;
        }

        public override string ToString()
            => this.Name;
    }
}
