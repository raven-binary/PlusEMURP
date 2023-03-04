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

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class TradeWebEvent : IWebEvent
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
                #region Add Item
                case "add":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().isTradingWith == null)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().isTradingWith);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }

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

                            string ToSlot = null;
                            if (Client.GetHabbo().TradeSlot1 == null)
                            {
                                ToSlot = "1";
                                Client.GetHabbo().TradeSlot1 = Item;
                                Client.GetHabbo().TradeSlot1Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot2 == null)
                            {
                                ToSlot = "2";
                                Client.GetHabbo().TradeSlot2 = Item;
                                Client.GetHabbo().TradeSlot2Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot3 == null)
                            {
                                ToSlot = "3";
                                Client.GetHabbo().TradeSlot3 = Item;
                                Client.GetHabbo().TradeSlot3Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot4 == null)
                            {
                                ToSlot = "4";
                                Client.GetHabbo().TradeSlot4 = Item;
                                Client.GetHabbo().TradeSlot4Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot5 == null)
                            {
                                ToSlot = "5";
                                Client.GetHabbo().TradeSlot5 = Item;
                                Client.GetHabbo().TradeSlot5Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot6 == null)
                            {
                                ToSlot = "6";
                                Client.GetHabbo().TradeSlot6 = Item;
                                Client.GetHabbo().TradeSlot6Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot7 == null)
                            {
                                ToSlot = "7";
                                Client.GetHabbo().TradeSlot7 = Item;
                                Client.GetHabbo().TradeSlot7Durability = Durability;
                            }
                            else if (Client.GetHabbo().TradeSlot8 == null)
                            {
                                ToSlot = "8";
                                Client.GetHabbo().TradeSlot8 = Item;
                                Client.GetHabbo().TradeSlot8Durability = Durability;
                            }
                            else
                            {
                                Client.SendWhisper("Your trade slots is full!");
                            }

                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;" + ToSlot + ";" + Item + ";" + Quantity + ";" + Durability);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;" + ToSlot + ";" + Item + ";" + Quantity + ";" + Durability);
                        }
                        else if (Quantity > 0)
                        {
                            if (Client.GetHabbo().TradeSlot1 == Item && Client.GetHabbo().TradeSlot1Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot1Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;1;" + Item + ";" + Client.GetHabbo().TradeSlot1Quantity + ";" + Client.GetHabbo().TradeSlot1Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;1;" + Item + ";" + Client.GetHabbo().TradeSlot1Quantity + ";" + Client.GetHabbo().TradeSlot1Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot2 == Item && Client.GetHabbo().TradeSlot2Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot2Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;2;" + Item + ";" + Client.GetHabbo().TradeSlot2Quantity + ";" + Client.GetHabbo().TradeSlot2Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;2;" + Item + ";" + Client.GetHabbo().TradeSlot2Quantity + ";" + Client.GetHabbo().TradeSlot2Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot3 == Item && Client.GetHabbo().TradeSlot3Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot3Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;3;" + Item + ";" + Client.GetHabbo().TradeSlot3Quantity + ";" + Client.GetHabbo().TradeSlot3Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;3;" + Item + ";" + Client.GetHabbo().TradeSlot3Quantity + ";" + Client.GetHabbo().TradeSlot3Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot4 == Item && Client.GetHabbo().TradeSlot4Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot4Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;4;" + Item + ";" + Client.GetHabbo().TradeSlot4Quantity + ";" + Client.GetHabbo().TradeSlot4Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;4;" + Item + ";" + Client.GetHabbo().TradeSlot4Quantity + ";" + Client.GetHabbo().TradeSlot4Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot5 == Item && Client.GetHabbo().TradeSlot5Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot5Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;5;" + Item + ";" + Client.GetHabbo().TradeSlot5Quantity + ";" + Client.GetHabbo().TradeSlot5Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;5;" + Item + ";" + Client.GetHabbo().TradeSlot5Quantity + ";" + Client.GetHabbo().TradeSlot5Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot6 == Item && Client.GetHabbo().TradeSlot6Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot6Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;6;" + Item + ";" + Client.GetHabbo().TradeSlot6Quantity + ";" + Client.GetHabbo().TradeSlot6Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;6;" + Item + ";" + Client.GetHabbo().TradeSlot6Quantity + ";" + Client.GetHabbo().TradeSlot6Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot7 == Item && Client.GetHabbo().TradeSlot7Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot7Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;7;" + Item + ";" + Client.GetHabbo().TradeSlot7Quantity + ";" + Client.GetHabbo().TradeSlot7Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;7;" + Item + ";" + Client.GetHabbo().TradeSlot7Quantity + ";" + Client.GetHabbo().TradeSlot7Durability);
                            }
                            else if (Client.GetHabbo().TradeSlot8 == Item && Client.GetHabbo().TradeSlot8Quantity < 5)
                            {
                                Client.GetHabbo().TradeSlot8Quantity += 1;
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;8;" + Item + ";" + Client.GetHabbo().TradeSlot8Quantity + ";" + Client.GetHabbo().TradeSlot8Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;8;" + Item + ";" + Client.GetHabbo().TradeSlot8Quantity + ";" + Client.GetHabbo().TradeSlot8Durability);
                            }
                            else
                            {
                                if (Client.GetHabbo().TradeSlot1 == null)
                                {
                                    Client.GetHabbo().TradeSlot1 = Item;
                                    Client.GetHabbo().TradeSlot1Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;1;" + Item + ";" + Client.GetHabbo().TradeSlot1Quantity + ";" + Client.GetHabbo().TradeSlot1Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;1;" + Item + ";" + Client.GetHabbo().TradeSlot1Quantity + ";" + Client.GetHabbo().TradeSlot1Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot2 == null)
                                {
                                    Client.GetHabbo().TradeSlot2 = Item;
                                    Client.GetHabbo().TradeSlot2Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;2;" + Item + ";" + Client.GetHabbo().TradeSlot2Quantity + ";" + Client.GetHabbo().TradeSlot2Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;2;" + Item + ";" + Client.GetHabbo().TradeSlot2Quantity + ";" + Client.GetHabbo().TradeSlot2Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot3 == null)
                                {
                                    Client.GetHabbo().TradeSlot3 = Item;
                                    Client.GetHabbo().TradeSlot3Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;3;" + Item + ";" + Client.GetHabbo().TradeSlot3Quantity + ";" + Client.GetHabbo().TradeSlot3Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;3;" + Item + ";" + Client.GetHabbo().TradeSlot3Quantity + ";" + Client.GetHabbo().TradeSlot3Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot4 == null)
                                {
                                    Client.GetHabbo().TradeSlot4 = Item;
                                    Client.GetHabbo().TradeSlot4Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;4;" + Item + ";" + Client.GetHabbo().TradeSlot4Quantity + ";" + Client.GetHabbo().TradeSlot4Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;4;" + Item + ";" + Client.GetHabbo().TradeSlot4Quantity + ";" + Client.GetHabbo().TradeSlot4Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot5 == null)
                                {
                                    Client.GetHabbo().TradeSlot5 = Item;
                                    Client.GetHabbo().TradeSlot5Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;5;" + Item + ";" + Client.GetHabbo().TradeSlot5Quantity + ";" + Client.GetHabbo().TradeSlot5Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;5;" + Item + ";" + Client.GetHabbo().TradeSlot5Quantity + ";" + Client.GetHabbo().TradeSlot5Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot6 == null)
                                {
                                    Client.GetHabbo().TradeSlot6 = Item;
                                    Client.GetHabbo().TradeSlot6Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;6;" + Item + ";" + Client.GetHabbo().TradeSlot6Quantity + ";" + Client.GetHabbo().TradeSlot6Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;6;" + Item + ";" + Client.GetHabbo().TradeSlot6Quantity + ";" + Client.GetHabbo().TradeSlot6Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot7 == null)
                                {
                                    Client.GetHabbo().TradeSlot7 = Item;
                                    Client.GetHabbo().TradeSlot7Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;7;" + Item + ";" + Client.GetHabbo().TradeSlot7Quantity + ";" + Client.GetHabbo().TradeSlot7Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;7;" + Item + ";" + Client.GetHabbo().TradeSlot7Quantity + ";" + Client.GetHabbo().TradeSlot7Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot8 == null)
                                {
                                    Client.GetHabbo().TradeSlot8 = Item;
                                    Client.GetHabbo().TradeSlot8Quantity += 1;
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-," + 1);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;8;" + Item + ";" + Client.GetHabbo().TradeSlot8Quantity + ";" + Client.GetHabbo().TradeSlot8Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;8;" + Item + ";" + Client.GetHabbo().TradeSlot8Quantity + ";" + Client.GetHabbo().TradeSlot8Durability);
                                }
                                else
                                {
                                    Client.SendWhisper("Your trade slots is full!");
                                }
                            }
                        }
                    }
                    break;
                #endregion
                #region Take Item
                case "take-item":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetHabbo().getCooldown("trade_take_item_" + Slot))
                            return;

                        Client.GetHabbo().addCooldown("trade_take_item_" + Slot, 500);

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().isTradingWith);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }

                        string FromSlot = "";
                        if (Slot == "slot1" && Client.GetHabbo().TradeSlot1 != null)
                        {
                            FromSlot = "1";
                            if (Client.GetHabbo().TradeSlot1Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot1, Client.GetHabbo().TradeSlot1Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot1 = null;
                                Client.GetHabbo().TradeSlot1Quantity = 0;
                                Client.GetHabbo().TradeSlot1Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot1Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot1Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot1, 1);

                                if (Client.GetHabbo().TradeSlot1Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot1 = null;
                                    Client.GetHabbo().TradeSlot1Quantity = 0;
                                    Client.GetHabbo().TradeSlot1Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;1;" + Client.GetHabbo().TradeSlot1 + ";" + Client.GetHabbo().TradeSlot1Quantity + ";" + Client.GetHabbo().TradeSlot1Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;1;" + Client.GetHabbo().TradeSlot1 + ";" + Client.GetHabbo().TradeSlot1Quantity + ";" + Client.GetHabbo().TradeSlot1Durability);
                                }
                            }
                        }
                        else if (Slot == "slot2" && Client.GetHabbo().TradeSlot2 != null)
                        {
                            FromSlot = "2";
                            if (Client.GetHabbo().TradeSlot2Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot2, Client.GetHabbo().TradeSlot2Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot2 = null;
                                Client.GetHabbo().TradeSlot2Quantity = 0;
                                Client.GetHabbo().TradeSlot2Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot2Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot2Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot2, 1);

                                if (Client.GetHabbo().TradeSlot2Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot2 = null;
                                    Client.GetHabbo().TradeSlot2Quantity = 0;
                                    Client.GetHabbo().TradeSlot2Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;2;" + Client.GetHabbo().TradeSlot2 + ";" + Client.GetHabbo().TradeSlot2Quantity + ";" + Client.GetHabbo().TradeSlot2Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;2;" + Client.GetHabbo().TradeSlot2 + ";" + Client.GetHabbo().TradeSlot2Quantity + ";" + Client.GetHabbo().TradeSlot2Durability);
                                }
                            }
                        }
                        else if (Slot == "slot3" && Client.GetHabbo().TradeSlot3 != null)
                        {
                            FromSlot = "3";
                            if (Client.GetHabbo().TradeSlot3Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot3, Client.GetHabbo().TradeSlot3Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot3 = null;
                                Client.GetHabbo().TradeSlot3Quantity = 0;
                                Client.GetHabbo().TradeSlot3Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot3Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot3Quantity -= 1;
                                Client.GetHabbo().AddToInventory3(Client.GetHabbo().TradeSlot3, 1);

                                if (Client.GetHabbo().TradeSlot3Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot3 = null;
                                    Client.GetHabbo().TradeSlot3Quantity = 0;
                                    Client.GetHabbo().TradeSlot3Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;3;" + Client.GetHabbo().TradeSlot3 + ";" + Client.GetHabbo().TradeSlot3Quantity + ";" + Client.GetHabbo().TradeSlot3Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;3;" + Client.GetHabbo().TradeSlot3 + ";" + Client.GetHabbo().TradeSlot3Quantity + ";" + Client.GetHabbo().TradeSlot3Durability);
                                }
                            }
                        }
                        else if (Slot == "slot4" && Client.GetHabbo().TradeSlot4 != null)
                        {
                            FromSlot = "4";
                            if (Client.GetHabbo().TradeSlot4Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot4, Client.GetHabbo().TradeSlot4Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot4 = null;
                                Client.GetHabbo().TradeSlot4Quantity = 0;
                                Client.GetHabbo().TradeSlot4Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot4Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot4Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot4, 1);

                                if (Client.GetHabbo().TradeSlot4Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot4 = null;
                                    Client.GetHabbo().TradeSlot4Quantity = 0;
                                    Client.GetHabbo().TradeSlot4Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;4;" + Client.GetHabbo().TradeSlot4 + ";" + Client.GetHabbo().TradeSlot4Quantity + ";" + Client.GetHabbo().TradeSlot4Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;4;" + Client.GetHabbo().TradeSlot4 + ";" + Client.GetHabbo().TradeSlot4Quantity + ";" + Client.GetHabbo().TradeSlot4Durability);
                                }
                            }
                        }
                        else if (Slot == "slot5" && Client.GetHabbo().TradeSlot5 != null)
                        {
                            FromSlot = "5";
                            if (Client.GetHabbo().TradeSlot5Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot5, Client.GetHabbo().TradeSlot5Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot5 = null;
                                Client.GetHabbo().TradeSlot5Quantity = 0;
                                Client.GetHabbo().TradeSlot5Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot5Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot5Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot5, 1);

                                if (Client.GetHabbo().TradeSlot5Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot5 = null;
                                    Client.GetHabbo().TradeSlot5Quantity = 0;
                                    Client.GetHabbo().TradeSlot5Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;5;" + Client.GetHabbo().TradeSlot5 + ";" + Client.GetHabbo().TradeSlot5Quantity + ";" + Client.GetHabbo().TradeSlot5Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;5;" + Client.GetHabbo().TradeSlot5 + ";" + Client.GetHabbo().TradeSlot5Quantity + ";" + Client.GetHabbo().TradeSlot5Durability);
                                }
                            }
                        }
                        else if (Slot == "slot6" && Client.GetHabbo().TradeSlot6 != null)
                        {
                            FromSlot = "6";
                            if (Client.GetHabbo().TradeSlot6Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot6, Client.GetHabbo().TradeSlot6Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot6 = null;
                                Client.GetHabbo().TradeSlot6Quantity = 0;
                                Client.GetHabbo().TradeSlot6Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot6Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot6Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot6, 1);

                                if (Client.GetHabbo().TradeSlot6Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot6 = null;
                                    Client.GetHabbo().TradeSlot6Quantity = 0;
                                    Client.GetHabbo().TradeSlot6Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;6;" + Client.GetHabbo().TradeSlot6 + ";" + Client.GetHabbo().TradeSlot6Quantity + ";" + Client.GetHabbo().TradeSlot6Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;6;" + Client.GetHabbo().TradeSlot6 + ";" + Client.GetHabbo().TradeSlot6Quantity + ";" + Client.GetHabbo().TradeSlot6Durability);
                                }
                            }
                        }
                        else if (Slot == "slot7" && Client.GetHabbo().TradeSlot7 != null)
                        {
                            FromSlot = "7";
                            if (Client.GetHabbo().TradeSlot7Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot7, Client.GetHabbo().TradeSlot7Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot7 = null;
                                Client.GetHabbo().TradeSlot7Quantity = 0;
                                Client.GetHabbo().TradeSlot7Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot7Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot7Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot7, 1);

                                if (Client.GetHabbo().TradeSlot7Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot7 = null;
                                    Client.GetHabbo().TradeSlot7Quantity = 0;
                                    Client.GetHabbo().TradeSlot7Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;7;" + Client.GetHabbo().TradeSlot7 + ";" + Client.GetHabbo().TradeSlot7Quantity + ";" + Client.GetHabbo().TradeSlot7Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;7;" + Client.GetHabbo().TradeSlot7 + ";" + Client.GetHabbo().TradeSlot7Quantity + ";" + Client.GetHabbo().TradeSlot7Durability);
                                }
                            }
                        }
                        else if (Slot == "slot8" && Client.GetHabbo().TradeSlot8 != null)
                        {
                            FromSlot = "8";
                            if (Client.GetHabbo().TradeSlot8Durability > 0)
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot8, Client.GetHabbo().TradeSlot8Durability);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);

                                Client.GetHabbo().TradeSlot8 = null;
                                Client.GetHabbo().TradeSlot8Quantity = 0;
                                Client.GetHabbo().TradeSlot8Durability = 0;
                            }
                            else if (Client.GetHabbo().TradeSlot8Quantity > 0)
                            {
                                Client.GetHabbo().TradeSlot8Quantity -= 1;
                                Client.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot8, 1);

                                if (Client.GetHabbo().TradeSlot8Quantity <= 0)
                                {
                                    Client.GetHabbo().TradeSlot8 = null;
                                    Client.GetHabbo().TradeSlot8Quantity = 0;
                                    Client.GetHabbo().TradeSlot8Durability = 0;
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-take-item;me;" + FromSlot);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-take-item;partner;" + FromSlot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-add;me;8;" + Client.GetHabbo().TradeSlot8 + ";" + Client.GetHabbo().TradeSlot8Quantity + ";" + Client.GetHabbo().TradeSlot8Durability);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-add;partner;8;" + Client.GetHabbo().TradeSlot8 + ";" + Client.GetHabbo().TradeSlot8Quantity + ";" + Client.GetHabbo().TradeSlot8Durability);
                                }
                            }
                        }
                    }
                    break;
                #endregion
                #region Offer Money
                case "offer-money":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().isTradingWith == null)
                            return;

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().isTradingWith);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Money = ReceivedData[1];

                        int Amount;
                        string Montant = Money;
                        if (!int.TryParse(Montant, out Amount) || Convert.ToInt32(Money) <= 0 || Montant.StartsWith("0"))
                        {
                            Client.SendWhisper("The amount is invalid");
                            return;
                        }

                        if (Convert.ToInt32(Montant) > Client.GetHabbo().Credits)
                        {
                            Client.SendWhisper("You do not have that much money");
                            return;
                        }

                        Client.GetHabbo().TradeMoney = Convert.ToInt32(Money);

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-money;me;" + Client.GetHabbo().TradeMoney);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-money;partner;" + Client.GetHabbo().TradeMoney);
                    }
                    break;
                #endregion
                #region Accept
                case "accept":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().isTradingWith == null)
                            return;

                        if (Client.GetHabbo().getCooldown("trade_accept"))
                            return;

                        Client.GetHabbo().addCooldown("trade_accept", 1000);

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().isTradingWith);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }

                        if (TargetClient.GetHabbo().TradeConfirmed)
                        {
                            #region Trade Me
                            if (TargetClient.GetHabbo().TradeMoney > 0)
                            {
                                Client.GetHabbo().Credits += TargetClient.GetHabbo().TradeMoney;
                                Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                Client.GetHabbo().RPCache(3);

                                TargetClient.GetHabbo().Credits -= TargetClient.GetHabbo().TradeMoney;
                                TargetClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                TargetClient.GetHabbo().RPCache(3);
                            }
                            if (TargetClient.GetHabbo().TradeSlot1 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot1Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot1, TargetClient.GetHabbo().TradeSlot1Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot1Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot1, TargetClient.GetHabbo().TradeSlot1Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot2 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot2Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot2, TargetClient.GetHabbo().TradeSlot2Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot2Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot2, TargetClient.GetHabbo().TradeSlot2Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot3 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot3Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot3, TargetClient.GetHabbo().TradeSlot3Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot3Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot3, TargetClient.GetHabbo().TradeSlot3Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot4 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot4Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot4, TargetClient.GetHabbo().TradeSlot4Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot4Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot4, TargetClient.GetHabbo().TradeSlot4Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot5 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot5Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot5, TargetClient.GetHabbo().TradeSlot5Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot5Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot5, TargetClient.GetHabbo().TradeSlot5Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot6 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot6Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot6, TargetClient.GetHabbo().TradeSlot6Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot6Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot6, TargetClient.GetHabbo().TradeSlot6Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot7 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot7Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot7, TargetClient.GetHabbo().TradeSlot7Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot7Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot7, TargetClient.GetHabbo().TradeSlot7Quantity);
                                }
                            }
                            if (TargetClient.GetHabbo().TradeSlot8 != null)
                            {
                                if (TargetClient.GetHabbo().TradeSlot8Durability > 0)
                                {
                                    Client.GetHabbo().AddToInventory(TargetClient.GetHabbo().TradeSlot8, TargetClient.GetHabbo().TradeSlot8Durability);
                                }
                                else if (TargetClient.GetHabbo().TradeSlot8Quantity > 0)
                                {
                                    Client.GetHabbo().AddToInventory2(TargetClient.GetHabbo().TradeSlot8, TargetClient.GetHabbo().TradeSlot8Quantity);
                                }
                            }
                            #endregion
                            #region Trade Partner
                            if (Client.GetHabbo().TradeMoney > 0)
                            {
                                TargetClient.GetHabbo().Credits += Client.GetHabbo().TradeMoney;
                                TargetClient.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                TargetClient.GetHabbo().RPCache(3);

                                Client.GetHabbo().Credits -= Client.GetHabbo().TradeMoney;
                                Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                                Client.GetHabbo().RPCache(3);
                            }
                            if (Client.GetHabbo().TradeSlot1 != null)
                            {
                                if (Client.GetHabbo().TradeSlot1Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot1, Client.GetHabbo().TradeSlot1Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot1Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot1, Client.GetHabbo().TradeSlot1Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot2 != null)
                            {
                                if (Client.GetHabbo().TradeSlot2Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot2, Client.GetHabbo().TradeSlot2Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot2Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot2, Client.GetHabbo().TradeSlot2Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot3 != null)
                            {
                                if (Client.GetHabbo().TradeSlot3Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot3, Client.GetHabbo().TradeSlot3Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot3Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot3, Client.GetHabbo().TradeSlot3Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot4 != null)
                            {
                                if (Client.GetHabbo().TradeSlot4Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot4, Client.GetHabbo().TradeSlot4Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot4Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot4, Client.GetHabbo().TradeSlot4Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot5 != null)
                            {
                                if (Client.GetHabbo().TradeSlot5Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot5, Client.GetHabbo().TradeSlot5Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot5Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot5, Client.GetHabbo().TradeSlot5Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot6 != null)
                            {
                                if (Client.GetHabbo().TradeSlot6Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot6, Client.GetHabbo().TradeSlot6Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot6Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot6, Client.GetHabbo().TradeSlot6Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot7 != null)
                            {
                                if (Client.GetHabbo().TradeSlot7Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot7, Client.GetHabbo().TradeSlot7Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot7Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot7, Client.GetHabbo().TradeSlot7Quantity);
                                }
                            }
                            if (Client.GetHabbo().TradeSlot8 != null)
                            {
                                if (Client.GetHabbo().TradeSlot8Durability > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory(Client.GetHabbo().TradeSlot8, Client.GetHabbo().TradeSlot8Durability);
                                }
                                else if (Client.GetHabbo().TradeSlot8Quantity > 0)
                                {
                                    TargetClient.GetHabbo().AddToInventory2(Client.GetHabbo().TradeSlot8, Client.GetHabbo().TradeSlot8Quantity);
                                }
                            }
                            #endregion

                            Client.GetHabbo().isTradingWith = null;
                            Client.GetHabbo().TradeConfirmed = false;
                            Client.GetHabbo().TradeMoney = 0;
                            Client.GetHabbo().TradeSlot1 = null;
                            Client.GetHabbo().TradeSlot1Quantity = 0;
                            Client.GetHabbo().TradeSlot1Durability = 0;
                            Client.GetHabbo().TradeSlot2 = null;
                            Client.GetHabbo().TradeSlot2Quantity = 0;
                            Client.GetHabbo().TradeSlot2Durability = 0;
                            Client.GetHabbo().TradeSlot3 = null;
                            Client.GetHabbo().TradeSlot3Quantity = 0;
                            Client.GetHabbo().TradeSlot3Durability = 0;
                            Client.GetHabbo().TradeSlot4 = null;
                            Client.GetHabbo().TradeSlot4Quantity = 0;
                            Client.GetHabbo().TradeSlot4Durability = 0;
                            Client.GetHabbo().TradeSlot5 = null;
                            Client.GetHabbo().TradeSlot5Quantity = 0;
                            Client.GetHabbo().TradeSlot5Durability = 0;
                            Client.GetHabbo().TradeSlot6 = null;
                            Client.GetHabbo().TradeSlot6Quantity = 0;
                            Client.GetHabbo().TradeSlot6Durability = 0;
                            Client.GetHabbo().TradeSlot7 = null;
                            Client.GetHabbo().TradeSlot7Quantity = 0;
                            Client.GetHabbo().TradeSlot7Durability = 0;
                            Client.GetHabbo().TradeSlot8 = null;
                            Client.GetHabbo().TradeSlot8Quantity = 0;
                            Client.GetHabbo().TradeSlot8Durability = 0;

                            TargetClient.GetHabbo().isTradingWith = null;
                            TargetClient.GetHabbo().TradeConfirmed = false;
                            TargetClient.GetHabbo().TradeMoney = 0;
                            TargetClient.GetHabbo().TradeSlot1 = null;
                            TargetClient.GetHabbo().TradeSlot1Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot1Durability = 0;
                            TargetClient.GetHabbo().TradeSlot2 = null;
                            TargetClient.GetHabbo().TradeSlot2Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot2Durability = 0;
                            TargetClient.GetHabbo().TradeSlot3 = null;
                            TargetClient.GetHabbo().TradeSlot3Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot3Durability = 0;
                            TargetClient.GetHabbo().TradeSlot4 = null;
                            TargetClient.GetHabbo().TradeSlot4Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot4Durability = 0;
                            TargetClient.GetHabbo().TradeSlot5 = null;
                            TargetClient.GetHabbo().TradeSlot5Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot5Durability = 0;
                            TargetClient.GetHabbo().TradeSlot6 = null;
                            TargetClient.GetHabbo().TradeSlot6Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot6Durability = 0;
                            TargetClient.GetHabbo().TradeSlot7 = null;
                            TargetClient.GetHabbo().TradeSlot7Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot7Durability = 0;
                            TargetClient.GetHabbo().TradeSlot8 = null;
                            TargetClient.GetHabbo().TradeSlot8Quantity = 0;
                            TargetClient.GetHabbo().TradeSlot8Durability = 0;

                            Client.GetHabbo().TradeToken = 0;
                            TargetClient.GetHabbo().TradeToken = 0;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-accept;end");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;end");

                            for (int i = 1; i <= 10; i++)
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-accept;slots;" + i);
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;slots;" + i);
                            }
                        }
                        else
                        {
                            Client.GetHabbo().TradeConfirmed = true;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-accept;me");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;partner");
                        }
                    }
                    break;
                #endregion
                #region Decline
                case "decline":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().isTradingWith == null)
                            return;

                        if (Client.GetHabbo().getCooldown("trade_decline"))
                            return;

                        Client.GetHabbo().addCooldown("trade_decline", 1000);

                        GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetHabbo().isTradingWith);
                        if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Client.GetHabbo().CurrentRoom)
                        {
                            Client.SendWhisper("Player not found in this room");
                            return;
                        }

                        User.Say("declines the trade");
                        Client.GetHabbo().isTradingWith = null;
                        Client.GetHabbo().TradeConfirmed = false;
                        Client.GetHabbo().TradeMoney = 0;
                        Client.GetHabbo().TradeSlot1 = null;
                        Client.GetHabbo().TradeSlot1Quantity = 0;
                        Client.GetHabbo().TradeSlot1Durability = 0;
                        Client.GetHabbo().TradeSlot2 = null;
                        Client.GetHabbo().TradeSlot2Quantity = 0;
                        Client.GetHabbo().TradeSlot2Durability = 0;
                        Client.GetHabbo().TradeSlot3 = null;
                        Client.GetHabbo().TradeSlot3Quantity = 0;
                        Client.GetHabbo().TradeSlot3Durability = 0;
                        Client.GetHabbo().TradeSlot4 = null;
                        Client.GetHabbo().TradeSlot4Quantity = 0;
                        Client.GetHabbo().TradeSlot4Durability = 0;
                        Client.GetHabbo().TradeSlot5 = null;
                        Client.GetHabbo().TradeSlot5Quantity = 0;
                        Client.GetHabbo().TradeSlot5Durability = 0;
                        Client.GetHabbo().TradeSlot6 = null;
                        Client.GetHabbo().TradeSlot6Quantity = 0;
                        Client.GetHabbo().TradeSlot6Durability = 0;
                        Client.GetHabbo().TradeSlot7 = null;
                        Client.GetHabbo().TradeSlot7Quantity = 0;
                        Client.GetHabbo().TradeSlot7Durability = 0;
                        Client.GetHabbo().TradeSlot8 = null;
                        Client.GetHabbo().TradeSlot8Quantity = 0;
                        Client.GetHabbo().TradeSlot8Durability = 0;

                        TargetClient.GetHabbo().isTradingWith = null;
                        TargetClient.GetHabbo().TradeConfirmed = false;
                        TargetClient.GetHabbo().TradeMoney = 0;
                        TargetClient.GetHabbo().TradeSlot1 = null;
                        TargetClient.GetHabbo().TradeSlot1Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot1Durability = 0;
                        TargetClient.GetHabbo().TradeSlot2 = null;
                        TargetClient.GetHabbo().TradeSlot2Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot2Durability = 0;
                        TargetClient.GetHabbo().TradeSlot3 = null;
                        TargetClient.GetHabbo().TradeSlot3Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot3Durability = 0;
                        TargetClient.GetHabbo().TradeSlot4 = null;
                        TargetClient.GetHabbo().TradeSlot4Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot4Durability = 0;
                        TargetClient.GetHabbo().TradeSlot5 = null;
                        TargetClient.GetHabbo().TradeSlot5Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot5Durability = 0;
                        TargetClient.GetHabbo().TradeSlot6 = null;
                        TargetClient.GetHabbo().TradeSlot6Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot6Durability = 0;
                        TargetClient.GetHabbo().TradeSlot7 = null;
                        TargetClient.GetHabbo().TradeSlot7Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot7Durability = 0;
                        TargetClient.GetHabbo().TradeSlot8 = null;
                        TargetClient.GetHabbo().TradeSlot8Quantity = 0;
                        TargetClient.GetHabbo().TradeSlot8Durability = 0;

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-accept;end");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;end");

                        for (int i = 1; i <= 10; i++)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "trade-accept;slots;" + i);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;slots;" + i);
                        }
                    }
                break;
                #endregion
            }
        }
    }
}