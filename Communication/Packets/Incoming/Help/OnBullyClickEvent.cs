﻿using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Help
{
    internal class OnBullyClickEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            //I am a very boring packet.
        }
    }
}