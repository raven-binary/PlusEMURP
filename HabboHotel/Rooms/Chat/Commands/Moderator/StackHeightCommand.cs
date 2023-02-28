using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Inventory.Furni;
using System.Globalization;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class StackHeightCommand : IChatCommand
    {
        public string PermissionRequired
        {
            get
            {
                return "command_setsh";
            }
        }
        public string Parameters
        {
            get
            {
                return "%message%";
            }
        }
        public string Description
        {
            get
            {
                return "Set the Stack Height.";
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {

            if (!Room.CheckRights(Session, false, false))
            {
                Session.SendNotification("You don't have permission to use this command.");
                return;
            }

            RoomUser user = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (user == null)
            {
                Session.SendNotification("User not found.");
                return;
            }


            if (Params.Length < 2)
            {
                Session.SendWhisper("Please enter a numeric value or type ':setsh -' to turn it off");
                return;
            }

            if (Params[1] == "-")
            {
                Session.SendWhisper("Stack Height Disabled");
                Session.GetHabbo().ForceHeight = -1;
                return;
            }

            double value;
            bool checkIfParsable = Double.TryParse(Params[1], out value);
            if (checkIfParsable == false)
            {
                Session.SendWhisper("Please enter a numeric value or type ':setsh -' to turn it off");
                return;
            }


            double HeightValue = Convert.ToDouble(Params[1]);
            if (HeightValue < 0 || HeightValue > 100)
            {
                Session.SendWhisper("Please enter a value between 0 and 100");
                return;
            }

            Session.GetHabbo().ForceHeight = HeightValue;
            Session.SendWhisper("Stack Height Is: " + Convert.ToString(HeightValue));
        }
    }
}