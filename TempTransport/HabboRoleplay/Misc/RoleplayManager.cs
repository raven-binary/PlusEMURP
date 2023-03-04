using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboRoleplay.Misc;
using Plus;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Pathfinding;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using log4net;
using Plus.HabboHotel.Items;

namespace Plus.HabboRoleplay.Misc
{
    public static class RoleplayManager
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboRoleplay.Misc");

        public static Dictionary<string, int> ActiveCooldowns = new Dictionary<string, int>();

        public static bool CourtUsing = false;
        public static bool CourtStarting = false;
        public static bool CourtStarted = false;
        public static bool CourtMembersTeleport = false;
        public static bool CourtVoteStarted = false;
        public static int CourtGuiltyVotes = 0;
        public static int CourtInnocentVotes = 0;
        public static bool CourtEnded = false;
        public static int CourtAfterChargesSeconds = 5;

        public static GameClient Defendant = null;
        public static List<GameClient> InvitedUsersToJuryDuty = new List<GameClient>();
        public static List<GameClient> InvitedUsersToRemove = new List<GameClient>();

        /// <summary>
        /// Thread-safe dictionary containing wanted info
        /// </summary>
        public static Dictionary<int, string> WantedList = new Dictionary<int, string>();

        /// <summary>
        /// PeakRP Fight System
        /// </summary>
        public static bool Hit(RoomUser User, RoomUser TargetUser)
        {
            if (User.SetX == TargetUser.SetX && User.SetY == TargetUser.SetY
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY
                    || User.SetX == TargetUser.SetX && User.SetY == TargetUser.SetY - 1
                    || User.SetX == TargetUser.SetX && User.SetY == TargetUser.SetY - 1
                    || User.SetX == TargetUser.SetX && User.SetY == TargetUser.SetY + 1
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY

                    //right
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 5 && TargetUser.RotBody == 3 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 3 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                     || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 1 && TargetUser.RotBody == 3 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 3 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking
                     //
                     || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 1 && TargetUser.RotBody == 7 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 7 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking
                    //d
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 7 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 5 && TargetUser.RotBody == 7 && User.IsWalking && TargetUser.IsWalking
                     //left
                     || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 7 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 5 && TargetUser.RotBody == 7 && User.IsWalking && TargetUser.IsWalking
                     || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 7 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 1 && TargetUser.RotBody == 7 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 3 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 1 && TargetUser.RotBody == 3 && User.IsWalking && TargetUser.IsWalking

                     || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 3 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 5 && TargetUser.RotBody == 3 && User.IsWalking && TargetUser.IsWalking


                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 3 && TargetUser.RotBody == 7 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 7 && TargetUser.RotBody == 3 && User.IsWalking && TargetUser.IsWalking

                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 3 && TargetUser.RotBody == 7 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 7 && TargetUser.RotBody == 3 && User.IsWalking && TargetUser.IsWalking

                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 1 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 5 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking

                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 1 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 5 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX + 1 && User.SetY == TargetUser.SetY - 1 && User.RotBody == 1 && TargetUser.RotBody == 5 && User.IsWalking && TargetUser.IsWalking
                    || User.SetX == TargetUser.SetX - 1 && User.SetY == TargetUser.SetY + 1 && User.RotBody == 5 && TargetUser.RotBody == 1 && User.IsWalking && TargetUser.IsWalking)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sends the user to the room via a thread (instant room loading without glitches)
        /// </summary>
        public static void InstantRL(GameClient Client, int RoomId)
        {
            Room OldRoom;

            Room NewRoom = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Client.GetHabbo().CurrentRoomId, out OldRoom))
                return;

            if (OldRoom.GetRoomUserManager() != null)
                OldRoom.GetRoomUserManager().RemoveUserFromRoom(Client, false, false);

            OldRoom.GetRoomItemHandler().RemoveItems2(Client);

            foreach (RoomUser RoomUser in OldRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (RoomUser == null)
                    continue;

                Client.SendMessage(new UserRemoveComposer(RoomUser.VirtualId));
            }

            Client.GetHabbo().CurrentRoomId = NewRoom.Id;
            NewRoom.GetRoomUserManager().AddAvatarToRoom(Client);

            Client.SendMessage(new ObjectsComposer(NewRoom.GetRoomItemHandler().GetFloor.ToArray(), NewRoom));
            Client.SendMessage(new ItemsComposer(NewRoom.GetRoomItemHandler().GetWall.ToArray(), NewRoom));
            Client.GetRoleplay().SendWeb("change-room;" + NewRoom.Name);
            Client.SendWhisper(NewRoom.Name, 1);
            Client.GetRoleplay().ResetEffect();

