using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorCapture : IFurniInteractor
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

            if (!Session.GetHabbo().CurrentRoom.Description.Contains("GHETTO"))
                return;

            if (Session.GetHabbo().CurrentRoom.Capture == Session.GetHabbo().Gang)
            {
                Session.SendWhisper("Your gang already controls this place.");
                return;
            }

            if (Session.GetHabbo().Gang == 0)
            {
                Session.SendWhisper("You can't take this place because you don't have a gang.");
                return;
            }

            if (User.Capture != 0)
            {
                Session.SendWhisper("You already control this place. ");
                return;
            }

            if (PlusEnvironment.GetGame().GetClientManager().checkCapture(Session.GetHabbo().CurrentRoomId) == true)
            {
                Session.SendWhisper("A citizen already controls this place.");
                return;
            }
            
            User.CaptureProgress = 0;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "capture;update;" + User.CaptureProgress);
            User.Capture = Session.GetHabbo().CurrentRoomId;
            User.GetClient().Shout("*Starts controlling this place*");
            if (Session.GetHabbo().CurrentRoom.Capture != 0)
            {
                PlusEnvironment.GetGame().GetClientManager().sendGangMsg(Session.GetHabbo().CurrentRoom.Capture, Session.GetHabbo().Username + " begins to control [" + Session.GetHabbo().CurrentRoom.Id + "] " + Session.GetHabbo().CurrentRoom.Name + ".");
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}