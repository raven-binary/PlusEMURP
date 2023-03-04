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
    class QuitjobCommand : IChatCommand
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
            get { return "Quits your current job"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            string Username = Session.GetHabbo().Username;
            int idUser;
            int groupId;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id` FROM `users` WHERE `username` = @username LIMIT 1");
                dbClient.AddParameter("username", Username);
                idUser = dbClient.getInteger();

                dbClient.SetQuery("SELECT `group_id` FROM `group_memberships` WHERE `user_id` = @userId LIMIT 1");
                dbClient.AddParameter("userId", idUser);
                groupId = dbClient.getInteger();
            }

            if (Session.GetHabbo().JobId == 8)
            {
                Session.SendWhisper("You don't have any job to quit");
                return;
            }


            Group Group = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(8, out Group))
            {
                Session.SendWhisper("The corp id is invaild");
                return;
            }

            if (Session.GetHabbo().Working)
                Session.GetHabbo().stopWork();

            User.Say("quits their job");
            Session.GetHabbo().setFavoriteGroup(Group.Id);
            Group.QuitJob(Session.GetHabbo().Id);
        }
    }
}