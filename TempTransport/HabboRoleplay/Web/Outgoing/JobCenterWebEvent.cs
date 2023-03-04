using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;
using System.Threading;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GroupsRank;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class JobCenterWebEvent : IWebEvent
    {
        /// <summary>
        /// Executes socket data.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Data"></param>
        /// <param name="Socket"></param>
        public void Execute(GameClient Client, string Data, IWebSocketConnection Socket)
        {

            if (!PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Client, true) || !PlusEnvironment.GetGame().GetWebEventManager().SocketReady(Socket))
                return;

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
            if (User == null)
                return;

            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {
                #region Accept
                case "accept":
                    {
                        if (PlusEnvironment.getCooldown("jobcenter" + Client.GetHabbo().Id))
                        {
                            Client.SendWhisper("You must wait an hour before you can get another job");
                            return;
                        }

                        if (Client.GetHabbo().JobId != 8)
                        {
                            Client.SendWhisper("You already have a job, quit your job and try again");
                            return;
                        }

                        string[] ReceivedData = Data.Split(',');
                        int Id = Convert.ToInt32(ReceivedData[1]);

                        DataRow JobCenter = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Id);
                            JobCenter = dbClient.getRow();
                        }

                        if (Convert.ToInt32(JobCenter["job_center"]) != 1)
                        {
                            Client.SendWhisper("This corporation does not accept new hires");
                            return;
                        }

                        Group Group = null;
                        if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Id, out Group))
                        {
                            Client.SendWhisper("This corporation id is invaild");
                            return;
                        }

                        GroupRank NewRank = null;
                        PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(Id, 7, out NewRank);
                        if (NewRank == null)
                        {
                            Client.SendWhisper("An error has occurred");
                            return;
                        }

                        Group.AddMemberByForce(Client.GetHabbo().Id);
                        Client.GetHabbo().setFavoriteGroup(Group.Id);

                        //Client.GetHabbo().addCooldown("jobcenter", Convert.ToInt32(PlusEnvironment.ConvertHoursToMilliseconds(1)));
                        PlusEnvironment.addCooldown("jobcenter" + Client.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertMinutesToMilliseconds(2)));
                        User.Say("accepts the job at " + Group.Name + " as " + NewRank.Name);
                    }
                    break;
                #endregion
            }
        }
    }
}