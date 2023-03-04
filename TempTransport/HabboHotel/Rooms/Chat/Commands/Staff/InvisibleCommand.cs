using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class InvisibleCommand : IChatCommand
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
            get { return "Toggle Invisible Mode"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            Session.SendWhisper("You are now " + (Session.GetHabbo().IsInvisible ? "shown" : "hidden"));
            if (Session.GetHabbo().IsInvisible)
            {
                Session.GetHabbo().IsInvisible = false;
                return;
            }

            Session.GetHabbo().IsInvisible = true;
        }
    }
}