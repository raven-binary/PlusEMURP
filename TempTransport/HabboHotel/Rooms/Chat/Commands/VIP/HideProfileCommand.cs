using System;
using System.Data;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class HideProfileCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 1)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "vip"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Hides your profiles from others to show"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (PlusEnvironment.getCooldown("hideprofile" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before you can toggle your profile status again");
                return;
            }

            DataRow User = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id,hide_profile FROM `users` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", Session.GetHabbo().Id);
                User = dbClient.getRow();
            }

            if (User == null)
                return;

            int HideProfile = Convert.ToInt32(User["hide_profile"]);
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (HideProfile == 1)
                {
                    dbClient.SetQuery("UPDATE `users` SET `hide_profile` = '0'  WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1;");
                    dbClient.RunQuery();
                    Session.SendWhisper("Your profile is now visible");
                }
                else if (HideProfile == 0)
                {
                    dbClient.SetQuery("UPDATE `users` SET `hide_profile` = '1'  WHERE `id` = '" + Session.GetHabbo().Id + "' LIMIT 1;");
                    dbClient.RunQuery();
                    Session.SendWhisper("Your profile is now hidden");
                }
            }
            PlusEnvironment.addCooldown("hideprofile" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(5)));
        }
    }
}
