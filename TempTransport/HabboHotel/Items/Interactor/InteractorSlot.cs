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
    public class InteractorSlot : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session.GetHabbo().CurrentRoomId != 46 || Session.GetHabbo().Menotted == true)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

            if (Session.GetHabbo().getCooldown("slot_machine"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            if (User.connectedToSlot == true)
            {
                Session.SendWhisper("You are already playing.");
                return;
            }

            Session.GetHabbo().addCooldown("slot_machine", 2000);
            User.connectedToSlot = true;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "slot_machine;connect;" + Session.GetHabbo().Casino_Jetons);
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}