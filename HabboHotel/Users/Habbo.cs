﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Core;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Roleplay.Combat;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Rooms.Chat.Commands;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Users.Badges;
using Plus.HabboHotel.Users.Clothing;
using Plus.HabboHotel.Users.Effects;
using Plus.HabboHotel.Users.Ignores;
using Plus.HabboHotel.Users.Inventory;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.HabboHotel.Users.Navigator.SavedSearches;
using Plus.HabboHotel.Users.Permissions;
using Plus.HabboHotel.Users.Process;
using Plus.HabboHotel.Users.Relationships;

namespace Plus.HabboHotel.Users
{
    public class Habbo
    {
        // Generic player values.
        public int Id { get; set; }
        public string Username { get; set; }
        public int Rank { get; set; }
        public string Motto { get; set; }
        public string Look { get; set; }
        public string Gender { get; set; }
        public string FootballLook { get; set; }
        public string FootballGender { get; set; }
        public int Credits { get; set; }
        public int Duckets { get; set; }
        public int Diamonds { get; set; }
        public int GotwPoints { get; set; }
        public int HomeRoom { get; set; }
        public double LastOnline { get; set; }
        public double AccountCreated { get; set; }
        public List<int> ClientVolume { get; set; }
        public double LastNameChange { get; set; }
        public string MachineId { get; set; }
        public bool ChatPreference { get; set; }
        public bool FocusPreference { get; set; }
        public bool IsExpert { get; set; }
        public int VipRank { get; set; }
        public int Health { get; set; }

        // Abilities triggered by generic events.
        public bool AppearOffline { get; set; }
        public bool AllowTradingRequests { get; set; }
        public bool AllowUserFollowing { get; set; }
        public bool AllowFriendRequests { get; set; }
        public bool AllowMessengerInvites { get; set; }
        public bool AllowPetSpeech { get; set; }
        public bool AllowBotSpeech { get; set; }
        public bool AllowPublicRoomStatus { get; set; }
        public bool AllowConsoleMessages { get; set; }
        public bool AllowGifts { get; set; }
        public bool AllowMimic { get; set; }
        public bool ReceiveWhispers { get; set; }
        public bool IgnorePublicWhispers { get; set; }
        public bool PlayingFastFood { get; set; }
        public FriendBarState FriendBarState { get; set; }
        public int ChristmasDay { get; set; }
        public int WantsToRideHorse { get; set; }
        public int TimeAfk { get; set; }
        public bool DisableForcedEffects { get; set; }

        // Player saving.
        private bool _disconnected;
        private bool _habboSaved;
        public bool ChangingName { get; set; }

        // Counters
        public int FriendCount { get; set; }
        public double FloodTime { get; set; }
        public double TimeMuted { get; set; }
        public double TradingLockExpiry { get; set; }
        public int BannedPhraseCount { get; set; }
        public double SessionStart { get; set; }
        public int MessengerSpamCount { get; set; }
        public double MessengerSpamTime { get; set; }
        public int CreditsUpdateTick { get; set; }

        // Room related
        public int TentId { get; set; }
        public int HopperId { get; set; }
        public bool IsHopping { get; set; }
        public int TeleportId { get; set; }
        public bool IsTeleporting { get; set; }
        public int TeleportingRoomId { get; set; }
        public bool RoomAuthOk { get; set; }
        public int CurrentRoomId { get; set; }

        public double ForceHeight;

        internal void updateHealth()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET health = @health WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("health", this.Health);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        // Advertising reporting system.
        public bool HasSpoken { get; set; }
        public double LastAdvertiseReport { get; set; }
        public bool AdvertisingReported { get; set; }
        public bool AdvertisingReportedBlocked { get; set; }

        // Values generated within the game.
        public bool WiredInteraction { get; set; }
        public int QuestLastCompleted { get; set; }
        public bool InventoryAlert { get; set; }
        public bool IgnoreBobbaFilter { get; set; }
        public bool WiredTeleporting { get; set; }
        public int CustomBubbleId { get; set; }
        public int TempInt { get; set; }
        public bool OnHelperDuty { get; set; }

