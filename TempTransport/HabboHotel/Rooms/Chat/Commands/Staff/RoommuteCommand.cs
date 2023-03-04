using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class RoommuteCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 5)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Mute/unmute a room"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Room.RoomMuted == false)
            {
                Room.RoomMuted = true;

                Session.SendWhisper("The room has been muted.");
            }
            else
            {
                Room.RoomMuted = false;
                Session.SendWhisper("The space mute has been removed.");
            }

        }
    }
}