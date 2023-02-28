﻿using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class FastwalkCommand : IChatCommand
    {
        public string PermissionRequired => "command_fastwalk";

        public string Parameters => "";

        public string Description => "Gives you the ability to walk very fast.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            user.FastWalking = !user.FastWalking;

            if (user.SuperFastWalking)
                user.SuperFastWalking = false;

            session.SendWhisper("Walking mode updated.");
        }
    }
}