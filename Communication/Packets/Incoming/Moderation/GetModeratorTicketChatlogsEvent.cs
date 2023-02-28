﻿using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    internal class GetModeratorTicketChatlogsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().GetPermissions().HasRight("mod_tickets"))
                return;

            int ticketId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetModerationManager().TryGetTicket(ticketId, out ModerationTicket ticket) || ticket.Room == null)
                return;

            if (!RoomFactory.TryGetData(ticket.Room.Id, out RoomData data))
                return;

            session.SendPacket(new ModeratorTicketChatlogComposer(ticket, data, ticket.Timestamp));
        }
    }
}