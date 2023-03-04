using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class XmouseCommand : IChatCommand
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
            get { return "Toggle Xmouse"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendWhisper("Xmouse " + (Session.GetHabbo().Xmouse ? "disabled" : "enabled"));
            Session.GetHabbo().Xmouse = !Session.GetHabbo().Xmouse;
        }
    }
}