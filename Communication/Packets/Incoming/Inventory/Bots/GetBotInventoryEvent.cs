using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Inventory.Bots;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Inventory.Bots;

namespace Plus.Communication.Packets.Incoming.Inventory.Bots
{
    internal class GetBotInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session.GetHabbo().GetInventoryComponent() == null)
                return;

            ICollection<Bot> bots = session.GetHabbo().GetInventoryComponent().GetBots();
            session.SendPacket(new BotInventoryComposer(bots));
        }
    }
}