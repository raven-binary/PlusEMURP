using System;
using System.Linq;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Incoming.Users
{
    internal class GetRelationshipsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Habbo habbo = PlusEnvironment.GetHabboById(packet.PopInt());
            if (habbo == null)
                return;

            var rand = new Random();
            habbo.Relationships = habbo.Relationships.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);

            session.SendPacket(new GetRelationshipsComposer(habbo));
        }
    }
}