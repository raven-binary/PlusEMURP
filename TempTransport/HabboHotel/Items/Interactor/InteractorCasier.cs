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
    public class InteractorCasier : IFurniInteractor
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

            if (User.usingCasier)
                return;

            if(User.Tased || Session.GetHabbo().Menotted == true)
            {
                Session.SendWhisper("You cannot open your locker while immobilized.");
                return;
            }

            if(Session.GetHabbo().getCooldown("open_casier"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            if(User.isTradingItems)
            {
                Session.SendWhisper("You cannot open your locker while trading.");
                return;
            }

            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y) && Item.GetBaseItem().SpriteId != 6116)
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            Session.GetHabbo().addCooldown("open_casier", 5000);
            User.usingCasier = true;
            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);
            User.GetClient().Shout("*Enter the code to open the locker*");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "casier;open;" + Session.GetHabbo().CasierWeed + ";" + Session.GetHabbo().CasierCocktails);
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}