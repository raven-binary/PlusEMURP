using System;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Rcon.Commands.User
{
    internal class TakeUserCurrencyCommand : IRconCommand
    {
        public string Description => "This command is used to take a specified amount of a specified currency from a user.";

        public string Parameters => "%userId% %currency% %amount%";

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

            if (!int.TryParse(parameters[2], out int amount))
                return false;

            switch (currency)
            {
                default:
                    return false;

                case "coins":
                case "credits":
                {
                    client.GetHabbo().Credits -= amount;

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("credits", client.GetHabbo().Credits);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    client.SendPacket(new CreditBalanceComposer(client.GetHabbo().Credits));
                    break;
                }

                case "pixels":
                case "duckets":
                {
                    client.GetHabbo().Duckets -= amount;

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `activity_points` = @duckets WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("duckets", client.GetHabbo().Duckets);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().Duckets, amount));
                    break;
                }

                case "diamonds":
                {
                    client.GetHabbo().Diamonds -= amount;

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `vip_points` = @diamonds WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("diamonds", client.GetHabbo().Diamonds);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().Diamonds, 0, 5));
                    break;
                }

                case "gotw":
                {
                    client.GetHabbo().GotwPoints -= amount;

                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `users` SET `gotw_points` = @gotw WHERE `id` = @id LIMIT 1");
                        dbClient.AddParameter("gotw", client.GetHabbo().GotwPoints);
                        dbClient.AddParameter("id", userId);
                        dbClient.RunQuery();
                    }

                    client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().GotwPoints, 0, 103));
                    break;
                }
            }

            return true;
        }
    }
}