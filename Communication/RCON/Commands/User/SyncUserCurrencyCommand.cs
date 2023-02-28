﻿using System;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    internal class SyncUserCurrencyCommand : IRconCommand
    {
        public string Description => "This command is used to sync a users specified currency to the database.";

        public string Parameters => "%userId% %currency%";

        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int userId))
                return false;

            GameClient client = PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the currency type
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string currency = Convert.ToString(parameters[1]);

            switch (currency)
            {
                default:
                    return false;

                case "coins":
                case "credits":
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("credits", client.GetHabbo().Credits);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    break;
                }

                case "pixels":
                case "duckets":
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `activity_points` = @duckets WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("duckets", client.GetHabbo().Duckets);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    break;
                }

                case "diamonds":
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `vip_points` = @diamonds WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("diamonds", client.GetHabbo().Diamonds);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    break;
                }

                case "gotw":
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `gotw_points` = @gotw WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("gotw", client.GetHabbo().GotwPoints);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    break;
                }
            }

            return true;
        }
    }
}