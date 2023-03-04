using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PayTicketCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetRoleplay().TicketFrom != null)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Pays your ticket"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Session.GetRoleplay().TicketFrom);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Session.GetHabbo().Credits >= Session.GetRoleplay().Wan.ArrestTime() * 7)
            {
                User.Say("pays their ticket of " + Session.GetRoleplay().Wan.ArrestTime() * 7 + " dollars");
                Session.GetRoleplay().TicketTimer = 0;
                Session.GetRoleplay().TicketFrom = null;
                Session.GetHabbo().Credits -= Session.GetRoleplay().Wan.ArrestTime() * 7;
                Session.GetRoleplay().RPCache(3);
                Session.GetRoleplay().Wan.Remove();

                TargetClient.GetRoleplay().TicketsGiven += 1;
                TargetClient.GetRoleplay().Inventory.Add("handcuffs", "coploadout", 1, 0, true, false);

                TargetClient.GetRoleplay().EndEscorting(false);
                TargetClient.GetHabbo().Credits += Session.GetRoleplay().Wan.ArrestTime() * 7 * 34 / 100;
                TargetClient.GetRoleplay().RPCache(3);
                TargetClient.SendWhisper("You have received " + PlusEnvironment.ConvertToPrice(Session.GetRoleplay().Wan.ArrestTime() * 7 * 34 / 100) + " dollars from your ticket for " + Session.GetHabbo().Username);

                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + TargetClient.GetHabbo().Username + "</span> ticketed <span class=\"green\">" + Session.GetHabbo().Username + "</span>");
                Webhook.SendWebhook(":ticket: " + TargetClient.GetHabbo().Username + " ticketed " + Session.GetHabbo().Username);
            }
            else
            {
                User.Say("is too poor to pay ticket");
                Session.GetRoleplay().TicketTimer = 0;
                Session.GetRoleplay().TicketFrom = null;
            }
        }
    }
}