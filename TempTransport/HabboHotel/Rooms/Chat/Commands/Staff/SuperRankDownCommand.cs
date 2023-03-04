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
    class SuperRankDownCommand : IChatCommand
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
            get { return "Demote a user"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :superrankown <username>", 1);
                return;
            }

            if (Session.GetHabbo().getCooldown("superrankdown_commmand") == true)
            {
                Session.SendWhisper("Wait a minute...", 1);
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.", 1);
                return;
            }

            if (TargetClient.GetHabbo().JobId == 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " has no job.", 1);
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(TargetClient.GetHabbo().JobId, out Group))
            {
                Session.SendWhisper("An error has occurred.", 1);
                return;
            }

            GroupRank NewRank = null;
            PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(Convert.ToInt32(TargetClient.GetHabbo().JobId), TargetClient.GetHabbo().RankId + 1, out NewRank);
            if (NewRank == null)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has the lowest rank.", 1);
                return;
            }

            if (TargetClient.GetHabbo().RankInfo.Rank == 2)
            {
                Group.removeAdmin(TargetClient.GetHabbo().Id);
            }
            Session.GetHabbo().addCooldown("superrankdown_command", 2000);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Group.PromoteRank(TargetClient.GetHabbo().Id);
            
            User.GetClient().Shout("*Demotes " + TargetClient.GetHabbo().Username + " to " + NewRank.Name + " at " + Group.Name + "*");
        }
    }
}