﻿using System.Collections.Generic;
using System.Linq;
using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    internal class ToggleMuteToolEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null || !room.CheckRights(session, true))
                return;

            room.RoomMuted = !room.RoomMuted;

            List<RoomUser> roomUsers = room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser roomUser in roomUsers.ToList())
            {
                if (roomUser == null || roomUser.GetClient() == null)
                    continue;

                roomUser.GetClient()
                    .SendWhisper(room.RoomMuted ? "This room has been muted" : "This room has been unmuted");
            }

            room.SendPacket(new RoomMuteSettingsComposer(room.RoomMuted));
        }
    }
}