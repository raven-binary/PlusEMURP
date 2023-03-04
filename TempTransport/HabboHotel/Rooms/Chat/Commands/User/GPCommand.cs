using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class GPCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetRoleplay().GP > 0)
                return true;

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
            get { return "Removes your god-protection"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().GPWarning == false)
            {
                Session.SendWhisper("Type :gp again to remove your god protection");
                Session.GetHabbo().GPWarning = true;
                return;
            }
            else if (Session.GetHabbo().GPWarning)
            {
                Session.SendWhisper("You are no more under god protection");
                Session.GetRoleplay().GP = 0;
            }
        }
    }
}