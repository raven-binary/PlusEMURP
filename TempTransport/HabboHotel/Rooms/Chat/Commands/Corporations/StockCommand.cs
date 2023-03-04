using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus.Database.Interfaces;
using System.Data;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Notifications;
namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class StockCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Shows stock of each corporation"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            StringBuilder Stocks = new StringBuilder();

            DataRow Hospital = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", 2);
                Hospital = dbClient.getRow();
            }

            DataRow Armoury = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", 4);
                Armoury = dbClient.getRow();
            }

            DataRow Forever21 = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", 5);
                Forever21 = dbClient.getRow();
            }

            DataRow Starbucks = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", 6);
                Starbucks = dbClient.getRow();
            }

            Stocks.Append(Hospital["name"] + "\n - Stock: " + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Hospital["stock"])) + "\n\n");
            Stocks.Append(Armoury["name"] + "\n - Stock: " + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Armoury["stock"])) + "\n\n");
            Stocks.Append(Forever21["name"] + "\n - Stock: " + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Forever21["stock"])) + "\n\n");
            Stocks.Append(Starbucks["name"] + "\n - Stock: " + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Starbucks["stock"])) + "\n\n");

            Session.SendMessage(new MOTDNotificationComposer("Corporation stocks\n\n" + Stocks.ToString()));
        }
    }
}
