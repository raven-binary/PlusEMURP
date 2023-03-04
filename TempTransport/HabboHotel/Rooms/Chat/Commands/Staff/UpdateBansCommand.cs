using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;

using Plus.Database.Interfaces;


namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class UpdateBansCommand : IChatCommand
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
            get { return "Update ban list."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            PlusEnvironment.GetGame().GetModerationManager().ReCacheBans();
            PlusEnvironment.GetGame().GetClientManager().sendStaffMsg("The ban list has been reloaded.");
            return;
        }
    }
}
