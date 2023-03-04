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
    public class InteractorLockpick : IFurniInteractor
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

            if (Session.GetHabbo().usingSafeRob)
                return;

            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, User.Coordinate.X, User.Coordinate.Y))
            {
                User.MoveToIfCanWalk(Item.SquareInFront);
                return;
            }

            if (Session.GetRoleplay().Passive)
            {
                Session.SendWhisper("You cannot lockpick while in passive mode");
                return;
            }

            if (Session.GetHabbo().RPItems().Lockpicks < 1)
            {
                Session.SendWhisper("You need a lockpick to rob this safe");
                return;
            }

            if (PlusEnvironment.LockpickUsing.ContainsKey(Item.Id))
            {
                return;
            }

            if (PlusEnvironment.Lockpick.ContainsKey(Item.Id))
            {
                Session.SendWhisper("This safe has already been robbed");
                return;
            }

            User.RotHead = 6;
            User.RotBody = 6;
            User.UpdateNeeded = true;
            User.Say("begins to lockpicking the safe");
            User.ApplyEffect(45);
            Session.GetHabbo().usingSafeRob = true;
            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Lockpicking safe" + ";" + 30000 + ";" + 30);
            PlusEnvironment.LockpickUsing.Add(Item.Id, DateTime.Now);
            System.Timers.Timer RobWaitTimer = new System.Timers.Timer(30000);
            RobWaitTimer.Interval = 30000;
            RobWaitTimer.Elapsed += delegate
            {
                if (Session.GetHabbo().usingSafeRob && Session.GetHabbo().PlayToken ==  tokenNumber)
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "hide");

                    Random LockRobRecompense = new Random();
                    int wins = LockRobRecompense.Next(1, 2);

                    Random RMoney = new Random();
                    int money = RMoney.Next(110, 230);
                    if (wins == 1)
                    {
                        if (Session.GetHabbo().usingSafeRob && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            User.Say("takes $" + money + " from the safe");
                            Session.GetHabbo().Credits += money;
                            Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);

                            Session.SendWhisper("You have successfully robbed the safe");
                            Session.GetHabbo().resetEffectEvent();
                            Session.GetHabbo().usingSafeRob = false;
                            Session.GetHabbo().InventoryUpdate2("lockpick", "-", 1, 0);
                            PlusEnvironment.LockpickUsing.Remove(Item.Id);

                            if (Session.GetHabbo().Gang > 0)
                            {
                                Session.GetHabbo().updateGangHeists();
                            }
                            PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"green\">" + Session.GetHabbo().Username + "</span> heisted " + Session.GetHabbo().CurrentRoom.Name);

                            Item.ExtraData = "1";
                            Item.UpdateState(false, true);
                            PlusEnvironment.Lockpick.Add(Item.Id, DateTime.Now);

                            System.Timers.Timer RemoveItemIdTimer = new System.Timers.Timer(900000);
                            RemoveItemIdTimer.Interval = 900000;
                            RemoveItemIdTimer.Elapsed += delegate
                            {
                                PlusEnvironment.Lockpick.Remove(Item.Id);
                                Item.ExtraData = "0";
                                Item.UpdateState(false, true);
                                RemoveItemIdTimer.Stop();
                            };
                            RemoveItemIdTimer.Start();
                        }
                    }
                    else if (wins == 2)
                    {
                        if (Session.GetHabbo().usingSafeRob && Session.GetHabbo().PlayToken == tokenNumber)
                        {
                            User.Say("attempts to take money from the safe, but it's empty");
                            Session.GetHabbo().usingSafeRob = false;
                            Item.ExtraData = "1";
                            Item.UpdateState(false, true);
                            PlusEnvironment.Lockpick.Add(Item.Id, DateTime.Now);

                            System.Timers.Timer RemoveItemIdTimer = new System.Timers.Timer(900000);
                            RemoveItemIdTimer.Interval = 900000;
                            RemoveItemIdTimer.Elapsed += delegate
                            {
                                PlusEnvironment.Lockpick.Remove(Item.Id);
                                Item.ExtraData = "0";
                                Item.UpdateState(false, true);
                                RemoveItemIdTimer.Stop();
                            };
                            RemoveItemIdTimer.Start();
                        }
                    }
                    RobWaitTimer.Stop();
                }
            };
            RobWaitTimer.Start();
        }

        public void OnWiredTrigger(Item Item)
        {
        }
    }
}