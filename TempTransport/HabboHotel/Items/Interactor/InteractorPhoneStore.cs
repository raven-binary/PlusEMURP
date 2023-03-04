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
    public class InteractorPhoneStore : IFurniInteractor
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

            User.usingPhoneStore = true;
            //User.SetRot(Pathfinding.Rotation.Calculate(User.Coordinate.X, User.Coordinate.Y, Item.GetX, Item.GetY), false);
            User.GetClient().Shout("*Looks at cell phones*");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "phonestore;open;");
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}