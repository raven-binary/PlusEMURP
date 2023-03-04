using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using Plus.Net;
using Plus.Core;
using System.Threading;
using Plus.Communication.Packets.Incoming;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Interfaces;
using Plus.HabboHotel.Users.UserDataManagement;

using ConnectionManager;

using Plus.Communication.Packets.Outgoing.Sound;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;


using Plus.Communication.Encryption.Crypto.Prng;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.Communication.Packets.Outgoing.BuildersClub;
using Plus.HabboHotel.Moderation;

using Plus.Database.Interfaces;
using Plus.Utilities;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Permissions;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Campaigns;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Fleck;
using System.Collections.Concurrent;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboRoleplay.Misc;
using WebHook;

using Plus.HabboRoleplay.RoleplayUsers;

namespace Plus.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly int _id;
        private Habbo _habbo;
        public RoleplayUser _roleplay;
        public string MachineId;
        private bool _disconnected;
        public ARC4 RC4Client = null;
        private GamePacketParser _packetParser;
        private ConnectionInformation _connection;
        public int PingCount { get; set; }
        public bool LoggingOut = false;
        public ConcurrentDictionary<string, int> Timers;

        public GameClient(int ClientId, ConnectionInformation pConnection)
        {
            this._id = ClientId;
            this._connection = pConnection;
            this._packetParser = new GamePacketParser(this);

            this.PingCount = 0;
        }

        private void SwitchParserRequest()
        {
            _packetParser.SetConnection(_connection);
            _packetParser.onNewPacket += parser_onNewPacket;
            byte[] data = (_connection.parser as InitialPacketParser).currentData;
            _connection.parser.Dispose();
            _connection.parser = _packetParser;
            _connection.parser.handlePacketData(data);
        }

        private void parser_onNewPacket(ClientPacket Message)
        {
            try
            {
                PlusEnvironment.GetGame().GetPacketManager().TryExecutePacket(this, Message);
            }
            catch (Exception e)
            {
                Logging.LogPacketException(Message.ToString(), e.ToString());
            }
        }

        private void PolicyRequest()
        {
            _connection.SendData(PlusEnvironment.GetDefaultEncoding().GetBytes("<?xml version=\"1.0\"?>\r\n" +
                   "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                   "<cross-domain-policy>\r\n" +
                   "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                   "</cross-domain-policy>\x0"));
        }

        public void StartConnection()
        {
            if (_connection == null)
                return;

            this.PingCount = 0;

            (_connection.parser as InitialPacketParser).PolicyRequest += PolicyRequest;
            (_connection.parser as InitialPacketParser).SwitchParserRequest += SwitchParserRequest;
            _connection.startPacketProcessing();
        }

        public bool TryAuthenticate(string AuthTicket)
        {
            try
            {
                byte errorCode = 0;
                UserData userData = UserDataFactory.GetUserData(AuthTicket, out errorCode);
                if (errorCode == 1 || errorCode == 2)
                {
                    Disconnect();
                    return false;
                }

                #region Ban Checking
                //Let's have a quick search for a ban before we successfully authenticate..
                ModerationBan BanRecord = null;
                if (!string.IsNullOrEmpty(MachineId))
                {
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(MachineId, out BanRecord))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().MachineBanCheck(MachineId))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }

                if (userData.user != null)
                {
                    //Now let us check for a username ban record..
                    BanRecord = null;
                    if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(userData.user.Username, out BanRecord))
                    {
                        if (PlusEnvironment.GetGame().GetModerationManager().UsernameBanCheck(userData.user.Username))
                        {
                            Disconnect();
                            return false;
                        }
                    }
                }
                #endregion

                PlusEnvironment.GetGame().GetClientManager().RegisterClient(this, userData.userID, userData.user.Username);
                _habbo = userData.user;
                if (_habbo != null)
                {
                    #region Roleplay Data
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("SELECT * FROM `user_rp_stats` WHERE `user_id` = '" + userData.userID + "' LIMIT 1");
                        DataRow RPStatsRow = dbClient.getRow();

                        if (RPStatsRow == null)
                        {
                            dbClient.RunQuery("INSERT INTO `user_rp_stats` (`user_id`) VALUES ('" + userData.userID + "')");
                            dbClient.SetQuery("SELECT * FROM `user_rp_stats` WHERE `user_id` = '" + userData.userID + "' LIMIT 1");
                            RPStatsRow = dbClient.getRow();
                        }

                        dbClient.SetQuery("SELECT * FROM `inventory` WHERE `user_id` = '" + userData.userID + "' LIMIT 1");
                        DataRow InventoryRow = dbClient.getRow();

                        if (InventoryRow == null)
                        {
                            dbClient.RunQuery("INSERT INTO `inventory` (`user_id`) VALUES ('" + userData.userID + "')");
                            dbClient.SetQuery("SELECT * FROM `inventory` WHERE `user_id` = '" + userData.userID + "' LIMIT 1");
                            InventoryRow = dbClient.getRow();
                        }

                        dbClient.SetQuery("SELECT * FROM `users_wanted` WHERE `user_id` = '" + userData.userID + "' LIMIT 1");
                        DataRow WantedListRow = dbClient.getRow();

                        if (WantedListRow == null)
                        {
                            dbClient.RunQuery("INSERT INTO `users_wanted` (`user_id`) VALUES ('" + userData.userID + "')");
                            dbClient.SetQuery("SELECT * FROM `users_wanted` WHERE `user_id` = '" + userData.userID + "' LIMIT 1");
                            WantedListRow = dbClient.getRow();
                        }

                        _roleplay = new RoleplayUser(this, RPStatsRow, InventoryRow, WantedListRow);
                    }
                    #endregion

                    userData.user.Init(this, userData);

                    SendMessage(new AuthenticationOKComposer());
                    SendMessage(new AvatarEffectsComposer(_habbo.Effects().GetAllEffects));
                    SendMessage(new NavigatorSettingsComposer(_habbo.HomeRoom));
                    SendMessage(new FavouritesComposer(userData.user.FavoriteRooms));
                    SendMessage(new FigureSetIdsComposer(_habbo.GetClothing().GetClothingAllParts));
                    SendMessage(new UserRightsComposer(_habbo.Rank));
                    SendMessage(new AvailabilityStatusComposer());
                    SendMessage(new AchievementScoreComposer(_habbo.GetStats().AchievementPoints));
                    SendMessage(new BuildersClubMembershipComposer());
                    SendMessage(new CfhTopicsInitComposer());
                    SendMessage(new BadgeDefinitionsComposer(PlusEnvironment.GetGame().GetAchievementManager()._achievements));
                    SendMessage(new SoundSettingsComposer(_habbo.ClientVolume, _habbo.ChatPreference, _habbo.AllowMessengerInvites, _habbo.FocusPreference, FriendBarStateUtility.GetInt(_habbo.FriendbarState)));

                    if (!string.IsNullOrEmpty(MachineId))
                    {
                        if (this._habbo.MachineId != MachineId)
                        {
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("UPDATE `users` SET `machine_id` = @MachineId WHERE `id` = @id LIMIT 1");
                                dbClient.AddParameter("MachineId", MachineId);
                                dbClient.AddParameter("id", _habbo.Id);
                                dbClient.RunQuery();
                            }
                        }
                        _habbo.MachineId = MachineId;
                    }

                    PermissionGroup PermissionGroup = null;
                    if (PlusEnvironment.GetGame().GetPermissionManager().TryGetGroup(_habbo.Rank, out PermissionGroup))
                    {
                        if (!String.IsNullOrEmpty(PermissionGroup.Badge))
                            if (!_habbo.GetBadgeComponent().HasBadge(PermissionGroup.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(PermissionGroup.Badge, true, this);
                    }

                    SubscriptionData SubData = null;
                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(this._habbo.VIPRank, out SubData))
                    {
                        if (!String.IsNullOrEmpty(SubData.Badge))
                        {
                            if (!_habbo.GetBadgeComponent().HasBadge(SubData.Badge))
                                _habbo.GetBadgeComponent().GiveBadge(SubData.Badge, true, this);
                        }
                    }

                    if (!PlusEnvironment.GetGame().GetCacheManager().ContainsUser(_habbo.Id))
                        PlusEnvironment.GetGame().GetCacheManager().GenerateUser(_habbo.Id);

                    _habbo.InitProcess();

                    if (userData.user.GetPermissions().HasRight("mod_tickets"))
                    {
                        SendMessage(new ModeratorInitComposer(
                          PlusEnvironment.GetGame().GetModerationManager().UserMessagePresets,
                          PlusEnvironment.GetGame().GetModerationManager().RoomMessagePresets,
                          PlusEnvironment.GetGame().GetModerationManager().UserActionPresets,
                          PlusEnvironment.GetGame().GetModerationTool().GetTickets));
                    }

                    if (GetHabbo().Username.Length >= 16 || GetHabbo().Username.Contains(">") || GetHabbo().Username.Contains("<") || GetHabbo().Username.Contains("="))
                    {
                        PlusEnvironment.GetGame().GetClientManager().StaffWhisper(GetHabbo().Username + " was kicked out of the game by the system on suspicion of scripting! (Invalid or too long Username)");
                        Disconnect();
                    }
                    PlusEnvironment.GetGame().GetRewardManager().CheckRewards(this);
                    GetRoleplay().Login();
                    return true;
                }
            }
            catch (Exception e)
            {
                Logging.LogCriticalException("Bug during user login: " + e);
            }
            return false;
        }

        public void SendWhisper(string Message, int Colour = 0)
        {
            if (this == null || GetHabbo() == null || GetHabbo().CurrentRoom == null)
                return;

            RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Username);
            if (User == null)
                return;

            SendMessage(new WhisperComposer(User.VirtualId, Message, 0, (Colour == 0 ? User.LastBubble : Colour)));
        }

        public void Shout(string Message, int Colour = 0)
        {
            PlusEnvironment.Shout(this, Message, Colour);
        }

        public void SendNotification(string Message)
        {
            SendMessage(new BroadcastMessageAlertComposer(Message));
        }

        public void SendMessage(IServerPacket Message)
        {
            byte[] bytes = Message.GetBytes();

            if (Message == null)
                return;

            if (GetConnection() == null)
                return;

            GetConnection().SendData(bytes);
        }

        public int ConnectionID
        {
            get { return _id; }
        }

        public ConnectionInformation GetConnection()
        {
            return _connection;
        }

        public Habbo GetHabbo()
        {
            return _habbo;
        }

        public RoleplayUser GetRoleplay()
        {
            return _roleplay;
        }

        public void Disconnect()
        {
            try
            {
                if (GetHabbo() != null)
                {
                    #region WebSocket 
                    PlusEnvironment.GetGame().GetWebEventManager().CloseSocketByGameClient(((this.GetHabbo() == null) ? 0 : this.GetHabbo().Id));
                    #endregion

                    System.Timers.Timer LogoutTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(20));
                    LogoutTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(20);
                    LogoutTimer.Elapsed += delegate
                    {
                        using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.RunQuery(GetHabbo().GetQueryString);
                        }
                        LogoutTimer.Stop();
                    };
                    LogoutTimer.Start();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e.ToString());
            }


            if (!_disconnected)
            {
                if (_connection != null)
                    _connection.Dispose();
                _disconnected = true;
            }
        }

        public void Dispose(int ClientId)
        {
            if (GetHabbo() != null && GetHabbo().InRoom)
            {
                RoomUser User = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().Id);
                User.Say(GetHabbo().Username + " is logging out in 20 seconds", 1);
                GetHabbo().Effects().ApplyEffect(95);
                GetHabbo().isDisconnecting = true;
                User.isDisconnecting = true;
                User.Freezed = true;
                User.CanWalk = false;
                GetRoleplay().RoomX = User.X;
                GetRoleplay().RoomY = User.Y;

                if (GetHabbo().Working)
                    GetHabbo().stopWork();

                GetHabbo().updateHomeRoom(GetHabbo().CurrentRoomId);
                GetRoleplay().RPCache(4);

                #region Paramedic
                if (GetHabbo().IsWaitingForParamedic)
                {
                    GetRoleplay().Dead = true;
                    GetHabbo().RPCache(5);

                    RoomUser TargetUser = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().WaitingForParamedicFrom);
                    if (TargetUser != null)
                    {
                        TargetUser.GetClient().SendMessage(new RoomForwardComposer(62));
                    }
                }
                else if (GetHabbo().UsingParamedic)
                {
                    RoomUser TargetUser = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetHabbo().ParamedicUsername);
                    if (TargetUser == null)
                        return;

                    TargetUser.GetClient().GetHabbo().IsWaitingForParamedic = false;
                    TargetUser.GetClient().GetHabbo().WaitingForParamedicFrom = null;
                    TargetUser.AllowOverride = false;
                    TargetUser.UltraFastWalking = false;

                    TargetUser.GetClient().GetHabbo().Hospital = 1;
                    TargetUser.GetClient().GetHabbo().updateHospitalEtat(TargetUser, 3);
                }
                #endregion

                #region ATM Claim
                if (GetHabbo().ClaimedATM != 0)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `atms` SET `claimed` = 0 WHERE `atm_id` = '" + GetHabbo().ClaimedATM + "' LIMIT 1;");
                        dbClient.RunQuery();
                    }
                }
                #endregion

                #region Court
                if (GetRoleplay().Trial)
                {
                    GetRoleplay().Prison = true;

                    RoleplayManager.CourtUsing = false;
                    RoleplayManager.CourtStarting = false;
                    RoleplayManager.CourtStarted = false;
                    RoleplayManager.CourtMembersTeleport = false;
                    RoleplayManager.Defendant = null;
                    RoleplayManager.CourtVoteStarted = false;
                    RoleplayManager.CourtGuiltyVotes = 0;
                    RoleplayManager.CourtInnocentVotes = 0;

                    lock (RoleplayManager.InvitedUsersToRemove)
                    {
                        foreach (GameClient TargetClient in RoleplayManager.InvitedUsersToRemove)
                        {
                            if (TargetClient == null || TargetClient.GetHabbo() == null)
                                continue;

                            TargetClient.SendWhisper("The jury duty session has been cancelled. Apologies for any inconvenience", 5);

                            if (RoleplayManager.CourtStarted)
                            {
                                RoomUser TargetUser = GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                                if (User == null)
                                    return;

                                TargetClient.GetRoleplay().JoinedCourt = false;
                                TargetClient.GetRoleplay().CourtVoted = false;
                                GetHabbo().CurrentRoom.GetGameMap().UpdateUserMovement(new Point(TargetUser.X, TargetUser.Y), new Point(TargetClient.GetHabbo().LastX, TargetClient.GetHabbo().LastY), TargetUser);
                                TargetUser.X = TargetClient.GetHabbo().LastX;
                                TargetUser.Y = TargetClient.GetHabbo().LastY;
                                TargetUser.Freezed = false;
                                TargetUser.Statusses.Clear();
                                TargetUser.PathRecalcNeeded = true;
                                TargetUser.SetPos(TargetClient.GetHabbo().LastX, TargetClient.GetHabbo().LastY, 0);
                                TargetUser.SetRot(TargetClient.GetHabbo().LastRot, false);
                                TargetUser.UpdateNeeded = true;
                            }
                        }
                    }
                    RoleplayManager.InvitedUsersToJuryDuty.Clear();
                }
                #endregion

            }

            System.Timers.Timer LogoutTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(20));
            LogoutTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(20);
            LogoutTimer.Elapsed += delegate
            {
                if (GetHabbo() != null)
                {
                    #region Wanted
                    if (PlusEnvironment.WantedList.ContainsKey(GetHabbo().Id))
                    {
                        PlusEnvironment.WantedList.Remove(GetHabbo().Id);
                        PlusEnvironment.GetGame().GetClientManager().LoadWantedList(true, GetHabbo().Id);
                    }
                    #endregion
                    GetHabbo().OnDisconnect();
                }

                if (GetRoleplay() != null && GetRoleplay().UserDataHandler != null)
                    GetRoleplay().OnDisconnect();

                PlusEnvironment.GetGame().GetClientManager().removeConnection(ClientId);
                this.MachineId = string.Empty;
                this._disconnected = true;
                this._habbo = null;
                this._roleplay = null;
                this._connection = null;
                this.RC4Client = null;
                this._packetParser = null;
                LogoutTimer.Stop();
            };
            LogoutTimer.Start();
        }
    }
}