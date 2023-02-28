using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.HabboHotel.Rooms.Chat.Commands;

namespace Plus.HabboHotel.Roleplay.Commands.General
{
    class RoomIDCommand : IChatCommand
    {
        public string PermissionRequired => "command_roomid";

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Displays the current RoomID."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null)
            {
                Session.SendWhisper("For some odd reason, this room's data could not be found!");
                return;
            }

            Session.SendWhisper("You are currently in RoomID: " + Room.Id + "");
            return;
        }
    }
}
