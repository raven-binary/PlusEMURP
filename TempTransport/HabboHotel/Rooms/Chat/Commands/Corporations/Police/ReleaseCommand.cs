using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;


using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ReleaseCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().RankId < 6 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<release> <username>"; }
        }

        public string Description
        {
            get { return "Releases player from jail"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            string Username = Params[1];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper("Player could not be found");
                return;
            }

            if (!TargetClient.GetRoleplay().Prison)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is not in arrested");
                return;
            }

            User.Say("releases " + TargetClient.GetHabbo().Username + " from prison");
            TargetClient.GetRoleplay().EndPrison();
        }
    }
}