using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DanceCommand :IChatCommand
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
            get { return "Begins dancing based on the dance id"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action while cuffed");
                return;
            }

            if (Params.Length == 1)
            {
                PlusEnvironment.GetGame().GetChatManager().GetCommands().Parse(Session, ":dance 4");
                return;
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("The dance ID must be between 1 and 4");
                    return;
                }

                User.DanceId = DanceId;
                Session.GetHabbo().CurrentRoom.SendMessage(new DanceComposer(User, DanceId));
            }
            else
                Session.SendWhisper("Please enter a valid dance id");
        }
    }
}
