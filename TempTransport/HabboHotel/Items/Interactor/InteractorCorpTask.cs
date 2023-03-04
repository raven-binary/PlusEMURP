using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorCorpTask : IFurniInteractor
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

            if (Item.GetBaseItem().SpriteId == 5300 && Session.GetHabbo().Working && Session.GetHabbo().JobId == 2 && Session.GetHabbo().CurrentRoomId == 62)
            {
                if ((DateTime.Now - Session.GetHabbo().StartedWork).TotalMinutes >= 2)
                {
                    User.Say("starts cleaning up the blood");
                    User.ApplyEffect(4);
                    Session.GetHabbo().UsingTask = "blood";

                    Random TokenRand = new Random();
                    int tokenNumber = TokenRand.Next(1600, 2894354);
                    Session.GetHabbo().PlayToken = tokenNumber;

                    System.Timers.Timer CleanBloodTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                    CleanBloodTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                    CleanBloodTimer.Elapsed += delegate
                    {
                        if (Session.GetHabbo().UsingTask != "blood" && Session.GetHabbo().PlayToken != tokenNumber)
                            return;

                        Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);

                        User.Say("finishes cleaning up the blood");
                        Session.SendWhisper("You have earned $25 for completing this task");
                        Session.GetHabbo().UpdateTasksCompleted();
                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().Credits += 25;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                        Session.GetHabbo().RPCache(3);
                        Session.GetHabbo().UsingTask = "";
                        Session.GetHabbo().PlayToken = 0;
                        CleanBloodTimer.Stop();
                    };
                    CleanBloodTimer.Start();
                }
                else
                {
                    Session.SendWhisper("You must wait 2 minutes to complete this task");
                    return;
                }
            }

            if (!Session.GetHabbo().getCooldown("f21rubbish") && Item.GetBaseItem().SpriteId == 4338 && Session.GetHabbo().Working && Session.GetHabbo().CurrentRoomId == Session.GetHabbo().TravailInfo.RoomId)
            {
                if ((DateTime.Now - Session.GetHabbo().StartedWork).TotalMinutes >= 2)
                {
                    User.Say("starts cleaning rubbish from the floor");
                    User.ApplyEffect(4);
                    Session.GetHabbo().UsingTask = "rubbish";
                    Session.GetHabbo().addCooldown("f21rubbish", 5000);

                    Random TokenRand = new Random();
                    int tokenNumber = TokenRand.Next(1600, 2894354);
                    Session.GetHabbo().PlayToken = tokenNumber;

                    System.Timers.Timer CleanBloodTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                    CleanBloodTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                    CleanBloodTimer.Elapsed += delegate
                    {
                        if (Session.GetHabbo().UsingTask == "rubbish" && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            User.Say("finishes cleaning up the rubbish");
                            Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                            Session.SendWhisper("You have earned $25 for completing this task");
                            Session.GetHabbo().UpdateTasksCompleted();
                            Session.GetHabbo().resetEffectEvent();
                            Session.GetHabbo().Credits += 25;
                            Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                            Session.GetHabbo().RPCache(3);
                            Session.GetHabbo().UsingTask = "";
                            Session.GetHabbo().PlayToken = 0;
                        }
                        CleanBloodTimer.Stop();
                    };
                    CleanBloodTimer.Start();
                }
                else
                {
                    Session.SendWhisper("You must wait 2 minutes to complete this task");
                    return;
                }
            }

            if (Item.GetBaseItem().SpriteId == 5495 && Session.GetHabbo().Working && Session.GetHabbo().JobId == 6 && Session.GetHabbo().CurrentRoomId == 94)
            {
                if ((DateTime.Now - Session.GetHabbo().StartedWork).TotalMinutes >= 2)
                {
                    User.Say("starts cleaning coffee spill");
                    User.ApplyEffect(4);
                    Session.GetHabbo().UsingTask = "coffespill";

                    Random TokenRand = new Random();
                    int tokenNumber = TokenRand.Next(1600, 2894354);
                    Session.GetHabbo().PlayToken = tokenNumber;

                    System.Timers.Timer CleanCoffeSpillTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(15));
                    CleanCoffeSpillTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(15);
                    CleanCoffeSpillTimer.Elapsed += delegate
                    {
                        if (Session.GetHabbo().UsingTask != "coffespill" && Session.GetHabbo().PlayToken != tokenNumber)
                            return;

                        Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);

                        User.Say("finishes cleaning coffee spill");
                        Session.SendWhisper("You have earned $25 for completing this task");
                        Session.GetHabbo().UpdateTasksCompleted();
                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().Credits += 25;
                        Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                        Session.GetHabbo().RPCache(3);
                        Session.GetHabbo().UsingTask = "";
                        Session.GetHabbo().PlayToken = 0;
                        CleanCoffeSpillTimer.Stop();
                    };
                    CleanCoffeSpillTimer.Start();
                }
                else
                {
                    Session.SendWhisper("You must wait 2 minutes to complete this task");
                    return;
                }
            }
            if (Item.GetBaseItem().SpriteId == 789987 && Session.GetHabbo().Working && Session.GetHabbo().CurrentRoomId == Session.GetHabbo().TravailInfo.RoomId)
            {
                if ((DateTime.Now - Session.GetHabbo().StartedWork).TotalMinutes >= 2)
                {
                    User.Say("picks up merchandise");
                    Session.GetHabbo().AddToInventory2("box", 1);
                    Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                }
                else
                {
                    Session.SendWhisper("You must wait 2 minutes to complete this task");
                    return;
                }
            }


            if (Item.GetBaseItem().SpriteId == 3358 && Session.GetHabbo().Working && Session.GetHabbo().CurrentRoomId == Session.GetHabbo().TravailInfo.RoomId)
            {
                if ((DateTime.Now - Session.GetHabbo().StartedWork).TotalMinutes >= 2)
                {
                    User.Say("picks up pile of letters");
                    Session.GetHabbo().AddToInventory2("letter", 1);
                    Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                }
                else
                {
                    Session.SendWhisper("You must wait 2 minutes to complete this task");
                    return;
                }
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}