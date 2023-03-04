using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Plus;
using Fleck;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.Moderation;

namespace Bobba.HabboRoleplay.Web.Outgoing
{
    class LoginWebEvent : IWebEvent
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
                #region connect
                case "connect":
                    {
                        /*Socket.Send("connected");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "user-stats;" + Client.GetHabbo().Id + ";" + Client.GetHabbo().Username + ";" + Client.GetHabbo().Look + ";" + Client.GetRoleplay().Passive + ";" + Client.GetRoleplay().Health + ";" + Client.GetRoleplay().HealthMax + ";" + Client.GetRoleplay().Energy);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "my_stats;" + Client.GetHabbo().Credits + ";" + Client.GetHabbo().Duckets + ";" + Client.GetHabbo().EventPoints);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "macro", "load");
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Client, "inventory", "load");
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "day-night;" + PlusStaticGameSettings.CurrentMood + ";" + PlusStaticGameSettings.DayNightTimer);*/
                    }
                    break;
                #endregion
                #region Login
                case "connected":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null)
                            return;

                        Client.GetHabbo().ConnectedWebsockets = true; //veryfing that the user is connected to websocket, otherwhise disconnect

                        if (Client.GetHabbo().Rank > 3)
                        {
                            User.IsWalking = false;
                            User.CanWalk = false;
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "secure-login;false;");

                            System.Timers.Timer VerifyTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1));
                            VerifyTimer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(1);
                            VerifyTimer.Elapsed += delegate
                            {
                                if (!Client.GetHabbo().isLoggedIn)
                                {
                                    PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Client.GetHabbo().Username + " kicked out from the client! Reason: Pin time expired");
                                    Task t = Task.Run(async delegate
                                    {
                                        await Task.Delay(1000);
                                        Client.Disconnect();
                                    });
                                }
                                VerifyTimer.Stop();
                            };
                            VerifyTimer.Start();
                        }

                        #region Marketplace Sold Notifactions
                        DateTime now = DateTime.Now;

                        DataRow Marketplace = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Marketplace = dbClient.getRow();
                        }

                        bool Expired = false;
                        for (int i = 1; i <= 10; i++)
                        {
                            if (Convert.ToString(Marketplace["slot" + i]) != "null" && Convert.ToInt32(Marketplace["slot" + i + "_sold"]) == 1)
                            {
                                if (Convert.ToInt32(Marketplace["slot" + i + "_durability"]) > 0)
                                {
                                    Client.SendWhisper("Your " + Convert.ToString(Marketplace["slot" + i]) + " has been sold in the marketplace");
                                }
                                else if (Convert.ToInt32(Marketplace["slot" + i + "_quantity"]) > 0)
                                {
                                    if (Convert.ToInt32(Marketplace["slot" + i + "_quantity"]) > 1)
                                    {
                                        Client.SendWhisper("Your (" + Convert.ToInt32(Marketplace["slot" + i + "_quantity"]) + ") " + Convert.ToString(Marketplace["slot" + i]) + " has been sold in the marketplace");
                                    }
                                    else
                                    {
                                        Client.SendWhisper("Your " + Convert.ToString(Marketplace["slot" + i]) + " has been sold in the marketplace");
                                    }
                                }
                            }
                            else if (Convert.ToString(Marketplace["slot" + i]) != "null" && Convert.ToInt32(Marketplace["slot" + i + "_sold"]) == 0 && Convert.ToDateTime(Marketplace["slot" + i + "_date"]) <= now.AddDays(-1))
                            {
                                Expired = true;
                            }
                        }

                        if (Expired)
                            Client.SendWhisper("Your marketplace offer has expired");
                        #endregion

                        DataRow Bounty = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `bounties` WHERE `user_id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Client.GetHabbo().Id);
                            Bounty = dbClient.getRow();
                        }

                        if (Bounty != null)
                        {
                            Client.GetHabbo().BountyTimer = Convert.ToInt32(Bounty["minutes"]);
                        }

                        if (Client.GetHabbo().JobId == 1)
                        {
                            if (PlusEnvironment.PoliceGeneralCalls >= 1)
                            {
                                for (int i = 1; i <= PlusEnvironment.PoliceGeneralCalls; i++)
                                {
                                    DataRow Call = null;
                                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.SetQuery("SELECT * FROM `police_calls` WHERE `id` = @id LIMIT 1");
                                        dbClient.AddParameter("id", i);
                                        Call = dbClient.getRow();
                                    }

                                    DataRow Users = null;
                                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                                        dbClient.AddParameter("id", Convert.ToInt32(Call["user_id"]));
                                        Users = dbClient.getRow();
                                    }

                                    int Id = Convert.ToInt32(Call["id"]);
                                    string Username = Convert.ToString(Users["username"]);
                                    string Look = Convert.ToString(Users["Look"]);
                                    int RoomId = Convert.ToInt32(Call["room_id"]);
                                    string message = Convert.ToString(Call["message"]);
                                    int Reponded = Convert.ToInt32(Call["responded_cop_id"]);

                                    Room RoomName = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-call;" + Username + ";" + Look + ";[" + RoomId + "] " + RoomName.Name + ";" + RoomId + ";" + message + ";" + Id);

                                    if (Reponded >= 1)
                                    {
                                        DataRow Cop = null;
                                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            dbClient.SetQuery("SELECT * FROM `users` WHERE `id` = @id LIMIT 1");
                                            dbClient.AddParameter("id", Reponded);
                                            Cop = dbClient.getRow();
                                        }

                                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-call-respond;" + Id + ";" + Convert.ToString(Cop["username"]));
                                    }
                                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "police-current-call;" + Id + ";" + PlusEnvironment.PoliceGeneralCalls);
                                }
                            }
                        }

                        string Bubble = "default";
                        if (Client.GetHabbo().CustomBubbleId == 1)
                        {
                            Bubble = "default";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 20)
                        {
                            Bubble = "puppy";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 19)
                        {
                            Bubble = "piglet";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 16)
                        {
                            Bubble = "hearts";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 17)
                        {
                            Bubble = "stars";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 21)
                        {
                            Bubble = "duck";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 24)
                        {
                            Bubble = "iconic";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 27)
                        {
                            Bubble = "storm";
                        }
                        else if (Client.GetHabbo().CustomBubbleId == 12)
                        {
                            Bubble = "galaxy";
                        }
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "chat-bubble;" + Bubble);

                        string Title = "default";
                        if (Client.GetHabbo().PrefixName == "null")
                        {
                            Title = "default";
                        }
                        else if (Client.GetHabbo().PrefixName == "Celebrity")
                        {
                            Title = "celebrity";
                        }
                        else if (Client.GetHabbo().PrefixName == "Princess")
                        {
                            Title = "princess";
                        }
                        else if (Client.GetHabbo().PrefixName == "Prince")
                        {
                            Title = "prince";
                        }
                        else if (Client.GetHabbo().PrefixName == "King")
                        {
                            Title = "king";
                        }
                        else if (Client.GetHabbo().PrefixName == "Queen")
                        {
                            Title = "queen";
                        }
                        else if (Client.GetHabbo().PrefixName == "ƒ")
                        {
                            Title = "heart";
                        }
                        else if (Client.GetHabbo().PrefixName == "†")
                        {
                            Title = "bomb";
                        }
                        else if (Client.GetHabbo().PrefixName == "¥")
                        {
                            Title = "star";
                        }
                        else if (Client.GetHabbo().PrefixName == "º")
                        {
                            Title = "lightning";
                        }
                        else if (Client.GetHabbo().PrefixName == "—")
                        {
                            Title = "music";
                        }
                        else if (Client.GetHabbo().PrefixName == "Collector")
                        {
                            Title = "collector";
                        }
                        else if (Client.GetHabbo().PrefixName == "Boss")
                        {
                            Title = "boss";
                        }
                        else if (Client.GetHabbo().PrefixName == "Don")
                        {
                            Title = "don";
                        }
                        else if (Client.GetHabbo().PrefixName == "LGBTQ")
                        {
                            Title = "lgbtq";
                        }
                        else if (Client.GetHabbo().PrefixName == "Baddie")
                        {
                            Title = "baddie";
                        }
                        else if (Client.GetHabbo().PrefixName == "Imposter")
                        {
                            Title = "imposter";
                        }
                        else if (Client.GetHabbo().PrefixName == "Royalty")
                        {
                            Title = "royalty";
                        }

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "prefix-name;" + Title);

                        int NextLevel;
                        int NextCombatXP;
                        string NextDamage;
                        int NextHealth;
                        if (Client.GetRoleplay().CombatXP >= 75 && Client.GetRoleplay().CombatXP < 150)
                        {
                            NextLevel = 2;
                            NextCombatXP = 150;
                            NextDamage = "1-3";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 1;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 150 && Client.GetRoleplay().CombatXP < 225)
                        {
                            NextLevel = 3;
                            NextCombatXP = 225;
                            NextDamage = "2-3";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 2;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 225 && Client.GetRoleplay().CombatXP < 325)
                        {
                            NextLevel = 4;
                            NextCombatXP = 325;
                            NextDamage = "2-4";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 3;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 325 && Client.GetRoleplay().CombatXP < 425)
                        {
                            NextLevel = 5;
                            NextCombatXP = 425;
                            NextDamage = "3-5";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 4;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 425 && Client.GetRoleplay().CombatXP < 525)
                        {
                            NextLevel = 6;
                            NextCombatXP = 525;
                            NextDamage = "4-5";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 5;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 525 && Client.GetRoleplay().CombatXP < 650)
                        {
                            NextLevel = 7;
                            NextCombatXP = 650;
                            NextDamage = "4-6";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 6;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 650 && Client.GetRoleplay().CombatXP < 775)
                        {
                            NextLevel = 8;
                            NextCombatXP = 775;
                            NextDamage = "5-6";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 7;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 775 && Client.GetRoleplay().CombatXP < 900)
                        {
                            NextLevel = 9;
                            NextCombatXP = 900;
                            NextDamage = "5-7";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 8;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 900 && Client.GetRoleplay().CombatXP < 1050)
                        {
                            NextLevel = 10;
                            NextCombatXP = 1050;
                            NextDamage = "6-7";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 9;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 1050 && Client.GetRoleplay().CombatXP < 1200)
                        {
                            NextLevel = 11;
                            NextCombatXP = 1200;
                            NextDamage = "6-8";
                            NextHealth = 3;
                            Client.GetRoleplay().CombatLevel = 10;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 1200 && Client.GetRoleplay().CombatXP < 1350)
                        {
                            NextLevel = 12;
                            NextCombatXP = 1350;
                            NextDamage = "7-8";
                            NextHealth = 4;
                            Client.GetRoleplay().CombatLevel = 11;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 1350 && Client.GetRoleplay().CombatXP < 1525)
                        {
                            NextLevel = 13;
                            NextCombatXP = 1525;
                            NextDamage = "7-9";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 12;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 1525 && Client.GetRoleplay().CombatXP < 1700)
                        {
                            NextLevel = 14;
                            NextCombatXP = 1700;
                            NextDamage = "8-9";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 13;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 1700 && Client.GetRoleplay().CombatXP < 1875)
                        {
                            NextLevel = 15;
                            NextCombatXP = 1875;
                            NextDamage = "8-10";
                            NextHealth = 3;
                            Client.GetRoleplay().CombatLevel = 14;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 1875 && Client.GetRoleplay().CombatXP < 2075)
                        {
                            NextLevel = 16;
                            NextCombatXP = 2075;
                            NextDamage = "9-10";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 15;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 2075 && Client.GetRoleplay().CombatXP < 2275)
                        {
                            NextLevel = 17;
                            NextCombatXP = 2275;
                            NextDamage = "9-11";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 16;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 2275 && Client.GetRoleplay().CombatXP < 2475)
                        {
                            NextLevel = 18;
                            NextCombatXP = 2475;
                            NextDamage = "10-11";
                            NextHealth = 3;
                            Client.GetRoleplay().CombatLevel = 17;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 2475 && Client.GetRoleplay().CombatXP < 2700)
                        {
                            NextLevel = 19;
                            NextCombatXP = 2700;
                            NextDamage = "10-12";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 18;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 2700 && Client.GetRoleplay().CombatXP < 2925)
                        {
                            NextLevel = 20;
                            NextCombatXP = 2925;
                            NextDamage = "11-12";
                            NextHealth = 4;
                            Client.GetRoleplay().CombatLevel = 19;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 2925 && Client.GetRoleplay().CombatXP < 3150)
                        {
                            NextLevel = 21;
                            NextCombatXP = 3150;
                            NextDamage = "11-13";
                            NextHealth = 1;
                            Client.GetRoleplay().CombatLevel = 20;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 3150 && Client.GetRoleplay().CombatXP < 3400)
                        {
                            NextLevel = 22;
                            NextCombatXP = 3400;
                            NextDamage = "12-13";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 21;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 3400 && Client.GetRoleplay().CombatXP < 3650)
                        {
                            NextLevel = 23;
                            NextCombatXP = 3650;
                            NextDamage = "12-14";
                            NextHealth = 3;
                            Client.GetRoleplay().CombatLevel = 22;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 3650 && Client.GetRoleplay().CombatXP < 3900)
                        {
                            NextLevel = 24;
                            NextCombatXP = 3900;
                            NextDamage = "13-14";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 23;
                        }
                        else if (Client.GetRoleplay().CombatXP >= 3900 && Client.GetRoleplay().CombatXP < 4150)
                        {
                            NextLevel = 25;
                            NextCombatXP = 4150;
                            NextDamage = "14-15";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 24;
                        }
                        else if (Client.GetRoleplay().CombatXP == 4150)
                        {
                            NextLevel = 25;
                            NextCombatXP = 4150;
                            NextDamage = "14-15";
                            NextHealth = 2;
                            Client.GetRoleplay().CombatLevel = 25;
                        }
                        else //level 1?!
                        {
                            NextLevel = 1;
                            NextCombatXP = 75;
                            NextDamage = "1-2";
                            NextHealth = 2;
                        }

                        DataRow GetJob = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("id", Client.GetRoleplay().JobId);
                            GetJob = dbClient.getRow();
                        }

                        DataRow GetJobRank = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `groups_rank` WHERE `job_id` = @job_id AND `rank_id` = @rank_id LIMIT 1");
                            dbClient.AddParameter("job_id", GetJob["id"]);
                            dbClient.AddParameter("rank_id", Client.GetRoleplay().JobRank);
                            GetJobRank = dbClient.getRow();
                        }

                        int NextFarmingXP;
                        int Level;
                        if (Client.GetRoleplay().FarmingXP >= 270 && Client.GetRoleplay().FarmingXP < 480)
                        {
                            Level = 2;
                            NextFarmingXP = 480;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 480 && Client.GetRoleplay().FarmingXP < 690)
                        {
                            Level = 3;
                            NextFarmingXP = 690;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 690 && Client.GetRoleplay().FarmingXP < 900)
                        {
                            Level = 4;
                            NextFarmingXP = 900;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 900 && Client.GetRoleplay().FarmingXP < 1110)
                        {
                            Level = 5;
                            NextFarmingXP = 1110;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 1110 && Client.GetRoleplay().FarmingXP < 1320)
                        {
                            Level = 6;
                            NextFarmingXP = 1320;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 1320 && Client.GetRoleplay().FarmingXP < 1530)
                        {
                            Level = 7;
                            NextFarmingXP = 1530;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 1530 && Client.GetRoleplay().FarmingXP < 1740)
                        {
                            Level = 8;
                            NextFarmingXP = 1740;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 1740 && Client.GetRoleplay().FarmingXP < 1950)
                        {
                            Level = 9;
                            NextFarmingXP = 1950;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 1950 && Client.GetRoleplay().FarmingXP < 2160)
                        {
                            Level = 10;
                            NextFarmingXP = 2160;
                        }
                        else if (Client.GetRoleplay().FarmingXP >= 2160)
                        {
                            Level = 10;
                            NextFarmingXP = 2160;
                        }
                        else
                        {
                            Level = 1;
                            NextFarmingXP = 270;
                        }

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "character;" + Client.GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth + ";" + Level + ";" + Client.GetRoleplay().FarmingXP + ";" + NextFarmingXP);
                    }
                    break;
                #endregion
                #region stats
                case "user-stats":
                {
                    //PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "user-stats;" + Client.GetHabbo().Id + ";" + Client.GetHabbo().Username + ";" + Client.GetHabbo().Look + ";" + PlusEnvironment.BoolToEnum(Client.GetRoleplay().Passive) + ";" + Client.GetRoleplay().Health + ";" + Client.GetRoleplay().HealthMax + ";" + Client.GetRoleplay().Energy);
                }
                break;
                #endregion
                #region Staff Login
                case "staff-login":
                    {
                        Room Room = Client.GetHabbo().CurrentRoom;
                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Client.GetHabbo().Id);
                        if (User == null || User.GetClient().GetHabbo().isLoggedIn)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string thisPin = ReceivedData[1];
                        string pin;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT `pin` FROM users WHERE `id` = " + Client.GetHabbo().Id + " LIMIT 1");
                            dbClient.AddParameter("password", thisPin);
                            pin = dbClient.getString();
                        }
                        if (Convert.ToInt32(pin) == 0)
                        {
                            Task t = Task.Run(async delegate
                            {
                                await Task.Delay(1000);
                                Client.Disconnect();
                            });
                        }
                        else if (pin == thisPin)
                        {
                            Client.GetHabbo().isLoggedIn = true;
                            PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Client.GetHabbo().Username + " has successfully verified themselves");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "secure-login;true;");
                            User.IsWalking = true;
                            User.CanWalk = true;
                        }
                        else if (pin != thisPin)
                        {
                            PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Client.GetHabbo().Username + " kicked out from the client! Reason: Wrong pin");
                            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "secure-login;wrong;");
                            Task t = Task.Run(async delegate
                            {
                                await Task.Delay(1000);
                                Client.Disconnect();
                            });
                        }
                    }
                    break;
                #endregion
                #region Client Ad pop-ups
                case "ad":
                    {
                        int AdSeconds = 30;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ad;show;" + AdSeconds);

                        System.Timers.Timer CloseAdTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1));
                        CloseAdTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(1);
                        CloseAdTimer.Elapsed += delegate
                        {
                            if (AdSeconds > 0)
                            {
                                AdSeconds -= 1;
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ad-timer;" + AdSeconds);
                            }
                            else
                            {
                                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "ad;hide");
                                CloseAdTimer.Stop();
                            }
                        };
                        CloseAdTimer.Start();
                    }
                break;
                #endregion
                #region VPN Detector
                case "user-check": //based on the browsertimezone and ip timezone
                    {
                        DataRow ServerStatus = null;
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("SELECT * FROM `server_status`;");
                            ServerStatus = dbClient.getRow();
                        }

                        if (Convert.ToInt32(ServerStatus["vpn_detect"]) == 0)
                            return;

                        string[] ReceivedData = Data.Split(',');
                        string Timezone = ReceivedData[1];

                        System.Net.WebClient wc = new System.Net.WebClient();

                        var json = wc.DownloadString("https://ipinfo.io/" + Client.GetHabbo().LastIp + "/json"); //to get the details from the user
                        dynamic myClass = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                        if (Timezone != Convert.ToString(myClass.timezone))
                        {
                            DataRow Row = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `vpn_whitelist` WHERE `ip` = @ip LIMIT 1;");
                                dbClient.AddParameter("ip", Client.GetHabbo().LastIp);
                                Row = dbClient.getRow();
                            }

                            if (Row == null)
                            {
                                Client.SendMessage(new BroadcastMessageAlertComposer("<font color = '#B40404'><font size= '14'><b>Houston, we have a problem</b></font></font>\n\nUnfortunately, we do not allow VPN or Proxy connections to the game as standard! Due to this, you will need to submit a ticket to become whitelisted."));
                                PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Client.GetHabbo().Username + " banned from the client! Reason: Using VPN/Proxy");

                                Task t = Task.Run(async delegate //let's disconnect the user from the client
                                {
                                    await Task.Delay(2000);
                                    PlusEnvironment.GetGame().GetModerationManager().BanUser("System", ModerationBanType.USERNAME, Client.GetHabbo().Username, "Using VPN/Proxy", 1746018565.40453);

                                    if (Client != null)
                                        Client.Disconnect();
                                });
                            }
                        }
                    }
                break;
                #endregion
            }
        }
    }
}
