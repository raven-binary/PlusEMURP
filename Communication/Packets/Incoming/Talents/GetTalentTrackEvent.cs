using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Talents;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Talents;

namespace Plus.Communication.Packets.Incoming.Talents
{
    internal class GetTalentTrackEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string type = packet.PopString();

            ICollection<TalentTrackLevel> levels = PlusEnvironment.GetGame().GetTalentTrackManager().GetLevels();

            session.SendPacket(new TalentTrackComposer(levels, type));
        }
    }
}