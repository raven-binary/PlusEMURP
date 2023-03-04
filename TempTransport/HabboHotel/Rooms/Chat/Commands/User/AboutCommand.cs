using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Threading;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class AboutCommand : IChatCommand
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
            get { return "Displays generic inofrmation that everybody loves to see"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            TimeSpan Uptime = DateTime.Now - PlusEnvironment.ServerStarted;
            int OnlineUsers = PlusEnvironment.GetGame().GetClientManager().Count;
            int RoomCount = PlusEnvironment.GetGame().GetRoomManager().Count;
            Session.SendMessage(new RoomNotificationComposer("Powered by " + PlusEnvironment.Hotelname + " Emulator",
                "<b>Credits:</b>\n" +
                "Lango (" + PlusEnvironment.Hotelname + " Owner)\n" +
                "St33lix (" + PlusEnvironment.Hotelname + " Owner & Dev)\n" +
                "Heroix (" + PlusEnvironment.Hotelname + " Dev)\n" +
                "Sledmore (PlusEMU Release)\n" +
                "Previous Contributors\n\n" +
                "<b>Server Stats:</b>\n" +
                "Online Users: " + OnlineUsers + "\n" +
                "Rooms Loaded: " + RoomCount, "plus", ""));
        }
    }
}