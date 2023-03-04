using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PopAlertCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 7 && Session.GetHabbo().isLoggedIn)
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
            get { return "Sends a message to the entire hotel"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specify a message");
                return;
            }

            string Message = Params[1];
            if (Params.Length == 2)
            {
                //PlusEnvironment.GetGame().GetClientManager().HotelAlert(Message);
                Webhook.SendWebhook("[HOTEL ALERT]: " + Message);
            }
            else if (Params.Length == 3)
            {
                //PlusEnvironment.GetGame().GetClientManager().HotelAlert(Message, Convert.ToInt32(Params[2]));
            }
        }
    }
}