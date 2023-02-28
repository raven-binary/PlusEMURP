using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    internal class PickTicketEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_tool"))
                return;

            packet.PopInt(); //Junk
            int ticketId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(ticketId, out ModerationTicket ticket))
                return;

            ticket.Moderator = session.GetHabbo();
            PlusEnvironment.GetGame().GetClientManager().SendPacket(new ModeratorSupportTicketComposer(session.GetHabbo().Id, ticket), "mod_tool");
        }
    }
}