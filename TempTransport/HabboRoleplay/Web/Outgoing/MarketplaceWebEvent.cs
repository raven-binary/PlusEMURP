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

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class MarketplaceWebEvent : IWebEvent
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
                #region Buy
                case "buy":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (PlusEnvironment.getCooldown("marketplace_buy" + Client.GetHabbo().Id))
                        {
                            if (!Client.GetHabbo().MarketplaceCooldown)
                            {
                                Client.GetHabbo().MarketplaceCooldown = true;
                                Client.SendWhisper("Calm down there! Please wait a tick before doing that again");
                                System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1));
                                Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(1);
                                Timer.Elapsed += delegate
                                {
                                    Client.GetHabbo().MarketplaceCooldown = false;
                                    Timer.Stop();
                                };
                                Timer.Start();
                            }
                            return;
                        }
                        PlusEnvironment.addCooldown("marketplace_buy" + Client.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(5)));

                        string[] ReceivedData = Data.Split(',');
                        int Id = Convert.ToInt32(ReceivedData[1]);
                        string Token = ReceivedData[2];

                        if (Token == "0" || Token.Length < 24)
                        {
                            Client.SendWhisper("Looks like a error occured, try again");
                            return;
                        }

                        DataRow Marketplace = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Id);
                            Marketplace = dbClient.getRow();
                        }
                        if (Marketplace == null)
                            return;

                        int UserId = Convert.ToInt32(Marketplace["user_id"]);
                        string Slot1Token = Convert.ToString(Marketplace["slot1_token"]);
                        string Slot2Token = Convert.ToString(Marketplace["slot2_token"]);
                        string Slot3Token = Convert.ToString(Marketplace["slot3_token"]);
                        string Slot4Token = Convert.ToString(Marketplace["slot4_token"]);
                        string Slot5Token = Convert.ToString(Marketplace["slot5_token"]);
                        string Slot6Token = Convert.ToString(Marketplace["slot6_token"]);
                        string Slot7Token = Convert.ToString(Marketplace["slot7_token"]);
                        string Slot8Token = Convert.ToString(Marketplace["slot8_token"]);
                        string Slot9Token = Convert.ToString(Marketplace["slot9_token"]);
                        string Slot10Token = Convert.ToString(Marketplace["slot10_token"]);

                        string Slot = null;
                        if (Slot1Token == Token)
                        {
                            Slot = "slot1";
                        }
                        else if (Slot2Token == Token)
                        {
                            Slot = "slot2";
                        }
                        else if (Slot3Token == Token)
                        {
                            Slot = "slot3";
                        }
                        else if (Slot4Token == Token)
                        {
                            Slot = "slot4";
                        }
                        else if (Slot5Token == Token)
                        {
                            Slot = "slot5";
                        }
                        else if (Slot6Token == Token)
                        {
                            Slot = "slot6";
                        }
                        else if (Slot7Token == Token)
                        {
                            Slot = "slot7";
                        }
                        else if (Slot8Token == Token)
                        {
                            Slot = "slot8";
                        }
                        else if (Slot9Token == Token)
                        {
                            Slot = "slot9";
                        }
                        else if (Slot10Token == Token)
                        {
                            Slot = "slot10";
                        }
                        else
                            return;

                        if (UserId == Client.GetHabbo().Id)
                            return;

                        string Item = Convert.ToString(Marketplace[Slot]);
                        int Durability = Convert.ToInt32(Marketplace[Slot + "_durability"]);
                        int Quantity = Convert.ToInt32(Marketplace[Slot + "_quantity"]);
                        int Price = Convert.ToInt32(Marketplace[Slot + "_price"]);
                        if (Price > Client.GetHabbo().Credits)
                        {
                            Client.SendWhisper("You need $" + Price + " to buy this item");
                            return;
                        }
                        Client.GetHabbo().Credits -= Price;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        Client.GetHabbo().RPCache(3);

                        if (Durability > 0)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `marketplace` SET `" + Slot + "_sold` = 1 WHERE `user_id` = '" + UserId + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }
                            GameClient Seller = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                            if (Seller != null)
                            {
                                Seller.SendWhisper("Your " + Item + " has been sold in the marketplace");
                            }

                            Client.GetHabbo().AddToInventory(Item, Durability);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace;preload");
                        }
                        else if (Quantity > 0)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `marketplace` SET `" + Slot + "_sold` = 1 WHERE `user_id` = '" + UserId + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }

                            GameClient Seller = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
                            if (Seller != null)
                            {
                                Seller.SendWhisper("Your (" + Quantity + ") " + Item + " has been sold in the marketplace");
                            }

                            Client.GetHabbo().AddToInventory2(Item, Quantity);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace;preload");
                        }
                        User.Say("purchases " + Item + " from the marketplace");
                    }
                    break;
                #endregion
                #region Create Sale
                case "create-sale":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().MarketplaceSales == Client.GetHabbo().MarketplaceSalesMax)
                        {
                            Client.SendWhisper("You have reached the maximum amount of sales (" + Client.GetHabbo().MarketplaceSales + "/" + Client.GetHabbo().MarketplaceSalesMax + ")");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Price = ReceivedData[1];

                        int Amount;
                        string Amount2 = Price;
                        if (!int.TryParse(Amount2, out Amount) || Convert.ToInt32(Price) <= 0 || Amount2.StartsWith("0"))
                        {
                            Client.SendWhisper("The price is invalid");
                            return;
                        }

                        if (Client.GetHabbo().CreateSaleItem == null)
                            return;

                        DataRow Marketplace = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Marketplace = dbClient.getRow();
                        }
                        if (Marketplace == null)
                            return;

                        string ToSlot = null;

                        string Slot1 = Convert.ToString(Marketplace["slot1"]);
                        string Slot2 = Convert.ToString(Marketplace["slot2"]);
                        string Slot3 = Convert.ToString(Marketplace["slot3"]);
                        string Slot4 = Convert.ToString(Marketplace["slot4"]);
                        string Slot5 = Convert.ToString(Marketplace["slot5"]);
                        string Slot6 = Convert.ToString(Marketplace["slot6"]);
                        string Slot7 = Convert.ToString(Marketplace["slot7"]);
                        string Slot8 = Convert.ToString(Marketplace["slot8"]);
                        string Slot9 = Convert.ToString(Marketplace["slot9"]);
                        string Slot10 = Convert.ToString(Marketplace["slot10"]);
                        if (Slot1 == "null")
                        {
                            ToSlot = "slot1";
                        }
                        else if (Slot2 == "null")
                        {
                            ToSlot = "slot2";
                        }
                        else if (Slot3 == "null")
                        {
                            ToSlot = "slot3";
                        }
                        else if (Slot4 == "null")
                        {
                            ToSlot = "slot4";
                        }
                        else if (Slot5 == "null")
                        {
                            ToSlot = "slot5";
                        }
                        else if (Slot6 == "null")
                        {
                            ToSlot = "slot6";
                        }
                        else if (Slot7 == "null")
                        {
                            ToSlot = "slot7";
                        }
                        else if (Slot8 == "null")
                        {
                            ToSlot = "slot8";
                        }
                        else if (Slot9 == "null")
                        {
                            ToSlot = "slot9";
                        }
                        else if (Slot10 == "null")
                        {
                            ToSlot = "slot10";
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE marketplace SET " + ToSlot + " = @slot, " + ToSlot + "_quantity = @quantity, " + ToSlot + "_durability = @durability, " + ToSlot + "_price = @price, " + ToSlot + "_token = @token, " + ToSlot + "_date = @date, " + ToSlot + "_sold = @sold, sales = sales + 1 WHERE user_id = @user_id LIMIT 1;");
                            dbClient.AddParameter("slot", Client.GetHabbo().CreateSaleItem);
                            dbClient.AddParameter("quantity", Client.GetHabbo().CreateSaleQuantity);
                            dbClient.AddParameter("durability", Client.GetHabbo().CreateSaleDurability);
                            dbClient.AddParameter("price", Price);
                            dbClient.AddParameter("token", PlusEnvironment.RandomString(24));
                            dbClient.AddParameter("date", DateTime.Now.ToUniversalTime());
                            dbClient.AddParameter("sold", 0);
                            dbClient.AddParameter("user_id", Client.GetHabbo().Id);
                            dbClient.RunQuery();

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace;preload");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "manage-sales;preload");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "create-sale;remove-item");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace-sales;" + Client.GetHabbo().MarketplaceSales + ";" + Client.GetHabbo().MarketplaceSalesMax);
                        }

                        /*using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `marketplace` SET `" + ToSlot + "` = '" + Client.GetHabbo().CreateSaleItem + "', `" + ToSlot + "_quantity` = '" + Client.GetHabbo().CreateSaleQuantity + "', `" + ToSlot + "_durability` = '" + Client.GetHabbo().CreateSaleDurability + "', `" + ToSlot + "_price` = '" + Price + "', `" + ToSlot + "_token` = '" + PlusEnvironment.RandomString(24) + "', `" + ToSlot + "_date` = " + DateTime.Now.ToUniversalTime() + ", `" + ToSlot + "_sold` = '0', `sales` = `sales` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }*/

                        //resetiing values
                        Client.GetHabbo().CreateSaleItem = null;
                        Client.GetHabbo().CreateSaleQuantity = 0;
                        Client.GetHabbo().CreateSaleDurability = 0;
                    }
                    break;
                #endregion
                #region Collect money
                case "collect-money":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (PlusEnvironment.getCooldown("collect_money" + Client.GetHabbo().Id))
                            return;
                        PlusEnvironment.addCooldown("collect_money" + Client.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(1)));

                        string[] ReceivedData = Data.Split(',');
                        string Token = ReceivedData[1];

                        if (Token == "0" || Token.Length < 24)
                        {
                            Client.SendWhisper("Looks like a error occured, try again");
                            return;
                        }

                        DataRow Marketplace = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Marketplace = dbClient.getRow();
                        }
                        if (Marketplace == null)
                            return;

                        int UserId = Convert.ToInt32(Marketplace["user_id"]);
                        if (UserId != Client.GetHabbo().Id)
                        {
                            Client.SendWhisper("Houston, we have a problem");
                            return;
                        }

                        string Slot1Token = Convert.ToString(Marketplace["slot1_token"]);
                        string Slot2Token = Convert.ToString(Marketplace["slot2_token"]);
                        string Slot3Token = Convert.ToString(Marketplace["slot3_token"]);
                        string Slot4Token = Convert.ToString(Marketplace["slot4_token"]);
                        string Slot5Token = Convert.ToString(Marketplace["slot5_token"]);
                        string Slot6Token = Convert.ToString(Marketplace["slot6_token"]);
                        string Slot7Token = Convert.ToString(Marketplace["slot7_token"]);
                        string Slot8Token = Convert.ToString(Marketplace["slot8_token"]);
                        string Slot9Token = Convert.ToString(Marketplace["slot9_token"]);
                        string Slot10Token = Convert.ToString(Marketplace["slot10_token"]);

                        string Slot = null;
                        if (Slot1Token == Token)
                        {
                            Slot = "slot1";
                        }
                        else if (Slot2Token == Token)
                        {
                            Slot = "slot2";
                        }
                        else if (Slot3Token == Token)
                        {
                            Slot = "slot3";
                        }
                        else if (Slot4Token == Token)
                        {
                            Slot = "slot4";
                        }
                        else if (Slot5Token == Token)
                        {
                            Slot = "slot5";
                        }
                        else if (Slot6Token == Token)
                        {
                            Slot = "slot6";
                        }
                        else if (Slot7Token == Token)
                        {
                            Slot = "slot7";
                        }
                        else if (Slot8Token == Token)
                        {
                            Slot = "slot8";
                        }
                        else if (Slot9Token == Token)
                        {
                            Slot = "slot9";
                        }
                        else if (Slot10Token == Token)
                        {
                            Slot = "slot10";
                        }
                        else
                            return;

                        string Item = Convert.ToString(Marketplace[Slot]);
                        int Sold = Convert.ToInt32(Marketplace[Slot + "_sold"]);
                        if (Sold == 0)
                        {
                            Client.SendWhisper("Houston, we have a problem");
                            return;
                        }
                        int Price = Convert.ToInt32(Marketplace[Slot + "_price"]);

                        Client.GetHabbo().Credits += Price;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        Client.GetHabbo().RPCache(3);
                        Client.SendWhisper("You have received $" + PlusEnvironment.ConvertToPrice(Price) + " for selling " + Item);

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `marketplace` SET `" + Slot + "` = 'null', `" + Slot + "_quantity` = '0', `" + Slot + "_durability` = '0', `" + Slot + "_price` = '0', `" + Slot + "_token` = '0', `" + Slot + "_date` = '2021-10-23 12:16:33', `" + Slot + "_sold` = '0', `sales` = `sales`- 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                            dbClient.RunQuery();
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "manage-sales;preload"); //since the query runned let's preload the '#manage-sales' box
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace-sales;" + Client.GetHabbo().MarketplaceSales + ";" + Client.GetHabbo().MarketplaceSalesMax);
                        }
                    }
                    break;
                #endregion
                #region Remove item
                case "remove-item":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Token = ReceivedData[1];

                        if (Token == "0" || Token.Length < 24)
                        {
                            Client.SendWhisper("Looks like a error occured, try again");
                            return;
                        }

                        DataRow Marketplace = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Marketplace = dbClient.getRow();
                        }
                        if (Marketplace == null)
                            return;

                        int UserId = Convert.ToInt32(Marketplace["user_id"]);
                        if (UserId != Client.GetHabbo().Id)
                        {
                            Client.SendWhisper("Houston, we have a problem");
                            return;
                        }

                        string Slot1Token = Convert.ToString(Marketplace["slot1_token"]);
                        string Slot2Token = Convert.ToString(Marketplace["slot2_token"]);
                        string Slot3Token = Convert.ToString(Marketplace["slot3_token"]);
                        string Slot4Token = Convert.ToString(Marketplace["slot4_token"]);
                        string Slot5Token = Convert.ToString(Marketplace["slot5_token"]);
                        string Slot6Token = Convert.ToString(Marketplace["slot6_token"]);
                        string Slot7Token = Convert.ToString(Marketplace["slot7_token"]);
                        string Slot8Token = Convert.ToString(Marketplace["slot8_token"]);
                        string Slot9Token = Convert.ToString(Marketplace["slot9_token"]);
                        string Slot10Token = Convert.ToString(Marketplace["slot10_token"]);

                        string Slot = null;
                        if (Slot1Token == Token)
                        {
                            Slot = "slot1";
                        }
                        else if (Slot2Token == Token)
                        {
                            Slot = "slot2";
                        }
                        else if (Slot3Token == Token)
                        {
                            Slot = "slot3";
                        }
                        else if (Slot4Token == Token)
                        {
                            Slot = "slot4";
                        }
                        else if (Slot5Token == Token)
                        {
                            Slot = "slot5";
                        }
                        else if (Slot6Token == Token)
                        {
                            Slot = "slot6";
                        }
                        else if (Slot7Token == Token)
                        {
                            Slot = "slot7";
                        }
                        else if (Slot8Token == Token)
                        {
                            Slot = "slot8";
                        }
                        else if (Slot9Token == Token)
                        {
                            Slot = "slot9";
                        }
                        else if (Slot10Token == Token)
                        {
                            Slot = "slot10";
                        }
                        else
                            return;

                        string Item = Convert.ToString(Marketplace[Slot]);
                        int Durability = Convert.ToInt32(Marketplace[Slot + "_durability"]);
                        int Quantity = Convert.ToInt32(Marketplace[Slot + "_quantity"]);
                        int Sold = Convert.ToInt32(Marketplace[Slot + "_sold"]);
                        if (Sold == 1)
                        {
                            Client.SendWhisper("Houston, we have a problem");
                            return;
                        }

                        if (Durability > 0)
                        {
                            Client.GetHabbo().AddToInventory(Item, Durability);
                        }
                        else if (Quantity > 0)
                        {
                            Client.GetHabbo().AddToInventory2(Item, Quantity);
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `marketplace` SET `" + Slot + "` = 'null', `" + Slot + "_quantity` = '0', `" + Slot + "_durability` = '0', `" + Slot + "_price` = '0', `" + Slot + "_token` = '0', `" + Slot + "_date` = '2021-10-23 12:16:33', `" + Slot + "_sold` = '0', `sales` = `sales`- 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                            dbClient.RunQuery();
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "manage-sales;preload");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace-sales;" + Client.GetHabbo().MarketplaceSales + ";" + Client.GetHabbo().MarketplaceSalesMax);
                        }
                    }
                    break;
                #endregion
                #region Take item
                case "take-item":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Token = ReceivedData[1];

                        if (Token == "0" || Token.Length < 24)
                        {
                            Client.SendWhisper("Looks like a error occured, try again");
                            return;
                        }

                        DataRow Marketplace = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Marketplace = dbClient.getRow();
                        }
                        if (Marketplace == null)
                            return;

                        int UserId = Convert.ToInt32(Marketplace["user_id"]);
                        if (UserId != Client.GetHabbo().Id)
                        {
                            Client.SendWhisper("Houston, we have a problem");
                            return;
                        }

                        string Slot1Token = Convert.ToString(Marketplace["slot1_token"]);
                        string Slot2Token = Convert.ToString(Marketplace["slot2_token"]);
                        string Slot3Token = Convert.ToString(Marketplace["slot3_token"]);
                        string Slot4Token = Convert.ToString(Marketplace["slot4_token"]);
                        string Slot5Token = Convert.ToString(Marketplace["slot5_token"]);
                        string Slot6Token = Convert.ToString(Marketplace["slot6_token"]);
                        string Slot7Token = Convert.ToString(Marketplace["slot7_token"]);
                        string Slot8Token = Convert.ToString(Marketplace["slot8_token"]);
                        string Slot9Token = Convert.ToString(Marketplace["slot9_token"]);
                        string Slot10Token = Convert.ToString(Marketplace["slot10_token"]);

                        string Slot = null;
                        if (Slot1Token == Token)
                        {
                            Slot = "slot1";
                        }
                        else if (Slot2Token == Token)
                        {
                            Slot = "slot2";
                        }
                        else if (Slot3Token == Token)
                        {
                            Slot = "slot3";
                        }
                        else if (Slot4Token == Token)
                        {
                            Slot = "slot4";
                        }
                        else if (Slot5Token == Token)
                        {
                            Slot = "slot5";
                        }
                        else if (Slot6Token == Token)
                        {
                            Slot = "slot6";
                        }
                        else if (Slot7Token == Token)
                        {
                            Slot = "slot7";
                        }
                        else if (Slot8Token == Token)
                        {
                            Slot = "slot8";
                        }
                        else if (Slot9Token == Token)
                        {
                            Slot = "slot9";
                        }
                        else if (Slot10Token == Token)
                        {
                            Slot = "slot10";
                        }
                        else
                            return;

                        string Item = Convert.ToString(Marketplace[Slot]);
                        int Durability = Convert.ToInt32(Marketplace[Slot + "_durability"]);
                        int Quantity = Convert.ToInt32(Marketplace[Slot + "_quantity"]);
                        int Sold = Convert.ToInt32(Marketplace[Slot + "_sold"]);
                        if (Sold == 1)
                        {
                            Client.SendWhisper("Houston, we have a problem");
                            return;
                        }

                        if (Durability > 0)
                        {
                            Client.GetHabbo().AddToInventory(Item, Durability);
                        }
                        else if (Quantity > 0)
                        {
                            Client.GetHabbo().AddToInventory2(Item, Quantity);
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `marketplace` SET `" + Slot + "` = 'null', `" + Slot + "_quantity` = '0', `" + Slot + "_durability` = '0', `" + Slot + "_price` = '0', `" + Slot + "_token` = '0', `" + Slot + "_date` = '2021-10-23 12:16:33', `" + Slot + "_sold` = '0', `sales` = `sales`- 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                            dbClient.RunQuery();
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "manage-sales;preload");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace;preload");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace-sales;" + Client.GetHabbo().MarketplaceSales + ";" + Client.GetHabbo().MarketplaceSalesMax);
                        }
                    }
                    break;
                #endregion

                #region Create Sale Add
                case "create-sale-add":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!Client.GetHabbo().usingCreateSale || Client.GetHabbo().CreateSaleItem != null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        DataRow Inventory = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `inventory` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Inventory = dbClient.getRow();
                        }
                        if (Inventory == null)
                            return;

                        string Item = Convert.ToString(Inventory[Slot]);
                        int Quantity = Convert.ToInt32(Inventory[Slot + "_quantity"]);
                        int Durability = Convert.ToInt32(Inventory[Slot + "_durability"]);

                        if (Durability > 0)
                        {
                            #region Slot
                            if (Slot == "slot1")
                            {
                                Client.GetHabbo().InventorySlot1 = null;
                                Client.GetHabbo().InventorySlot1Quantity = 0;
                                Client.GetHabbo().InventorySlot1Durability = 0;
                            }
                            else if (Slot == "slot2")
                            {
                                Client.GetHabbo().InventorySlot2 = null;
                                Client.GetHabbo().InventorySlot2Quantity = 0;
                                Client.GetHabbo().InventorySlot2Durability = 0;
                            }
                            else if (Slot == "slot3")
                            {
                                Client.GetHabbo().InventorySlot3 = null;
                                Client.GetHabbo().InventorySlot3Quantity = 0;
                                Client.GetHabbo().InventorySlot3Durability = 0;
                            }
                            else if (Slot == "slot4")
                            {
                                Client.GetHabbo().InventorySlot4 = null;
                                Client.GetHabbo().InventorySlot4Quantity = 0;
                                Client.GetHabbo().InventorySlot4Durability = 0;
                            }
                            else if (Slot == "slot5")
                            {
                                Client.GetHabbo().InventorySlot5 = null;
                                Client.GetHabbo().InventorySlot6Quantity = 0;
                                Client.GetHabbo().InventorySlot5Durability = 0;
                            }
                            else if (Slot == "slot6")
                            {
                                Client.GetHabbo().InventorySlot6 = null;
                                Client.GetHabbo().InventorySlot6Quantity = 0;
                                Client.GetHabbo().InventorySlot6Durability = 0;
                            }
                            else if (Slot == "slot7")
                            {
                                Client.GetHabbo().InventorySlot7 = null;
                                Client.GetHabbo().InventorySlot8Quantity = 0;
                                Client.GetHabbo().InventorySlot7Durability = 0;
                            }
                            else if (Slot == "slot8")
                            {
                                Client.GetHabbo().InventorySlot8 = null;
                                Client.GetHabbo().InventorySlot8Quantity = 0;
                                Client.GetHabbo().InventorySlot8Durability = 0;
                            }
                            else if (Slot == "slot9")
                            {
                                Client.GetHabbo().InventorySlot9 = null;
                                Client.GetHabbo().InventorySlot9Quantity = 0;
                                Client.GetHabbo().InventorySlot9Durability = 0;
                            }
                            else if (Slot == "slot10")
                            {
                                Client.GetHabbo().InventorySlot10 = null;
                                Client.GetHabbo().InventorySlot10Quantity = 0;
                                Client.GetHabbo().InventorySlot10Durability = 0;
                            }
                            #endregion

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_quantity = 0, " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;0;" + Slot + ";0");
                            }
                        }
                        else if (Quantity > 0)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + Quantity);
                        }

                        Client.GetHabbo().CreateSaleItem = Item;
                        Client.GetHabbo().CreateSaleQuantity = Quantity;
                        Client.GetHabbo().CreateSaleDurability = Durability;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "create-sale;add;" + Item + ";" + Quantity + ";" + Durability);
                    }
                    break;
                #endregion
                #region Create Sale Window
                case "create-sale-window":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().usingCreateSale)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "create-sale;hide");
                        }
                        else if (!Client.GetHabbo().usingCreateSale)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "create-sale;show");
                        }
                        Client.GetHabbo().usingCreateSale = !Client.GetHabbo().usingCreateSale;
                    }
                    break;
                #endregion
                #region Manage Sales Window
                case "manage-sales-window":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().usingManageSales)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "manage-sales;hide");
                        }
                        else if (!Client.GetHabbo().usingManageSales)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "marketplace-sales;" + Client.GetHabbo().MarketplaceSales + ";" + Client.GetHabbo().MarketplaceSalesMax);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "manage-sales;show");
                        }
                        Client.GetHabbo().usingManageSales = !Client.GetHabbo().usingManageSales;
                    }
                    break;
                    #endregion
            }
        }
    }
}