using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GroupsRank;
using Plus.Communication.Packets.Outgoing.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ShiftsCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().RankId == 1 | Session.GetHabbo().RankId == 2 | Session.GetHabbo().RankId == 3 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "manager"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Shows the current targeted players shifts"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :shifts <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("shifts_command") == true)
            {
                Session.SendWhisper("You must wait before you can use shifts command again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            if (TargetClient.GetHabbo().JobId != Session.GetHabbo().JobId)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            Session.GetHabbo().addCooldown("shifts_command", 1000);
            Session.SendWhisper(TargetClient.GetHabbo().RankInfo.Name + ", Total Shifts: " + TargetClient.GetRoleplay().CorpShifts + ", Weekly Shifts: 0");
        }
    }
}