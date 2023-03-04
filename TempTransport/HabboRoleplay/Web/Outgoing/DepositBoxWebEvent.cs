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
    class DepositBoxWebEvent : IWebEvent
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
                #region Remove from Inventory
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

                        DataRow Inventory = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `inventory` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Inventory = dbClient.getRow();
                        }
                        if (Inventory == null)
                            return;

                        DataRow Deposit = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `user_deposit_box` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Deposit = dbClient.getRow();
                        }
                        if (Deposit == null)
                            return;

                        string Item = Convert.ToString(Inventory[Slot]);
                        int Quantity = Convert.ToInt32(Inventory[Slot + "_quantity"]);
                        int Durability = Convert.ToInt32(Inventory[Slot + "_durability"]);

                        if (Durability > 0) //here we go
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                if (Convert.ToString(Deposit["slot1"]) == Item && Convert.ToInt32(Deposit["slot1_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot1_quantity` = `slot1_quantity` + 1, `slot1_durability` = `slot1_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot2"]) == Item && Convert.ToInt32(Deposit["slot2_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot2_quantity` = `slot2_quantity` + 1, `slot2_durability` = `slot2_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot3"]) == Item && Convert.ToInt32(Deposit["slot3_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot3_quantity` = `slot3_quantity` + 1, `slot3_durability` = `slot3_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot4"]) == Item && Convert.ToInt32(Deposit["slot4_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot4_quantity` = `slot4_quantity` + 1, `slot4_durability` = `slot4_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot5"]) == Item && Convert.ToInt32(Deposit["slot5_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot5_quantity` = `slot5_quantity` + 1, `slot5_durability` = `slot5_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot6"]) == Item && Convert.ToInt32(Deposit["slot6_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot6_quantity` = `slot6_quantity` + 1, `slot6_durability` = `slot6_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot7"]) == Item && Convert.ToInt32(Deposit["slot7_durability"]) == 3000 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot7_quantity` = `slot7_quantity` + 1, `slot7_durability` = `slot7_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot8"]) == Item && Convert.ToInt32(Deposit["slot8_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot8_quantity` = `slot8_quantity` + 1, `slot8_durability` = `slot8_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot9"]) == Item && Convert.ToInt32(Deposit["slot9_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot9_quantity` = `slot9_quantity` + 1, `slot9_durability` = `slot9_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot10"]) == Item && Convert.ToInt32(Deposit["slot10_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot10_quantity` = `slot10_quantity` + 1, `slot10_durability` = `slot10_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot11"]) == Item && Convert.ToInt32(Deposit["slot11_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot11_quantity` = `slot11_quantity` + 1, `slot11_durability` = `slot11_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot12"]) == Item && Convert.ToInt32(Deposit["slot12_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot12_quantity` = `slot12_quantity` + 1, `slot12_durability` = `slot12_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot13"]) == Item && Convert.ToInt32(Deposit["slot13_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot13_quantity` = `slot13_quantity` + 1, `slot13_durability` = `slot13_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot14"]) == Item && Convert.ToInt32(Deposit["slot14_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot14_quantity` = `slot14_quantity` + 1, `slot14_durability` = `slot14_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot15"]) == Item && Convert.ToInt32(Deposit["slot15_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot15_quantity` = `slot15_quantity` + 1, `slot15_durability` = `slot15_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot16"]) == Item && Convert.ToInt32(Deposit["slot16_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot16_quantity` = `slot16_quantity` + 1, `slot16_durability` = `slot16_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot17"]) == Item && Convert.ToInt32(Deposit["slot17_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot17_quantity` = `slot17_quantity` + 1, `slot17_durability` = `slot17_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot18"]) == Item && Convert.ToInt32(Deposit["slot18_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot18_quantity` = `slot18_quantity` + 1, `slot18_durability` = `slot18_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot19"]) == Item && Convert.ToInt32(Deposit["slot19_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot19_quantity` = `slot19_quantity` + 1, `slot19_durability` = `slot19_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot20"]) == Item && Convert.ToInt32(Deposit["slot20_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot20_quantity` = `slot20_quantity` + 1, `slot20_durability` = `slot20_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot21"]) == Item && Convert.ToInt32(Deposit["slot21_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot21_quantity` = `slot21_quantity` + 1, `slot21_durability` = `slot21_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot22"]) == Item && Convert.ToInt32(Deposit["slot22_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot22_quantity` = `slot22_quantity` + 1, `slot22_durability` = `slot22_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot23"]) == Item && Convert.ToInt32(Deposit["slot23_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot23_quantity` = `slot23_quantity` + 1, `slot23_durability` = `slot23_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else if (Convert.ToString(Deposit["slot24"]) == Item && Convert.ToInt32(Deposit["slot24_durability"]) == 300 && Durability == 300)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot24_quantity` = `slot24_quantity` + 1, `slot24_durability` = `slot24_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                else
                                {
                                    if (Convert.ToString(Deposit["slot1"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot1` = '" + Item + "', `slot1_quantity` = `slot1_quantity` + 1, `slot1_durability` = `slot1_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot2"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot2` = '" + Item + "', `slot2_quantity` = `slot2_quantity` + 1, `slot2_durability` = `slot2_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot3"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot3` = '" + Item + "', `slot3_quantity` = `slot3_quantity` + 1, `slot3_durability` = `slot3_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot4"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot4` = '" + Item + "', `slot4_quantity` = `slot4_quantity` + 1, `slot4_durability` = `slot4_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot5"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot5` = '" + Item + "', `slot5_quantity` = `slot5_quantity` + 1, `slot5_durability` = `slot5_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot6"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot6` = '" + Item + "', `slot6_quantity` = `slot6_quantity` + 1, `slot6_durability` = `slot6_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot7"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot7` = '" + Item + "', `slot7_quantity` = `slot7_quantity` + 1, `slot7_durability` = `slot7_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot8"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot8` = '" + Item + "', `slot8_quantity` = `slot8_quantity` + 1, `slot8_durability` = `slot8_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot9"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot9` = '" + Item + "', `slot9_quantity` = `slot9_quantity` + 1, `slot9_durability` = `slot9_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot10"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot10` = '" + Item + "', `slot10_quantity` = `slot10_quantity` + 1, `slot10_durability` = `slot10_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot11"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot11` = '" + Item + "', `slot11_quantity` = `slot11_quantity` + 1, `slot11_durability` = `slot11_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot12"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot12` = '" + Item + "', `slot12_quantity` = `slot12_quantity` + 1, `slot12_durability` = `slot12_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot13"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot13` = '" + Item + "', `slot13_quantity` = `slot13_quantity` + 1, `slot13_durability` = `slot13_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot14"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot14` = '" + Item + "', `slot14_quantity` = `slot14_quantity` + 1, `slot14_durability` = `slot14_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot15"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot15` = '" + Item + "', `slot15_quantity` = `slot15_quantity` + 1, `slot15_durability` = `slot15_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot16"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot16` = '" + Item + "', `slot16_quantity` = `slot16_quantity` + 1, `slot16_durability` = `slot16_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot17"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot17` = '" + Item + "', `slot17_quantity` = `slot17_quantity` + 1, `slot17_durability` = `slot17_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot18"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot18` = '" + Item + "', `slot18_quantity` = `slot18_quantity` + 1, `slot18_durability` = `slot18_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot19"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot19` = '" + Item + "', `slot19_quantity` = `slot19_quantity` + 1, `slot19_durability` = `slot19_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot20"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot20` = '" + Item + "', `slot20_quantity` = `slot20_quantity` + 1, `slot20_durability` = `slot20_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot21"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot21` = '" + Item + "', `slot21_quantity` = `slot21_quantity` + 1, `slot21_durability` = `slot21_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot22"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot22` = '" + Item + "', `slot22_quantity` = `slot22_quantity` + 1, `slot22_durability` = `slot22_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot23"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot23` = '" + Item + "', `slot23_quantity` = `slot23_quantity` + 1, `slot23_durability` = `slot23_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                    else if (Convert.ToString(Deposit["slot24"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot24` = '" + Item + "', `slot24_quantity` = `slot24_quantity` + 1, `slot24_durability` = `slot24_durability` + " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', " + Slot + "_durability = 0 WHERE user_id = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                    }
                                }
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "deposit-box;show");
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;1;" + Slot + ";0");
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "inventory-durability-update;0;" + Slot + ";0");
                            }
                        }
                        else if (Quantity > 0)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                if (Convert.ToString(Deposit["slot1"]) == Item && Convert.ToInt32(Deposit["slot1_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot1_quantity` = `slot1_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot2"]) == Item && Convert.ToInt32(Deposit["slot2_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot2_quantity` = `slot2_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot3"]) == Item && Convert.ToInt32(Deposit["slot3_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot3_quantity` = `slot3_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot4"]) == Item && Convert.ToInt32(Deposit["slot4_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot4_quantity` = `slot4_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot5"]) == Item && Convert.ToInt32(Deposit["slot5_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot5_quantity` = `slot5_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot6"]) == Item && Convert.ToInt32(Deposit["slot6_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot6_quantity` = `slot6_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot7"]) == Item && Convert.ToInt32(Deposit["slot7_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot7_quantity` = `slot7_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot8"]) == Item && Convert.ToInt32(Deposit["slot8_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot8_quantity` = `slot8_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot9"]) == Item && Convert.ToInt32(Deposit["slot9_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot9_quantity` = `slot9_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot10"]) == Item && Convert.ToInt32(Deposit["slot10_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot10_quantity` = `slot10_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot11"]) == Item && Convert.ToInt32(Deposit["slot11_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot11_quantity` = `slot11_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot12"]) == Item && Convert.ToInt32(Deposit["slot12_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot12_quantity` = `slot12_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot13"]) == Item && Convert.ToInt32(Deposit["slot13_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot13_quantity` = `slot13_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot14"]) == Item && Convert.ToInt32(Deposit["slot14_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot14_quantity` = `slot14_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot15"]) == Item && Convert.ToInt32(Deposit["slot15_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot15_quantity` = `slot15_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot16"]) == Item && Convert.ToInt32(Deposit["slot16_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot16_quantity` = `slot16_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot17"]) == Item && Convert.ToInt32(Deposit["slot17_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot17_quantity` = `slot17_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot18"]) == Item && Convert.ToInt32(Deposit["slot18_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot18_quantity` = `slot18_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot19"]) == Item && Convert.ToInt32(Deposit["slot19_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot19_quantity` = `slot19_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot20"]) == Item && Convert.ToInt32(Deposit["slot20_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot20_quantity` = `slot20_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot21"]) == Item && Convert.ToInt32(Deposit["slot21_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot21_quantity` = `slot21_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot22"]) == Item && Convert.ToInt32(Deposit["slot22_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot22_quantity` = `slot22_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot23"]) == Item && Convert.ToInt32(Deposit["slot23_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot23_quantity` = `slot23_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else if (Convert.ToString(Deposit["slot24"]) == Item && Convert.ToInt32(Deposit["slot24_quantity"]) < 50)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot24_quantity` = `slot24_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                }
                                else
                                {
                                    if (Convert.ToString(Deposit["slot1"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot1` = '" + Item + "', `slot1_quantity` = `slot1_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot2"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot2` = '" + Item + "', `slot2_quantity` = `slot2_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot3"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot3` = '" + Item + "', `slot3_quantity` = `slot3_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot4"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot4` = '" + Item + "', `slot4_quantity` = `slot4_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot5"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot5` = '" + Item + "', `slot5_quantity` = `slot5_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot6"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot6` = '" + Item + "', `slot6_quantity` = `slot6_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot7"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot7` = '" + Item + "', `slot7_quantity` = `slot7_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot8"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot8` = '" + Item + "', `slot8_quantity` = `slot8_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot9"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot9` = '" + Item + "', `slot9_quantity` = `slot9_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot10"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot10` = '" + Item + "', `slot10_quantity` = `slot10_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot11"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot11` = '" + Item + "', `slot11_quantity` = `slot11_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot12"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot12` = '" + Item + "', `slot12_quantity` = `slot12_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot13"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot13` = '" + Item + "', `slot13_quantity` = `slot13_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot14"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot14` = '" + Item + "', `slot14_quantity` = `slot14_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot15"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot15` = '" + Item + "', `slot15_quantity` = `slot15_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot16"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot16` = '" + Item + "', `slot16_quantity` = `slot16_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot17"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot17` = '" + Item + "', `slot17_quantity` = `slot17_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot18"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot18` = '" + Item + "', `slot18_quantity` = `slot18_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot19"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot19` = '" + Item + "', `slot19_quantity` = `slot19_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1"); ;
                                    }
                                    else if (Convert.ToString(Deposit["slot20"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot20` = '" + Item + "', `slot20_quantity` = `slot20_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot21"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot21` = '" + Item + "', `slot21_quantity` = `slot21_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot22"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot22` = '" + Item + "', `slot22_quantity` = `slot22_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot23"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot23` = '" + Item + "', `slot23_quantity` = `slot23_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else if (Convert.ToString(Deposit["slot24"]) == "null")
                                    {
                                        dbClient.SetQuery("UPDATE `user_deposit_box` SET `slot24` = '" + Item + "', `slot24_quantity` = `slot24_quantity` + 1 WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                        dbClient.RunQuery();
                                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "update_quantity," + Slot + ",-,1");
                                    }
                                    else
                                    {
                                        Client.SendWhisper("Your deposit box is full!");
                                    }
                                }
                            }
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "deposit-box;show");
                        }
                    }
                    break;
                #endregion
                #region Add to Inventory
                case "remove":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Slot = ReceivedData[1];

                        DataRow Deposit = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `user_deposit_box` WHERE `user_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Deposit = dbClient.getRow();
                        }
                        if (Deposit == null)
                            return;

                        string Item = Convert.ToString(Deposit[Slot]);
                        int Quantity = Convert.ToInt32(Deposit[Slot + "_quantity"]);
                        int Durability = Convert.ToInt32(Deposit[Slot + "_durability"]);


                        if (Durability > 0)
                        {
                            if (Durability > 300)
                            {
                                Durability = 300;
                            }

                            DataRow DepositCheck = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `user_deposit_box` SET `" + Slot + "_quantity` = `" + Slot + "_quantity` - 1, `" + Slot + "_durability` = `" + Slot + "_durability` - " + Durability + "  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                dbClient.RunQuery();

                                dbClient.SetQuery("SELECT * FROM `user_deposit_box` WHERE `user_id` = @id LIMIT 1;");
                                dbClient.AddParameter("id", Client.GetHabbo().Id);
                                DepositCheck = dbClient.getRow();

                                if (Convert.ToInt32(DepositCheck[Slot + "_durability"]) <= 0)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `" + Slot + "` = 'null', `" + Slot + "_quantity` = 0, `" + Slot + "_durability` = 0  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                            }

                            Client.GetHabbo().AddToInventory(Item, Durability);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "deposit-box;show");
                        }
                        else if (Quantity > 0)
                        {
                            DataRow DepositCheck = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `user_deposit_box` SET `" + Slot + "_quantity` = `" + Slot + "_quantity` - 1  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                dbClient.RunQuery();

                                dbClient.SetQuery("SELECT * FROM `user_deposit_box` WHERE `user_id` = @id LIMIT 1;");
                                dbClient.AddParameter("id", Client.GetHabbo().Id);
                                DepositCheck = dbClient.getRow();

                                if (Convert.ToInt32(DepositCheck[Slot + "_quantity"]) <= 0)
                                {
                                    dbClient.SetQuery("UPDATE `user_deposit_box` SET `" + Slot + "` = 'null', `" + Slot + "_quantity` = '0'  WHERE `user_id` = '" + Client.GetHabbo().Id + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                            }
                            Client.GetHabbo().AddToInventory2(Item, 1);
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "deposit-box;show");
                        }
                    }
                    break;
                #endregion
            }
        }
    }
}