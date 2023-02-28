﻿using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.Connection
{
    internal class GoToFlatEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!session.GetHabbo().EnterRoom(session.GetHabbo().CurrentRoom))
                session.SendPacket(new CloseConnectionComposer());
        }
    }
}