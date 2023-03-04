using System;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorPolice : IFurniInteractor
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

            if (Session.GetHabbo().JobId != 1 && !Session.GetHabbo().Working)
                return;

            if (!Session.GetRoleplay().Inventory.EquipHasItem("stungun") && !Session.GetRoleplay().Inventory.HasItem("stungun", false))
            {
                Session.GetRoleplay().Inventory.Add("stungun", "weapon", 0, 100, false, true);
            }
            else
            {
                if (Session.GetRoleplay().Inventory.EquipSlot1 == "stungun")
                {
                    Session.GetRoleplay().Inventory.EquipSlot1Durability = 100;
                    Session.GetRoleplay().SendWeb("inventory-durability-update;slot1;" + Session.GetRoleplay().Inventory.EquipSlot1Durability);
                }
                else if (Session.GetRoleplay().Inventory.EquipSlot2 == "stungun")
                {
                    Session.GetRoleplay().Inventory.EquipSlot2Durability = 100;
                    Session.GetRoleplay().SendWeb("inventory-durability-update;slot2;" + Session.GetRoleplay().Inventory.EquipSlot2Durability);
                }
                else
                {
                    Session.GetRoleplay().Inventory.HasItem("stungun", true);
                    Session.GetRoleplay().Inventory.Remove(Session.GetRoleplay().Inventory.FromSlot);
                    Session.GetRoleplay().Inventory.Add("stungun", "weapon", 0, 100, false, true);
                }

                Session.GetRoleplay().ResetEffect();
                Session.SendWhisper("Your stun gun's charges have been replenished");
            }

            if (!Session.GetRoleplay().Inventory.HasItem("handcuffs", false))
            {
                Session.GetRoleplay().Inventory.Add("handcuffs", "loadout", 1, 0, true, false);
                Session.SendWhisper("Your cuffs have been replenished");
            }

            if (!Session.GetRoleplay().Inventory.HasItem("stolenpepperspray", false))
            {
                Session.GetRoleplay().Inventory.Add("stolenpepperspray", "loadout", 1, 0, true, false);
                Session.SendWhisper("You have replenished your Cop Pepper Spray");
            }

            User.Say("replenishes their police gear");
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}