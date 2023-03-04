using System;
using System.Collections.Generic;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorAppleEnergy : IFurniInteractor
    {
        private Dictionary<int, int> AppleItemId = new Dictionary<int, int>();
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

            if (Session.GetHabbo().IsUsingAppleEnergy)
                return;

            if (AppleItemId.ContainsKey(Item.Id))
            {
                Session.SendWhisper("Someone is already eating this apple, choose another one");
                return;
            }

            AppleItemId.Add(Item.Id, Item.Id);

            User.Say("starts eating the apple");
            User.ApplyEffect(4);
            Session.GetHabbo().IsUsingAppleEnergy = true;

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;

            System.Timers.Timer AppleEnergyTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(20));
            AppleEnergyTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(20);
            AppleEnergyTimer.Elapsed += delegate
            {
                if (!Session.GetHabbo().IsUsingAppleEnergy && Session.GetHabbo().PlayToken != tokenNumber)
                    return;

                AppleItemId.Remove(Item.Id);
                Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                User.Say("eats the apple, restoring 15 energy");
                Session.GetRoleplay().Energy += 15;
                Session.GetHabbo().RPCache(1);
                Session.GetHabbo().resetEffectEvent();
                Session.GetHabbo().IsUsingAppleEnergy = false;
                Session.GetHabbo().PlayToken = 0;
                AppleEnergyTimer.Stop();
            };
            AppleEnergyTimer.Start();
        }
        public void OnWiredTrigger(Item Item)
        {
        }
    }
}