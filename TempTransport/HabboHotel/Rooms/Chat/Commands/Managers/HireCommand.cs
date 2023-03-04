using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GroupsRank;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class HireCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().RankId == 1 && Session.GetHabbo().Working || Session.GetHabbo().RankId == 2 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "manager"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Hires a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax  :hire <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("hire_command") == true)
            {
                Session.SendWhisper("You must wait before you can use hire command again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            if (TargetClient.GetHabbo().JobId != 8)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a job");
                return;
            }

            if (TargetClient.GetHabbo().JobId == Session.GetHabbo().JobId)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already works for you");
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(Session.GetHabbo().JobId), out Group))
            {
                Session.SendWhisper("The corp id is invalid");
                return;
            }

            GroupRank NewRank = null;
            PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(Convert.ToInt32(Session.GetHabbo().JobId), 7, out NewRank);
            
            if (NewRank == null)
            {
                Session.SendWhisper("An error has occurred");
                return;
            }

            Session.GetHabbo().addCooldown("hire_command", 2000);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Group.AddMemberByForce(TargetClient.GetHabbo().Id);
            TargetClient.GetHabbo().setFavoriteGroup(Group.Id);
            Group.ChangeTier(TargetClient.GetHabbo().Id, 0);
            User.Say("hires " + TargetClient.GetHabbo().Username);
            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"green\">" + Session.GetHabbo().Username + "</span> hired <span class=\"blue\">" + TargetClient.GetHabbo().Username + "</span> at " + Group.Name);
            Webhook.SendWebhook(":briefcase: " + Session.GetHabbo().Username + " hired " + TargetClient.GetHabbo().Username + " at " + Group.Name);
        }
    }
}