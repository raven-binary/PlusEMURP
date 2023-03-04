using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Collections.Generic;
using Plus.HabboHotel.Items;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class EmptyItemsCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "appart"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Empty your inventory"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendNotification("Are you sure you want to completely empty your inventory? Type \":emptyitems yes\" to confirm.");
                return;
            }
            else
            {
                if (Params.Length == 2 && Params[1].ToString() == "yes")
                {
                    Session.GetHabbo().GetInventoryComponent().ClearItems();
                    Session.SendWhisper("Your inventory has been emptied");
                    return;
                }
                else if (Params.Length == 2 && Params[1].ToString() != "yes")
                {
                    Session.SendWhisper("To clear your inventory :emptyitems yes");
                    return;
                }
            }
        }
    }
}