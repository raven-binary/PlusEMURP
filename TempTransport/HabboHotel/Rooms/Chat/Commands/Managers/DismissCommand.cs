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
using Plus.Communication.Packets.Outgoing.Groups;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DismissCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().RankId == 1 | Session.GetHabbo().RankId == 2 && Session.GetHabbo().Working)
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
            get { return "Dismisses a player from the corporation"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :dismiss <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("dismiss_command") == true)
            {
                Session.SendWhisper("You must wait before you can use dismiss command again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                DataRow GetUser = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `users` WHERE `username` = @username LIMIT 1");
                    dbClient.AddParameter("username", Username);
                    GetUser = dbClient.getRow();
                }

                if (GetUser == null)
                {
                    Session.SendWhisper("Player not found");
                    return;
                }

                DataRow GroupMemberships = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `group_memberships` WHERE `user_id` = @id LIMIT 1");
                    dbClient.AddParameter("id", GetUser["id"]);
                    GroupMemberships = dbClient.getRow();
                }

                if (Convert.ToInt32(GroupMemberships["group_id"]) != Session.GetHabbo().JobId)
                {
                    Session.SendWhisper(GetUser["username"] + " doesn't work for you");
                    return;
                }

                if (Session.GetHabbo().RankId == Convert.ToInt32(GroupMemberships["rank_id"]))
                {
                    Session.SendWhisper("You don’t have the permissions to demote this player");
                    return;
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `group_memberships` SET group_id = 8, rank_id = 1, tier = 0 WHERE user_id = @uid");
                    dbClient.AddParameter("uid", GetUser["id"]);
                    dbClient.RunQuery();
                }

                User.Say("dismisses " + GetUser["username"]);
                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"red\">" + GetUser["username"] + "</span> as been dismissed from " + Session.GetHabbo().TravailInfo.Name);
                Webhook.SendWebhook(":no_entry_sign: " + Session.GetHabbo().Username + " dismissed " + GetUser["username"] + " from " + Session.GetHabbo().TravailInfo.Name);
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id || TargetClient.GetHabbo().JobId != Session.GetHabbo().JobId)
                return;

            if (TargetClient.GetHabbo().RankId <= Session.GetHabbo().RankId)
            {
                Session.SendWhisper("You don’t have the permissions to dismiss a " + TargetClient.GetHabbo().RankInfo.Name);
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(8, out Group))
            {
                Session.SendWhisper("The corp id is invaild");
                return;
            }

            if (TargetClient.GetHabbo().Working)
                TargetClient.GetHabbo().stopWork();

            User.Say("dismisses " + Username);
            Session.GetHabbo().addCooldown("dismiss_command", 2000);

            TargetClient.GetHabbo().setFavoriteGroup(Group.Id);
            Group.QuitJob(TargetClient.GetHabbo().Id);
            Group.ChangeTier(TargetClient.GetHabbo().Id, 0);

            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"red\">" + TargetClient.GetHabbo().Username + "</span> as been dismissed from " + Session.GetHabbo().TravailInfo.Name);
            Webhook.SendWebhook(":no_entry_sign: " + Session.GetHabbo().Username + " dismissed " + TargetClient.GetHabbo().Username + " from " + Session.GetHabbo().TravailInfo.Name);
        }
    }
}