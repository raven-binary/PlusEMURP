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
    class SuperHireCommand : IChatCommand
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
            get { return "Super hires a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid Syntax :superhire <username> <job id>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room.");
                return;
            }

            if (TargetClient.GetRoleplay().JobId == 0)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " already has a job.");
                return;
            }

            int Amount;
            string TravailId = Params[2];
           
            if (!int.TryParse(TravailId, out Amount) || Convert.ToInt32(Params[2]) <= 0 || TravailId.StartsWith("0"))
            {
                Session.SendWhisper("Job ID is invalid.");
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Convert.ToInt32(TravailId), out Group))
            {
                Session.SendWhisper("Job ID is invalid.");
                return;
            }

            GroupRank NewRank = null;
            PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(Convert.ToInt32(TravailId), Convert.ToInt32(Params[3]), out NewRank);
            if (NewRank == null)
            {
                Session.SendWhisper("An error has occurred.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Group.AddMemberByForce(TargetClient.GetHabbo().Id);
            TargetClient.GetHabbo().setFavoriteGroup(Group.Id);
            Group.updateRankid(TargetClient.GetHabbo().Id, Convert.ToInt32(Params[3]));
            Group.ChangeTier(TargetClient.GetHabbo().Id, Convert.ToInt32(Params[3]));
            User.Say("super hires " + TargetClient.GetHabbo().Username + " to " + Group.Name + " as " + NewRank.Name);
        }
    }
}