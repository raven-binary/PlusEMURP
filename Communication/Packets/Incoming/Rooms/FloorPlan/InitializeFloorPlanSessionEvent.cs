﻿using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Rooms.FloorPlan
{
    internal class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            //Session.SendNotif("WARNING - THIS TOOL IS IN BETA, IT COULD CORRUPT YOUR ROOM IF YOU CONFIGURE THE MAP WRONG OR DISCONNECT YOU.");
        }
    }
}