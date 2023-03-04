using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class LoyerCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Rent an apartment"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Room.Loyer == 1 && Room.OwnerId != 3)
            {
                Session.SendWhisper("This apartment is already for rent.");
                return;
            }

            if (Room.getNameByModel() == "Appartement inconnu")
            {
                Session.SendWhisper("Impossible to put this apartment on sale because the model is not defined.");
                return;
            }

            if (Room.getPriceByModel() == 0)
            {
                Session.SendWhisper("Impossible to put this apartment on sale because the price is not defined.");
                return;
            }

            Room.setLocationRoom(Room.getPriceByModel());
        }
    }
}