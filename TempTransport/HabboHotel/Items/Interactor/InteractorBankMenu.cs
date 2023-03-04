using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorBankMenu : IFurniInteractor
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

            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
            {
                if (!Session.GetRoleplay().Inventory.HasItem("flashbang", false))
                {
                    Session.GetRoleplay().Inventory.Add("flashbang", "loadout", 1, 0, true, false);
                    Session.SendWhisper("Your flashbang has been replenished");
                    User.Say("replenishes their flashbang");
                }
                else
                {
                    Session.SendWhisper("You already have a flashbang");
                }
            }

            if (Session.GetHabbo().JobId == 3 && Session.GetHabbo().Working)
            {
                if (Session.GetHabbo().usingMenuSell)
                    return;

                Session.GetHabbo().usingMenuSell = true;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;0;Rent");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;Deposit Box Rent;Deposit Box Rent");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;0;Apartment Purchase");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;0;Apartment Sell (DANGEROUS)");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;show");
                User.CarryItem(1004);
            }
        }
        public void OnWiredTrigger(Item Item)
        {
        }
    }
}