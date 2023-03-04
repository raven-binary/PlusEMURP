using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;
using Plus.Database.Interfaces;
using System.Data;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class ATMWebEvent : IWebEvent
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
                #region Withdraw All
                case "withdraw-all":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        int Amount = Client.GetHabbo().Banque;

                        if (Client.GetHabbo().getCooldown("withdraw_cooldown"))
                        {
                            Client.SendWhisper("You must wait before you can withdraw money from the ATM again");
                            return;
                        }

                        if (User.usingATM)
                        {
                            if (Amount < 1)
                            {
                                Client.SendWhisper("You don't have any money in a bank account");
                                return;
                            }

                            int Fee = Amount * 5 / 100;

                            DataRow ATM = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                                dbClient.AddParameter("id", Client.GetHabbo().UsingItem);
                                ATM = dbClient.getRow();
                            }

                            int ATMId = Convert.ToInt32(ATM["atm_id"]);
                            int ATMAmount = Convert.ToInt32(ATM["amount"]);

                            if (Amount > ATMAmount)
                            {
                                Client.SendWhisper("This ATM doesn't have enough funds for this withdraw request");
                                return;
                            }

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `atms` SET `amount` = `amount` - " + Amount + ", `fee` = `fee` + " + Fee + " WHERE `atm_id` = '" + ATMId + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }

                            DataRow NewATM = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                                dbClient.AddParameter("id", Client.GetHabbo().UsingItem);
                                NewATM = dbClient.getRow();
                            }

                            int NewATMAmount = Convert.ToInt32(NewATM["amount"]);
                            int NewATMId = Convert.ToInt32(NewATM["atm_id"]);

                            Client.GetHabbo().addCooldown("withdraw_cooldown", 3000);
                            Client.GetHabbo().Banque -= Amount + Fee;
                            Client.GetHabbo().updateBanque();
                            Client.GetHabbo().Credits += Amount;
                            Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                            Client.GetHabbo().RPCache(3);
                            User.Say("withdraws $" + PlusEnvironment.ConvertToPrice(Amount) + " from their bank account");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ATM-update;" + Client.GetHabbo().Banque + ";" + PlusEnvironment.ConvertToPrice(NewATMAmount));

                            if (NewATMAmount < 100)
                            {
                                PlusEnvironment.GetGame().GetClientManager().ATMCall(Client.GetHabbo().CurrentRoom.Name, NewATMId);
                            }
                        }
                    }
                    break;
                #endregion
                #region Quick Withdraw
                case "quick-withdraw":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!User.usingATM)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string GetMoney = ReceivedData[1];
                        int Amount = Convert.ToInt32(GetMoney);

                        if (Client.GetHabbo().getCooldown("withdraw_cooldown"))
                        {
                            Client.SendWhisper("You must wait before you can withdraw money from the ATM again");
                            return;
                        }

                        if (!int.TryParse(Convert.ToString(Amount), out Amount) || Amount <= 0 || Convert.ToString(Amount).StartsWith("0"))
                        {
                            Client.SendWhisper("The amount is invalid");
                            return;
                        }

                        int Fee = Amount * 5 / 100;

                        if (Amount + Fee > Client.GetHabbo().Banque)
                        {
                            Client.SendWhisper("You don't have $" + PlusEnvironment.ConvertToPrice(Amount + Fee) + " in your bank account");
                            return;
                        }

                        DataRow ATM = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().UsingItem);
                            ATM = dbClient.getRow();
                        }

                        int ATMId = Convert.ToInt32(ATM["atm_id"]);
                        int ATMAmount = Convert.ToInt32(ATM["amount"]);

                        if (Amount > ATMAmount)
                        {
                            Client.SendWhisper("This ATM doesn't have enough funds for this withdraw request");
                            return;
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `atms` SET `amount` = `amount` - " + Amount + ", `fee` = `fee` + " + Fee + " WHERE `atm_id` = '" + ATMId + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }

                        DataRow NewATM = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().UsingItem);
                            NewATM = dbClient.getRow();
                        }

                        int NewATMAmount = Convert.ToInt32(NewATM["amount"]);
                        int NewATMId = Convert.ToInt32(NewATM["atm_id"]);

                        Client.GetHabbo().addCooldown("withdraw_cooldown", 3000);
                        Client.GetHabbo().Banque -= Amount + Fee;
                        Client.GetHabbo().updateBanque();
                        Client.GetHabbo().Credits += Amount;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        Client.GetHabbo().RPCache(3);
                        User.Say("withdraws $" + PlusEnvironment.ConvertToPrice(Amount) + " from their bank account");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ATM-update;" + Client.GetHabbo().Banque + ";" + PlusEnvironment.ConvertToPrice(NewATMAmount));

                        if (NewATMAmount < 100)
                        {
                            PlusEnvironment.GetGame().GetClientManager().ATMCall(Client.GetHabbo().CurrentRoom.Name, NewATMId);
                        }
                    }
                    break;
                #endregion
                #region Withdraw
                case "withdraw":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!User.usingATM)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string GetMoney = ReceivedData[1];
                        int Amount = Convert.ToInt32(GetMoney);

                        if (Client.GetHabbo().getCooldown("withdraw_cooldown"))
                        {
                            Client.SendWhisper("You must wait before you can withdraw money from the ATM again");
                            return;
                        }

                        if (!int.TryParse(Convert.ToString(Amount), out Amount) || Amount <= 0 || Convert.ToString(Amount).StartsWith("0"))
                        {
                            Client.SendWhisper("The amount is invalid");
                            return;
                        }

                        int Fee = Amount * 5 / 100;

                        if (Amount + Fee > Client.GetHabbo().Banque)
                        {
                            Client.SendWhisper("You don't have $" + PlusEnvironment.ConvertToPrice(Amount + Fee) + " in your bank account");
                            return;
                        }

                        DataRow ATM = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().UsingItem);
                            ATM = dbClient.getRow();
                        }

                        int ATMId = Convert.ToInt32(ATM["atm_id"]);
                        int ATMAmount = Convert.ToInt32(ATM["amount"]);

                        if (Amount > ATMAmount)
                        {
                            Client.SendWhisper("This ATM doesn't have enough funds for this withdraw request");
                            return;
                        }

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `atms` SET `amount` = `amount` - " + Amount + ", `fee` = `fee` + " + Fee + " WHERE `atm_id` = '" + ATMId + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }

                        DataRow NewATM = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().UsingItem);
                            NewATM = dbClient.getRow();
                        }

                        int NewATMAmount = Convert.ToInt32(NewATM["amount"]);
                        int NewATMId = Convert.ToInt32(NewATM["atm_id"]);

                        Client.GetHabbo().addCooldown("withdraw_cooldown", 3000);
                        Client.GetHabbo().Banque -= Amount + Fee;
                        Client.GetHabbo().updateBanque();
                        Client.GetHabbo().Credits += Amount;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        Client.GetHabbo().RPCache(3);
                        User.Say("withdraws $" + PlusEnvironment.ConvertToPrice(Amount) + " from their bank account");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ATM-update;" + Client.GetHabbo().Banque + ";" + PlusEnvironment.ConvertToPrice(NewATMAmount));

                        if (NewATMAmount < 100)
                        {
                            PlusEnvironment.GetGame().GetClientManager().ATMCall(Client.GetHabbo().CurrentRoom.Name, NewATMId);
                        }
                    }
                    break;
                #endregion
                #region Claim
                case "claim":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().RankId > 4 && !Client.GetHabbo().Working)
                            return;

                        if (Client.GetHabbo().CurrentRoomId != 97)
                        {
                            Client.SendWhisper("You can only claim ATM's at Regions Bank");
                            return;
                        }

                        if (Client.GetHabbo().ClaimedATM > 0)
                        {
                            Client.SendWhisper("You can only claim 1 ATM at a time");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        int ATMId = Convert.ToInt32(ReceivedData[1]);

                        Client.GetHabbo().ClaimedATM = ATMId;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `atms` SET `claimed` = " + Client.GetHabbo().Id + " WHERE `atm_id` = '" + ATMId + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "atm-management;show;");
                        PlusEnvironment.GetGame().GetClientManager().ATMClaimedWhisper(Client.GetHabbo().Username, ATMId);
                    }
                    break;
                #endregion
                #region Refill
                case "refill":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!User.usingATM || Client.GetHabbo().RankId > 4 && !Client.GetHabbo().Working)
                            return;

                        if (Client.GetHabbo().ClaimedATM != Client.GetHabbo().UsingItem)
                        {
                            Client.SendWhisper("You have to Claim this ATM to refill it");
                            return;
                        }

                        DataRow ATM = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().ClaimedATM);
                            ATM = dbClient.getRow();
                        }

                        int ATMAmount = Convert.ToInt32(ATM["amount"]);

                        if (ATMAmount > 1499)
                        {
                            return;
                        }

                        User.Say("begins refilling the ATM");
                        User.ApplyEffect(4);
                        User.RefillinAtm = true;

                        Random TokenRand = new Random();
                        int tokenNumber = TokenRand.Next(1600, 2894354);
                        Client.GetHabbo().PlayToken = tokenNumber;

                        System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(30));
                        timer1.Interval = 15000;
                        timer1.Elapsed += delegate
                        {
                            if (User.RefillinAtm && Client.GetHabbo().PlayToken == tokenNumber)
                            {
                                User.Say("finishes refilling the ATM");
                                Client.GetHabbo().resetEffectEvent();
                                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                {
                                    dbClient.SetQuery("UPDATE `atms` SET `amount` = 1500, `claimed` = 0 WHERE `atm_id` = '" + Client.GetHabbo().UsingItem + "' LIMIT 1;");
                                    dbClient.RunQuery();
                                }
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ATM-update;" + Client.GetHabbo().Banque + ";" + PlusEnvironment.ConvertToPrice(1500));
                                User.RefillinAtm = false;
                                Client.GetHabbo().PlayToken = 0;
                            }
                            timer1.Stop();
                        };
                        timer1.Start();
                    }
                    break;
                #endregion
                #region Fees
                case "fees":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (!User.usingATM || Client.GetHabbo().RankId > 4 && !Client.GetHabbo().Working)
                            return;

                        if (Client.GetHabbo().ClaimedATM != Client.GetHabbo().UsingItem)
                        {
                            Client.SendWhisper("You have to Claim this ATM to collect fees");
                            return;
                        }

                        DataRow ATM = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                            dbClient.AddParameter("id", Client.GetHabbo().ClaimedATM);
                            ATM = dbClient.getRow();
                        }

                        int ATMId = Convert.ToInt32(ATM["atm_id"]);
                        int ATMFee = Convert.ToInt32(ATM["fee"]);

                        if (ATMFee < 1)
                        {
                            Client.SendWhisper("There are no fees to collect in this ATM");
                            return;
                        }

                        User.Say("empties $" + PlusEnvironment.ConvertToPrice(ATMFee) + " fees from the ATM");
                        Client.SendWhisper("You have earned a $" + PlusEnvironment.ConvertToPrice(ATMFee) + " tip from collecting fees");
                        Client.GetHabbo().Credits += ATMFee;
                        Client.SendMessage(new CreditBalanceComposer(Client.GetHabbo().Credits));
                        Client.GetHabbo().RPCache(3);

                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `atms` SET `fee` = 0 WHERE `atm_id` = '" + ATMId + "' LIMIT 1;");
                            dbClient.RunQuery();
                        }
                    }
                    break;
                #endregion
                /*#region ATM Call
                case "call":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        if (Client.GetHabbo().RankId > 4 && !Client.GetHabbo().Working)
                            return;

                        if (Client.GetHabbo().CurrentRoomId != 97)
                        {
                            Client.SendWhisper("You can only accept/decline ATM calls in Reagion Bank");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "atm-call;hide;");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        string Aktion = ReceivedData[1];
                        int ATMId = Convert.ToInt32(ReceivedData[2]);

                        if (Aktion == "accept")
                        {
                            DataRow ATM = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `atms` WHERE `atm_id` = @id LIMIT 1;");
                                dbClient.AddParameter("id", Convert.ToInt32(ATMId));
                                ATM = dbClient.getRow();
                            }

                            if (Convert.ToInt32(ATM["claimed"]) > 0)
                            {
                                Client.SendWhisper("A worker accepted this ATM Call");
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "atm-call;hide;");
                                return;
                            }

                            User.Say("accepts the ATM Call");
                            Client.GetHabbo().ClaimedATM = ATMId;

                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `atms` SET `claimed` = " + Client.GetHabbo().Id + " WHERE `atm_id` = '" + ATMId + "' LIMIT 1;");
                                dbClient.RunQuery();
                            }
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "atm-call;hide;");
                            PlusEnvironment.GetGame().GetClientManager().ATMClaimedWhisper(Client.GetHabbo().Username, ATMId);
                        }
                        else if (Aktion == "decline")
                        {
                            User.Say("declines the ATM Call");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "atm-call;hide;");
                        }
                    }
                    break;
                    #endregion*/
            }
        }
    }
}
