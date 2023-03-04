using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ExitCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session != null && Session.GetHabbo().CurrentRoomId == 56 && Session.GetHabbo().footballTeam != null)
                return true;

            return false;
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
            get { return "stop playing."; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Session.GetHabbo().footballTeam = null;
            Session.GetHabbo().footballSpawnChair();
            User.GetClient().Shout("*Stop playing*");
        }
    }
}