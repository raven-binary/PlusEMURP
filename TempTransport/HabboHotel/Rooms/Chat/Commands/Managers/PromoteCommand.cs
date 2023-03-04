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

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PromoteCommand : IChatCommand
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
            get { return "Promotes a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :promote <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("promote_command") == true)
            {
                Session.SendWhisper("You must wait before you can use promote command again");
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

                if (Session.GetHabbo().RankId == 1 && Convert.ToInt32(GroupMemberships["rank_id"]) == 2)
                {
                    Session.SendWhisper("You don’t have the permissions to promote this player to " + Session.GetHabbo().RankInfo.Name);
                    return;
                }
                else if (Session.GetHabbo().RankId == 2 && Convert.ToInt32(GroupMemberships["rank_id"]) == 3)
                {
                    Session.SendWhisper("You don’t have the permissions to promote this player to " + Session.GetHabbo().RankInfo.Name);
                    return;
                }
                else if (Session.GetHabbo().RankId == 3 && Convert.ToInt32(GroupMemberships["rank_id"]) == 4)
                {
                    Session.SendWhisper("You don’t have the permissions to promote this player to " + Session.GetHabbo().RankInfo.Name);
                    return;
                }

                DataRow GroupRanks = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `groups_rank` WHERE `rank_id` = @id AND `job_id` = @group LIMIT 1");
                    dbClient.AddParameter("id", Convert.ToInt32(GroupMemberships["rank_id"]));
                    dbClient.AddParameter("group", Convert.ToInt32(GroupMemberships["group_id"]));
                    GroupRanks = dbClient.getRow();
                }

                if (Convert.ToInt32(GroupMemberships["rank_id"]) == 1)
                {
                    Session.SendWhisper("You don’t have the permissions to promote a " + GroupRanks["name"]);
                    return;
                }

                if (Convert.ToInt32(GroupMemberships["rank_id"]) == 4 || Convert.ToInt32(GroupMemberships["rank_id"]) == 5 || Convert.ToInt32(GroupMemberships["rank_id"]) == 6 || Convert.ToInt32(GroupMemberships["rank_id"]) == 7)
                {
                    if (Convert.ToInt32(GroupMemberships["tier"]) != 5)
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `group_memberships` SET tier = tier + 1 WHERE user_id = @uid");
                            dbClient.AddParameter("uid", GetUser["id"]);
                            dbClient.RunQuery();
                        }
                    }
                    else
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `group_memberships` SET rank_id = rank_id - 1, tier = tier - 4 WHERE user_id = @uid");
                            dbClient.AddParameter("uid", GetUser["id"]);
                            dbClient.RunQuery();
                        }
                    }
                }
                else
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `group_memberships` SET rank_id = rank_id - 1, tier = 5 WHERE user_id = @uid");
                        dbClient.AddParameter("uid", GetUser["id"]);
                        dbClient.RunQuery();
                    }
                }

                DataRow NewGroupMemberships = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `group_memberships` WHERE `user_id` = @id LIMIT 1");
                    dbClient.AddParameter("id", GetUser["id"]);
                    NewGroupMemberships = dbClient.getRow();
                }

                DataRow NewGroupRanks = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `groups_rank` WHERE `rank_id` = @id AND `job_id` = @group LIMIT 1");
                    dbClient.AddParameter("id", Convert.ToInt32(NewGroupMemberships["rank_id"]));
                    dbClient.AddParameter("group", Convert.ToInt32(NewGroupMemberships["group_id"]));
                    NewGroupRanks = dbClient.getRow();
                }

                if (Convert.ToInt32(NewGroupMemberships["rank_id"]) == 1 || Convert.ToInt32(NewGroupMemberships["rank_id"]) == 2 || Convert.ToInt32(NewGroupMemberships["rank_id"]) == 3)
                {
                    User.Say("promotes " + GetUser["username"] + " to " + NewGroupRanks["name"]);
                }
                else
                {
                    User.Say("promotes " + GetUser["username"] + " to " + NewGroupRanks["name"] + " " + PlusEnvironment.Tiers(Convert.ToInt32(NewGroupMemberships["tier"])));
                }
                return;
            }

            //for online players
            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (TargetClient.GetHabbo().JobId != Session.GetHabbo().JobId)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " doesn't work for you");
                return;
            }

            if (TargetClient.GetHabbo().RankId == Session.GetHabbo().RankId || TargetClient.GetHabbo().RankId == Session.GetHabbo().RankId + 1)
            {
                Session.SendWhisper("You don’t have the permissions to promote this player");
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(TargetClient.GetHabbo().JobId, out Group))
            {
                Session.SendWhisper("An error has occurred");
                return;
            }

           

            GroupRank NewRank = null;
            PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(Convert.ToInt32(TargetClient.GetHabbo().JobId), TargetClient.GetHabbo().RankId - 1, out NewRank);
            /*if (NewRank == null)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has the highest rank");
                return;
            }*/



            if (NewRank.RankId == 4 || NewRank.RankId == 5 || NewRank.RankId == 6 || NewRank.RankId == 7)
            {
                if (TargetClient.GetHabbo().Tier != 5)
                {
                    Group.PromoteTier(TargetClient.GetHabbo().Id, 1);
                }
                else
                {
                    Group.DemoteTier(TargetClient.GetHabbo().Id, 4);
                    Group.PromoteRank(TargetClient.GetHabbo().Id);
                }
            }
            else
            {
                Group.PromoteRank(TargetClient.GetHabbo().Id);
                Group.ChangeTier(TargetClient.GetHabbo().Id, 5);
            }



            if (NewRank.RankId == 1 || NewRank.RankId == 2 || NewRank.RankId == 3)
            {
                User.Say("promotes " + TargetClient.GetHabbo().Username + " to " + TargetClient.GetHabbo().RankInfo.Name);
            }
            else
            {
                User.Say("promotes " + TargetClient.GetHabbo().Username + " to " + TargetClient.GetHabbo().RankInfo.Name + " " + PlusEnvironment.Tiers(TargetClient.GetHabbo().Tier));
            }
            TargetClient.GetHabbo().RestartWork();
        }
    }
}