using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;


using Plus.HabboHotel.Moderation;

using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class IPBanCommand : IChatCommand
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
            get { return "<username> <reason>"; }
        }

        public string Description
        {
            get { return "Ban a citizen from the hotel with IP."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :ipban <username>");
                return;
            }

            Habbo Habbo = PlusEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("The user could not be found.");
                return;
            }

            if (Habbo.Rank != 1 && Habbo.Username != "Lango" || Habbo.Rank != 1 && Habbo.Username != "St33lix")
            {
                Session.SendWhisper("This citizen cannot be banned.");
                return;
            }

            String IPAddress = String.Empty;
            Double Expire = PlusEnvironment.GetUnixTimestamp() + 78892200;
            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");

                dbClient.SetQuery("SELECT `ip_last` FROM `users` WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                IPAddress = dbClient.getString();
            }

            string Reason = null;
            if (Params.Length >= 3)
                Reason = CommandManager.MergeParams(Params, 2);
            else
                Reason = "No reason given.";

            if (!string.IsNullOrEmpty(IPAddress))
                PlusEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.IP, IPAddress, Reason, Expire);
            PlusEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, Expire);

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();

            PlusEnvironment.GetGame().GetClientManager().sendStaffMsg(Username + " banned " + Session.GetHabbo().Username + " with IP from the hotel. reason: " + Reason);
        }
    }
}