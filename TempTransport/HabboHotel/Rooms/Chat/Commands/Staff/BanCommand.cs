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
    class BanCommand : IChatCommand
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
            get { return "<pseudonym> <hours or perm> <reason>"; }
        }

        public string Description
        {
            get { return "Ban a citizen from the hotel."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :ban <username> <hours> <reason>");
                return;
            }

            Habbo Habbo = PlusEnvironment.GetHabboByUsername(Params[1]);
            if (Habbo == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user in the database");
                return;
            }

            if (Habbo.Rank > 2)
            {
                Session.SendWhisper("Oops, you cannot ban that user");
                return;
            }

            Double Expire = 0;
            string Hours = Params[2];
            if (String.IsNullOrEmpty(Hours) || Hours == "perm")
                Expire = PlusEnvironment.GetUnixTimestamp() + 78892200;
            else
                Expire = (PlusEnvironment.GetUnixTimestamp() + (Convert.ToDouble(Hours) * 3600));

            string Reason = null;
            if (Params.Length >= 4)
                Reason = CommandManager.MergeParams(Params, 3);
            else
                Reason = "No reason specified";

            string Username = Habbo.Username;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `user_info` SET `bans` = `bans` + '1' WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1");
            }

            PlusEnvironment.GetGame().GetModerationManager().BanUser(Session.GetHabbo().Username, ModerationBanType.USERNAME, Habbo.Username, Reason, Expire);

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
                TargetClient.Disconnect();

            PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Session.GetHabbo().Username + " banned " + Username + " for " + Hours + " hour(s). Reason: " + Reason);
        }
    }
}