            foreach (RoomUser RoomUser in NewRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (RoomUser == null)
                    continue;

                Client.SendMessage(new UsersComposer(RoomUser));

                if (RoomUser.IsBot && RoomUser.BotData.DanceId > 0)
                    Client.SendMessage(new DanceComposer(RoomUser, RoomUser.BotData.DanceId));
                else if (!RoomUser.IsBot && !RoomUser.IsPet && RoomUser.IsDancing)
                    Client.SendMessage(new DanceComposer(RoomUser, RoomUser.DanceId));

                if (RoomUser.IsAsleep)
                    Client.SendMessage(new SleepComposer(RoomUser, true));

                if (RoomUser.CarryItemID > 0 && RoomUser.CarryTimer > 0)
                    Client.SendMessage(new CarryObjectComposer(RoomUser.VirtualId, RoomUser.CarryItemID));

                if (!RoomUser.IsBot && !RoomUser.IsPet && RoomUser.CurrentEffect > 0)
                    Client.SendMessage(new AvatarEffectComposer(RoomUser.VirtualId, RoomUser.CurrentEffect));
            }

            Client.SendMessage(new UserUpdateComposer(NewRoom.GetRoomUserManager().GetUserList().ToList()));
            Client.SendMessage(new RoomVisualizationSettingsComposer(-2, -2, true));

            #region Jukebox TV
            if (NewRoom.Id == 94)
            {
                Client.GetRoleplay().SendWeb("jukebox;show");
            }
            else if (OldRoom.Id == 94)
            {
                Client.GetRoleplay().SendWeb("jukebox;hide");
            }
            #endregion