        // FastFood
        public int FastFoodScore { get; set; }

        // Just random fun stuff.
        public int PetId { get; set; }

        // Anti-script placeholders.
        public DateTime LastGiftPurchaseTime { get; set; }
        public DateTime LastMottoUpdateTime { get; set; }
        public DateTime LastClothingUpdateTime { get; set; }
        public DateTime LastForumMessageUpdateTime { get; set; }

        public int GiftPurchasingWarnings { get; set; }
        public int MottoUpdateWarnings { get; set; }
        public int ClothingUpdateWarnings { get; set; }

        public bool SessionGiftBlocked { get; set; }
        public bool SessionMottoBlocked { get; set; }
        public bool SessionClothingBlocked { get; set; }

        public List<int> RatedRooms;

        private GameClient _client;
        private readonly HabboStats _habboStats;
        private HabboMessenger _messenger;
        private ProcessComponent _process;
        public ArrayList FavoriteRooms;
        public Dictionary<int, int> Quests;
        private BadgeComponent _badgeComponent;
        private InventoryComponent _inventoryComponent;
        public Dictionary<int, Relationship> Relationships;
        public ConcurrentDictionary<string, UserAchievement> Achievements;

        private readonly DateTime _timeCached;

        private SearchesComponent _navigatorSearches;
        private EffectsComponent _fx;
        private ClothingComponent _clothing;
        private PermissionComponent _permissions;
        private IgnoresComponent _ignores;

        public Combat Combat;
        public bool isDisconnecting = false;
        public double stackHeight = 0;

        public Habbo(int id, string username, int rank, string motto, string look, string gender, int credits, int activityPoints, int homeRoom,
            bool hasFriendRequestsDisabled, int lastOnline, bool appearOffline, bool hideInRoom, double createDate, int diamonds,
            string machineId, string clientVolume, bool chatPreference, bool focusPreference, bool petsMuted, bool botsMuted, bool advertisingReportBlocked, double lastNameChange,
            int gotwPoints, bool ignoreInvites, double timeMuted, double tradingLock, bool allowGifts, int friendBarState, bool disableForcedEffects, bool allowMimic, int vipRank, int health)
        {
            Id = id;
            Username = username;
            Rank = rank;
            Motto = motto;
            Look = look;
            Gender = gender.ToLower();
            FootballLook = PlusEnvironment.FilterFigure(look.ToLower());
            FootballGender = gender.ToLower();
            Credits = credits;
            Duckets = activityPoints;
            Diamonds = diamonds;
            GotwPoints = gotwPoints;
            HomeRoom = homeRoom;
            LastOnline = lastOnline;
            AccountCreated = createDate;
            ClientVolume = new List<int>();
            foreach (string str in clientVolume.Split(','))
            {
                if (int.TryParse(str, out _))
                    ClientVolume.Add(int.Parse(str));
                else
                    ClientVolume.Add(100);
            }

            LastNameChange = lastNameChange;
            MachineId = machineId;
            ChatPreference = chatPreference;
            FocusPreference = focusPreference;
            IsExpert = IsExpert;

            AppearOffline = appearOffline;
            AllowTradingRequests = true; //TODO
            AllowUserFollowing = true; //TODO
            AllowFriendRequests = hasFriendRequestsDisabled; //TODO
            AllowMessengerInvites = ignoreInvites;
            AllowPetSpeech = petsMuted;
            AllowBotSpeech = botsMuted;
            AllowPublicRoomStatus = hideInRoom;
            AllowConsoleMessages = true;
            AllowGifts = allowGifts;
            AllowMimic = allowMimic;
            ReceiveWhispers = true;
            IgnorePublicWhispers = false;
            PlayingFastFood = false;
            FriendBarState = FriendBarStateUtility.GetEnum(friendBarState);
            ChristmasDay = ChristmasDay;
            WantsToRideHorse = 0;
            TimeAfk = 0;
            DisableForcedEffects = disableForcedEffects;
            VipRank = vipRank;
            Health = health;

            _disconnected = false;
            _habboSaved = false;
            ChangingName = false;

            FloodTime = 0;
            FriendCount = 0;
            TimeMuted = timeMuted;
            _timeCached = DateTime.Now;

            TradingLockExpiry = tradingLock;
            if (TradingLockExpiry > 0 && PlusEnvironment.GetUnixTimestamp() > TradingLockExpiry)
            {
                TradingLockExpiry = 0;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + id + "' LIMIT 1");
                }
            }

