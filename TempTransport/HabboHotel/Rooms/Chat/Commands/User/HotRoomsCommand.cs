using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Notifications;
namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class HotRoomsCommand : IChatCommand
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
            get { return "Shows the rooms with the most people in"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            var This = Session.GetHabbo();
            StringBuilder Rooms = new StringBuilder();

            foreach (Room room in PlusEnvironment.GetGame().GetRoomManager().GetRooms().ToList().OrderByDescending(key => key.UserCount))
            {
                if (room.UserCount <= 0)
                    continue;

                Rooms.Append("" + room.RoomData.Name + " - Users: " + room.UserCount + "\n");
            }

            Session.SendMessage(new MOTDNotificationComposer("Popular Rooms:\n\n" + Rooms.ToString()));
        }
    }
}
