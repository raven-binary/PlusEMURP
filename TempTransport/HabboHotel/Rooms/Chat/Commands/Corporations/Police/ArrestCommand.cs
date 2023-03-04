using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ArrestCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working == true)
                return true;

            return false;
        }
        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Arrests a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient.GetHabbo() == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (!Session.GetHabbo().usingArrestActionPoint)
            {
                Session.SendWhisper("You have to be at the LVPD Prison to arrest " + TargetClient.GetHabbo().Username);
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (!TargetClient.GetRoleplay().Escort)
            {
                Session.SendWhisper(Username + " must be cuffed before you can put the player in prison");
                return;
            }

            if (TargetClient.GetRoleplay().EscortBy != Session.GetHabbo().Username)
            {
                Session.SendWhisper(Username + " have to be escorting by you to arrest them");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            string Charges = string.Empty;
            foreach (var Charge in TargetClient.GetRoleplay().Wan.Charges.OrderBy(p => p.Key))
            {
                if (String.IsNullOrWhiteSpace(Charges))
                {
                    Charges = TargetClient.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " " + Charge.Key;
                }
                else
                {
                    Charges += " and " + TargetClient.GetRoleplay().Wan.GetArrestTimeByCharge(Charge.Key) + " " + Charge.Key;
                }
            }

            User.Say("‘ sends " + TargetClient.GetHabbo().Username + " to prison for " + TargetClient.GetRoleplay().Wan.ArrestTime() + " minutes for " + Charges);
            Session.SendWhisper("You have received a $" + TargetClient.GetRoleplay().Wan.ArrestTime() * 220 / 100 + " tip for arresting " + TargetClient.GetHabbo().Username);
            Session.GetHabbo().Credits += TargetClient.GetRoleplay().Wan.ArrestTime() * 220 / 100;
            Session.GetRoleplay().RPCache(3);
            Session.GetRoleplay().EndEscorting(false);

            TargetClient.GetRoleplay().ResetAvatar();
            TargetClient.GetRoleplay().ResetEffect();
            TargetClient.GetRoleplay().SendToPrison(TargetClient.GetRoleplay().Wan.ArrestTime());
            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> arrested <span class=\"red\">" + TargetClient.GetHabbo().Username);
            Webhook.SendWebhook(":police_officer: " + Session.GetHabbo().Username + " arrested " + TargetClient.GetHabbo().Username);
        }
    }
}