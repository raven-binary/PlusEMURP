using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorCorpHack : IFurniInteractor
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

            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

            if (Session.GetHabbo().JobId == 8)
            {
                User.GetClient().SendWhisper("You can't hack a corp without a job");
                return;
            }

            if (Session.GetRoleplay().Passive)
            {
                User.GetClient().SendWhisper("You can't hack while in passive mode");
                return;
            }

            if (Session.GetHabbo().RPItems().NFCHacker < 1)
            {
                User.GetClient().SendWhisper("You need a NFC Hacker to hack a corp");
                return;
            }

            if (Session.GetHabbo().JobId == Session.GetHabbo().JobId)
            {
                User.GetClient().SendWhisper("You can't hack your own corp");
                return;
            }

        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}