using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Threading;

using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class BombCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "combat"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Uses a Bomb"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().InventorySlot1 == "bomb" || Session.GetHabbo().BombFromSlot == "slot1")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot1");
            }
            else if (Session.GetHabbo().InventorySlot2 == "bomb" || Session.GetHabbo().BombFromSlot == "slot2")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot2");
            }
            else if (Session.GetHabbo().InventorySlot3 == "bomb" || Session.GetHabbo().BombFromSlot == "slot3")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot3");
            }
            else if (Session.GetHabbo().InventorySlot4 == "bomb" || Session.GetHabbo().BombFromSlot == "slot4")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot4");
            }
            else if (Session.GetHabbo().InventorySlot5 == "bomb" || Session.GetHabbo().BombFromSlot == "slot5")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot5");
            }
            else if (Session.GetHabbo().InventorySlot6 == "bomb" || Session.GetHabbo().BombFromSlot == "slot6")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot6");
            }
            else if (Session.GetHabbo().InventorySlot7 == "bomb" || Session.GetHabbo().BombFromSlot == "slot7")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot7");
            }
            else if (Session.GetHabbo().InventorySlot8 == "bomb" || Session.GetHabbo().BombFromSlot == "slot8")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot8");
            }
            else if (Session.GetHabbo().InventorySlot9 == "bomb" || Session.GetHabbo().BombFromSlot == "slot9")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot9");
            }
            else if (Session.GetHabbo().InventorySlot10 == "bomb" || Session.GetHabbo().BombFromSlot == "slot10")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "bomb,slot10");
            }
            else
            {
                Session.SendWhisper("You don't have any bomb");
            }
        }
    }
}