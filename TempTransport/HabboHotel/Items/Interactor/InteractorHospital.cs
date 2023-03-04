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
    public class InteractorHospital : IFurniInteractor
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

            if (Session.GetHabbo().JobId != 2 || !Session.GetHabbo().Working)
                return;

            //Heal
            if (Item.GetBaseItem().SpriteId == 3586)
            {
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

                User.CarryItem(1013);
                Session.GetHabbo().CorpSell = "Heal";
                return;
            }

            //Medkit
            if (Item.GetBaseItem().SpriteId == 3608)
            {
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

                User.CarryItem(1013);
                Session.GetHabbo().CorpSell = "Medkit";
                return;
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}