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
    class ThrowingKnifeCommand : IChatCommand
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
            get { return "Throws a knive"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().InventorySlot1 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot1");
            }
            else if (Session.GetHabbo().InventorySlot2 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot2");
            }
            else if (Session.GetHabbo().InventorySlot3 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot3");
            }
            else if (Session.GetHabbo().InventorySlot4 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot4");
            }
            else if (Session.GetHabbo().InventorySlot5 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot5");
            }
            else if (Session.GetHabbo().InventorySlot6 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot6");
            }
            else if (Session.GetHabbo().InventorySlot7 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot7");
            }
            else if (Session.GetHabbo().InventorySlot8 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot8");
            }
            else if (Session.GetHabbo().InventorySlot9 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot9");
            }
            else if (Session.GetHabbo().InventorySlot10 == "throwingknife")
            {
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "item", "throwingknife,slot10");
            }
            else
            {
                Session.SendWhisper("You don't have any throwing knive");
            }
        }
    }
}