            BannedPhraseCount = 0;
            SessionStart = PlusEnvironment.GetUnixTimestamp();
            MessengerSpamCount = 0;
            MessengerSpamTime = 0;
            CreditsUpdateTick = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.tick"));

            TentId = 0;
            HopperId = 0;
            IsHopping = false;
            TeleportId = 0;
            IsTeleporting = false;
            TeleportingRoomId = 0;
            RoomAuthOk = false;
            CurrentRoomId = 0;

            HasSpoken = false;
            LastAdvertiseReport = 0;
            AdvertisingReported = false;
            AdvertisingReportedBlocked = advertisingReportBlocked;

            WiredInteraction = false;
            QuestLastCompleted = 0;
            InventoryAlert = false;
            IgnoreBobbaFilter = false;
            WiredTeleporting = false;
            CustomBubbleId = 0;
            OnHelperDuty = false;
            FastFoodScore = 0;
            PetId = 0;
            TempInt = 0;

            LastGiftPurchaseTime = DateTime.Now;
            LastMottoUpdateTime = DateTime.Now;
            LastClothingUpdateTime = DateTime.Now;
            LastForumMessageUpdateTime = DateTime.Now;

            GiftPurchasingWarnings = 0;
            MottoUpdateWarnings = 0;
            ClothingUpdateWarnings = 0;

            SessionGiftBlocked = false;
            SessionMottoBlocked = false;
            SessionClothingBlocked = false;

            FavoriteRooms = new ArrayList();
            Achievements = new ConcurrentDictionary<string, UserAchievement>();
            Relationships = new Dictionary<int, Relationship>();
            RatedRooms = new List<int>();

            Combat = new Combat(this);

            //TODO: Nope.
            InitPermissions();

