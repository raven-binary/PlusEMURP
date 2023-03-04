using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Linq;
using Plus.Database.Interfaces;
using System.Data;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorRobCash : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Session.GetHabbo().usingRobTill)
                return;

            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
                return;

            User.Freezed = true;

            if (Session.GetHabbo().CurrentRoom.Group.Id == Session.GetHabbo().JobId && Session.GetHabbo().RankId < 4 && Session.GetHabbo().Working)
            {
                if ((DateTime.Now - Session.GetHabbo().StartedWork).TotalMinutes >= 2)
                {
                    Group Corporation = null;
                    if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Session.GetHabbo().JobId, out Corporation))
                    {
                        if (PlusEnvironment.GetGame().GetClientManager().CheckIfInUse(Item.Id))
                        {
                            User.Freezed = false;
                            return;
                        }

                        if (PlusEnvironment.CorpCashTillCooldown.ContainsKey(Corporation.Id))
                        {
                            Session.SendWhisper("Till was recently emptied, till currently on cooldown");
                            User.Freezed = false;
                            return;
                        }

                        if (Corporation.Cash == 0)
                        {
                            Session.SendWhisper("The till is empty");
                            User.Freezed = false;
                            return;
                        }

                        Item.ExtraData = "1";
                        Item.UpdateState(false, true);

                        User.Say("moves " + PlusEnvironment.ConvertToPrice(Corporation.Cash) + " dollars from the till to the safe");
                        Session.SendWhisper("You have emptied $" + PlusEnvironment.ConvertToPrice(Corporation.Cash) + " from the till");
                        PlusEnvironment.CorpCashTillCooldown.Add(Corporation.Id, Corporation.Id);
                        User.Freezed = false;

                        Corporation.Safe += Corporation.Cash;
                        Corporation.Cash -= Corporation.Cash;
                        Corporation.updateCash();

                        System.Timers.Timer Timer = new System.Timers.Timer(1000);
                        Timer.Interval = 1000;
                        Timer.Elapsed += delegate
                        {
                            Item.ExtraData = "0";
                            Item.UpdateState(false, true);
                            Timer.Stop();
                        };
                        Timer.Start();

                        System.Timers.Timer RemoveCooldownTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(15));
                        RemoveCooldownTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(15);
                        RemoveCooldownTimer.Elapsed += delegate
                        {
                            PlusEnvironment.CorpCashTillCooldown.Remove(Corporation.Id);
                            RemoveCooldownTimer.Stop();
                        };
                        RemoveCooldownTimer.Start();
                    }
                }
                else
                {
                    Session.SendWhisper("You must wait 2 minutes to empty the till");
                    User.Freezed = false;
                }
            }
            else
            {
                Group Corporation = null;
                if (PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Session.GetHabbo().CurrentRoom.Group.Id, out Corporation))
                {
                    if (PlusEnvironment.GetGame().GetClientManager().CheckIfInUse(Item.Id))
                    {
                        User.Freezed = false;
                        return;
                    }

                    if (Session.GetRoleplay().Passive)
                    {
                        Session.SendWhisper("You cannot rob this till in passive mode");
                        User.Freezed = false;
                        return;
                    }

                    if (Session.GetHabbo().ArmeEquiped != "bat")
                    {
                        Session.SendWhisper("You must be holding a bat with at least 25% durability");
                        User.Freezed = false;
                        return;
                    }

                    if (PlusEnvironment.CorpCashTillRobbed.ContainsKey(Corporation.Id))
                    {
                        Session.SendWhisper("This till has already been robbed");
                        User.Freezed = false;
                        return;
                    }

                    if (Corporation.Cash == 0)
                    {
                        Session.SendWhisper("The till is empty");
                        User.Freezed = false;
                        return;
                    }

                    if (Session.GetHabbo().getCooldown("RobCorpTill"))
                    {
                        Session.SendWhisper("You must wait before you can rob the till again");
                        User.Freezed = false;
                        return;
                    }

                    Session.GetHabbo().UsingItem = Item.Id;
                    Session.GetHabbo().usingRobTill = true;
                    User.Say("starts smashing the till with their bat");
                    User.ApplyEffect(45);
                    Session.GetHabbo().addCooldown("RobCorpTill", 5000);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Smashing till" + ";" + 30000 + ";" + 30);

                    Random TokenRand = new Random();
                    int tokenNumber = TokenRand.Next(1600, 2894354);
                    Session.GetHabbo().PlayToken = tokenNumber;
                    int MoneyTaken = 0;

                    System.Timers.Timer RobTillTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(30));
                    RobTillTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(30);
                    RobTillTimer.Elapsed += delegate
                    {
                        if (Session.GetHabbo().usingRobTill && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "hide");
                            User.Say("successfully smashes the till and begins to take the money");
                            PlusEnvironment.CorpCashTillRobbed.Add(Corporation.Id, Corporation.Id);
                            RobTillTimer.Stop();

                            System.Timers.Timer RobTillTimer2 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(5));
                            RobTillTimer2.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(5);
                            RobTillTimer2.Elapsed += delegate
                            {
                                if (Session.GetHabbo().usingRobTill && Session.GetHabbo().PlayToken == tokenNumber)
                                {
                                    Random Random = new Random();
                                    int Money = Random.Next(1, Corporation.Cash);

                                    User.Say("takes $" + Money + " from the till");
                                    MoneyTaken += Money;

                                    Session.GetHabbo().Credits += Money;
                                    Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                                    Session.GetHabbo().RPCache(3);

                                    Corporation.Cash -= Money;
                                    Corporation.updateCash();

                                    if (Corporation.Cash <= 0 | MoneyTaken >= 300)
                                    {
                                        Item.ExtraData = "1";
                                        Item.UpdateState(false, true);

                                        User.Freezed = false;
                                        Session.GetHabbo().UsingItem = 0;
                                        Session.GetHabbo().usingRobTill = false;
                                        Session.GetHabbo().resetEffectEvent();
                                        Session.GetHabbo().UpdateInventory(Session.GetHabbo().InventoryEquipSlot1, 10);
                                        Session.SendWhisper("You have successfully robbed the till");
                                        PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"green\">" + Session.GetHabbo().Username + "</span> robbed " + Session.GetHabbo().CurrentRoom.Name);
                                        RobTillTimer2.Stop();
                                    }
                                }
                            };
                            RobTillTimer2.Start();
                        }
                        else
                        {
                            RobTillTimer.Stop();
                        }
                    };
                    RobTillTimer.Start();

                    System.Timers.Timer RemoveCooldownTimer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(15));
                    RemoveCooldownTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(15);
                    RemoveCooldownTimer.Elapsed += delegate
                    {
                        PlusEnvironment.CorpCashTillRobbed.Remove(Corporation.Id);
                        RemoveCooldownTimer.Stop();
                    };
                    RemoveCooldownTimer.Start();
                }
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}