            if (Client.GetRoleplay().Escorting)
            {
                GameClient Escorting = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Client.GetRoleplay().EscortUsername);
                InstantRL(Escorting, NewRoom.Id);
            }

            #region Taxi
            Taxi.Route(Client);
            #endregion
        }

        public static int TaxiCost(int from, int to)
        {
            int cost = 0;
            if (from == to)
                return 0;

            for (int i = from; i <= to; i = i * 2)
            {
                cost = i;
            }

            return cost;
        }

        /// <summary>
        /// Gets the distance between 2 points
        /// </summary>
        public static double GetDistanceBetweenPoints2D(Point From, Point To)
        {
            Vector2D Pos1 = new Vector2D(From.X, From.Y);
            Vector2D Pos2 = new Vector2D(To.X, To.Y);

            double XDistance = Math.Abs(Pos1.X - Pos2.X);
            double YDistance = Math.Abs(Pos1.Y - Pos2.Y);

            if (XDistance == 0 && YDistance == 0)
                return 0;

            if (XDistance == 0)
                return YDistance;

            if (YDistance == 0)
                return XDistance;

            double DiagonalDistance = Math.Sqrt(XDistance * XDistance + YDistance * YDistance);

            return DiagonalDistance;
        }

        /// <summary>
        /// Sends a livefeed to all players
        /// </summary>
        public static void LiveFeed(string Message)
        {
            foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || !Client.GetRoleplay().Livefeed)
                    continue;

                try
                {
                    PlusEnvironment.LastActionMessage += 1;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "live-feed;" + PlusEnvironment.LastActionMessage + ";" + Message);
                }
                catch (Exception E)
                {

                }
            }
        }

        /// <summary>
        /// Loads wanted list with the current players
        /// </summary>
        public static void LoadWantedList(bool Remove = false, int Id = 0)
        {
            foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client != null && Client.GetHabbo() != null)
                {
                    PlusEnvironment.WantedListColor = true;
                    if (Remove)
                    {
                        Client.GetRoleplay().SendWeb("wanted-list-remove;" + Id);
                        LoadWantedList();
                    }
                    else if (WantedList.Count == 0)
                    {
                        Client.GetRoleplay().SendWeb("wanted-list-empty");
                    }
                    else
                    {
                        Client.GetRoleplay().SendWeb("wanted-list-new");
                        foreach (var Users in WantedList.OrderBy(p => p.Value))
                        {
                            GameClient User = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Users.Key);
                            if (User != null)
                            {
                                PlusEnvironment.WantedListColor = !PlusEnvironment.WantedListColor;
                                Client.GetRoleplay().SendWeb("wanted-list;" + User.GetRoleplay().Wan.WantedId + ";" + User.GetHabbo().Username + ";" + User.GetHabbo().Look + ";" + User.GetRoleplay().Wan.ArrestTime() + ";" + User.GetRoleplay().Wan.Assault + ";" + User.GetRoleplay().Wan.Murder + ";" + User.GetRoleplay().Wan.Copassault + ";" + User.GetRoleplay().Wan.Copmurder + ";" + User.GetRoleplay().Wan.Ganghomicide + ";" + User.GetRoleplay().Wan.Obstruction + ";" + User.GetRoleplay().Wan.Hacking + ";" + User.GetRoleplay().Wan.Trespassing + ";" + User.GetRoleplay().Wan.Robbery + ";" + User.GetRoleplay().Wan.Illegalarea + ";" + User.GetRoleplay().Wan.Jailbreak + ";" + User.GetRoleplay().Wan.Terrorism + ";" + User.GetRoleplay().Wan.Drugs + ";" + User.GetRoleplay().Wan.Execution + ";" + User.GetRoleplay().Wan.Escaping + ";" + User.GetRoleplay().Wan.NonCompliance + ";" + User.GetRoleplay().Wan.CallAbuse + ";" + User.GetRoleplay().Wan.Passed + ";" + (PlusEnvironment.WantedListColor ? "bg-dark-0" : "bg-dark-1"));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates target stats
        /// </summary>
       /* public static void JuryCall(int FromUserId)
        {
            GameClient Random = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(PlusEnvironment.GetGame().GetClientManager().PickRandomUserId());
            if (Random != null && Random.GetHabbo().Id != FromUserId && !Random.GetRoleplay().CalledToCourt)
            {
                Random.GetRoleplay().CalledToCourt = true;
                Random.SendWhisper("You have been requested at Las Vegas Justice Court to take part in jury duty. You have 2 minutes to get there.", 5);
                Random.SendWhisper("Click the jury button in the top right or walk to the court", 5);
                Random.GetRoleplay().SendWeb("jury;show");
            }

            if (!CourtStarted && CourtJoinedUsers < 5)
            {
                System.Timers.Timer StartTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1));
                StartTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(1);
                StartTimer.Elapsed += delegate
                {
                    JuryCall(FromUserId);
                    StartTimer.Stop();
                };
                StartTimer.Start();
            }
        }*/

        /// <summary>
        /// Updates target stats
        /// </summary>
        public static void UpdateTargetStats(int TargetId)
        {
            var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(TargetId);
            if (Target == null)
                return;

            foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null || Client.GetConnection() == null || Client.GetRoleplay().TargetId != TargetId)
                    continue;

                try
                {
                    if (Target.GetHabbo().usingArena)
                    {
                        Client.GetRoleplay().SendWeb("target-stats-update;" + PlusEnvironment.BoolToEnum(Target.GetRoleplay().Passive) + ";" + Target.GetHabbo().ArenaHealth + ";" + Target.GetHabbo().ArenaHealthMax + ";" + Target.GetHabbo().ArenaEnergy);
                    }
                    else
                    {
                        Client.GetRoleplay().SendWeb("target-stats-update;" + PlusEnvironment.BoolToEnum(Target.GetRoleplay().Passive) + ";" + Target.GetRoleplay().Health + ";" + Target.GetRoleplay().HealthMax + ";" + Target.GetRoleplay().Energy + ";" + Target.GetRoleplay().Aggression);
                    }
                }
                catch
                {
                    UpdateTargetStats(TargetId);
                }
            }
        }

        /// <summary>
        /// Adds cooldown to the user
        /// </summary>
        public static void AddCooldown(string Type, int Time)
        {
            if (ActiveCooldowns.ContainsKey(Type))
                return;

            ActiveCooldowns.Add(Type, Time);

            System.Timers.Timer Timer = new System.Timers.Timer(Time);
            Timer.Interval = Time;
            Timer.Elapsed += delegate
            {
                ActiveCooldowns.Remove(Type);
                Timer.Stop();
            };
            Timer.Start();
        }

        /// <summary>
        /// Checks if the user has the cooldown
        /// </summary>
        public static bool GetCooldown(string Type)
        {
            try
            {
                if (ActiveCooldowns.ContainsKey(Type))
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return false;
        }

        /// <summary>
        /// Updates the weekly shifts
        /// </summary>
        public static void WeeklyShifts()
        {
            System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(1));
            Timer.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(1);
            Timer.Elapsed += delegate
            {
                DateTime now = DateTime.Now;
                if (now.DayOfWeek == DayOfWeek.Monday && now.Hour == 00 && now.Minute == 00)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `user_rp_stats` SET weekly_shifts = '0'");
                        dbClient.RunQuery();

                        foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
                        {
                            if (Client == null || Client.GetConnection() == null || Client.GetRoleplay() == null)
                                continue;

                            Client.GetRoleplay().WeeklyShifts = 0;
                        }
                        log.Info("Weekly shifts have been reset");
                    }
                }
            };
            Timer.Start();
        }

        public static void SaveData()
        {
            foreach (GameClient Client in PlusEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (Client == null)
                    continue;

                if (Client.GetRoleplay() != null && Client.GetRoleplay().UserDataHandler != null)
                {
                    try
                    {
                        Client.GetRoleplay().RPCache(4);
                        Client.GetRoleplay().UserDataHandler.SaveData();
                        Client.GetRoleplay().UserDataHandler.SaveWantedData();
                        Client.GetRoleplay().UserDataHandler = null;
                        log.Info("Saving users roleplay data...");
                    }
                    catch
                    {
                    }
                }
            }
            log.Info("Done saving users roleplay data!");
        }
    }
}
