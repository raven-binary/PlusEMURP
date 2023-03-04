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
    public class InteractorFarm : IFurniInteractor
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

            User.CanWalk = false;
            User.CanWalk = true;

            if (Session.GetRoleplay().Energy < 1)
            {
                Session.SendWhisper("You need at least 2 energy to farm");
                return;
            }

            if (Session.GetHabbo().getCooldown("farming"))
            {
                Session.SendWhisper("You must wait before you can farm again");
                return;
            }

            int MainTime = 30;
            for (int i = 0; i < Session.GetRoleplay().FarmingLevel; i++)
            {
                MainTime -= 1;
            }

            if (Item.GetBaseItem().SpriteId == 246839)
            {
                Random CreateToken = new Random();
                int PlayToken = CreateToken.Next(1600, 2894354);
                Session.GetHabbo().PlayToken = PlayToken;
                Session.GetHabbo().UsingFarm = "coffee beans";

                User.Say("starts farming the coffee plant");
                User.ApplyEffect(4);

                System.Timers.Timer FarmingTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(MainTime));
                FarmingTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(MainTime);
                FarmingTimer.Elapsed += delegate
                {
                    if (Session.GetHabbo().PlayToken == PlayToken && Session.GetHabbo().UsingFarm == Session.GetHabbo().UsingFarm)
                    {
                        User.Say("farms 1 Coffee Bean");
                        Session.GetHabbo().AddToInventory2("coffee_bean", 1);
                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().FarmingXP();
                        Session.GetRoleplay().Energy -= 1;
                        Session.GetHabbo().RPCache(1);
                        Session.GetHabbo().PlayToken = 0;
                        Session.GetHabbo().UsingFarm = "";
                        Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    }
                    FarmingTimer.Stop();
                };
                FarmingTimer.Start();
            }

            if (Item.GetBaseItem().SpriteId == 3407)
            {
                Random CreateToken = new Random();
                int PlayToken = CreateToken.Next(1600, 2894354);
                Session.GetHabbo().PlayToken = PlayToken;
                Session.GetHabbo().UsingFarm = "mending weed";

                User.Say("starts farming the weed plant");
                User.ApplyEffect(4);

                System.Timers.Timer FarmingTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(MainTime));
                FarmingTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(MainTime);
                FarmingTimer.Elapsed += delegate
                {
                    if (Session.GetHabbo().PlayToken == PlayToken && Session.GetHabbo().UsingFarm == Session.GetHabbo().UsingFarm)
                    {
                        User.Say("farms 1 Mending Weed");
                        Session.GetHabbo().AddToInventory2("healingcrop", 1);
                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().FarmingXP();
                        Session.GetRoleplay().Energy -= 1;
                        Session.GetHabbo().RPCache(1);
                        Session.GetHabbo().PlayToken = 0;
                        Session.GetHabbo().UsingFarm = "";
                        Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    }
                    FarmingTimer.Stop();
                };
                FarmingTimer.Start();
            }

            if (Item.GetBaseItem().SpriteId == 808200152)
            {
                Random CreateToken = new Random();
                int PlayToken = CreateToken.Next(1600, 2894354);
                Session.GetHabbo().PlayToken = PlayToken;
                Session.GetHabbo().UsingFarm = "wool";

                User.Say("starts farming the wool plant");
                User.ApplyEffect(4);

                System.Timers.Timer FarmingTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(MainTime));
                FarmingTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(MainTime);
                FarmingTimer.Elapsed += delegate
                {
                    if (Session.GetHabbo().PlayToken == PlayToken && Session.GetHabbo().UsingFarm == Session.GetHabbo().UsingFarm)
                    {
                        User.Say("farms 1 Wool");
                        Session.GetHabbo().AddToInventory2("wool", 1);
                        Session.GetHabbo().resetEffectEvent();
                        Session.GetHabbo().FarmingXP();
                        Session.GetRoleplay().Energy -= 1;
                        Session.GetHabbo().RPCache(1);
                        Session.GetHabbo().PlayToken = 0;
                        Session.GetHabbo().UsingFarm = "";
                        Session.GetHabbo().CurrentRoom.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    }
                    FarmingTimer.Stop();
                };
                FarmingTimer.Start();
            }
            Session.GetHabbo().addCooldown("farming", 5000);
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}