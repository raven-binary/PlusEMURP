using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Notifications;
namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class RoomIDCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

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
            get { return "Zeigt dir den Raum ID an."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Room == null)
            {
                Session.SendWhisper("For some odd reason, this room's data could not be found!");
                return;
            }

            Session.SendWhisper("You are currently in RoomID: " + Room.Id + "!");
            return;
        }
    }
}
