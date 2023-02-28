using MySqlX.XDevAPI;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using System;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Combat
{
    internal class HitCommand : IChatCommand
    {
        public string PermissionRequired => "command_hit";

        public string Parameters => "%username%";

        public string Description => "Did they insult you? Hit back!";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (@params.Length == 1)
            {
                session.SendWhisper("Please enter the username of the user you wish to hit.");
                return;
            }

            if (!room.CombatEnabled && !session.GetHabbo().GetPermissions().HasRight("room_override_custom_config"))
            {
                session.SendWhisper("Oops, it appears that the room owner has disabled the ability to use the push command in here.");
                return;
            }

            GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(@params[1]);
            if (targetClient == null)
            {
                session.SendWhisper("An error occoured whilst finding that user, maybe they're not online.");
                return;
            }

            RoomUser targetUser = room.GetRoomUserManager().GetRoomUserByHabbo(targetClient.GetHabbo().Id);
            if (targetUser == null)
            {
                session.SendWhisper(
                    "An error occoured whilst finding that user, maybe they're not online or in this room.");
                return;
            }

            RoomUser thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (thisUser == null)
                return;

            if (Gamemap.TileDistance(thisUser.X, thisUser.Y, targetUser.X, targetUser.Y) > 1.5)
            {
                session.SendWhisper("You're too far my guy");
                return;
            }
            
            if (targetClient.GetHabbo().Username == session.GetHabbo().Username)
            {
                session.SendWhisper("Come on, surely you don't want to hurt yourself!");
                return;
            }

            if (session.GetHabbo().Combat != null)
            {
                session.GetHabbo().Combat.Hit(thisUser, targetUser, targetClient);
            }
            else
            {
                Console.WriteLine("Combat method is null");
            }
        }
    }
}