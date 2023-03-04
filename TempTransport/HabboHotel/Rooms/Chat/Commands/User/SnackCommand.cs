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
    class SnackCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Uses a Snack from your inventory"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().InventorySlot1 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot1");
            }
            else if (Session.GetHabbo().InventorySlot2 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot2");
            }
            else if (Session.GetHabbo().InventorySlot3 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot3");
            }
            else if (Session.GetHabbo().InventorySlot4 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot4");
            }
            else if (Session.GetHabbo().InventorySlot5 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot5");
            }
            else if (Session.GetHabbo().InventorySlot6 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot6");
            }
            else if (Session.GetHabbo().InventorySlot7 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot7");
            }
            else if (Session.GetHabbo().InventorySlot8 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot8");
            }
            else if (Session.GetHabbo().InventorySlot9 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot9");
            }
            else if (Session.GetHabbo().InventorySlot10 == "snack")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "snack,slot10");
            }
            else
            {
                Session.SendWhisper("You don't have any snack");
            }
        }
    }
}