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
    class VirerCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank != 8)
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
            get { return "Super removes a user from a job"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :removejob <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("virer_command") == true)
            {
                Session.SendWhisper("Wait a minute...");
                return;
            }

            string Username = Params[1];
            int idUser;
            int groupId;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @username LIMIT 1");
                dbClient.AddParameter("username", Username);
                idUser = dbClient.getInteger();

                if (idUser == 0)
                {
                    Session.SendWhisper("The citizen could not be found.");
                    return;
                }

                dbClient.SetQuery("SELECT `group_id` FROM `group_memberships` WHERE `user_id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", idUser);
                groupId = dbClient.getInteger();
            }

            if (groupId == 1)
            {
                Session.SendWhisper(Username + " hat kein job.");
                return;
            }

            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group))
            {
                Session.SendWhisper("An error has occurred.");
                return;
            }

            Session.GetHabbo().addCooldown("virer_command", 2000);
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Group.sendToChomage(idUser);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient != null)
            {
                TargetClient.SendMessage(new GroupInfoComposer(Group, Session));
                TargetClient.GetHabbo().setFavoriteGroup(1);

                if (TargetClient.GetHabbo().Working)
                {
                    TargetClient.GetHabbo().Working = false;
                    TargetClient.GetHabbo().resetAvatarEvent();
                    TargetClient.SendWhisper(Session.GetHabbo().Username + " fired you from your job.");
                }
            }

            Group GroupBase = null;
            PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(1, out GroupBase);
            GroupBase.AddMemberList(idUser);
            User.GetClient().Shout("*Quit " + Username + " from their job*");
        }
    }
}