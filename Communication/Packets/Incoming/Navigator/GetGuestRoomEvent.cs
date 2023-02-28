﻿using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Navigator
{
    internal class GetGuestRoomEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int roomId = packet.PopInt();

            if (!RoomFactory.TryGetData(roomId, out RoomData data))
                return;

            bool enter = packet.PopInt() == 1;
            bool forward = packet.PopInt() == 1;

            session.SendPacket(new GetGuestRoomResultComposer(session, data, enter, forward));
        }
    }
}