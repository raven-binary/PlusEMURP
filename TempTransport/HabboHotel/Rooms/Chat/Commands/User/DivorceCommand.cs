using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using System.Data;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Users.Relationships;
using Plus;
using WebHook;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DivorceCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
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
            get { return "Divorce from your relation"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :divorce <username>");
                return;
            }

            if (Session.GetRoleplay().MarriedTo == 0)
            {
                Session.SendWhisper("You are not married!");
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

                DataRow UserRPStats = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `user_rp_stats` WHERE `user_id` = @uid LIMIT 1");
                    dbClient.AddParameter("uid", Convert.ToInt32(GetUser["id"]));
                    UserRPStats = dbClient.getRow();
                }

                if (Convert.ToInt32(UserRPStats["married_to"]) != Session.GetHabbo().Id)
                {
                    Session.SendWhisper("You are not married with " + Username);
                    return;
                }

                Session.GetRoleplay().MarriedTo = 0;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `user_rp_stats` SET married_to = @target WHERE user_id = @uid");
                    dbClient.AddParameter("target", "0");
                    dbClient.AddParameter("uid", Session.GetHabbo().Id);
                    dbClient.RunQuery();

                    dbClient.SetQuery("UPDATE `user_rp_stats` SET married_to = @target WHERE user_id = @uid");
                    dbClient.AddParameter("target", "0");
                    dbClient.AddParameter("uid", Convert.ToInt32(GetUser["id"]));
                    dbClient.RunQuery();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", Convert.ToInt32(GetUser["id"]));
                    dbClient.RunQuery();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Convert.ToInt32(GetUser["id"]) + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", Session.GetHabbo().Id);
                    dbClient.RunQuery();

                    if (Session.GetHabbo().Relationships.ContainsKey(Convert.ToInt32(GetUser["id"])))
                        Session.GetHabbo().Relationships.Remove(Convert.ToInt32(GetUser["id"]));
                }

                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> has divorced <span class=\"red\">" + GetUser["username"] + "</span>");
                Webhook.SendWebhook(":ring: " + Session.GetHabbo().Username + " has divorced " + GetUser["username"]);
            }
            else
            {
                if (TargetClient.GetRoleplay().MarriedTo != Session.GetHabbo().Id)
                {
                    Session.SendWhisper("You are not married with " + TargetClient.GetHabbo().Username);
                    return;
                }

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `user_rp_stats` SET married_to = @target WHERE user_id = @uid");
                    dbClient.AddParameter("target", "0");
                    dbClient.AddParameter("uid", Session.GetHabbo().Id);
                    dbClient.RunQuery();

                    dbClient.SetQuery("UPDATE `user_rp_stats` SET married_to = @target WHERE user_id = @uid");
                    dbClient.AddParameter("target", "0");
                    dbClient.AddParameter("uid", TargetClient.GetHabbo().Id);
                    dbClient.RunQuery();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + Session.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", TargetClient.GetHabbo().Id);
                    dbClient.RunQuery();

                    dbClient.SetQuery("DELETE FROM `user_relationships` WHERE `user_id` = '" + TargetClient.GetHabbo().Id + "' AND `target` = @target LIMIT 1");
                    dbClient.AddParameter("target", Session.GetHabbo().Id);
                    dbClient.RunQuery();

                    if (Session.GetHabbo().Relationships.ContainsKey(TargetClient.GetHabbo().Id))
                        Session.GetHabbo().Relationships.Remove(TargetClient.GetHabbo().Id);

                    if (TargetClient.GetHabbo().Relationships.ContainsKey(Session.GetHabbo().Id))
                        TargetClient.GetHabbo().Relationships.Remove(Session.GetHabbo().Id);
                }

                TargetClient.SendWhisper(Session.GetHabbo().Username + " divorced you :(");
                Session.GetRoleplay().MarriedTo = 0;
                TargetClient.GetRoleplay().MarriedTo = 0;

                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> has divorced <span class=\"red\">" + TargetClient.GetHabbo().Username + "</span>");
                Webhook.SendWebhook(":ring: " + Session.GetHabbo().Username + " has divorced " + TargetClient.GetHabbo().Username);
            }
        }
    }
}
