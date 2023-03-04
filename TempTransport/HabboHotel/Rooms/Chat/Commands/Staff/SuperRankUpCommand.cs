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

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SuperRankUpCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 4 || Session.GetHabbo().Rank == 5 || Session.GetHabbo().Rank == 6 || Session.GetHabbo().Rank == 7 || Session.GetHabbo().Rank == 8)
            {
                return true;
            }

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Super ranks a user"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :superrankup <username>", 1);
                return;
            }

            if (Session.GetHabbo().getCooldown("superrankup_command") == true)
            {
                Session.SendWhisper("Wait a minute...", 1);
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.");
                return;
            }

            if (TargetClient.GetHabbo().JobId == 8)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " has no job.");
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(TargetClient.GetHabbo().JobId, out Group))
            {
                Session.SendWhisper("An error has occurred.");
                return;
            }

            GroupRank NewRank = null;
            PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(Convert.ToInt32(TargetClient.GetHabbo().JobId), TargetClient.GetHabbo().RankId - 1, out NewRank);
            if (NewRank == null)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has the highest rank.");
                return;
            }

            Session.GetHabbo().addCooldown("superrankup_command", 2000);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Group.updateRank(TargetClient.GetHabbo().Id);
            if (NewRank.Rank == 1 || NewRank.Rank == 2)
            {
                Group.MakeAdmin(TargetClient.GetHabbo().Id);
            }

            User.GetClient().Shout("*Promotes " + TargetClient.GetHabbo().Username + " to " + NewRank.Name + " at " + Group.Name + "*");
        }
    }
}