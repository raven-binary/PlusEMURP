using System;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Pathfinding;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorCreation : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
        }

        public void OnRemove(GameClient Session, Item Item)
        {
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Session.GetHabbo() == null)
                return;

            RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

            bool CanMakeCreation = false;
            if (Session.GetHabbo().TravailInfo.Usine == 1 && Session.GetHabbo().Working == true)
                CanMakeCreation = true;

            if (!CanMakeCreation)
                return;

            if (Session.GetHabbo().getCooldown("make_creation"))
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            if (User.makeCreation == true)
            {
                Session.SendWhisper("You already do " + User.CreationName + ".");
                return;
            }

            if (Session.GetHabbo().JobId == 19)
            {
                Session.GetHabbo().addCooldown("make_creation", 2000);
                User.makeCreation = true;
                Random Creation = new Random();
                int CreationItem = Creation.Next(1, 3);
                string Name;
                if(CreationItem == 2)
                {
                    Name = "a soap";
                }
                else
                {
                    Name = "a Doliprane";
                }
                User.creationTimer = 15;
                User.CreationName = Name;
                User.OnChat(User.LastBubble, "* Begins to realize " + Name+ " *", true);
                return;
            }
            else if (Session.GetHabbo().JobId == 20)
            {
                Session.GetHabbo().addCooldown("make_creation", 2000);
                User.makeCreation = true;
                Random Creation = new Random();
                int CreationItem = Creation.Next(1, 7);
                string Name;
                if (CreationItem == 2)
                {
                    Name = "an Ak47";
                }
                else if (CreationItem == 3)
                {
                    Name = "an Uzi";
                }
                else if (CreationItem == 4)
                {
                    Name = "a saber";
                }
                else if (CreationItem == 5)
                {
                    Name = "a bat";
                }
                else if (CreationItem == 6)
                {
                    Name = "Ammo(s) for Uzi";
                }
                else
                {
                    Name = "Ammo(s) for AK47";
                }
                User.creationTimer = 15;
                User.CreationName = Name;
                User.GetClient().Shout("*Begins to realize " + Name + " *");
                return;
            }
            else
                return;
            
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}