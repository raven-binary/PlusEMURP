﻿using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class InfoRetrieveEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            session.SendPacket(new UserObjectComposer(session.GetHabbo()));
            session.SendPacket(new UserPerksComposer(session.GetHabbo()));
        }
    }
}