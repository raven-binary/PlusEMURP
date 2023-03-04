using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using System.Data;
using Plus.HabboHotel.Rooms;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class InventoryWebEvent : IWebEvent
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
                #region Inventory click
                case "click":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Client.GetHabbo().usingDepositBox)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "depositbox", "add," + Slot);
                            return;
                        }

                        if (Client.GetHabbo().usingCreateSale)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "marketplace", "create-sale-add," + Slot);
                            return;
                        }

                        if (Client.GetHabbo().isTradingWith != null)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "trade", "add," + Slot);
                            return;
                        }

                        if (Client.GetRoleplay().Bin.Using)
                        {
                            Client.GetRoleplay().Bin.Add(Slot);
                            return;
                        }

                        if (Slot == "slot1")
                        {
                            if (Client.GetRoleplay().Inventory.Slot1Type == "weapon")
                            {
                                Client.GetRoleplay().Inventory.Equip(Slot, Client.GetRoleplay().Inventory.Slot1, Client.GetRoleplay().Inventory.Slot1Type, Client.GetRoleplay().Inventory.Slot1Durability);
                            }
                            else if (Client.GetRoleplay().Inventory.Slot1 == "medkit" || Client.GetRoleplay().Inventory.Slot1 == "snack" || Client.GetRoleplay().Inventory.Slot1 == "flashbang" || Client.GetRoleplay().Inventory.Slot1 == "heroindrug")
                            {
                                Client.GetRoleplay().SendExecuteWeb("item", Client.GetRoleplay().Inventory.Slot1 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot1Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot1Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot1Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot1);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot1);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot1);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot1 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot1 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot1 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot1 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot1 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot1 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot2")
                        {
                            if (Client.GetRoleplay().Inventory.Slot2Type == "weapon")
                            {
                                Client.GetRoleplay().Inventory.Equip(Slot, Client.GetRoleplay().Inventory.Slot1, Client.GetRoleplay().Inventory.Slot1Type, Client.GetRoleplay().Inventory.Slot1Durability);
                            }
                            else if (Client.GetRoleplay().Inventory.Slot2 == "medkit" || Client.GetRoleplay().Inventory.Slot2 == "snack" || Client.GetRoleplay().Inventory.Slot2 == "flashbang" || Client.GetRoleplay().Inventory.Slot2 == "heroindrug")
                            {
                                Client.GetRoleplay().SendExecuteWeb("item", Client.GetRoleplay().Inventory.Slot2 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot2Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot2Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot2Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot2);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot2);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot2);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot2 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot2 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot2 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot2 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot2 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot2 + "," + Slot);
                            }

                        }
                        else if (Slot == "slot3")
                        {
                            if (Client.GetRoleplay().Inventory.Slot3Type == "weapon")
                            {
                                Client.GetRoleplay().Inventory.Equip(Slot, Client.GetRoleplay().Inventory.Slot3Type, Client.GetRoleplay().Inventory.Slot3Type, Client.GetRoleplay().Inventory.Slot3Durability);
                            }
                            else if (Client.GetRoleplay().Inventory.Slot3 == "medkit" || Client.GetRoleplay().Inventory.Slot3 == "snack" || Client.GetRoleplay().Inventory.Slot3 == "flashbang" || Client.GetRoleplay().Inventory.Slot3 == "heroindrug")
                            {
                                Client.GetRoleplay().SendExecuteWeb("item", Client.GetRoleplay().Inventory.Slot3 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot3Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot3Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot3Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot3);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot3);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot3);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot3 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot3 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot3 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot3 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot3 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot4")
                        {
                            if (Client.GetHabbo().InventorySlot4 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if(Client.GetHabbo().InventorySlot4 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot4Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot4);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot4);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot4);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot4 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot4 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot4 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot4 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot4 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot4 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot5")
                        {
                            if (Client.GetHabbo().InventorySlot5 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if(Client.GetHabbo().InventorySlot5 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot5Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot5);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot5);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot5);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot5 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot5 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot5 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot5 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot5 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot5 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot6")
                        {
                            if (Client.GetHabbo().InventorySlot6 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if(Client.GetHabbo().InventorySlot6 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot6Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot6);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot6);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot6);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot6 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot6 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot6 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot6 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot6 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot6 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot7")
                        {
                            if (Client.GetHabbo().InventorySlot7 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if(Client.GetHabbo().InventorySlot7 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot7Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot7);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot7);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot7);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot7 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot7 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot7 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot7 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot7 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot7 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot8")
                        {
                            if (Client.GetHabbo().InventorySlot8 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if(Client.GetHabbo().InventorySlot8 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot8Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot8);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot8);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot8);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot8 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot8 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot8 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot8 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot8 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot8 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot9")
                        {
                            if (Client.GetHabbo().InventorySlot9 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot9Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot9);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot9);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot9);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot9 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot9 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot9 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot9 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot9 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot9 + "," + Slot);
                            }
                        }
                        else if (Slot == "slot10")
                        {
                            if (Client.GetHabbo().InventorySlot10 == "stungun")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,stungun," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "bat")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,bat," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "sword")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,sword," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "axe")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "arme,axe," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "medkit")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "medkit," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "snack")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "snack," + Slot + "");
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "bodyarmour")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bodyarmour," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "letter")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "managermail," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "box")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "box," + Slot + "," + Client.GetHabbo().InventorySlot10Durability);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "coffee_bean")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot10);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "healingcrop")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot10);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "wool")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "stock-texts," + Client.GetHabbo().InventorySlot10);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "bomb" || Client.GetHabbo().BombFromSlot == Slot)
                            {
                                if (Client.GetHabbo().BombFromSlot == null)
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Slot);
                                }
                                else
                                {
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "bomb," + Client.GetHabbo().BombFromSlot);
                                }
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "flashbang")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", "flashbang," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "heroindrug")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot10 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "suicidevest")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot10 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "throwingknife")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot10 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "lockpick")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot10 + "," + Slot);
                            }
                            else if (Client.GetHabbo().InventorySlot10 == "stolenpepperspray")
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "item", Client.GetHabbo().InventorySlot10 + "," + Slot);
                            }
                        }
                        else
                        {
                            Client.SendWhisper("This inventory slot does not exist");
                        }
                    }
                    break;
                #endregion

                #region Inventory Equip
                case "equip":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Weapon = ReceivedData[1];
                        int Durability = Convert.ToInt32(ReceivedData[2]);
                        string FromSlot = ReceivedData[3];

                        if (Client.GetHabbo().InventoryEquipSlot1 == null)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-equip;add;1;" + Weapon + ";" + Durability);
                            Client.GetHabbo().InventoryEquipSlot1Item = Weapon;
                            Client.GetHabbo().InventoryEquipSlot1Durability = Durability;
                        }
                        else if (Client.GetHabbo().InventoryEquipSlot2 == null)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-equip;add;2;" + Weapon + ";" + Durability);
                            Client.GetHabbo().InventoryEquipSlot2Item = Weapon;
                            Client.GetHabbo().InventoryEquipSlot2Durability = Durability;
                        }

                        if (Weapon == "stungun")
                        {
                            User.ApplyEffect(182);
                        }
                        else if (Weapon == "bat")
                        {
                            User.ApplyEffect(591);
                        }
                        else if (Weapon == "sword")
                        {
                            User.ApplyEffect(162);
                        }
                        else if (Weapon == "axe")
                        {
                            User.ApplyEffect(707);
                        }

                        if (Client.GetHabbo().ArmeEquiped == null)
                        {
                            if (FromSlot == "slot1")
                            {
                                Client.GetHabbo().InventorySlot1 = null;
                                Client.GetHabbo().InventorySlot1Durability = 0;
                            }
                            else if (FromSlot == "slot2")
                            {
                                Client.GetHabbo().InventorySlot2 = null;
                                Client.GetHabbo().InventorySlot2Durability = 0;
                            }
                            else if (FromSlot == "slot3")
                            {
                                Client.GetHabbo().InventorySlot3 = null;
                                Client.GetHabbo().InventorySlot3Durability = 0;
                            }
                            else if (FromSlot == "slot4")
                            {
                                Client.GetHabbo().InventorySlot4 = null;
                                Client.GetHabbo().InventorySlot4Durability = 0;
                            }
                            else if (FromSlot == "slot5")
                            {
                                Client.GetHabbo().InventorySlot5 = null;
                                Client.GetHabbo().InventorySlot5Durability = 0;
                            }
                            else if (FromSlot == "slot6")
                            {
                                Client.GetHabbo().InventorySlot6 = null;
                                Client.GetHabbo().InventorySlot6Durability = 0;
                            }
                            else if (FromSlot == "slot7")
                            {
                                Client.GetHabbo().InventorySlot7 = null;
                                Client.GetHabbo().InventorySlot7Durability = 0;
                            }
                            else if (FromSlot == "slot8")
                            {
                                Client.GetHabbo().InventorySlot8 = null;
                                Client.GetHabbo().InventorySlot8Durability = 0;
                            }
                            else if (FromSlot == "slot9")
                            {
                                Client.GetHabbo().InventorySlot9 = null;
                                Client.GetHabbo().InventorySlot9Durability = 0;
                            }
                            else if (FromSlot == "slot10")
                            {
                                Client.GetHabbo().InventorySlot10 = null;
                                Client.GetHabbo().InventorySlot10Durability = 0;
                            }
                            Client.GetHabbo().ArmeEquiped = Weapon;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                        }
                    }
                    break;
                #endregion

                #region Inventory Unequip
                case "unequip":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;
                        
                        User.UnIdle();

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        if (Slot == "slot1")
                        {
                            Client.GetRoleplay().Inventory.Unequip(1);
                        }
                        else if (Slot == "slot2")
                        {
                            Client.GetRoleplay().Inventory.Unequip(2);
                        }
                        Client.GetHabbo().resetEffectEvent();
                    }
                    break;
                #endregion

                #region Replace Equip
                case "replace-equip":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];
                        string FromSlot = ReceivedData[2];
                    
                        if (Slot == "slot1")
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-equip;remove;1");

                            if (Client.GetHabbo().InventoryFull())
                            {
                                if (FromSlot == "slot1")
                                {
                                    Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot2")
                                {
                                    Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot3")
                                {
                                    Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot4")
                                {
                                    Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot5")
                                {
                                    Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot6")
                                {
                                    Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot7")
                                {
                                    Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot8")
                                {
                                    Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot9")
                                {
                                    Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                else if (FromSlot == "slot10")
                                {
                                    Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventoryEquipSlot1Item;
                                    Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventoryEquipSlot1Durability;
                                }
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + FromSlot + ";" + Client.GetHabbo().InventoryEquipSlot1Item + ";0;" + Client.GetHabbo().InventoryEquipSlot1Durability);
                            }
                            else
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().InventoryEquipSlot1Item, Client.GetHabbo().InventoryEquipSlot1Durability);
                            }
                            Client.GetHabbo().ArmeEquiped = null;
                        }
                        else if (Slot == "slot2")
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-equip;remove;2");

                            if (Client.GetHabbo().InventoryFull())
                            {
                                if (FromSlot == "slot1")
                                {
                                    Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot2")
                                {
                                    Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot3")
                                {
                                    Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot4")
                                {
                                    Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot5")
                                {
                                    Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot6")
                                {
                                    Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot7")
                                {
                                    Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot8")
                                {
                                    Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot9")
                                {
                                    Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                else if (FromSlot == "slot10")
                                {
                                    Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventoryEquipSlot2Item;
                                    Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventoryEquipSlot2Durability;
                                }
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + FromSlot + ";" + Client.GetHabbo().InventoryEquipSlot2Item + ";0;" + Client.GetHabbo().InventoryEquipSlot2Durability);
                            }
                            else
                            {
                                Client.GetHabbo().AddToInventory(Client.GetHabbo().InventoryEquipSlot2Item, Client.GetHabbo().InventoryEquipSlot2Durability);
                            }
                            Client.GetHabbo().InventoryEquipSlot2Item = null;
                            Client.GetHabbo().InventoryEquipSlot2Durability = 0;
                        }
                    }
                    break;
                #endregion

                #region Inventory Add
                case "add":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];
                        string Item = ReceivedData[2];

                        DataRow GetInventory = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery("UPDATE inventory SET " + Slot + " = '" + Item + "'  WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                            dbClient.SetQuery("SELECT * FROM `Inventory` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            GetInventory = dbClient.getRow();
                        }
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + Slot + ";" + GetInventory[Slot] + ";" + GetInventory[Slot + "_quantity"] + ";" + GetInventory[Slot + "_durability"]);

                    }
                    break;
                #endregion

                #region Inventory Quantity Update
                case "update_quantity":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];
                        string Status = ReceivedData[2];
                        string Quantity = ReceivedData[3];

                        if (Status == "+")
                        {
                            if (Slot == "slot1")
                            {
                                Client.GetHabbo().InventorySlot1Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot2")
                            {
                                Client.GetHabbo().InventorySlot2Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot3")
                            {
                                Client.GetHabbo().InventorySlot3Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot4")
                            {
                                Client.GetHabbo().InventorySlot4Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot5")
                            {
                                Client.GetHabbo().InventorySlot5Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot6")
                            {
                                Client.GetHabbo().InventorySlot6Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot7")
                            {
                                Client.GetHabbo().InventorySlot7Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot8")
                            {
                                Client.GetHabbo().InventorySlot8Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot9")
                            {
                                Client.GetHabbo().InventorySlot9Quantity += Convert.ToInt32(Quantity);
                            }
                            else if (Slot == "slot10")
                            {
                                Client.GetHabbo().InventorySlot10Quantity += Convert.ToInt32(Quantity);
                            }

                            DataRow GetInventory = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE inventory SET " + Slot + "_quantity = " + Slot + "_quantity + " + Quantity + " WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                dbClient.SetQuery("SELECT * FROM `Inventory` WHERE `user_id` = @id LIMIT 1");
                                dbClient.AddParameter("id", Client.GetHabbo().Id);
                                GetInventory = dbClient.getRow();
                            }
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-quantity-update;" + Slot + ";" + GetInventory[Slot + "_quantity"]);
                        }
                        else if (Status == "-")
                        {
                            if (Slot == "slot1")
                            {
                                Client.GetHabbo().InventorySlot1Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot1Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot1 = null;
                                }
                            }
                            else if (Slot == "slot2")
                            {
                                Client.GetHabbo().InventorySlot2Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot2Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot2 = null;
                                }
                            }
                            else if (Slot == "slot3")
                            {
                                Client.GetHabbo().InventorySlot3Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot3Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot3 = null;
                                }
                            }
                            else if (Slot == "slot4")
                            {
                                Client.GetHabbo().InventorySlot4Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot4Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot4 = null;
                                }
                            }
                            else if (Slot == "slot5")
                            {
                                Client.GetHabbo().InventorySlot5Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot5Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot5 = null;
                                }
                            }
                            else if (Slot == "slot6")
                            {
                                Client.GetHabbo().InventorySlot6Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot6Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot6 = null;
                                }
                            }
                            else if (Slot == "slot7")
                            {
                                Client.GetHabbo().InventorySlot7Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot7Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot7 = null;
                                }
                            }
                            else if (Slot == "slot8")
                            {
                                Client.GetHabbo().InventorySlot8Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot8Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot8 = null;
                                }
                            }
                            else if (Slot == "slot9")
                            {
                                Client.GetHabbo().InventorySlot9Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot9Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot9 = null;
                                }
                            }
                            else if (Slot == "slot10")
                            {
                                Client.GetHabbo().InventorySlot10Quantity -= Convert.ToInt32(Quantity);
                                if (Client.GetHabbo().InventorySlot10Quantity <= 0)
                                {
                                    Client.GetHabbo().InventorySlot10 = null;
                                }
                            }

                            DataRow GetInventory = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.RunQuery("UPDATE inventory SET " + Slot + "_quantity = " + Slot + "_quantity - " + Quantity + " WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");

                                dbClient.SetQuery("SELECT * FROM `Inventory` WHERE `user_id` = @id LIMIT 1");
                                dbClient.AddParameter("id", Client.GetHabbo().Id);
                                GetInventory = dbClient.getRow();
                            }

                            if (Convert.ToInt32(GetInventory[Slot + "_quantity"]) == 0)
                            {
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null' WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                }
                            }
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-quantity-update;" + Slot + ";" + GetInventory[Slot + "_quantity"]);

                            if (Slot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot1Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot1 = null;
                                }
                            }
                            else if (Slot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot2Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot2 = null;
                                }
                            }
                            else if (Slot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot3Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot3 = null;
                                }
                            }
                            else if (Slot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot4Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot4 = null;
                                }
                            }
                            else if (Slot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot5Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot5 = null;
                                }
                            }
                            else if (Slot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot6Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot6 = null;
                                }
                            }
                            else if (Slot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot7Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot7 = null;
                                }
                            }
                            else if (Slot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot8Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot8 = null;
                                }
                            }
                            else if (Slot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot9Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot9 = null;
                                }
                            }
                            else if (Slot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot10Quantity == 0)
                                {
                                    Client.GetHabbo().InventorySlot10 = null;
                                }
                            }

                        }
                    }
                    break;
                #endregion

                #region Move Items
                case "move":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string FromSlot = ReceivedData[1];
                        string ToSlot = ReceivedData[2];

                        if (FromSlot == "slot1")
                        {
                            if (Client.GetHabbo().InventorySlot1 == null)
                                return;

                            if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot3Quantity = 0;
                                            Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot4Quantity = 0;
                                            Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot5Quantity = 0;
                                            Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot6Quantity = 0;
                                            Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot7Quantity = 0;
                                            Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot8Quantity = 0;
                                            Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot9Quantity = 0;
                                            Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot1Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot1 + ";0;" + Client.GetHabbo().InventorySlot1Durability);
                                            Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot1;
                                            Client.GetHabbo().InventorySlot10Quantity = 0;
                                            Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot1Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot1 = null;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot1Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot1;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot1Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot1);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot1Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot2")
                        {
                            if (Client.GetHabbo().InventorySlot2 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot1Quantity = 0;
                                            Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot3Quantity = 0;
                                            Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot4Quantity = 0;
                                            Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot5Quantity = 0;
                                            Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot6Quantity = 0;
                                            Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot7Quantity = 0;
                                            Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot8Quantity = 0;
                                            Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot9Quantity = 0;
                                            Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot2Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot2 + ";0;" + Client.GetHabbo().InventorySlot2Durability);
                                            Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot2;
                                            Client.GetHabbo().InventorySlot10Quantity = 0;
                                            Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot2Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot2 = null;
                                            Client.GetHabbo().InventorySlot2Quantity = 0;
                                            Client.GetHabbo().InventorySlot2Durability = 0;
                                        }
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot2Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot2;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot2Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot2);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot2Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot3")
                        {
                            if (Client.GetHabbo().InventorySlot3 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot3Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot3 + ";0;" + Client.GetHabbo().InventorySlot3Durability);
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot3;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot3Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot3 = null;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot3Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot3;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot3Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot3);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot3Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot4")
                        {
                            if (Client.GetHabbo().InventorySlot4 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot4Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot4 + ";0;" + Client.GetHabbo().InventorySlot4Durability);
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot4;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot4Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot4 = null;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot4Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot4;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot4Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot4);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot4Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot5")
                        {
                            if (Client.GetHabbo().InventorySlot5 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot5Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot5 + ";0;" + Client.GetHabbo().InventorySlot5Durability);
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot5;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot5Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot5 = null;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot5Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot5;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot5Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot5);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot5Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot6")
                        {
                            if (Client.GetHabbo().InventorySlot6 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot6Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot6 + ";0;" + Client.GetHabbo().InventorySlot6Durability);
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot6;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot6Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot6 = null;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot6Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot6;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot6Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot6);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot6Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot7")
                        {
                            if (Client.GetHabbo().InventorySlot7 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot7Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot7 + ";0;" + Client.GetHabbo().InventorySlot7Durability);
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot7;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot7Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot7 = null;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot7Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot7;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot7Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot7);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot7Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot8")
                        {
                            if (Client.GetHabbo().InventorySlot8 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot8Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot8 + ";0;" + Client.GetHabbo().InventorySlot8Durability);
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot8;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot8Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot8 = null;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot8Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot8;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot8Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot8);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot8Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot9")
                        {
                            if (Client.GetHabbo().InventorySlot9 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9Durability > 0)
                                    {
                                        if (Client.GetHabbo().InventorySlot3 == null)
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                            Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot9;
                                            Client.GetHabbo().InventorySlot3Quantity = 0;
                                            Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot9Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot9 = null;
                                            Client.GetHabbo().InventorySlot9Quantity = 0;
                                            Client.GetHabbo().InventorySlot9Durability = 0;
                                        }
                                    }
                                    else if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot9Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot9;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot9Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot9 = null;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9Durability > 0)
                                    {
                                        if (Client.GetHabbo().InventorySlot10 == null)
                                        {
                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot9 + ";0;" + Client.GetHabbo().InventorySlot9Durability);
                                            Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot9;
                                            Client.GetHabbo().InventorySlot10Quantity = 0;
                                            Client.GetHabbo().InventorySlot10Durability = Client.GetHabbo().InventorySlot9Durability;

                                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                            Client.GetHabbo().InventorySlot9 = null;
                                            Client.GetHabbo().InventorySlot9Quantity = 0;
                                            Client.GetHabbo().InventorySlot9Durability = 0;
                                        }
                                    }
                                    else if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot10")
                            {
                                if (Client.GetHabbo().InventorySlot9Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot10 == null)
                                    {
                                        Client.GetHabbo().InventorySlot10 = Client.GetHabbo().InventorySlot9;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot9Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot9);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot9Quantity);
                                    }
                                }
                            }
                        }
                        else if (FromSlot == "slot10")
                        {
                            if (Client.GetHabbo().InventorySlot10 == null)
                                return;

                            if (ToSlot == "slot1")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot1Quantity = 0;
                                        Client.GetHabbo().InventorySlot1Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot1 == null)
                                    {
                                        Client.GetHabbo().InventorySlot1 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot2")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot2Quantity = 0;
                                        Client.GetHabbo().InventorySlot2Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot2 == null)
                                    {
                                        Client.GetHabbo().InventorySlot2 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot3")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot3Quantity = 0;
                                        Client.GetHabbo().InventorySlot3Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot3 == null)
                                    {
                                        Client.GetHabbo().InventorySlot3 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot4")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot4Quantity = 0;
                                        Client.GetHabbo().InventorySlot4Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot4 == null)
                                    {
                                        Client.GetHabbo().InventorySlot4 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot5")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot5Quantity = 0;
                                        Client.GetHabbo().InventorySlot5Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot5 == null)
                                    {
                                        Client.GetHabbo().InventorySlot5 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot6")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot6Quantity = 0;
                                        Client.GetHabbo().InventorySlot6Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot6 == null)
                                    {
                                        Client.GetHabbo().InventorySlot6 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot7")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot7Quantity = 0;
                                        Client.GetHabbo().InventorySlot7Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot7 == null)
                                    {
                                        Client.GetHabbo().InventorySlot7 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot8")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot8Quantity = 0;
                                        Client.GetHabbo().InventorySlot8Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot8 == null)
                                    {
                                        Client.GetHabbo().InventorySlot8 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                            else if (ToSlot == "slot9")
                            {
                                if (Client.GetHabbo().InventorySlot10Durability > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-add;" + ToSlot + ";" + Client.GetHabbo().InventorySlot10 + ";0;" + Client.GetHabbo().InventorySlot10Durability);
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot10;
                                        Client.GetHabbo().InventorySlot9Quantity = 0;
                                        Client.GetHabbo().InventorySlot9Durability = Client.GetHabbo().InventorySlot10Durability;

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;" + FromSlot + ";0");
                                        Client.GetHabbo().InventorySlot10 = null;
                                        Client.GetHabbo().InventorySlot10Quantity = 0;
                                        Client.GetHabbo().InventorySlot10Durability = 0;
                                    }
                                }
                                else if (Client.GetHabbo().InventorySlot10Quantity > 0)
                                {
                                    if (Client.GetHabbo().InventorySlot9 == null)
                                    {
                                        Client.GetHabbo().InventorySlot9 = Client.GetHabbo().InventorySlot10;
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + ToSlot + ",+," + Client.GetHabbo().InventorySlot10Quantity);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "add," + ToSlot + "," + Client.GetHabbo().InventorySlot10);
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + FromSlot + ",-," + Client.GetHabbo().InventorySlot10Quantity);
                                    }
                                }
                            }
                        }
                    }
                    break;
                    #endregion
            }
        }
    }
}
