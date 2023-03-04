using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Pathfinding;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorCafe : IFurniInteractor
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

            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

            if (Session.GetHabbo().JobId != 6 || Session.GetHabbo().Working == false)
                return;

            //Coffee
            if (Item.GetBaseItem().SpriteId == 65001061)
            {
                if (User.CarryItemID != 0)
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;0;" + Session.GetHabbo().PlayToken);
                    Session.GetHabbo().PlayToken = 0;
                }

                if (PlusEnvironment.CoffeMachine.ContainsKey(Item.Id))
                {
                    User.Say("starts repairing coffee machine");
                    User.ApplyEffect(4);
                    Session.GetHabbo().UsingTask = "cofferepair";
                    Random RndmTokenPlay = new Random();
                    int PlayToken = RndmTokenPlay.Next(1600, 2894354);
                    Session.GetHabbo().PlayToken = PlayToken;

                    System.Timers.Timer RepairCoffeMachineTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                    RepairCoffeMachineTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                    RepairCoffeMachineTimer.Elapsed += delegate
                    {
                        if (Session.GetHabbo().UsingTask == "cofferepair" && Session.GetHabbo().PlayToken == PlayToken)
                        {
                            User.Say("finishes repairing coffee machine");
                            Session.SendWhisper("You have earned $25 for completing this task");
                            PlusEnvironment.StarbucksCoffeRepair(Session.GetHabbo().Id, Item.GetX, Item.GetY, "remove");
                            PlusEnvironment.CoffeSelled = 0;
                            PlusEnvironment.CoffeMachine.Remove(Item.Id);
                            Session.GetHabbo().UpdateTasksCompleted();
                            Session.GetHabbo().resetEffectEvent();
                            Session.GetHabbo().Credits += 25;
                            Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                            Session.GetHabbo().RPCache(3);
                            Session.GetHabbo().UsingTask = "";
                            Session.GetHabbo().PlayToken = 0;
                        }
                        RepairCoffeMachineTimer.Stop();
                    };
                    RepairCoffeMachineTimer.Start();
                    return;
                }

                if (PlusEnvironment.CoffeSelled >= 2)
                {
                    PlusEnvironment.StarbucksCoffeRepair(Session.GetHabbo().Id, Item.GetX, Item.GetY, "add");
                    PlusEnvironment.CoffeMachine.Add(Item.Id, Item.Id);
                }

                Random TokenRand = new Random();
                int tokenNumber = TokenRand.Next(1600, 2894354);
                Session.GetHabbo().PlayToken = tokenNumber;

                int Precent = 100;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;show;" + Precent + ";" + Session.GetHabbo().PlayToken);

                System.Timers.Timer Count = new System.Timers.Timer(1000);
                Count.Interval = 1000;
                Count.Elapsed += delegate
                {
                    if (Session.GetHabbo().PlayToken == tokenNumber)
                    {
                        Precent -= 5;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;count;" + Precent + ";" + Session.GetHabbo().PlayToken);

                        if (Precent <= 0 && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;" + Precent + ";" + Session.GetHabbo().PlayToken);
                            Session.GetHabbo().PlayToken = 0;
                            User.CarryItem(0);
                            Session.GetHabbo().CorpSell = null;
                            Count.Stop();
                        }
                    }
                    else
                    {
                        Count.Dispose();
                    }
                };
                Count.Start();

                User.CarryItem(41);
                Session.GetHabbo().CorpSell = "Coffee";
                return;
            }

            //Snack
            if (Item.GetBaseItem().SpriteId == 7873)
            {
                if (Session.GetHabbo().RankId == 7)
                {
                    Session.SendWhisper("You are not able to sell snacks yet");
                    return;
                }

                if (User.CarryItemID != 0)
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;0;" + Session.GetHabbo().PlayToken);
                    Session.GetHabbo().PlayToken = 0;
                }

                Random TokenRand = new Random();
                int tokenNumber = TokenRand.Next(1600, 2894354);
                Session.GetHabbo().PlayToken = tokenNumber;

                int Precent = 100;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;show;" + Precent + ";" + Session.GetHabbo().PlayToken);

                System.Timers.Timer Count = new System.Timers.Timer(1000);
                Count.Interval = 1000;
                Count.Elapsed += delegate
                {
                    if (Session.GetHabbo().PlayToken == tokenNumber)
                    {
                        Precent -= 5;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;count;" + Precent + ";" + Session.GetHabbo().PlayToken);

                        if (Precent <= 0 && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "UserEffects_effects_2AlOo;hide;" + Precent + ";" + Session.GetHabbo().PlayToken);
                            Session.GetHabbo().PlayToken = 0;
                            User.CarryItem(0);
                            Session.GetHabbo().CorpSell = null;
                            Count.Stop();
                        }
                    }
                    else
                    {
                        Count.Dispose();
                    }
                };
                Count.Start();

                User.CarryItem(51);
                Session.GetHabbo().CorpSell = "Snack";
                return;
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}