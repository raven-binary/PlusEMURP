using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class HotelAlertCommand : IChatCommand
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
            get { return "<message>"; }
        }

        public string Description
        {
            get { return "Sends a pop up message to the entire hotel"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specify a message");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            PlusEnvironment.GetGame().GetClientManager().HotelAlert("", Message);
        }
    }
}
