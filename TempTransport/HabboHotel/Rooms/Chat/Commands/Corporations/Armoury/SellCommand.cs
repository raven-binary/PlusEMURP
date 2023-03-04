using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SellWeaponCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<usenname>"; }
        }

        public string Description
        {
            get { return "Sells then item in your hand"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :sell <username>");
                return;
            }

            string Username = Params[1];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found");
                return;
            }
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (Math.Abs(User.X - TargetUser.X) > 2 || Math.Abs(User.Y - TargetUser.Y) > 2)
            {
                Session.SendWhisper("You must next to the target");
                return;
            }

            if (TargetUser.Transaction != null || TargetUser.isTradingItems)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a transaction in progress");
                return;
            }

            if (Session.GetHabbo().JobId == 4)
            {
                if (!PlusEnvironment.checkIfItemExist(Session.GetHabbo().CorpSell, "arme"))
                {
                    Session.SendWhisper("That weapon doesn't exist");
                    return;
                }

                if (Params.Length < 3)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                int num;
                if (!Int32.TryParse(Params[2], out num) || Params[2].StartsWith("0"))
                {
                    Session.SendWhisper("The amount is invalid");
                    return;
                }

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;0;" + Session.GetHabbo().PlayToken);
                Session.GetHabbo().PlayToken = 0;

                int Much = Convert.ToInt32(Params[2]);
                int Price = PlusEnvironment.getPriceOfItem(Session.GetHabbo().CorpSell) * Much;
                int Stock = Much * PlusEnvironment.GetStockOfItem(Session.GetHabbo().CorpSell);

                Group Armoury = null;
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(4, out Armoury))
                {
                    if (Armoury.ChiffreAffaire < Stock)
                    {
                        Session.SendWhisper("There is " + PlusEnvironment.ConvertToPrice(Armoury.ChiffreAffaire) + " stock available, the corporation needs " + PlusEnvironment.ConvertToPrice(Stock) + " stock for this item to be sold", 34);
                        TargetClient.SendWhisper("There is " + PlusEnvironment.ConvertToPrice(Armoury.ChiffreAffaire) + " stock available, the corporation needs " + PlusEnvironment.ConvertToPrice(Stock) + " stock for this item to be sold", 34);
                        return;
                    }
                }

                if (Price > TargetClient.GetHabbo().Credits)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + Price);
                    return;
                }

                if (Much > 5)
                {
                    Session.SendWhisper("You can sell only 5 " + Session.GetHabbo().CorpSell + "'s at a time");
                    return;
                }

                if (TargetClient.GetHabbo().InventoryFull())
                {
                    Session.SendWhisper("This player inventory is full!");
                    return;
                }

                User.CarryItem(0);

                User.Say("offers " + TargetClient.GetHabbo().Username + " " + Much + "x " + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + " for $" + Price);
                TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
                Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;

                Random TokenRand = new Random();
                int tokenNumber = TokenRand.Next(1600, 2894354);
                TargetClient.GetHabbo().OfferToken = tokenNumber;

                TargetUser.Transaction = "armoury:" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + ":" + Price + ":" + Much + ":" + Stock;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to sell you " + Much + "x  <b>" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + "</b> for <b>$" + Price);
                Session.GetHabbo().CorpSell = null;

                System.Timers.Timer OfferExpireTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                OfferExpireTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                OfferExpireTimer.Elapsed += delegate
                {
                    if (TargetClient != null && TargetClient.GetHabbo().OfferToken == tokenNumber)
                    {
                        TargetClient.SendWhisper("The offer has expired");
                        Session.SendWhisper("The offer has expired");

                        Session.GetHabbo().TransactionTo = null;
                        Session.GetHabbo().CorpSell = null;
                        TargetUser.Transaction = null;
                        TargetClient.GetHabbo().TransactionFrom = null;
                        TargetClient.GetHabbo().OfferToken = 0;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction-hide;");
                    }
                    OfferExpireTimer.Stop();
                };
                OfferExpireTimer.Start();
            }
            else if (Session.GetHabbo().JobId == 2)
            {
                if (Session.GetHabbo().CorpSell == null)
                {
                    Session.SendWhisper("You have nothing on your hand to sell");
                    return;
                }

                if (Params.Length < 3)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                int num;
                if (!Int32.TryParse(Params[2], out num) || Params[2].StartsWith("0"))
                {
                    Session.SendWhisper("The amount is invalid");
                    return;
                }

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;0;" + Session.GetHabbo().PlayToken);
                Session.GetHabbo().PlayToken = 0;

                int Much = Convert.ToInt32(Params[2]);
                int Price = PlusEnvironment.getPriceOfItem(Session.GetHabbo().CorpSell) * Much;
                int Stock = Much * PlusEnvironment.GetStockOfItem(Session.GetHabbo().CorpSell);

                Group Hospital = null;
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(2, out Hospital))
                {
                    if (Hospital.ChiffreAffaire < Stock)
                    {
                        Session.SendWhisper("There is " + PlusEnvironment.ConvertToPrice(Hospital.ChiffreAffaire) + " stock available, the corporation needs " + PlusEnvironment.ConvertToPrice(Stock) + " stock for this item to be sold", 34);
                        TargetClient.SendWhisper("There is " + PlusEnvironment.ConvertToPrice(Hospital.ChiffreAffaire) + " stock available, the corporation needs " + PlusEnvironment.ConvertToPrice(Stock) + " stock for this item to be sold", 34);
                        return;
                    }
                }

                if (Session.GetHabbo().CorpSell == "Heal" && Much > 1)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                if (Much > 5)
                {
                    Session.SendWhisper("You can sell only 5 " + Session.GetHabbo().CorpSell + "'s at a time");
                    return;
                }

                if (Price > TargetClient.GetHabbo().Credits)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + Price);
                    return;
                }

                if (Session.GetHabbo().CorpSell == "Medkit" && TargetClient.GetHabbo().InventoryFull())
                {
                    Session.SendWhisper("This player inventory is full!");
                    return;
                }

                User.CarryItem(0);

                User.Say("offers " + TargetClient.GetHabbo().Username + " " + Much + "x " + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + " for $" + Price);
                TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
                Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;

                Random TokenRand = new Random();
                int tokenNumber = TokenRand.Next(1600, 2894354);
                TargetClient.GetHabbo().OfferToken = tokenNumber;

                TargetUser.Transaction = "hospital:" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + ":" + Price + ":" + Much + ":" + Stock;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to sell you " + Much + "x  <b>" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + "</b> for <b>$" + Price);
                Session.GetHabbo().CorpSell = null;

                System.Timers.Timer OfferExpireTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                OfferExpireTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                OfferExpireTimer.Elapsed += delegate
                {
                    if (TargetClient != null && TargetClient.GetHabbo().OfferToken == tokenNumber)
                    {
                        TargetClient.SendWhisper("The offer has expired");
                        Session.SendWhisper("The offer has expired");

                        Session.GetHabbo().TransactionTo = null;
                        Session.GetHabbo().CorpSell = null;
                        TargetUser.Transaction = null;
                        TargetClient.GetHabbo().TransactionFrom = null;
                        TargetClient.GetHabbo().OfferToken = 0;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction-hide;");
                    }
                    OfferExpireTimer.Stop();
                };
                OfferExpireTimer.Start();
            }
            else if (Session.GetHabbo().JobId == 6)
            {
                if (Session.GetHabbo().CorpSell == null)
                {
                    Session.SendWhisper("You have nothing on your hand to sell");
                    return;
                }

                if (Params.Length < 3)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                int num;
                if (!Int32.TryParse(Params[2], out num) || Params[2].StartsWith("0"))
                {
                    Session.SendWhisper("The amount is invalid");
                    return;
                }

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;0;" + Session.GetHabbo().PlayToken);
                Session.GetHabbo().PlayToken = 0;

                int Much = Convert.ToInt32(Params[2]);
                int Price = PlusEnvironment.getPriceOfItem(Session.GetHabbo().CorpSell) * Much;
                int Stock = Much * PlusEnvironment.GetStockOfItem(Session.GetHabbo().CorpSell);

                Group Starbucks = null;
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(6, out Starbucks))
                {
                    if (Starbucks.ChiffreAffaire < Stock)
                    {
                        Session.SendWhisper("There is " + PlusEnvironment.ConvertToPrice(Starbucks.ChiffreAffaire) + " stock available, the corporation needs " + PlusEnvironment.ConvertToPrice(Stock) + " stock for this item to be sold", 34);
                        TargetClient.SendWhisper("There is " + PlusEnvironment.ConvertToPrice(Starbucks.ChiffreAffaire) + " stock available, the corporation needs " + PlusEnvironment.ConvertToPrice(Stock) + " stock for this item to be sold", 34);
                        return;
                    }
                }

                if (Session.GetHabbo().CorpSell == "Coffee" && Much > 1)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                if (Much > 5)
                {
                    Session.SendWhisper("You can sell only 5 " + Session.GetHabbo().CorpSell + "'s at a time");
                    return;
                }

                if (Price > TargetClient.GetHabbo().Credits)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + Price);
                    return;
                }

                if (Session.GetHabbo().CorpSell == "Snack" && TargetClient.GetHabbo().InventoryFull())
                {
                    Session.SendWhisper("This player inventory is full!");
                    return;
                }

                User.CarryItem(0);

                User.Say("offers " + TargetClient.GetHabbo().Username + " " + Much + "x " + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + " for $" + Price);
                TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
                Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;

                Random TokenRand = new Random();
                int tokenNumber = TokenRand.Next(1600, 2894354);
                TargetClient.GetHabbo().OfferToken = tokenNumber;

                TargetUser.Transaction = "starbucks:" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + ":" + Price + ":" + Much + ":" + Stock;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to sell you " + Much + "x  <b>" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + "</b> for <b>$" + Price);
                Session.GetHabbo().CorpSell = null;

                System.Timers.Timer OfferExpireTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                OfferExpireTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                OfferExpireTimer.Elapsed += delegate
                {
                    if (TargetClient != null && TargetClient.GetHabbo().OfferToken == tokenNumber)
                    {
                        TargetClient.SendWhisper("The offer has expired");
                        Session.SendWhisper("The offer has expired");

                        Session.GetHabbo().TransactionTo = null;
                        Session.GetHabbo().CorpSell = null;
                        TargetUser.Transaction = null;
                        TargetClient.GetHabbo().TransactionFrom = null;
                        TargetClient.GetHabbo().OfferToken = 0;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction-hide;");
                    }
                    OfferExpireTimer.Stop();
                };
                OfferExpireTimer.Start();
            }
            else if (Session.GetHabbo().JobId == 3)
            {
                if (Session.GetHabbo().CorpSell == null)
                {
                    Session.SendWhisper("You have nothing on your hand to sell");
                    return;
                }

                if (Params.Length < 3)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                int num;
                if (!Int32.TryParse(Params[2], out num) || Params[2].StartsWith("0"))
                {
                    Session.SendWhisper("The amount is invalid");
                    return;
                }

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;0;" + Session.GetHabbo().PlayToken);
                Session.GetHabbo().PlayToken = 0;

                int Much = Convert.ToInt32(Params[2]);
                int Price = PlusEnvironment.getPriceOfItem(Session.GetHabbo().CorpSell);

                if (Much > 1)
                {
                    PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":sell " + TargetUser.GetUsername() + " 1");
                    return;
                }

                if (Session.GetHabbo().CorpSell == "Deposit Box Rent" && TargetClient.GetRoleplay().DepositRent > DateTime.Now)
                {
                    Session.SendWhisper("This player has already paid their Deposit Box Rent");
                    return;
                }

                if (Price > TargetClient.GetHabbo().Credits)
                {
                    Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + Price);
                    return;
                }

                

                User.Say("offers " + TargetClient.GetHabbo().Username + " " + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + " for $" + Price);
                TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
                Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;

                Random TokenRand = new Random();
                int tokenNumber = TokenRand.Next(1600, 2894354);
                TargetClient.GetHabbo().OfferToken = tokenNumber;

                TargetUser.Transaction = "bank:sell:" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + ":" + Price;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> offers you <b>" + PlusEnvironment.getNameOfItem(Session.GetHabbo().CorpSell) + "</b> for <b>$" + Price);
                Session.GetHabbo().CorpSell = null;

                System.Timers.Timer OfferExpireTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                OfferExpireTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                OfferExpireTimer.Elapsed += delegate
                {
                    if (TargetClient != null && TargetClient.GetHabbo().OfferToken == tokenNumber)
                    {
                        TargetClient.SendWhisper("The offer has expired");
                        Session.SendWhisper("The offer has expired");

                        Session.GetHabbo().TransactionTo = null;
                        Session.GetHabbo().CorpSell = null;
                        TargetUser.Transaction = null;
                        TargetClient.GetHabbo().TransactionFrom = null;
                        TargetClient.GetHabbo().OfferToken = 0;
                        
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction-hide;");
                    }
                    OfferExpireTimer.Stop();
                };
                OfferExpireTimer.Start();
            }
        }
    }
}