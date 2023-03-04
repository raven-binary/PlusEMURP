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
    public class InteractorEvent : IFurniInteractor
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
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y) && Item.GetBaseItem().SpriteId != 4614)
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }
            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}