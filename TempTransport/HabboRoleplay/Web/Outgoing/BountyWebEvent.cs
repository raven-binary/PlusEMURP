using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using System.Text.RegularExpressions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;
using System.Threading;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using WebHook;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class BountyWebEvent : IWebEvent
    {
        /// <summary>
        /// Executes socket data.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        /// <param name="Socket"></param>
        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {

            if (!PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Client, true) || !PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Socket))
                return;

            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {
                #region Create
                case "create":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Username = ReceivedData[1];
                        string Amount = ReceivedData[2];

                        if (PlusEnvironment.getCooldown("create_bounty" + Client.GetHabbo().Id))
                        {
                            Client.SendWhisper("You must 5 minutes before placing another bounty");
                            return;
                        }

                        if (Username == "")
                        {
                            Client.SendWhisper("Please enter a username");
                            return;
                        }

                        int BountyAmount;
                        if (int.TryParse(Amount, out BountyAmount) || Amount.StartsWith("0") || !Char.IsDigit(Convert.ToChar(Amount)))
                        {
                            if (BountyAmount < 500)
                            {
                                Client.SendWhisper("The minimum amount to place a bounty is $500");
                                return;
                            }
                        }

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
                        if (TargetClient == null)
                        {
                            Client.SendWhisper(Username + " could not be found");
                            return;
                        }

                        if (TargetClient == Client)
                        {
                            Client.SendWhisper("You cannot place a bounty on yourself");
                            return;
                        }

                        if (BountyAmount + 100 > Client.GetHabbo().Credits)
                        {
                            Client.SendWhisper("You do not have enough money for this transaction");
                            return;
                        }




                        
                        Client.GetHabbo().Credits = Client.GetHabbo().Credits - BountyAmount - 100;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));

                        Client.GetHabbo().updateCredits();

                        PlusEnvironment.addCooldown("create_bounty" + Client.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertMinutesToMilliseconds(5)));
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "my_stats;" + Client.GetHabbo().Credits + ";" + Client.GetHabbo().Duckets + ";" + Client.GetHabbo().EventPoints);
                        DataRow Check = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `bounties` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", TargetClient.GetHabbo().Id);
                            Check = dbClient.getRow();
                        }

                        if (Check != null)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `bounties` SET `amount` = `amount` + " + BountyAmount + ", `minutes` = 60  WHERE `user_id` = '" + TargetClient.GetHabbo().Id + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }

                            PlusEnvironment.GetGame().GetClientManager().HotelWhisper("a $" + PlusEnvironment.ConvertToPrice(BountyAmount) + " bounty has been placed on " + TargetClient.GetHabbo().Username + ", bringing the total to $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Check["amount"]) + BountyAmount));
                            Webhook.SendWebhook(":moneybag: a $" + PlusEnvironment.ConvertToPrice(BountyAmount) + " bounty has been placed on **" + TargetClient.GetHabbo().Username + "**, bringing the total to $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Check["amount"]) + BountyAmount));

                        }
                        else
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("INSERT INTO `bounties` (`user_id`,`amount`,`minutes`) VALUES (@userId, @amount, '60')");
                                dbClient.AddParameter("userId", TargetClient.GetHabbo().Id);
                                dbClient.AddParameter("amount", BountyAmount);
                                dbClient.RunQuery();
                            }

                            PlusEnvironment.GetGame().GetClientManager().HotelWhisper("a $" + PlusEnvironment.ConvertToPrice(BountyAmount) + " bounty has been placed on " + TargetClient.GetHabbo().Username);
                            Webhook.SendWebhook(":moneybag: a $" + PlusEnvironment.ConvertToPrice(BountyAmount) + " bounty has been placed on **" + TargetClient.GetHabbo().Username + "**");
                        }
                        TargetClient.GetHabbo().BountyTimer = 60;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "bounty;show");
                    }
                    break;
                #endregion
            }
        }
    }
}