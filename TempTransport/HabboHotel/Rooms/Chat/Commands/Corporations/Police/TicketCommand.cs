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
    class TicketCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
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
            get { return "Offers a ticket to target"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (!TargetClient.GetRoleplay().Escort)
            {
                Session.SendWhisper("You can't offer a ticket to a player where isn't cuffed");
                return;
            }

            if (TargetClient.GetRoleplay().EscortBy != Session.GetHabbo().Username)
            {
                Session.SendWhisper("You can't offer a ticket to someone who isn't cuffed by you");
                return;
            }

            if (TargetClient.GetRoleplay().Wan.ArrestTime() > 15)
            {
                Session.SendWhisper("You can't offer a ticket to someone who has 15 minutes charges");
                return;
            }

            if (TargetClient.GetRoleplay().Wan.Copmurder > 0)
            {
                Session.SendWhisper("This player is charged for copmurder and can't get ticketed");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            TargetClient.GetRoleplay().TicketTimer = 20;
            TargetClient.GetRoleplay().TicketFrom = Session.GetHabbo().Username;

            User.Say("offers a ticket of " + TargetClient.GetRoleplay().Wan.ArrestTime() * 7 + " dollars to " + TargetClient.GetHabbo().Username);
            TargetClient.SendWhisper("You have been offered a ticket of " + TargetClient.GetRoleplay().Wan.ArrestTime() * 7 + " dollars, pay it using :payticket - it will expire in 20 seconds");

            //Webhook.SendCopFeed(Session.GetHabbo().RankInfo.Name + " **" + Session.GetHabbo().Username + "** offered a __$" + TargetClient.GetRoleplay().Wan.ArrestTime() * 7 + "__ ticket to **" + TargetClient.GetHabbo().Username + "**");
        }
    }
}