            #region Stats

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts`,`combat_level` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", id);
                DataRow statRow = dbClient.GetRow();

                if (statRow == null) //No row, add it yo
                {
                    dbClient.RunQuery("INSERT INTO `user_stats` (`id`) VALUES ('" + id + "')");
                    dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts`,`combat_level` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                    dbClient.AddParameter("user_id", id);
                    statRow = dbClient.GetRow();
                }

                try
                {
                    _habboStats = new HabboStats(Convert.ToInt32(statRow["roomvisits"]), Convert.ToDouble(statRow["onlineTime"]), Convert.ToInt32(statRow["respect"]), Convert.ToInt32(statRow["respectGiven"]), Convert.ToInt32(statRow["giftsGiven"]),
                        Convert.ToInt32(statRow["giftsReceived"]), Convert.ToInt32(statRow["dailyRespectPoints"]), Convert.ToInt32(statRow["dailyPetRespectPoints"]), Convert.ToInt32(statRow["AchievementScore"]),
                        Convert.ToInt32(statRow["quest_id"]), Convert.ToInt32(statRow["quest_progress"]), Convert.ToInt32(statRow["groupid"]), Convert.ToString(statRow["respectsTimestamp"]), Convert.ToInt32(statRow["forum_posts"]), Convert.ToInt32(statRow["combat_level"]));

                    if (Convert.ToString(statRow["respectsTimestamp"]) != DateTime.Today.ToString("MM/dd"))
                    {
                        _habboStats.RespectsTimestamp = DateTime.Today.ToString("MM/dd");

                        int dailyRespects = 10;

                        if (_permissions.HasRight("mod_tool"))
                            dailyRespects = 20;
                        else if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(vipRank, out SubscriptionData subData))
                            dailyRespects = subData.Respects;

                        _habboStats.DailyRespectPoints = dailyRespects;
                        _habboStats.DailyPetRespectPoints = dailyRespects;

                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + dailyRespects + "', `dailyPetRespectPoints` = '" + dailyRespects + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + id + "' LIMIT 1");
                    }
                }
                catch (Exception e)
                {
                    ExceptionLogger.LogException(e);
                }
            }

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(_habboStats.FavouriteGroupId, out Group _))
                _habboStats.FavouriteGroupId = 0;

            #endregion
        }

        public IChatCommand ChatCommand { get; set; }

        public HabboStats GetStats()
        {
            return _habboStats;
        }

        public bool InRoom => CurrentRoomId >= 1 && CurrentRoom != null;

        public Room CurrentRoom
        {
            get
            {
                if (CurrentRoomId <= 0)
                    return null;

                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(CurrentRoomId, out Room room))
                    return room;

                return null;
            }
        }

        public bool CacheExpired()
        {
            TimeSpan span = DateTime.Now - _timeCached;
            return span.TotalMinutes >= 30;
        }

        public string GetQueryString
        {
            get
            {
                _habboSaved = true;
                return "UPDATE `users` SET `online` = '0', `last_online` = '" + PlusEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + Duckets + "', `credits` = '" + Credits + "', `vip_points` = '" + Diamonds + "', `home_room` = '" + HomeRoom + "', `gotw_points` = '" + GotwPoints + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(FriendBarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (PlusEnvironment.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestId + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts + "' WHERE `id` = '" + Id + "' LIMIT 1;";
            }
        }

        public int JobId { get; internal set; }
        public bool Working { get; internal set; }
        public int HealthMax { get; internal set; }

        public bool InitProcess()
        {
            _process = new ProcessComponent();

            return _process.Init(this);
        }

        public bool InitSearches()
        {
            _navigatorSearches = new SearchesComponent();

            return _navigatorSearches.Init(this);
        }

        public bool InitFx()
        {
            _fx = new EffectsComponent();

            return _fx.Init(this);
        }

        public bool InitClothing()
        {
            _clothing = new ClothingComponent();

            return _clothing.Init(this);
        }

        public bool InitIgnores()
        {
            _ignores = new IgnoresComponent();

            return _ignores.Init(this);
        }

        private bool InitPermissions()
        {
            _permissions = new PermissionComponent();

            return _permissions.Init(this);
        }

        public void InitInformation(UserData.UserData data)
        {
            _badgeComponent = new BadgeComponent(this, data);
            Relationships = data.Relations;
        }

        public void Init(GameClient client, UserData.UserData data)
        {
            Achievements = data.Achievements;

            FavoriteRooms = new ArrayList();
            foreach (int id in data.FavoritedRooms)
            {
                FavoriteRooms.Add(id);
            }

            _client = client;
            _badgeComponent = new BadgeComponent(this, data);
            _inventoryComponent = new InventoryComponent(Id, client);

            Quests = data.Quests;

            _messenger = new HabboMessenger(Id);
            _messenger.Init(data.Friends, data.Requests);
            FriendCount = Convert.ToInt32(data.Friends.Count);
            _disconnected = false;
            Relationships = data.Relations;

            InitSearches();
            InitFx();
            InitClothing();
            InitIgnores();
        }

        public PermissionComponent GetPermissions()
        {
            return _permissions;
        }

        public IgnoresComponent GetIgnores()
        {
            return _ignores;
        }

        public void OnDisconnect()
        {
            if (_disconnected)
                return;

            try
            {
                _process?.Dispose();
            }
            catch
            {
            }

            _disconnected = true;

            PlusEnvironment.GetGame().GetClientManager().UnregisterClient(Id, Username);

            if (!_habboSaved)
            {
                _habboSaved = true;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `users` SET `online` = '0', `last_online` = '" + PlusEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + Duckets + "', `credits` = '" + Credits + "', `vip_points` = '" + Diamonds + "', `home_room` = '" + HomeRoom + "', `gotw_points` = '" + GotwPoints + "', `time_muted` = '" + TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(FriendBarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + _habboStats.RoomVisits + "', `onlineTime` = '" + (PlusEnvironment.GetUnixTimestamp() - SessionStart + _habboStats.OnlineTime) + "', `respect` = '" + _habboStats.Respect + "', `respectGiven` = '" + _habboStats.RespectGiven + "', `giftsGiven` = '" + _habboStats.GiftsGiven + "', `giftsReceived` = '" + _habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + _habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + _habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + _habboStats.AchievementPoints + "', `quest_id` = '" + _habboStats.QuestId + "', `quest_progress` = '" + _habboStats.QuestProgress + "', `groupid` = '" + _habboStats.FavouriteGroupId + "',`forum_posts` = '" + _habboStats.ForumPosts + "' WHERE `id` = '" + Id + "' LIMIT 1;");

                    if (GetPermissions().HasRight("mod_tickets"))
                        dbClient.RunQuery("UPDATE `moderation_tickets` SET `status` = 'open', `moderator_id` = '0' WHERE `status` ='picked' AND `moderator_id` = '" + Id + "'");
                }
            }

            Dispose();

            _client = null;
        }

        public void Dispose()
        {
            _inventoryComponent?.SetIdleState();

            if (InRoom && CurrentRoom != null)
                CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(_client, false);

            if (_messenger != null)
            {
                _messenger.AppearOffline = true;
                _messenger.Destroy();
            }

            _fx?.Dispose();

            _clothing?.Dispose();

            _permissions?.Dispose();

            if (_ignores != null)
                _permissions.Dispose();
        }

        public void CheckCreditsTimer()
        {
            try
            {
                CreditsUpdateTick--;

                if (CreditsUpdateTick <= 0)
                {
                    int creditUpdate = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.credit_reward"));
                    int ducketUpdate = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.ducket_reward"));

                    if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(VipRank, out SubscriptionData subData))
                    {
                        creditUpdate += subData.Credits;
                        ducketUpdate += subData.Duckets;
                    }

                    Credits += creditUpdate;
                    Duckets += ducketUpdate;

                    _client.SendPacket(new CreditBalanceComposer(Credits));
                    _client.SendPacket(new HabboActivityPointNotificationComposer(Duckets, ducketUpdate));

                    CreditsUpdateTick = Convert.ToInt32(PlusEnvironment.GetSettingsManager().TryGetValue("user.currency_scheduler.tick"));
                }
            }
            catch
            {
            }
        }

        public GameClient GetClient()
        {
            if (_client != null)
                return _client;

            return PlusEnvironment.GetGame().GetClientManager().GetClientByUserId(Id);
        }

        public HabboMessenger GetMessenger()
        {
            return _messenger;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return _badgeComponent;
        }

        public InventoryComponent GetInventoryComponent()
        {
            return _inventoryComponent;
        }

        public SearchesComponent GetNavigatorSearches()
        {
            return _navigatorSearches;
        }

        public EffectsComponent Effects()
        {
            return _fx;
        }

        public ClothingComponent GetClothing()
        {
            return _clothing;
        }

        public int GetQuestProgress(int p)
        {
            Quests.TryGetValue(p, out int progress);
            return progress;
        }

        public UserAchievement GetAchievementData(string p)
        {
            Achievements.TryGetValue(p, out UserAchievement achievement);
            return achievement;
        }

        public void ChangeName(string username)
        {
            LastNameChange = PlusEnvironment.GetUnixTimestamp();
            Username = username;

            SaveKey("username", username);
            SaveKey("last_change", LastNameChange.ToString());
        }

        public void SaveKey(string key, string value)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET " + key + " = @value WHERE `id` = '" + Id + "' LIMIT 1;");
                dbClient.AddParameter("value", value);
                dbClient.RunQuery();
            }
        }

        public void PrepareRoom(int id, string password)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null)
                return;

            if (GetClient().GetHabbo().InRoom)
            {
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(GetClient().GetHabbo().CurrentRoomId, out Room oldRoom))
                    return;

                if (oldRoom.GetRoomUserManager() != null)
                    oldRoom.GetRoomUserManager().RemoveUserFromRoom(GetClient(), false);
            }

            if (GetClient().GetHabbo().IsTeleporting && GetClient().GetHabbo().TeleportingRoomId != id)
            {
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            if (!PlusEnvironment.GetGame().GetRoomManager().TryLoadRoom(id, out Room room))
            {
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            if (room.IsCrashed)
            {
                GetClient().SendNotification("This room has crashed! :(");
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            GetClient().GetHabbo().CurrentRoomId = room.RoomId;

            if (room.GetRoomUserManager().UserCount >= room.UsersMax && !GetClient().GetHabbo().GetPermissions().HasRight("room_enter_full") && GetClient().GetHabbo().Id != room.OwnerId)
            {
                GetClient().SendPacket(new CantConnectComposer(1));
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }


            if (!GetPermissions().HasRight("room_ban_override") && room.GetBans().IsBanned(Id))
            {
                RoomAuthOk = false;
                GetClient().GetHabbo().RoomAuthOk = false;
                GetClient().SendPacket(new CantConnectComposer(4));
                GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }

            GetClient().SendPacket(new OpenConnectionComposer());
            if (!room.CheckRights(GetClient(), true, true) && !GetClient().GetHabbo().IsTeleporting && !GetClient().GetHabbo().IsHopping)
            {
                if (room.Access == RoomAccess.Doorbell && !GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                {
                    if (room.UserCount > 0)
                    {
                        GetClient().SendPacket(new DoorbellComposer(""));
                        room.SendPacket(new DoorbellComposer(GetClient().GetHabbo().Username), true);
                        return;
                    }

                    GetClient().SendPacket(new FlatAccessDeniedComposer(""));
                    GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }

                if (room.Access == RoomAccess.Password && !GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                {
                    if (password.ToLower() != room.Password.ToLower() || string.IsNullOrWhiteSpace(password))
                    {
                        GetClient().SendPacket(new GenericErrorComposer(-100002));
                        GetClient().SendPacket(new CloseConnectionComposer());
                        return;
                    }
                }
            }

            if (!EnterRoom(room))
                GetClient().SendPacket(new CloseConnectionComposer());
        }

        public bool EnterRoom(Room room)
        {
            if (room == null)
                GetClient().SendPacket(new CloseConnectionComposer());

            GetClient().SendPacket(new RoomReadyComposer(room.RoomId, room.ModelName));
            if (room.Wallpaper != "0.0")
                GetClient().SendPacket(new RoomPropertyComposer("wallpaper", room.Wallpaper));
            if (room.Floor != "0.0")
                GetClient().SendPacket(new RoomPropertyComposer("floor", room.Floor));

            GetClient().SendPacket(new RoomPropertyComposer("landscape", room.Landscape));
            GetClient().SendPacket(new RoomRatingComposer(room.Score, !(GetClient().GetHabbo().RatedRooms.Contains(room.RoomId) || room.OwnerId == GetClient().GetHabbo().Id)));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("INSERT INTO user_roomvisits (user_id,room_id,entry_timestamp,exit_timestamp,hour,minute) VALUES ('" + GetClient().GetHabbo().Id + "','" + GetClient().GetHabbo().CurrentRoomId + "','" + PlusEnvironment.GetUnixTimestamp() + "','0','" + DateTime.Now.Hour + "','" + DateTime.Now.Minute + "');"); // +
            }

            if (room.OwnerId != Id)
            {
                GetClient().GetHabbo().GetStats().RoomVisits += 1;
                PlusEnvironment.GetGame().GetAchievementManager().ProgressAchievement(GetClient(), "ACH_RoomEntry", 1);
            }

            return true;
        }
    }
}