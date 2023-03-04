using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Speech;
using System.Collections.Generic;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PurgeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Purge start/stop"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            PlusEnvironment.PurgeLoading = true;
            //PlusEnvironment.GetGame().GetClientManager().HotelAlert("Purge Event will be starting in 1 minutes!");

            PlusEnvironment.GetGame().GetClientManager().WorldEvent("show", 10, 5, "Purge", "No safe zones, no passive, 100% anarchy", 0, 0, 0);
            PlusEnvironment.Purge = true;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `server_status` SET `purge` = '1' LIMIT 1;");
                dbClient.RunQuery();
            }

            System.Timers.Timer endTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(5));
            endTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(5);
            endTimer.Elapsed += delegate
            {
                PlusEnvironment.Purge = false;
                DataRow Winner = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `world_event_joins` ORDER BY `collected` DESC LIMIT 1");
                    Winner = dbClient.getRow();
                }

                DataRow User = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", Winner["user_id"]);
                    User = dbClient.getRow();
                }

                //PlusEnvironment.GetGame().GetClientManager().HotelAlert(User["username"] + " wins the Purge Event with " + Winner["collected"] + " damage!");
                PlusEnvironment.GetGame().GetClientManager().WorldEvent("hide", 0, 0, "", "", 0, 0, 0);
                endTimer.Stop();
            };
            endTimer.Start();
        }
    }
}