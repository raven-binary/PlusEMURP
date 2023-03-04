using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class InventorySlotCommand : IChatCommand
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
            get { return "Selects the item in the corresponding slot"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Please specific a slot");
                return;
            }

            int Slot;
            string Number = Params[1];
            if (!int.TryParse(Number, out Slot) || Convert.ToInt32(Params[1]) <= 0 || Convert.ToInt32(Params[1]) > 10|| Number.StartsWith("0"))
            {
                Session.SendWhisper("Please specific a inventory slot (between 1 and 10)");
                return;
            }

            Session.GetRoleplay().SendExecuteWeb("inventory", "click,slot" + Slot);
        }
    }
}
