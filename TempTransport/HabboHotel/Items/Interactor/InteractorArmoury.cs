using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorArmoury : IFurniInteractor
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

            if (Session.GetHabbo().JobId != 4 || !Session.GetHabbo().Working)
                return;

            if (Session.GetHabbo().usingMenuSell)
                return;

            Session.GetHabbo().usingMenuSell = true;
            if (Session.GetHabbo().RankId == 6 || Session.GetHabbo().RankId == 5 || Session.GetHabbo().RankId == 4 || Session.GetHabbo().RankId == 3 || Session.GetHabbo().RankId == 2 || Session.GetHabbo().RankId == 1)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;bat;Bat");
            }
            
            if (Session.GetHabbo().RankId == 5 || Session.GetHabbo().RankId == 4 || Session.GetHabbo().RankId == 3 || Session.GetHabbo().RankId == 2 || Session.GetHabbo().RankId == 1)
            {
                
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;axe;Axe");
            }
            if (Session.GetHabbo().RankId == 4 || Session.GetHabbo().RankId == 3 || Session.GetHabbo().RankId == 2 || Session.GetHabbo().RankId == 1)
            {

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;sword;Sword");
            }

            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;vest;Body Armour");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;Throwing Knife;Throwing Knife");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;create;lockpick;Lockpick");
        
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "button-menu;show");
           
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}