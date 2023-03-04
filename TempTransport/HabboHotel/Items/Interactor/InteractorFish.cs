using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Moderation;
using Plus.HabboHotel.GameClients;
using System.Collections;
using System.Collections.Concurrent;
using System.Drawing;

using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Core;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Quests;
using Plus.HabboHotel.Rooms.Games;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.Users.Inventory;
using Plus.Communication.Packets.Incoming;

using Plus.Utilities;

using System.Data;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.Communication.Packets.Outgoing.Handshake;
using System.Text.RegularExpressions;
using Plus.HabboHotel.Rooms.Games.Teams;
using System.Threading.Tasks;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.Pathfinding;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Items.Interactor
{
    public class InteractorFish : IFurniInteractor
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
            if (User.usingFish)
                return;

            if (Session.GetHabbo().Angelrute == 0)
            {
                Session.SendWhisper("You don't have a fishing rod.");
                return;
            }

            if (Session.GetRoleplay().Energy < 15)
            {
                Session.SendWhisper("Your energy is very low and you cannot fish.");
                return;
            }

            if (Session.GetHabbo().getCooldown("open_fish"))
            {
                Session.SendWhisper("Warte kurz...");
                return;
            }

            if (User.isTradingItems)
            {
                Session.SendWhisper("You cannot fish while trading.");
                return;
            }

            if (User.IsAsleep)
            {
                Session.SendWhisper("You cannot fish while afk.");
                return;
            }

            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y) && Item.GetBaseItem().SpriteId != 6116)
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            Session.GetHabbo().addCooldown("open_fish", 5000);
            User.usingFish = true;
            User.ApplyEffect(593);
            Session.Shout("*Starts fishing*");
            //User.farmingTimer = 1;

        }
        public void OnWiredTrigger(Item Item)
        {
        }
    }
}