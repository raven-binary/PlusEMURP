using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using System.Text.RegularExpressions;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using System.Data;
using System.Threading;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class ProfileWebEvent : IWebEvent
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

            string Action = (Data.Contains(',') ? Data.Split(',')[0] : Data);

            switch (Action)
            {
                #region Profile
                case "open":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Profile = ReceivedData[1];
                        bool isBot = Convert.ToBoolean(ReceivedData[2]);
                        if (isBot)
                            return;

                        int ProfileId;
                        ProfileId = Convert.ToInt32(Profile);

                        DataRow GetProfile = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", ProfileId);
                            GetProfile = dbClient.getRow();
                        }

                        if (GetProfile == null)
                        {
                            Client.SendWhisper("This user profile cannot be open");
                            return;
                        }

                        if (Client.GetHabbo().Id != Convert.ToInt32(GetProfile["id"]) && Convert.ToInt32(GetProfile["hide_profile"]) == 1 && Client.GetHabbo().Rank < 4)
                        {
                            Client.SendWhisper("This user profile has been set to private");
                            return;
                        }

                        int Online = 0;
                        if (Convert.ToInt32(GetProfile["online"]) == 1 && Convert.ToInt32(GetProfile["hide_online"]) == 0)
                        {
                            Online = 1;
                        }

                        DataRow RPStats = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `user_rp_stats` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", ProfileId);
                            RPStats = dbClient.getRow();
                        }

                        DataRow UserJob = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `group_memberships` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", ProfileId);
                            UserJob = dbClient.getRow();
                        }

                        string Tier = Convert.ToString(UserJob["tier"]);
                        if (Tier == "1")
                            Tier = "I";
                        else if (Tier == "2")
                            Tier = "II";
                        else if (Tier == "3")
                            Tier = "III";
                        else if (Tier == "4")
                            Tier = "IV";
                        else if (Tier == "5")
                            Tier = "V";
                        else
                            Tier = "?";

                        DataRow GetJob = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", UserJob["group_id"]);
                            GetJob = dbClient.getRow();
                        }

                        DataRow GetJobRank = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `groups_rank` WHERE `job_id` = @job_id AND `rank_id` = @rank_id LIMIT 1");
                            dbClient.AddParameter("job_id", UserJob["group_id"]);
                            dbClient.AddParameter("rank_id", UserJob["rank_id"]);
                            GetJobRank = dbClient.getRow();
                        }

                        GameClient ProfileUser = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Convert.ToString(GetProfile["username"]));
                        bool OnDuty = false;
                        if (ProfileUser != null && ProfileUser.GetHabbo().Working)
                        {
                            OnDuty = true;
                        }

                        string MarriedTo = null;
                        DataRow MarriedToUser = null;
                        if (Convert.ToInt32(RPStats["married_to"]) != 0)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("id", Convert.ToInt32(RPStats["married_to"]));
                                MarriedToUser = dbClient.getRow();
                                MarriedTo = Convert.ToString(MarriedToUser["username"]);
                            }
                        }
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "user-profile;" + GetProfile["id"] + ";" + GetProfile["username"] + ";" + GetProfile["rank"] + ";" + GetProfile["look"] + ";" + GetProfile["hide_profile"] + ";" + RPStats["bio"] + ";" + Online + ";" + PlusEnvironment.TimeElapsed(PlusEnvironment.UnixTimeStampToDateTime(Convert.ToInt64(GetProfile["last_online"]))) + ";" + RPStats["combat_level"] + ";" + RPStats["farming_level"] + ";" + UserJob["group_id"] + ";" + GetJob["badge"] + ";" + GetJob["name"] + ";" + GetJobRank["name"] + " " + Tier + ";" + OnDuty + ";" + RPStats["corp_shifts"] + ";" + RPStats["weekly_shifts"] + ";" + RPStats["married_to"] + ";" + MarriedTo + ";" + RPStats["kills"] + ";" + RPStats["deaths"] + ";" + RPStats["arrests"] + ";" +  RPStats["assists"] + ";" + RPStats["damage_dealt"] + ";" + RPStats["damage_taken"] + ";" + RPStats["shifts"] + ";" + RPStats["sales"] + ";" + RPStats["corp_tasks"]);
                    }
                    break;
                #endregion
                #region Edit Bio
                case "bio":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        if (Room == null)
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Bio = ReceivedData[1];

                        Client.GetRoleplay().Bio = Bio;
                        Client.GetRoleplay().UpdateValue("bio" , Bio);
                    }
                break;
                #endregion
            }
        }
    }
}