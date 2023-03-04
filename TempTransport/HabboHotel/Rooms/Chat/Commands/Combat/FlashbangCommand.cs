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
    class FlashbangCommand : IChatCommand
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
            get { return "Uses a Flashbang"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().InventorySlot1 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot1");
            }
            else if (Session.GetHabbo().InventorySlot2 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot2");
            }
            else if (Session.GetHabbo().InventorySlot3 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot3");
            }
            else if (Session.GetHabbo().InventorySlot4 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot4");
            }
            else if (Session.GetHabbo().InventorySlot5 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot5");
            }
            else if (Session.GetHabbo().InventorySlot6 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot6");
            }
            else if (Session.GetHabbo().InventorySlot7 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot7");
            }
            else if (Session.GetHabbo().InventorySlot8 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot8");
            }
            else if (Session.GetHabbo().InventorySlot9 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot9");
            }
            else if (Session.GetHabbo().InventorySlot10 == "flashbang")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "flashbang,slot10");
            }
            else
            {
                Session.SendWhisper("You don't have any flashbang");
            }
        }
    }
}