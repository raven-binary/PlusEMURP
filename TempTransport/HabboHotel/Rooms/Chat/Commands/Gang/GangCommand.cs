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
    class GangCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "gang"; }
        }

        public string Parameters
        {
            get { return "<message>"; }
        }

        public string Description
        {
            get { return "Sends a message to all online gang members"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().Gang == 0)
            {
                Session.SendWhisper("You are not in any gang");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            if(Message == null || Message == "")
            {
                Session.SendWhisper("Enter your message");
                return;
            }

            PlusEnvironment.GetGame().GetClientManager().GangWhisper(Session.GetHabbo().Gang, Session.GetHabbo().Username, Message);
        }
    }
}