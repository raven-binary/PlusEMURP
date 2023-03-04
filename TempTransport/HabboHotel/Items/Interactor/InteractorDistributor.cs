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
    public class InteractorDistributor : IFurniInteractor
    {
        public void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateNeeded = true;

            if (Item.InteractingUser > 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.CanWalk = true;
                }
            }
        }

        public void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser > 0)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Item.InteractingUser);

                if (User != null)
                {
                    User.CanWalk = true;
                }
            }
        }

        public void OnTrigger(GameClient Session, Item Item, int Request, bool HasRights)
        {
            if (Item.ExtraData != "1" && Item.GetBaseItem().VendingIds.Count >= 1 && Item.InteractingUser == 0 && Session != null)
            {
                RoomUser User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
                if (User.IsTrading)
                    return;

                if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
                {
                    User.MoveToIfCanWalk(Item.SquareInFront);
                    return;
                }

                User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);

                if (Session.GetHabbo().getCooldown("manger_distributor"))
                {
                    Session.SendWhisper("Wait a minute...");
                    return;
                }

                if (Session.GetRoleplay().Energy > 99)
                {
                    Session.SendWhisper("Your energy is already full.");
                    return;
                }

                if (Session.GetHabbo().Credits > 50)
                {
                    Session.SendWhisper("You need 50 euros to buy chips!");
                    return;
                }

                Session.GetHabbo().addCooldown("manger_distributor", 2000);
                Item.InteractingUser = Session.GetHabbo().Id;
                User.CanWalk = false;
                User.ClearMovement(true);
                User.SetRot(Rotation.Calculate(User.X, User.Y, Item.GetX, Item.GetY), false);

                Item.RequestUpdate(2, true);

                Item.ExtraData = "1";
                Item.UpdateState(false, true);
                int Energy = 5;
                int NumberEnergy = 100 - Energy;
                Session.GetHabbo().Credits -= 50;
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                User.GetClient().Shout("*Buy a pack of chips [+" + Energy + "% Energy, -50$]*");
                if (Session.GetRoleplay().Energy >= NumberEnergy)
                {
                    Session.GetRoleplay().Energy = 100;
                    Session.GetHabbo().updateEnergy();

                }
                else
                {
                    Session.GetRoleplay().Energy += Energy;
                    Session.GetHabbo().updateEnergy();
                }
            }
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}