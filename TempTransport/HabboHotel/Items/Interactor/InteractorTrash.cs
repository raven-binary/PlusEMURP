using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorTrash : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            if (Session.GetRoleplay().usingTrash)
                return;

            if (Session.GetHabbo().getCooldown("trash"))
            {
                Session.SendWhisper("You must wait before you can search the bin again");
                return;
            }

            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

            if (PlusEnvironment.Trash.ContainsKey(Item.Id))
            {
                Session.SendWhisper("There is nothing to find in the bin at this time");
                return;
            }

            Session.GetRoleplay().usingTrash = true;
            Session.GetHabbo().addCooldown("trash", 5000);
            User.Say("starts trawling through the bin looking for something useful");

            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Searching bin" + ";" + 30000 + ";" + 30);
            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;

            System.Timers.Timer TrashWaitTimer = new System.Timers.Timer(30000);
            TrashWaitTimer.Interval = 30000;
            TrashWaitTimer.Elapsed += delegate
            {
                if (Session.GetHabbo().usingTrash && Session.GetHabbo().PlayToken == tokenNumber)
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "hide");
                    Session.GetHabbo().usingTrash = false;

                    Random TrashRandom = new Random();
                    int Trash = TrashRandom.Next(1, 4);

                    Random RMoney = new Random();
                    int Money = RMoney.Next(5, 30);
                    if (Trash == 1)
                    {
                        User.Say("finds " + Money + " coins");
                        Session.GetHabbo().Credits += Money;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                        Session.GetHabbo().RPCache(4);
                    }
                    else if (Trash == 2)
                    {
                        Random RLockpicks = new Random();
                        int Lockpicks = RLockpicks.Next(1, 6);

                        User.Say("finds " + Lockpicks + " Lockpicks");
                        Session.GetHabbo().AddToInventory2("lockpick", Lockpicks);
                    }
                    else if (Trash == 3)
                    {
                        User.Say("finds " + Money + " coins");
                        Session.GetHabbo().Credits += Money;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                    }
                    else
                    {
                        Session.SendWhisper("You didn't find anything in the bin");
                    }

                    Item.ExtraData = "1";
                    Item.UpdateState(false, true);
                    PlusEnvironment.Trash.Add(Item.Id, DateTime.Now);
                    System.Timers.Timer timer1 = new System.Timers.Timer(900000);
                    timer1.Interval = 900000;
                    timer1.Elapsed += delegate
                    {
                        PlusEnvironment.Trash.Remove(Item.Id);
                        Item.ExtraData = "0";
                        Item.UpdateState(false, true);
                        timer1.Stop();
                    };
                    timer1.Start();
                }
                TrashWaitTimer.Stop();
            };
            TrashWaitTimer.Start();
            }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}