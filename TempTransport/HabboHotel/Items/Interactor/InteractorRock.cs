using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorRock : IFurniInteractor
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
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            if (Session.GetHabbo().getCooldown("farming_rock"))
            {
                Session.SendWhisper("You must wait before you can smash a rock again");
                return;
            }

            Session.GetHabbo().addCooldown("farming_rock", 5000);
            User.ApplyEffect(594);

            User.RockId = Item.Id;

            if (Session.GetRoleplay().Prison)
            {
                User.Say("starts smashing the rock");
            }
            else
            {
                User.Say("swings their pickaxe at the rock");

                int MainTime = 30;
                for (int i = 0; i < Session.GetRoleplay().FarmingLevel; i++)
                {
                    MainTime -= 1;
                }

                /*System.Timers.Timer FarmingTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(MainTime));
                FarmingTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(MainTime);
                FarmingTimer.Elapsed += delegate
                {
                    if (Session.GetHabbo().PlayToken == PlayToken && Session.GetHabbo().UsingFarm == Session.GetHabbo().UsingFarm)
                    {
                        Random Random = new Random();
                        int Chance = Random.Next(1, 11);

                        if (Chance >= 1 && Chance <= 8)
                        {
                            User.Say("mines 1 Iron Ore from the rock");
                            Session.GetHabbo().AddToInventory2("ironore", 1);
                        }
                        else
                        {
                            User.Say("mines 1 Coal from the rock");
                            Session.GetHabbo().AddToInventory2("coal", 1);
                        }

                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().FarmingXP();
                        Session.GetRoleplay().Energy -= 1;
                        Session.GetHabbo().RPCache(1);
                        Session.GetHabbo().PlayToken = 0;
                        Session.GetHabbo().UsingFarm = "";
                        Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    }
                    FarmingTimer.Stop();
                };
                FarmingTimer.Start();*/
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}