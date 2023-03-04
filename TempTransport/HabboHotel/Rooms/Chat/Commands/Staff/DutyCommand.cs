using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Moderation;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class DutyCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 5 && Session.GetHabbo().isLoggedIn)
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
            get { return "Toggle Duty Mode"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            Session.SendWhisper("Duty Mode " + (Session.GetRoleplay().Duty ? "disabled" : "enabled"));
            Session.GetRoleplay().Duty = !Session.GetRoleplay().Duty;
            Session.GetRoleplay().ResetEffect();
        }
    }
}
