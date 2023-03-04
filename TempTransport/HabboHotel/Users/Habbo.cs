using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

using log4net;
using System.Drawing;

using Plus.Core;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.GroupsRank;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Users.Badges;
using Plus.HabboHotel.Users.Inventory;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;
using Plus.HabboHotel.Users.UserDataManagement;
using Plus.HabboHotel.Users.Process;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Users.Navigator.SavedSearches;
using Plus.HabboHotel.Users.Effects;
using Plus.HabboHotel.Users.Messenger.FriendBar;
using Plus.HabboHotel.Users.Clothing;
using Plus.Communication.Packets.Outgoing.Navigator;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Handshake;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms.Chat.Commands;
using Plus.HabboHotel.Users.Permissions;
using Plus.HabboHotel.Subscriptions;
using Plus.HabboHotel.Users.Calendar;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.Items;
using Fleck;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.Pathfinding;
using SharedPacketLib;
using WebHook;
using Plus.Communication.Packets.Outgoing;
using Plus.HabboRoleplay.Misc;

using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;

using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;



using System.Threading.Tasks;
using Plus.HabboHotel.Discord.Services;

namespace Plus.HabboHotel.Users
{
    public class Habbo
    {
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboHotel.Users");
        #region RP
        public bool usingArrestActionPoint = false;
        public bool LoadedPos = false;
        public bool LoadedSockets = false;
        public bool isLoggedIn = false;
        public int FarmToken = 0;
        public int PlayToken = 0;
        public bool TaxiRide = false;
        public int TaxiRideId = 0;
        public bool TalkToGang = false;
        public bool usingPassive = false;
        public bool usingArena = false;
        public int ArenaHealth;
        public int ArenaHealthMax;
        public int ArenaEnergy;
        public bool BodyArmour = false;
        public bool Stunned = false;
        public string StunnedBy = null;
        public int Aggression = 0;
        public int AggressionToken = 0;
        public bool GPWarning = false;
        public bool UsingMedkit = false;
        public bool UsingSnack = false;
        public int MedkitCooldownTimer = 0;
        public bool UsingHospitalHeal = false;
        public bool usingRobTill = false;

        //Inventory Start
        public string InventoryEquipSlot1 = null;
        public string InventoryEquipSlot2 = null;
        public string InventoryEquipSlot1Item = null;
        public string InventoryEquipSlot2Item = null;
        public int InventoryEquipSlot1Durability = 0;
        public int InventoryEquipSlot2Durability = 0;
        public string InventorySlot1 = null;
        public string InventorySlot2 = null;
        public string InventorySlot3 = null;
        public string InventorySlot4 = null;
        public string InventorySlot5 = null;
        public string InventorySlot6 = null;
        public string InventorySlot7 = null;
        public string InventorySlot8 = null;
        public string InventorySlot9 = null;
        public string InventorySlot10 = null;
        public string InventorySlot1Type = null;
        public string InventorySlot2Type = null;
        public string InventorySlot3Type = null;
        public string InventorySlot4Type = null;
        public string InventorySlot5Type = null;
        public string InventorySlot6Type = null;
        public string InventorySlot7Type = null;
        public string InventorySlot8Type = null;
        public string InventorySlot9Type = null;
        public string InventorySlot10Type = null;
        public int InventorySlot1Quantity = 0;
        public int InventorySlot2Quantity = 0;
        public int InventorySlot3Quantity = 0;
        public int InventorySlot4Quantity = 0;
        public int InventorySlot5Quantity = 0;
        public int InventorySlot6Quantity = 0;
        public int InventorySlot7Quantity = 0;
        public int InventorySlot8Quantity = 0;
        public int InventorySlot9Quantity = 0;
        public int InventorySlot10Quantity = 0;
        public int InventorySlot1Durability = 0;
        public int InventorySlot2Durability = 0;
        public int InventorySlot3Durability = 0;
        public int InventorySlot4Durability = 0;
        public int InventorySlot5Durability = 0;
        public int InventorySlot6Durability = 0;
        public int InventorySlot7Durability = 0;
        public int InventorySlot8Durability = 0;
        public int InventorySlot9Durability = 0;
        public int InventorySlot10Durability = 0;
        //Inventory End
        public bool Macro = false;
        public string Macro1_1 = "null";
        public string Macro1_2 = "null";
        public string Macro1_3 = "null";
        public string Macro2_1 = "null";
        public string Macro2_2 = "null";
        public string Macro2_3 = "null";
        public string Macro3_1 = "null";
        public string Macro3_2 = "null";
        public string Macro3_3 = "null";

        public string Macro1_1Key = "null";
        public string Macro1_2Key = "null";
        public string Macro1_3Key = "null";
        public string Macro2_1Key = "null";
        public string Macro2_2Key = "null";
        public string Macro2_3Key = "null";
        public string Macro3_1Key = "null";
        public string Macro3_2Key = "null";
        public string Macro3_3Key = "null";
        //
        public bool Duty = false;
        public string UsingTask = "";
        public string UsingFarm = "";
        public bool UsingManagerMail = false;
        public bool UsingArmouryMerchandise = false;
        public bool IsInvisible = false;
        public bool IsUsingAppleEnergy = false;
        //

        //Hospital
        public bool UsingParamedic = false;
        public bool IsWaitingForParamedic = false;
        public string WaitingForParamedicFrom = null;
        public string ParamedicUsername = null;

        public string ArenaTo = null;
        public string ArenaFrom = null;


        public bool usingSuicide = false;
        public bool usingHeroinDrug = false;
        //Apex Gym
        public bool usingGymMembershipPurchase = false;
        public bool usingCrosstrainer = false;
        public bool usingTreadMill = false;
        public bool usingTrampoline = false;
        public bool usedCrosstrainer = false;
        public bool usedTreadMill = false;
        public bool usedTrampoline = false;
        public int CrosstrainerXP = 0;
        public int TreadMillXP = 0;
        public int TrampolineXP = 0;
        //
        public bool usingSafeRob = false;
        public bool usingTrash = false;
        //
        public bool usingMenuSell = false;
        public string CorpSell;
        public bool usingSellingStock = false;
        public bool usingDepositBox = false;
        public int ClaimedATM = 0;
        public bool UsingCityHallActionPoint = false;
        //Marry System
        public bool usingChapelActionPoint = false;
        public string MarryingTo = null;
        public bool IsWaitingToMarry = false;
        public bool IsWaitingToMarryReply = false;
        public bool IsWaitingToMarryReplyDone = false;
        //Hug
        public string LastHug = null;
        //Pepper Spray Movements
        public bool SprayMov1 = false;
        public bool SprayMov2 = false;
        public bool SprayMov3 = false;
        public bool SprayMov4 = false;
        public bool SprayMov5 = false;
        //Marketplace
        public bool usingMarketplace = false;
        public bool usingManageSales = false;
        public bool usingCreateSale = false;
        public string CreateSaleItem = null;
        public int CreateSaleQuantity = 0;
        public int CreateSaleDurability = 0;
        public bool MarketplaceCooldown = false;
        //Trading
        public int TradeToken = 0;
        public string isTradingWith = null;
        public bool TradeConfirmed = false;
        public int TradeMoney = 0;
        public string TradeSlot1 = null;
        public int TradeSlot1Quantity = 0;
        public int TradeSlot1Durability = 0;
        public string TradeSlot2 = null;
        public int TradeSlot2Quantity = 0;
        public int TradeSlot2Durability = 0;
        public string TradeSlot3 = null;
        public int TradeSlot3Quantity = 0;
        public int TradeSlot3Durability = 0;
        public string TradeSlot4 = null;
        public int TradeSlot4Quantity = 0;
        public int TradeSlot4Durability = 0;
        public string TradeSlot5 = null;
        public int TradeSlot5Quantity = 0;
        public int TradeSlot5Durability = 0;
        public string TradeSlot6 = null;
        public int TradeSlot6Quantity = 0;
        public int TradeSlot6Durability = 0;
        public string TradeSlot7 = null;
        public int TradeSlot7Quantity = 0;
        public int TradeSlot7Durability = 0;
        public string TradeSlot8 = null;
        public int TradeSlot8Quantity = 0;
        public int TradeSlot8Durability = 0;
        //
        #endregion
        public int MouseClicks = 0;
        public bool ClickThru = false;
        public bool Xmouse = false;
        public string Hair = null; //for forever21
        public bool ThrowingBomb = false;
        public string BombFromSlot = null;
        public int KniveCooldownTimer = 0;
        public int Suicidecool = 0;
        public bool isBleeding = false;
        public int BleedsOut = 0;
        public string LastHitFrom = null;
        public string LastHitFromAssist = null;
        public bool RoomLoad = false;
        public bool isInTurf = false;
        public int TurfInsideId = 0;
        public bool HP20 = false;
        public bool HP10 = false;
        public bool isUsingSkateboard = false;
        public bool isDisconnecting = false;
        public int LastX = 0;
        public int LastY = 0;
        public int LastRot = 0;

        //Police
        public int PoliceCallViewing = 0;
        public bool isUsingPoliceCar = false;
        public bool Escort = false;
        public string EscortBy = null;
        public bool Escorting = false;
        public string EscortUsername = null;
        public bool Cuffed = false;
        public bool isUncuffing = false;
        public bool CanUseJailGate = false;

        public string LockpickingTo = null;
        public string LockpickingFrom = null;

        //Higher/Lower Casino Game
        public bool CardsPlaying = false;
        public string CardsWith = null;
        public int CardsOffer = 0;
        public bool MyTurn = false;
        public int CurrentCard = 0;

        public bool ConnectedWebsockets = false;

        //
        public string FreeInventory = null;
        public DateTime StartedWork;
        public int OfferToken = 0;
        public string TransactionFrom = null;
        public string TransactionTo = null;
        public int UsingItem = 0;
        public bool UsingBounties = false;
        public int BountyTimer = 0;
        //

        //Purge
        public int LastDamage = 0;

        public int Ticket = 0;
        public int TicketTimer = 0;
        public string TicketFrom = null;
        //Wanted List
        public DateTime WantedAddedTime = DateTime.Now;
        public int WantedPassed = 0;

        //Charges
        public int assault = 0;
        public int murder = 0;
        public int ganghomicide = 0;
        public int copassault = 0;
        public int copmurder = 0;
        public int obstruction = 0;
        public int hacking = 0;
        public int trespassing = 0;
        public int robbery = 0;
        public int illegalarea = 0;
        public int jailbreak = 0;
        public int terrorism = 0;
        public int drugs = 0;
        public int execution = 0;
        public int escaping = 0;
        public int nonCompliance = 0;
        public int callAbuse = 0;
        //
        public bool UsingJukebox = false;

        //Generic player values.
        private int _id;
        private string _username;
        private long _discordId;
        private int _rank;
        private string _motto;
        private string _look;
        private int _JobId;
        private int _antiAFKAttempt = 0;
        private string _conduit;
        private Dictionary<int, DateTime> _EventDirectionary = new Dictionary<int, DateTime>();

        private int _EventCount = 0;
        private string _EventItem = null;
        private string _footballTeam = null;
        private bool _isCalling = false;
        private string _inCallWithUsername = null;
        private string _receiveCallUsername = null;
        private int _timeAppel = 0;
        private string _callingToken = null;
        private string _gender;
        private string _footballLook;
        private string _footballGender;
        private bool _telephoneEteint = false;
        private int _credits;
        private int _duckets;
        private int _diamonds;
        private int _gotwPoints;
        private int _homeRoom;
        private double _lastOnline;
        private string _lastIp;
        private double _accountCreated;
        private List<int> _clientVolume;
        private double _lastNameChange;
        private string _machineId;
        private bool _chatPreference;
        private bool _focusPreference;
        private bool _isExpert;
        private int _vipRank;
        private int _Health;
        private int _HealthMax;
        private int _Energy;
        private int _Telephone;
        private string _TelephoneName;
        private int _TelephoneForfait;
        private int _TelephoneForfaitSms;
        private int _TelephoneForfaitType;
        private DateTime _TelephoneForfaitReset;
        private int _Banque;
        private int _Timer;
        private bool _Working;
        private bool _CanChangeRoom;
        private bool _Menotted;
        private string _MenottedUsername;
        private int _Chargeur;
        private string _ArmeEquiped;
        private string _Commande;
        private int _Prison;
        private int _Hospital;
        private int _Arrests;
        private int _Deaths;
        private int _Kills;
        private int _Coca;
        private int _Fanta;
        private int _Blur;
        private int _SmokeTimer;
        private int _AudiA8;
        private int _Porsche911;
        private int _FiatPunto;
        private int _VolkswagenJetta;
        private int _BmwI8;
        private string _Cb;
        private int _Sucette;
        private int _Pain;
        private int _Doliprane;
        private int _Mutuelle;
        private DateTime _MutuelleDate;
        private int _Gang;
        private int _GangRank;
        private int _Bat;
        private int _Sabre;
        private int _Ak47;
        private int _Ak47_Munitions;
        private int _Uzi;
        private int _Uzi_Munitions;
        private int _shifts;
        private int _passive;
        private int _angelrute;
        private int _tuna;
        private int _catfish;
        private int _salmon;
        private int _hummer;
        private int _carrots;
        private int _shovel;
        private int _lockpick;
        private int _combatlevel;
        private int _combatxp;
        private int _farminglevel;
        private int _sales;
        private int _cheetos;

        //Abilitys triggered by generic events.
        private bool _appearOffline;
        private bool _allowTradingRequests;
        private bool _allowUserFollowing;
        private bool _allowFriendRequests;
        private bool _allowMessengerInvites;
        private bool _allowPetSpeech;
        private bool _allowBotSpeech;
        private bool _allowPublicRoomStatus;
        private bool _allowConsoleMessages;
        private bool _allowGifts;
        private bool _allowMimic;
        private bool _receiveWhispers;
        private bool _ignorePublicWhispers;
        private bool _playingFastFood;
        private FriendBarState _friendbarState;
        private int _christmasDay;
        private int _wantsToRideHorse;
        private int _timeAFK;
        private bool _disableForcedEffects;

        //Player saving.
        private bool _disconnected;
        public bool _habboSaved;
        private bool _changingName;

        //Counters
        private double _floodTime;
        private int _friendCount;
        private double _timeMuted;
        private double _tradingLockExpiry;
        private int _bannedPhraseCount;
        private double _sessionStart;
        private int _messengerSpamCount;
        private double _messengerSpamTime;
        private int _creditsTickUpdate;
        private int _EnergyUpdate;
        private int _OneMinuteUpdate;
        private double _Pierre;
        private int _Confirmed;
        private int _Coiffure;
        private string _PoliceCasier;
        private int _Carte;
        private int _Facebook;
        private int _Creations;
        private int _Permis_arme;
        private int _Clipper;
        private int _Weed;
        private int _PhilipMo;
        private int _EventDay;
        private int _EventPoints;
        private int _Casino_Jetons;
        private int _Quizz_Points;
        private int _Cocktails;
        private int _WhiteHoverboard;
        private int _AudiA3;
        private decimal _Eau;
        private int _CasierWeed;
        private int _CasierCocktails;

        //Room related
        private int _tentId;
        private int _hopperId;
        private bool _isHopping;
        private int _teleportId;
        private bool _isTeleporting;
        private int _teleportingRoomId;
        private bool _roomAuthOk;
        private int _currentRoomId;

        //Advertising reporting system.
        private bool _hasSpoken;
        private bool _advertisingReported;
        private double _lastAdvertiseReport;
        private bool _advertisingReportBlocked;

        //Values generated within the game.
        private bool _wiredInteraction;
        private int _questLastCompleted;
        private bool _inventoryAlert;
        private bool _ignoreBobbaFilter;
        private bool _wiredTeleporting;
        private int _customBubbleId;
        private int _tempInt;
        private bool _onHelperDuty;

        public string _nameColor;
        public string _prefixName;
        public string PreviousUsername = null;

        //Fastfood
        private int _fastfoodScore;

        //Just random fun stuff.
        private int _petId;

        //Anti-script placeholders.
        private DateTime _lastGiftPurchaseTime;
        private DateTime _lastMottoUpdateTime;
        private DateTime _lastClothingUpdateTime;
        private DateTime _lastForumMessageUpdateTime;

        private int _giftPurchasingWarnings;
        private int _mottoUpdateWarnings;
        private int _clothingUpdateWarnings;

        private bool _sessionGiftBlocked;
        private bool _sessionMottoBlocked;
        private bool _sessionClothingBlocked;

        public List<int> RatedRooms;
        public List<int> MutedUsers;
        public List<RoomData> UsersRooms;

        private GameClient _client;
        private HabboStats _habboStats;
        private RPItems _rpItems;
        private RPStats _rpStats;
        private HabboMessenger Messenger;
        private ProcessComponent _process;
        public ArrayList FavoriteRooms;
        public Dictionary<int, int> quests;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        public Dictionary<int, Relationship> Relationships;
        public ConcurrentDictionary<string, UserAchievement> Achievements;
        public ConcurrentDictionary<string, int> ActiveCooldowns;
        public double StackHeight = 0;

        private DateTime _timeCached;

        private SearchesComponent _navigatorSearches;
        private EffectsComponent _fx;
        private ClothingComponent _clothing;
        private PermissionComponent _permissions;

        private IChatCommand _iChatCommand;

        public Habbo(int Id, string Username, int Rank, string Motto, string Look, string Gender, int Credits, int ActivityPoints, long discordId, int HomeRoom,
            bool HasFriendRequestsDisabled, int LastOnline, string LastIp, bool AppearOffline, bool HideInRoom, double CreateDate, int Diamonds,
            string machineID, string clientVolume, bool ChatPreference, bool FocusPreference, int Bubble, string NameColor, string Prefix, bool PetsMuted, bool BotsMuted, bool AdvertisingReportBlocked, double LastNameChange,
            int GOTWPoints, bool IgnoreInvites, double TimeMuted, double TradingLock, bool AllowGifts, int FriendBarState, bool DisableForcedEffects, bool AllowMimic, int VIPRank, int Banque, int Timer, bool Working, int Prison, int Hospital, int Gang, int GangRank)
        {
            this._id = Id;
            this._username = Username;
            this._discordId = discordId;
            this.PreviousUsername = Username;
            this._rank = Rank;
            this._motto = Motto;
            this._look = PlusEnvironment.GetGame().GetAntiMutant().RunLook(Look);
            this._gender = Gender.ToLower();
            this._footballLook = PlusEnvironment.FilterFigure(Look.ToLower());
            this._footballGender = Gender.ToLower();
            this._credits = Credits;
            this._duckets = ActivityPoints;
            this._diamonds = Diamonds;
            this._gotwPoints = GOTWPoints;
            this._homeRoom = HomeRoom;
            this._lastOnline = LastOnline;
            this._lastIp = LastIp;
            this._accountCreated = CreateDate;
            this._clientVolume = new List<int>();
            foreach (string Str in clientVolume.Split(','))
            {
                int Val = 0;
                if (int.TryParse(Str, out Val))
                    this._clientVolume.Add(int.Parse(Str));
                else
                    this._clientVolume.Add(100);
            }

            this._lastNameChange = LastNameChange;
            this._machineId = machineID;
            this._chatPreference = ChatPreference;
            this._focusPreference = FocusPreference;
            this._customBubbleId = Bubble;
            this._nameColor = NameColor;
            this._prefixName = Prefix;
            this._isExpert = IsExpert == true;

            this._appearOffline = AppearOffline;
            this._allowTradingRequests = true;//TODO
            this._allowUserFollowing = true;//TODO
            this._allowFriendRequests = HasFriendRequestsDisabled;//TODO
            this._allowMessengerInvites = IgnoreInvites;
            this._allowPetSpeech = PetsMuted;
            this._allowBotSpeech = BotsMuted;
            this._allowPublicRoomStatus = HideInRoom;
            this._allowConsoleMessages = true;
            this._allowGifts = AllowGifts;
            this._allowMimic = AllowMimic;
            this._receiveWhispers = true;
            this._ignorePublicWhispers = false;
            this._playingFastFood = false;
            this._friendbarState = FriendBarStateUtility.GetEnum(FriendBarState);
            this._christmasDay = ChristmasDay;
            this._wantsToRideHorse = 0;
            this._timeAFK = 0;
            this._disableForcedEffects = DisableForcedEffects;
            this._vipRank = VIPRank;
            this._Banque = Banque;
            this._Timer = Timer;
            this._Working = false;
            this._Prison = Prison;
            this._Hospital = Hospital;
            this._Gang = Gang;
            this._GangRank = GangRank;
            this.ActiveCooldowns = new ConcurrentDictionary<string, int>();
            this._CanChangeRoom = true;
            this._disconnected = false;
            this._habboSaved = false;
            this._changingName = false;

            this._floodTime = 0;
            this._friendCount = 0;
            this._timeMuted = TimeMuted;
            this._timeCached = DateTime.Now;

            this._tradingLockExpiry = TradingLock;
            if (this._tradingLockExpiry > 0 && PlusEnvironment.GetUnixTimestamp() > this.TradingLockExpiry)
            {
                this._tradingLockExpiry = 0;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_info` SET `trading_locked` = '0' WHERE `user_id` = '" + Id + "' LIMIT 1");
                }
            }

            this._bannedPhraseCount = 0;
            this._sessionStart = PlusEnvironment.GetUnixTimestamp();
            this._messengerSpamCount = 0;
            this._messengerSpamTime = 0;
            this._creditsTickUpdate = PlusStaticGameSettings.UserCreditsUpdateTimer;

            this._tentId = 0;
            this._hopperId = 0;
            this._isHopping = false;
            this._teleportId = 0;
            this._isTeleporting = false;
            this._teleportingRoomId = 0;
            this._roomAuthOk = false;
            this._currentRoomId = 0;

            this._hasSpoken = false;
            this._lastAdvertiseReport = 0;
            this._advertisingReported = false;
            this._advertisingReportBlocked = AdvertisingReportBlocked;

            this._wiredInteraction = false;
            this._questLastCompleted = 0;
            this._inventoryAlert = false;
            this._ignoreBobbaFilter = false;
            this._wiredTeleporting = false;
            this._onHelperDuty = false;
            this._fastfoodScore = 0;
            this._petId = 0;
            this._tempInt = 0;

            this._lastGiftPurchaseTime = DateTime.Now;
            this._lastMottoUpdateTime = DateTime.Now;
            this._lastClothingUpdateTime = DateTime.Now;
            this._lastForumMessageUpdateTime = DateTime.Now;

            this._giftPurchasingWarnings = 0;
            this._mottoUpdateWarnings = 0;
            this._clothingUpdateWarnings = 0;

            this._sessionGiftBlocked = false;
            this._sessionMottoBlocked = false;
            this._sessionClothingBlocked = false;

            this.FavoriteRooms = new ArrayList();
            this.MutedUsers = new List<int>();
            this.Achievements = new ConcurrentDictionary<string, UserAchievement>();
            this.Relationships = new Dictionary<int, Relationship>();
            this.RatedRooms = new List<int>();
            this.UsersRooms = new List<RoomData>();
            //TODO: Nope.
            this.InitPermissions();

            #region Stats
            DataRow StatRow = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts`,`CashRobbed` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", Id);
                StatRow = dbClient.getRow();

                if (StatRow == null)//No row, add it yo
                {
                    dbClient.RunQuery("INSERT INTO `user_stats` (`id`) VALUES ('" + Id + "')");
                    dbClient.RunQuery("INSERT INTO `macros` (`user_id`) VALUES ('" + Id + "')");
                    dbClient.SetQuery("SELECT `id`,`roomvisits`,`onlinetime`,`respect`,`respectgiven`,`giftsgiven`,`giftsreceived`,`dailyrespectpoints`,`dailypetrespectpoints`,`achievementscore`,`quest_id`,`quest_progress`,`groupid`,`tickets_answered`,`respectstimestamp`,`forum_posts`,`CashRobbed` FROM `user_stats` WHERE `id` = @user_id LIMIT 1");
                    dbClient.AddParameter("user_id", Id);
                    StatRow = dbClient.getRow();

                }

                try
                {
                    this._habboStats = new HabboStats(Convert.ToInt32(StatRow["roomvisits"]), Convert.ToDouble(StatRow["onlineTime"]), Convert.ToInt32(StatRow["respect"]), Convert.ToInt32(StatRow["respectGiven"]), Convert.ToInt32(StatRow["giftsGiven"]),
                        Convert.ToInt32(StatRow["giftsReceived"]), Convert.ToInt32(StatRow["dailyRespectPoints"]), Convert.ToInt32(StatRow["dailyPetRespectPoints"]), Convert.ToInt32(StatRow["AchievementScore"]),
                        Convert.ToInt32(StatRow["quest_id"]), Convert.ToInt32(StatRow["quest_progress"]), this.JobId, Convert.ToString(StatRow["respectsTimestamp"]), Convert.ToInt32(StatRow["forum_posts"]), Convert.ToInt32(StatRow["CashRobbed"]));

                    if (Convert.ToString(StatRow["respectsTimestamp"]) != DateTime.Today.ToString("MM/dd"))
                    {
                        this._habboStats.RespectsTimestamp = DateTime.Today.ToString("MM/dd");
                        SubscriptionData SubData = null;

                        int DailyRespects = 3;

                        if (this._permissions.HasRight("mod_tool"))
                            DailyRespects = 3;
                        else if (PlusEnvironment.GetGame().GetSubscriptionManager().TryGetSubscriptionData(VIPRank, out SubData))
                            DailyRespects = SubData.Respects;

                        this._habboStats.DailyRespectPoints = DailyRespects;
                        this._habboStats.DailyPetRespectPoints = DailyRespects;

                        dbClient.RunQuery("UPDATE `user_stats` SET `dailyRespectPoints` = '" + DailyRespects + "', `dailyPetRespectPoints` = '" + DailyRespects + "', `respectsTimestamp` = '" + DateTime.Today.ToString("MM/dd") + "' WHERE `id` = '" + Id + "' LIMIT 1");
                    }
                }
                catch (Exception e)
                {
                    Logging.LogException(e.ToString());
                }
            }

            Group G = null;
            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(this._habboStats.FavouriteGroupId, out G))
                this._habboStats.FavouriteGroupId = 0;

            #endregion

            #region Group Insert
            DataRow GroupRow = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`group_id`,`user_id`,`rank_id` FROM `group_memberships` WHERE `user_id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", Id);
                GroupRow = dbClient.getRow();

                if (GroupRow == null)//No row, add it yo
                {
                    dbClient.RunQuery("INSERT INTO `group_memberships` (group_id, user_id, rank_id) VALUES (@group_id, @user_id, @rank_id)");
                    dbClient.AddParameter("group_id", "1");
                    dbClient.AddParameter("user_id", Id);
                    dbClient.AddParameter("rank_id", "1");
                    GroupRow = dbClient.getRow();
                }
            }
            #endregion

            #region Items
            DataRow ItemRow = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `user_id`,`taser`,`bat`,`bat_damage`,`sword`,`sword_damage`,`axe`,`axe_damage`,`knifes`,`body_armour`,`medkits`,`snacks`, `carrots`,`nfcHacker`,`sprays`,`lockpicks` FROM `user_items` WHERE `user_id` = @user_id LIMIT 1");
                dbClient.AddParameter("user_id", Id);
                ItemRow = dbClient.getRow();

                if (ItemRow == null)
                {
                    dbClient.RunQuery("INSERT INTO `user_items` (`user_id`) VALUES ('" + Id + "')");
                    dbClient.SetQuery("SELECT `user_id`,`taser`,`bat`,`bat_damage`,`sword`,`sword_damage`,`axe`,`axe_damage`,`knifes`,`body_armour`,`medkits`,`snacks`,`carrots`,`nfcHacker`,`sprays`,`lockpicks` FROM `user_items` WHERE `user_id` = @user_id LIMIT 1");
                    dbClient.AddParameter("user_id", Id);
                    ItemRow = dbClient.getRow();
                }

                try
                {
                    this._rpItems = new RPItems(Convert.ToString(ItemRow["taser"]), Convert.ToString(ItemRow["bat"]), Convert.ToInt32(ItemRow["bat_damage"]), Convert.ToString(ItemRow["sword"]), Convert.ToInt32(ItemRow["sword_damage"]), Convert.ToString(ItemRow["axe"]), Convert.ToInt32(ItemRow["axe_damage"]), Convert.ToInt32(ItemRow["knifes"]), Convert.ToInt32(ItemRow["body_armour"]), Convert.ToInt32(ItemRow["medkits"]), Convert.ToInt32(ItemRow["snacks"]), Convert.ToInt32(ItemRow["carrots"]), Convert.ToInt32(ItemRow["nfcHacker"]), Convert.ToInt32(ItemRow["sprays"]), Convert.ToInt32(ItemRow["lockpicks"]));
                }
                catch (Exception e)
                {
                    Logging.LogException(e.ToString());
                }
            }
            #endregion

            #region Marketplace Insert
            DataRow Marketplace = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `marketplace` WHERE `user_id` = @user_id LIMIT 1;");
                dbClient.AddParameter("user_id", Id);
                Marketplace = dbClient.getRow();

                if (Marketplace == null)//No row, add it yo
                {
                    dbClient.RunQuery("INSERT INTO `marketplace` (`user_id`) VALUES ('" + Id + "')");
                }
            }
            #endregion
        }

        public long DiscordId
        {
            get { return this._discordId;  }
            set { this._discordId = value;  }
        }

        public int Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }

        public int Rank
        {
            get { return this._rank; }
            set { this._rank = value; }
        }

        public string Motto
        {
            get { return this._motto; }
            set { this._motto = value; }
        }

        public string Look
        {
            get { return this._look; }
            set { this._look = value; }
        }

        public int HideOnline
        {
            get
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT hide_online FROM `users` WHERE id = @userId");
                    dbClient.AddParameter("userId", this.Id);
                    int Integer = dbClient.getInteger();
                    return Integer;
                }
            }
        }

        public int JobId
        {
            get
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT group_id FROM `group_memberships` WHERE user_id = @userId LIMIT 1");
                    dbClient.AddParameter("userId", this.Id);
                    int JobIdreturn = dbClient.getInteger();
                    return JobIdreturn;
                }
            }
            set { this._JobId = value; }
        }

        public int RankId
        {
            get
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT rank_id FROM `group_memberships` WHERE user_id = @userId");
                    dbClient.AddParameter("userId", this.Id);
                    int rankIdReturn = dbClient.getInteger();
                    return rankIdReturn;
                }
            }
        }

        public int Tier
        {
            get
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT tier FROM `group_memberships` WHERE user_id = @userId");
                    dbClient.AddParameter("userId", this.Id);
                    int rankIdReturn = dbClient.getInteger();
                    return rankIdReturn;
                }
            }
        }

        public int MarketplaceSales
        {
            get
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT sales FROM `marketplace` WHERE user_id = @userId");
                    dbClient.AddParameter("userId", this.Id);
                    int SalesReturn = dbClient.getInteger();
                    return SalesReturn;
                }
            }
        }

        public int MarketplaceSalesMax
        {
            get
            {
                if (this.Rank > 1)
                {
                    return 10;
                }
                else
                {
                    return 5;
                }
            }
        }

        public Group TravailInfo
        {
            get
            {
                Group G = null;
                PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(this.JobId, out G);
                return G;
            }
        }

        public GroupRank RankInfo
        {
            get
            {
                GroupRank G = null;
                PlusEnvironment.GetGame().getGroupRankManager().TryGetRank(this.JobId, this.RankId, out G);
                return G;
            }
        }


        public string Conduit
        {
            get { return this._conduit; }
            set { this._conduit = value; }
        }

        public Dictionary<int, DateTime> EventDirectionary
        {
            get { return this._EventDirectionary; }
            set { this._EventDirectionary = value; }
        }

        public int EventCount
        {
            get { return this._EventCount; }
            set { this._EventCount = value; }
        }

        public string EventItem
        {
            get { return this._EventItem; }
            set { this._EventItem = value; }
        }

        public string footballTeam
        {
            get { return this._footballTeam; }
            set { this._footballTeam = value; }
        }

        public bool isCalling
        {
            get { return this._isCalling; }
            set { this._isCalling = value; }
        }

        public string inCallWithUsername
        {
            get { return this._inCallWithUsername; }
            set { this._inCallWithUsername = value; }
        }

        public int AntiAFKAttempt
        {
            get { return this._antiAFKAttempt; }
            set { this._antiAFKAttempt = value; }
        }

        public bool TelephoneEteint
        {
            get { return this._telephoneEteint; }
            set { this._telephoneEteint = value; }
        }

        public string receiveCallUsername
        {
            get { return this._receiveCallUsername; }
            set { this._receiveCallUsername = value; }
        }

        public int timeAppel
        {
            get { return this._timeAppel; }
            set { this._timeAppel = value; }
        }

        public string callingToken
        {
            get { return this._callingToken; }
            set { this._callingToken = value; }
        }

        public string Gender
        {
            get { return this._gender; }
            set { this._gender = value; }
        }

        public string FootballLook
        {
            get { return this._footballLook; }
            set { this._footballLook = value; }
        }

        public string FootballGender
        {
            get { return this._footballGender; }
            set { this._footballGender = value; }
        }

        public int Credits
        {
            get { return this._credits; }
            set { this._credits = value; }
        }

        public int Shifts
        {
            get { return this._shifts; }
            set { this._shifts = value; }
        }

        public int Passive
        {
            get { return this._passive; }
            set { this._passive = value; }
        }

        public int Angelrute
        {
            get { return this._angelrute; }
            set { this._angelrute = value; }
        }

        public int Tuna
        {
            get { return this._tuna; }
            set { this._tuna = value; }
        }

        public int Catfish
        {
            get { return this._catfish; }
            set { this._catfish = value; }
        }

        public int Salmon
        {
            get { return this._salmon; }
            set { this._salmon = value; }
        }

        public int Hummer
        {
            get { return this._hummer; }
            set { this._hummer = value; }
        }

        public int Carrots
        {
            get { return this._carrots; }
            set { this._carrots = value; }
        }

        public int Shovel
        {
            get { return this._shovel; }
            set { this._shovel = value; }
        }

        public int Lockpick
        {
            get { return this._lockpick; }
            set { this._lockpick = value; }
        }

        public int CombatLevel
        {
            get { return this._combatlevel; }
            set { this._combatlevel = value; }
        }

        public int CombatXP
        {
            get { return this._combatxp; }
            set { this._combatxp = value; }
        }

        public int FarmingLevel
        {
            get { return this._farminglevel; }
            set { this._farminglevel = value; }
        }

        public int Sales
        {
            get { return this._sales; }
            set { this._sales = value; }
        }

        public int Cheetos
        {
            get { return this._cheetos; }
            set { this._cheetos = value; }
        }

        public int Duckets
        {
            get { return this._duckets; }
            set { this._duckets = value; }
        }

        public int Diamonds
        {
            get { return this._diamonds; }
            set { this._diamonds = value; }
        }

        public int GOTWPoints
        {
            get { return this._gotwPoints; }
            set { this._gotwPoints = value; }
        }

        public int HomeRoom
        {
            get { return this._homeRoom; }
            set { this._homeRoom = value; }
        }

        public double LastOnline
        {
            get { return this._lastOnline; }
            set { this._lastOnline = value; }
        }

        public string LastIp
        {
            get { return this._lastIp; }
            set { this._lastIp = value; }
        }

        public double AccountCreated
        {
            get { return this._accountCreated; }
            set { this._accountCreated = value; }
        }

        public List<int> ClientVolume
        {
            get { return this._clientVolume; }
            set { this._clientVolume = value; }
        }

        public double LastNameChange
        {
            get { return this._lastNameChange; }
            set { this._lastNameChange = value; }
        }

        public string MachineId
        {
            get { return this._machineId; }
            set { this._machineId = value; }
        }

        public bool ChatPreference
        {
            get { return this._chatPreference; }
            set { this._chatPreference = value; }
        }
        public bool FocusPreference
        {
            get { return this._focusPreference; }
            set { this._focusPreference = value; }
        }

        public bool IsExpert
        {
            get { return this._isExpert; }
            set { this._isExpert = value; }
        }

        public bool AppearOffline
        {
            get { return this._appearOffline; }
            set { this._appearOffline = value; }
        }

        public int VIPRank
        {
            get { return this._vipRank; }
            set { this._vipRank = value; }
        }

        public int Arrests
        {
            get { return this._Arrests; }
            set { this._Arrests = value; }
        }

        public int Coca
        {
            get { return this._Coca; }
            set { this._Coca = value; }
        }

        public int Fanta
        {
            get { return this._Fanta; }
            set { this._Fanta = value; }
        }

        public int Blur
        {
            get { return this._Blur; }
            set { this._Blur = value; }
        }

        public int SmokeTimer
        {
            get { return this._SmokeTimer; }
            set { this._SmokeTimer = value; }
        }

        public int AudiA8
        {
            get { return this._AudiA8; }
            set { this._AudiA8 = value; }
        }

        public int Porsche911
        {
            get { return this._Porsche911; }
            set { this._Porsche911 = value; }
        }

        public int FiatPunto
        {
            get { return this._FiatPunto; }
            set { this._FiatPunto = value; }
        }

        public int VolkswagenJetta
        {
            get { return this._VolkswagenJetta; }
            set { this._VolkswagenJetta = value; }
        }

        public int BmwI8
        {
            get { return this._BmwI8; }
            set { this._BmwI8 = value; }
        }

        public string Cb
        {
            get { return this._Cb; }
            set { this._Cb = value; }
        }

        public int Sucette
        {
            get { return this._Sucette; }
            set { this._Sucette = value; }
        }

        public int Pain
        {
            get { return this._Pain; }
            set { this._Pain = value; }
        }

        public int Doliprane
        {
            get { return this._Doliprane; }
            set { this._Doliprane = value; }
        }

        public int Kills
        {
            get { return this._Kills; }
            set { this._Kills = value; }
        }

        public int Deaths
        {
            get { return this._Deaths; }
            set { this._Deaths = value; }
        }

        public int Prison
        {
            get { return this._Prison; }
            set { this._Prison = value; }
        }

        public int Hospital
        {
            get { return this._Hospital; }
            set { this._Hospital = value; }
        }

        public int Health
        {
            get { return this._Health; }
            set { this._Health = value; }
        }

        public int HealthMax
        {
            get { return this._HealthMax; }
            set { this._HealthMax = value; }
        }

        public double Pierre
        {
            get { return this._Pierre; }
            set { this._Pierre = value; }
        }

        public int Confirmed
        {
            get { return this._Confirmed; }
            set { this._Confirmed = value; }
        }

        public int Coiffure
        {
            get { return this._Coiffure; }
            set { this._Coiffure = value; }
        }

        public string PoliceCasier
        {
            get { return this._PoliceCasier; }
            set { this._PoliceCasier = value; }
        }

        public int Carte
        {
            get { return this._Carte; }
            set { this._Carte = value; }
        }

        public int Facebook
        {
            get { return this._Facebook; }
            set { this._Facebook = value; }
        }

        public int Creations
        {
            get { return this._Creations; }
            set { this._Creations = value; }
        }

        public int Permis_arme
        {
            get { return this._Permis_arme; }
            set { this._Permis_arme = value; }
        }

        public int Clipper
        {
            get { return this._Clipper; }
            set { this._Clipper = value; }
        }

        public int Weed
        {
            get { return this._Weed; }
            set { this._Weed = value; }
        }

        public int PhilipMo
        {
            get { return this._PhilipMo; }
            set { this._PhilipMo = value; }
        }

        public int EventDay
        {
            get { return this._EventDay; }
            set { this._EventDay = value; }
        }

        public int EventPoints
        {
            get { return this._EventPoints; }
            set { this._EventPoints = value; }
        }

        public int Casino_Jetons
        {
            get { return this._Casino_Jetons; }
            set { this._Casino_Jetons = value; }
        }

        public int Quizz_Points
        {
            get { return this._Quizz_Points; }
            set { this._Quizz_Points = value; }
        }

        public int Cocktails
        {
            get { return this._Cocktails; }
            set { this._Cocktails = value; }
        }

        public int WhiteHoverboard
        {
            get { return this._WhiteHoverboard; }
            set { this._WhiteHoverboard = value; }
        }

        public int AudiA3
        {
            get { return this._AudiA3; }
            set { this._AudiA3 = value; }
        }

        public decimal Eau
        {
            get { return this._Eau; }
            set { this._Eau = value; }
        }

        public int CasierWeed
        {
            get { return this._CasierWeed; }
            set { this._CasierWeed = value; }
        }

        public int CasierCocktails
        {
            get { return this._CasierCocktails; }
            set { this._CasierCocktails = value; }
        }

        public int Energy
        {
            get { return this._Energy; }
            set { this._Energy = value; }
        }

        public int Telephone
        {
            get { return this._Telephone; }
            set { this._Telephone = value; }
        }

        public string TelephoneName
        {
            get { return this._TelephoneName; }
            set { this._TelephoneName = value; }
        }

        public int TelephoneForfait
        {
            get { return this._TelephoneForfait; }
            set { this._TelephoneForfait = value; }
        }

        public int TelephoneForfaitSms
        {
            get { return this._TelephoneForfaitSms; }
            set { this._TelephoneForfaitSms = value; }
        }

        public int TelephoneForfaitType
        {
            get { return this._TelephoneForfaitType; }
            set { this._TelephoneForfaitType = value; }
        }

        public DateTime TelephoneForfaitReset
        {
            get { return this._TelephoneForfaitReset; }
            set { this._TelephoneForfaitReset = value; }
        }

        public int Banque
        {
            get { return this._Banque; }
            set { this._Banque = value; }
        }

        public int Timer
        {
            get { return this._Timer; }
            set { this._Timer = value; }
        }

        public bool Working
        {
            get { return this._Working; }
            set { this._Working = value; }
        }
        public bool CanChangeRoom
        {
            get { return this._CanChangeRoom; }
            set { this._CanChangeRoom = value; }
        }

        public bool Menotted
        {
            get { return this._Menotted; }
            set { this._Menotted = value; }
        }

        public string MenottedUsername
        {
            get { return this._MenottedUsername; }
            set { this._MenottedUsername = value; }
        }

        public int Chargeur
        {
            get { return this._Chargeur; }
            set { this._Chargeur = value; }
        }

        public string ArmeEquiped
        {
            get { return this._ArmeEquiped; }
            set { this._ArmeEquiped = value; }
        }

        public string Commande
        {
            get { return this._Commande; }
            set { this._Commande = value; }
        }

        public int Mutuelle
        {
            get { return this._Mutuelle; }
            set { this._Mutuelle = value; }
        }

        public DateTime MutuelleDate
        {
            get { return this._MutuelleDate; }
            set { this._MutuelleDate = value; }
        }

        public int Gang
        {
            get { return this._Gang; }
            set { this._Gang = value; }
        }

        public int GangRank
        {
            get { return this._GangRank; }
            set { this._GangRank = value; }
        }

        public int Bat
        {
            get { return this._Bat; }
            set { this._Bat = value; }
        }

        public int Sabre
        {
            get { return this._Sabre; }
            set { this._Sabre = value; }
        }

        public int Ak47
        {
            get { return this._Ak47; }
            set { this._Ak47 = value; }
        }

        public int AK47_Munitions
        {
            get { return this._Ak47_Munitions; }
            set { this._Ak47_Munitions = value; }
        }

        public int Uzi
        {
            get { return this._Uzi; }
            set { this._Uzi = value; }
        }

        public int Uzi_Munitions
        {
            get { return this._Uzi_Munitions; }
            set { this._Uzi_Munitions = value; }
        }

        public int TempInt
        {
            get { return this._tempInt; }
            set { this._tempInt = value; }
        }

        public bool AllowTradingRequests
        {
            get { return this._allowTradingRequests; }
            set { this._allowTradingRequests = value; }
        }

        public bool AllowUserFollowing
        {
            get { return this._allowUserFollowing; }
            set { this._allowUserFollowing = value; }
        }

        public bool AllowFriendRequests
        {
            get { return this._allowFriendRequests; }
            set { this._allowFriendRequests = value; }
        }

        public bool AllowMessengerInvites
        {
            get { return this._allowMessengerInvites; }
            set { this._allowMessengerInvites = value; }
        }

        public bool AllowPetSpeech
        {
            get { return this._allowPetSpeech; }
            set { this._allowPetSpeech = value; }
        }

        public bool AllowBotSpeech
        {
            get { return this._allowBotSpeech; }
            set { this._allowBotSpeech = value; }
        }

        public bool AllowPublicRoomStatus
        {
            get { return this._allowPublicRoomStatus; }
            set { this._allowPublicRoomStatus = value; }
        }

        public bool AllowConsoleMessages
        {
            get { return this._allowConsoleMessages; }
            set { this._allowConsoleMessages = value; }
        }

        public bool AllowGifts
        {
            get { return this._allowGifts; }
            set { this._allowGifts = value; }
        }

        public bool AllowMimic
        {
            get { return this._allowMimic; }
            set { this._allowMimic = value; }
        }

        public bool ReceiveWhispers
        {
            get { return this._receiveWhispers; }
            set { this._receiveWhispers = value; }
        }

        public bool IgnorePublicWhispers
        {
            get { return this._ignorePublicWhispers; }
            set { this._ignorePublicWhispers = value; }
        }

        public bool PlayingFastFood
        {
            get { return this._playingFastFood; }
            set { this._playingFastFood = value; }
        }

        public FriendBarState FriendbarState
        {
            get { return this._friendbarState; }
            set { this._friendbarState = value; }
        }

        public int ChristmasDay
        {
            get { return this._christmasDay; }
            set { this._christmasDay = value; }
        }

        public int WantsToRideHorse
        {
            get { return this._wantsToRideHorse; }
            set { this._wantsToRideHorse = value; }
        }

        public int TimeAFK
        {
            get { return this._timeAFK; }
            set { this._timeAFK = value; }
        }

        public bool DisableForcedEffects
        {
            get { return this._disableForcedEffects; }
            set { this._disableForcedEffects = value; }
        }

        public bool ChangingName
        {
            get { return this._changingName; }
            set { this._changingName = value; }
        }

        public int FriendCount
        {
            get { return this._friendCount; }
            set { this._friendCount = value; }
        }

        public double FloodTime
        {
            get { return this._floodTime; }
            set { this._floodTime = value; }
        }

        public int BannedPhraseCount
        {
            get { return this._bannedPhraseCount; }
            set { this._bannedPhraseCount = value; }
        }

        public bool RoomAuthOk
        {
            get { return this._roomAuthOk; }
            set { this._roomAuthOk = value; }
        }

        public int CurrentRoomId
        {
            get { return this._currentRoomId; }
            set { this._currentRoomId = value; }
        }

        public int QuestLastCompleted
        {
            get { return this._questLastCompleted; }
            set { this._questLastCompleted = value; }
        }

        public int MessengerSpamCount
        {
            get { return this._messengerSpamCount; }
            set { this._messengerSpamCount = value; }
        }

        public double MessengerSpamTime
        {
            get { return this._messengerSpamTime; }
            set { this._messengerSpamTime = value; }
        }

        public double TimeMuted
        {
            get { return this._timeMuted; }
            set { this._timeMuted = value; }
        }

        public double TradingLockExpiry
        {
            get { return this._tradingLockExpiry; }
            set { this._tradingLockExpiry = value; }
        }

        public double SessionStart
        {
            get { return this._sessionStart; }
            set { this._sessionStart = value; }
        }

        public int TentId
        {
            get { return this._tentId; }
            set { this._tentId = value; }
        }

        public int HopperId
        {
            get { return this._hopperId; }
            set { this._hopperId = value; }
        }

        public bool IsHopping
        {
            get { return this._isHopping; }
            set { this._isHopping = value; }
        }

        public int TeleporterId
        {
            get { return this._teleportId; }
            set { this._teleportId = value; }
        }

        public bool IsTeleporting
        {
            get { return this._isTeleporting; }
            set { this._isTeleporting = value; }
        }

        public int TeleportingRoomID
        {
            get { return this._teleportingRoomId; }
            set { this._teleportingRoomId = value; }
        }

        public bool HasSpoken
        {
            get { return this._hasSpoken; }
            set { this._hasSpoken = value; }
        }

        public double LastAdvertiseReport
        {
            get { return this._lastAdvertiseReport; }
            set { this._lastAdvertiseReport = value; }
        }

        public bool AdvertisingReported
        {
            get { return this._advertisingReported; }
            set { this._advertisingReported = value; }
        }

        public bool AdvertisingReportedBlocked
        {
            get { return this._advertisingReportBlocked; }
            set { this._advertisingReportBlocked = value; }
        }

        public bool WiredInteraction
        {
            get { return this._wiredInteraction; }
            set { this._wiredInteraction = value; }
        }

        public bool InventoryAlert
        {
            get { return this._inventoryAlert; }
            set { this._inventoryAlert = value; }
        }

        public bool IgnoreBobbaFilter
        {
            get { return this._ignoreBobbaFilter; }
            set { this._ignoreBobbaFilter = value; }
        }

        public bool WiredTeleporting
        {
            get { return this._wiredTeleporting; }
            set { this._wiredTeleporting = value; }
        }

        public int CustomBubbleId
        {
            get { return this._customBubbleId; }
            set { this._customBubbleId = value; }
        }

        public string NameColor
        {
            get { return this._nameColor; }
            set { this._nameColor = value; }
        }

        public string PrefixName
        {
            get { return this._prefixName; }
            set { this._prefixName = value; }
        }

        public bool OnHelperDuty
        {
            get { return this._onHelperDuty; }
            set { this._onHelperDuty = value; }
        }

        public int FastfoodScore
        {
            get { return this._fastfoodScore; }
            set { this._fastfoodScore = value; }
        }

        public int PetId
        {
            get { return this._petId; }
            set { this._petId = value; }
        }

        public int CreditsUpdateTick
        {
            get { return this._creditsTickUpdate; }
            set { this._creditsTickUpdate = value; }
        }

        public int EnergyUpdate
        {
            get { return this._EnergyUpdate; }
            set { this._EnergyUpdate = value; }
        }

        public int OneMinuteUpdate
        {
            get { return this._OneMinuteUpdate; }
            set { this._OneMinuteUpdate = value; }
        }

        public IChatCommand IChatCommand
        {
            get { return this._iChatCommand; }
            set { this._iChatCommand = value; }
        }

        public DateTime LastGiftPurchaseTime
        {
            get { return this._lastGiftPurchaseTime; }
            set { this._lastGiftPurchaseTime = value; }
        }

        public DateTime LastMottoUpdateTime
        {
            get { return this._lastMottoUpdateTime; }
            set { this._lastMottoUpdateTime = value; }
        }

        public DateTime LastClothingUpdateTime
        {
            get { return this._lastClothingUpdateTime; }
            set { this._lastClothingUpdateTime = value; }
        }

        public DateTime LastForumMessageUpdateTime
        {
            get { return this._lastForumMessageUpdateTime; }
            set { this._lastForumMessageUpdateTime = value; }
        }

        public int GiftPurchasingWarnings
        {
            get { return this._giftPurchasingWarnings; }
            set { this._giftPurchasingWarnings = value; }
        }

        public int MottoUpdateWarnings
        {
            get { return this._mottoUpdateWarnings; }
            set { this._mottoUpdateWarnings = value; }
        }

        public int ClothingUpdateWarnings
        {
            get { return this._clothingUpdateWarnings; }
            set { this._clothingUpdateWarnings = value; }
        }

        public bool SessionGiftBlocked
        {
            get { return this._sessionGiftBlocked; }
            set { this._sessionGiftBlocked = value; }
        }

        public bool SessionMottoBlocked
        {
            get { return this._sessionMottoBlocked; }
            set { this._sessionMottoBlocked = value; }
        }

        public bool SessionClothingBlocked
        {
            get { return this._sessionClothingBlocked; }
            set { this._sessionClothingBlocked = value; }
        }

        public HabboStats GetStats()
        {
            return this._habboStats;
        }
        public RPItems RPItems()
        {
            return this._rpItems;
        }
        public bool InRoom
        {
            get
            {
                return CurrentRoomId >= 1 && CurrentRoom != null;
            }
        }

        public Room CurrentRoom
        {
            get
            {
                if (CurrentRoomId <= 0)
                    return null;

                Room _room = null;
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(CurrentRoomId, out _room))
                    return _room;

                return null;
            }
        }

        public bool CacheExpired()
        {
            TimeSpan Span = DateTime.Now - _timeCached;
            return (Span.TotalMinutes >= 30);
        }

        public string GetQueryString
        {
            get
            {
                this._habboSaved = true;
                return "UPDATE `users` SET `online` = '0', `last_online` = '" + PlusEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + this.Duckets + "', `credits` = '" + this.Credits + "', `vip_points` = '" + this.Diamonds + "', `gotw_points` = '" + this.GOTWPoints + "', `time_muted` = '" + this.TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(this._friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + this._habboStats.RoomVisits + "', `onlineTime` = '" + (PlusEnvironment.GetUnixTimestamp() - SessionStart + this._habboStats.OnlineTime) + "', `respect` = '" + this._habboStats.Respect + "', `respectGiven` = '" + this._habboStats.RespectGiven + "', `giftsGiven` = '" + this._habboStats.GiftsGiven + "', `giftsReceived` = '" + this._habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + this._habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + this._habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + this._habboStats.AchievementPoints + "', `quest_id` = '" + this._habboStats.QuestID + "', `quest_progress` = '" + this._habboStats.QuestProgress + "', `groupid` = '" + this._habboStats.FavouriteGroupId + "',`forum_posts` = '" + this._habboStats.ForumPosts + "',`CashRobbed` = '" + this._habboStats.CashRobbed + "' WHERE `id` = '" + this.Id + "' LIMIT 1;";
            }
        }

        public IWebSocketConnection WebSocketConnection
        {
            get
            {
                if (PlusEnvironment.GetGame().GetWebEventManager() != null)
                    return PlusEnvironment.GetGame().GetWebEventManager().GetUsersConnection(this.GetClient());
                else
                    return null;
            }
        }

        public bool InitProcess()
        {
            this._process = new ProcessComponent();

            if (this._process.Init(this))
                return true;
            return false;
        }

        public bool InitSearches()
        {
            this._navigatorSearches = new SearchesComponent();
            if (this._navigatorSearches.Init(this))
                return true;
            return false;
        }

        public bool InitFX()
        {
            this._fx = new EffectsComponent();
            if (this._fx.Init(this))
                return true;
            return false;
        }

        public bool InitClothing()
        {
            this._clothing = new ClothingComponent();
            if (this._clothing.Init(this))
                return true;
            return false;
        }

        private bool InitPermissions()
        {
            this._permissions = new PermissionComponent();
            if (this._permissions.Init(this))
                return true;
            return false;
        }

        public void InitInformation(UserData data)
        {
            BadgeComponent = new BadgeComponent(this, data);
            Relationships = data.Relations;
        }
        public void Init(GameClient client, UserData data)
        {
            this.Achievements = data.achievements;

            this.FavoriteRooms = new ArrayList();
            foreach (int id in data.favouritedRooms)
            {
                FavoriteRooms.Add(id);
            }

            this.MutedUsers = data.ignores;

            this._client = client;
            BadgeComponent = new BadgeComponent(this, data);
            InventoryComponent = new InventoryComponent(Id, client);

            quests = data.quests;

            Messenger = new HabboMessenger(Id);
            Messenger.Init(data.friends, data.requests);
            this._friendCount = Convert.ToInt32(data.friends.Count);
            this._disconnected = false;
            UsersRooms = data.rooms;
            Relationships = data.Relations;

            this.InitSearches();
            this.InitFX();
            this.InitClothing();
        }

        public PermissionComponent GetPermissions()
        {
            return this._permissions;
        }

        public void OnDisconnect()
        {

            if (this._disconnected)
                return;

            try
            {
                if (this._process != null)
                    this._process.Dispose();
            }
            catch { }

            this._disconnected = true;


            IServiceProvider discordServices = PlusEnvironment.GetGame().GetDiscordManager().GetDiscordServices();

            var userservice = discordServices.GetRequiredService<HRPUserService>();

            _ = userservice.DisconnectUser(this);

            PlusEnvironment.GetGame().GetClientManager().UnregisterClient(Id, Username);

            if (!this._habboSaved)
            {
                this._habboSaved = true;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `users` SET `online` = '0', `last_online` = '" + PlusEnvironment.GetUnixTimestamp() + "', `activity_points` = '" + this.Duckets + "', `credits` = '" + this.Credits + "', `vip_points` = '" + this.Diamonds + "', `gotw_points` = '" + this.GOTWPoints + "', `time_muted` = '" + this.TimeMuted + "',`friend_bar_state` = '" + FriendBarStateUtility.GetInt(this._friendbarState) + "' WHERE id = '" + Id + "' LIMIT 1;UPDATE `user_stats` SET `roomvisits` = '" + this._habboStats.RoomVisits + "', `onlineTime` = '" + (PlusEnvironment.GetUnixTimestamp() - this.SessionStart + this._habboStats.OnlineTime) + "', `respect` = '" + this._habboStats.Respect + "', `respectGiven` = '" + this._habboStats.RespectGiven + "', `giftsGiven` = '" + this._habboStats.GiftsGiven + "', `giftsReceived` = '" + this._habboStats.GiftsReceived + "', `dailyRespectPoints` = '" + this._habboStats.DailyRespectPoints + "', `dailyPetRespectPoints` = '" + this._habboStats.DailyPetRespectPoints + "', `AchievementScore` = '" + this._habboStats.AchievementPoints + "', `quest_id` = '" + this._habboStats.QuestID + "', `quest_progress` = '" + this._habboStats.QuestProgress + "', `groupid` = '" + this._habboStats.FavouriteGroupId + "',`forum_posts` = '" + this._habboStats.ForumPosts + "',`CashRobbed` = '" + this._habboStats.CashRobbed + "' WHERE `id` = '" + this.Id + "' LIMIT 1;");

                    if (GetPermissions().HasRight("mod_tickets"))
                        dbClient.RunQuery("UPDATE `moderation_tickets` SET `status` = 'open', `moderator_id` = '0' WHERE `status` ='picked' AND `moderator_id` = '" + Id + "'");
                }
            }
            this.Dispose();
            this._client = null;
        }

        public string getNameOfThisGang(int Id)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT name FROM `gang` WHERE id = @gangId");
                dbClient.AddParameter("gangId", Id);
                string GangName = dbClient.getString();
                return GangName;
            }
        }

        public string getNameOfGang()
        {
            if (this.Gang == 0)
                return "%null%";

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT name FROM `gang` WHERE id = @gangId");
                dbClient.AddParameter("gangId", this.Gang);
                string GangName = dbClient.getString();
                return GangName;
            }
        }

        public int getOwnerOfGang()
        {
            if (this.Gang == 0)
                return 0;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT owner FROM `gang` WHERE id = @gangId");
                dbClient.AddParameter("gangId", this.Gang);
                int OwnerUser = dbClient.getInteger();
                return OwnerUser;
            }
        }

 

        public int countUserOfGang()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT COUNT(0) FROM `users` WHERE `gang` = @gang_id");
                dbClient.AddParameter("gang_id", this.Gang);
                int GangCount = dbClient.getInteger();
                return GangCount;
            }
        }

        public void insertLastAction(string Action)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO last_actions (user_id, action_maked) VALUES (@user_id, @action);");
                dbClient.AddParameter("user_id", this.Id);
                dbClient.AddParameter("action", Action);
                dbClient.RunQuery();
            }
        }

        public void createGang(string Name)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO gang (name, owner, created) VALUES (@name, '" + this.Id + "',@time);");
                dbClient.AddParameter("name", Name);
                dbClient.AddParameter("time", PlusEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();

                dbClient.SetQuery("SELECT id FROM `gang` ORDER by id DESC LIMIT 1");
                int gangId = dbClient.getInteger();

                dbClient.SetQuery("UPDATE users SET gang = @gang_id, gang_rank = 1 WHERE id = @userId");
                dbClient.AddParameter("gang_id", gangId);
                dbClient.AddParameter("userId", this.Id);
                dbClient.RunQuery();

                dbClient.SetQuery("INSERT INTO gang_ranks (gang_id, rank_id, list) VALUES (@gang, '1', '1');");
                dbClient.AddParameter("gang", gangId);
                dbClient.RunQuery();

                this.Gang = gangId;
                this.GangRank = 1;
            }
        }

        public void stopWork()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);

            this.Working = false;
            User.GetClient().SendWhisper("You have stopped your shift");
            this.resetAvatarEvent();

            if (this.JobId == 1)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(GetClient(), "police-calls;hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(GetClient(), "police-calls-icon;hide");
            }
            else if (this.JobId == 2 && this.UsingParamedic)
            {
                RoomUser ParamedicUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.ParamedicUsername);
                if (ParamedicUser == null)
                    return;

                ParamedicUser.GetClient().GetHabbo().WaitingForParamedicFrom = null;
                ParamedicUser.GetClient().GetHabbo().IsWaitingForParamedic = false;
                ParamedicUser.SuperFastWalking = false;
                ParamedicUser.AllowOverride = false;

                ParamedicUser.GetClient().GetHabbo().Hospital = 1;
                ParamedicUser.GetClient().GetHabbo().updateHospitalEtat(ParamedicUser, 3);

                this.UsingParamedic = false;
                this.ParamedicUsername = null;
                User.FastWalking = false;
                this.resetEffectEvent();
            }
        }

        public void RestartWork()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);

            if (this.Working)
            {
                this.Working = false;
                this.resetAvatarEvent();
                this.updateAvatarEvent(this.RankInfo.Look_F, this.RankInfo.Look_H, "[WORKING] " + this.RankInfo.Name + ", " + this.TravailInfo.Name);
                this.Working = true;
                this.GetClient().SendWhisper("You have started your shift");
                this.RPCache(1);
            }
        }

        public void HPBarley()
        {
            
            this.StopAction();
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);

            if (this.GetClient().GetRoleplay().Health < 20 && this.GetClient().GetRoleplay().Health > 10 && !this.HP20)
            {
                this.HP20 = true;
                User.Say("barely manages to keep upright", 5, true);
                User.LastBubble = 5;
            }
            else if (this.GetClient().GetRoleplay().Health < 10 && this.GetClient().GetRoleplay().Health > 0 && !this.HP10)
            {
                this.HP10 = true;
                User.Say("barely manages to keep upright", 3, true);
                User.LastBubble = 3;
            }
            else if (this.GetClient().GetRoleplay().Health <= 0)
            {
                this.GetClient().GetRoleplay().Health = this.GetClient().GetRoleplay().Health - this.GetClient().GetRoleplay().Health;
                this.GetClient().GetHabbo().updateHealth();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "my_stats;" + this.GetClient().GetHabbo().Credits + ";" + this.GetClient().GetHabbo().Duckets + ";" + this.GetClient().GetHabbo().EventPoints);


              

                this.HP20 = false;
                this.HP10 = false;
                this.isBleeding = false;

                System.Timers.Timer Timer = new System.Timers.Timer(100);
                Timer.Interval = 100;
                Timer.Elapsed += delegate
                {
                    if (!User.Statusses.ContainsValue("lay") && User.Z != 0.35)
                    {
                        User.Freezed = true;
                        if (User.RotBody % 2 == 0)
                        {
                            if (User != null)
                            {
                                try
                                {
                                    User.Statusses.Add("lay", "1.0 null");
                                    User.Z -= 0.35;
                                    User.isLying = true;
                                    User.UpdateNeeded = true;
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            User.RotBody--;
                            User.Statusses.Add("lay", "1.0 null");
                            User.Z -= 0.35;
                            User.isLying = true;
                            User.UpdateNeeded = true;
                        }
                    }
                    // since the player is lying, call the paramedic
                    this.IsWaitingForParamedic = true;
                    PlusEnvironment.GetGame().GetClientManager().ParamedicCall(this.Username, this.CurrentRoom.Name);
                    Timer.Stop();
                };
                Timer.Start();

                #region LastKillFrom
                if (this.LastHitFrom != null)
                {
                    RoomUser LastHitFromUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.LastHitFrom);

                    if (LastHitFromUser != null)
                    {
                        string charge = null;
                        if (this.JobId == 1 && this.Working)
                        {
                            charge = "copmurder";
                            LastHitFromUser.GetClient().GetHabbo().copmurder += 1;
                        }
                        else if (this.Cuffed)
                        {
                            charge = "execution";
                            LastHitFromUser.GetClient().GetHabbo().execution += 1;
                        }
                        else if (LastHitFromUser.GetClient().GetHabbo().Gang == this.Gang && LastHitFromUser.GetClient().GetHabbo().ganghomicide == 0 || this.JobId == 1 && !this.Working && LastHitFromUser.GetClient().GetHabbo().ganghomicide == 0)
                        {
                            charge = "ganghomicide";
                            LastHitFromUser.GetClient().GetHabbo().ganghomicide += 1;
                        }
                        else
                        {
                            charge = "murder";
                            LastHitFromUser.GetClient().GetHabbo().murder += 1;
                        }

                        if (charge != null)
                        {
                            //astHitFromUser.GetClient().GetHabbo().AddToWantedList(charge);
                            PlusEnvironment.GetGame().GetClientManager().LoadWantedList();
                            LastHitFromUser.GetClient().SendWhisper("You have been charged " + charge, 7);
                            PlusEnvironment.GetGame().GetClientManager().PoliceRadio(LastHitFromUser.GetClient().GetHabbo().Username + " charged with " + charge + " @ " + LastHitFromUser.GetClient().GetHabbo().CurrentRoom.Name);
                        }

                        if (LastHitFromUser.GetClient().GetHabbo().Gang != 0 && this.Gang == 0)
                        {
                            LastHitFromUser.GetClient().GetHabbo().updateGangKill();
                            this.updateGangDeaths();
                        }

                        LastHitFromUser.GetClient().GetRoleplay().Kills += 1;
                        LastHitFromUser.GetClient().GetHabbo().updateKills();
                        this.GetClient().GetRoleplay().Deaths += 1;

                        int Credits = 0;
                        if (this.Rank < 3 && this.Hospital == 0)
                        {
                            if (this.usingSuicide)
                            {
                                Credits = this.Credits;
                            }
                            else
                            {
                                decimal decimalCredits = Convert.ToInt32(this.Credits);
                                Credits = Convert.ToInt32((decimalCredits / 100m) * 50m);
                            }
                        }
                        if (Credits > 0)
                        {
                            this.Credits -= Credits;
                            this.GetClient().SendMessage(new CreditBalanceComposer(this.Credits));
                            this.RPCache(3);
                            LastHitFromUser.GetClient().GetHabbo().Credits += Credits;
                            LastHitFromUser.GetClient().SendMessage(new CreditBalanceComposer(this.Credits));
                            LastHitFromUser.GetClient().GetHabbo().RPCache(3);

                            if (LastHitFromUser.GetClient().GetRoleplay().CustomKOMessage != "")
                            {
                                LastHitFromUser.Say("ª " + LastHitFromUser.GetClient().GetRoleplay().CustomKOMessage + " " + this.Username + " and steals " + PlusEnvironment.ConvertToPrice(Credits) + " dollars", 0, true);
                            }
                            else
                            {
                                LastHitFromUser.Say("ª lands the final blow on " + this.Username + " and steals " + PlusEnvironment.ConvertToPrice(Credits) + " dollars", 0, true);
                            }

                            #region Escorting
                            if (this.Escorting)
                            {
                                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.EscortUsername);
                                RoomUser TargetUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

                                TargetUser.UltraFastWalking = false;
                                TargetClient.GetHabbo().Escort = false;
                                TargetClient.GetHabbo().EscortBy = null;
                                TargetClient.GetHabbo().Cuffed = false;
                                TargetClient.GetHabbo().resetAvatarEvent();

                                this.Escorting = false;
                                this.EscortUsername = null;
                            }
                            #endregion

                            if (this.JobId == 1 && this.Working)
                            {
                                this.stopWork();
                                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + LastHitFromUser.GetClient().GetHabbo().Username + "</span> knocked out on-duty cop <span class=\"red\">" + this.Username + "</span>, stealing $" + Credits);
                                Webhook.SendWebhook(":punch: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** has knocked out on-duty cop **" + this.Username + "**, stealing $" + PlusEnvironment.ConvertToPrice(Credits));
                            }
                            else if (this.Cuffed)
                            {
                                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.EscortBy);
                                RoomUser TargetUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

                                TargetClient.GetHabbo().Escorting = false;
                                TargetClient.GetHabbo().EscortUsername = null;

                                this.Escort = false;
                                this.EscortBy = null;
                                this.Cuffed = false;
                                this.resetAvatarEvent();

                                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + LastHitFromUser.GetClient().GetHabbo().Username + "</span> executed <span class=\"red\">" + this.Username + "</span>, stealing $" + Credits);
                                Webhook.SendWebhook(":punch: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** executed **" + this.Username + "**, stealing $" + PlusEnvironment.ConvertToPrice(Credits));
                            }
                            else
                            {
                                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + LastHitFromUser.GetClient().GetHabbo().Username + "</span> knocked out <span class=\"red\">" + this.Username + "</span>, stealing $" + Credits);
                                Webhook.SendWebhook(":punch: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** has knocked out **" + this.Username + "**, stealing $" + PlusEnvironment.ConvertToPrice(Credits));
                            }
                        }
                        else
                        {
                            if (LastHitFromUser.GetClient().GetRoleplay().CustomKOMessage != "")
                            {
                                LastHitFromUser.Say("ª " + LastHitFromUser.GetClient().GetRoleplay().CustomKOMessage + " " + this.Username, 0, true);
                            }
                            else
                            {
                                LastHitFromUser.Say("ª lands the final blow on " + this.Username, 0, true);
                            }

                            #region Escorting
                            if (this.Escorting)
                            {
                                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.EscortUsername);
                                RoomUser TargetUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

                                TargetUser.UltraFastWalking = false;
                                TargetClient.GetHabbo().Escort = false;
                                TargetClient.GetHabbo().EscortBy = null;
                                TargetClient.GetHabbo().Cuffed = false;
                                TargetClient.GetHabbo().resetAvatarEvent();

                                this.Escorting = false;
                                this.EscortUsername = null;
                            }
                            #endregion

                            if (this.JobId == 1 && this.Working)
                            {
                                this.stopWork();
                                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + LastHitFromUser.GetClient().GetHabbo().Username + "</span> knocked out on-duty cop <span class=\"red\">" + this.Username + "</span>");
                                Webhook.SendWebhook(":punch: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** has knocked out on-duty cop **" + this.Username + "**");
                            }
                            else if (this.Cuffed)
                            {
                                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.EscortBy);
                                RoomUser TargetUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

                                TargetClient.GetHabbo().Escorting = false;
                                TargetClient.GetHabbo().EscortUsername = null;

                                this.Escort = false;
                                this.EscortBy = null;
                                this.Cuffed = false;
                                this.resetAvatarEvent();

                                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + LastHitFromUser.GetClient().GetHabbo().Username + "</span> executed <span class=\"red\">" + this.Username + "</span>");
                                Webhook.SendWebhook(":punch: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** executed **" + this.Username + "**");
                            }
                            else
                            {
                                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + LastHitFromUser.GetClient().GetHabbo().Username + "</span> knocked out <span class=\"red\">" + this.Username + "</span>");
                                Webhook.SendWebhook(":punch: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** has knocked out **" + this.Username + "**");
                            }
                        }
                        this.GetClient().SendWhisper("You have died and lost " + Credits + " dollars");
                       
                        if (this.BountyTimer != 0)
                        {
                            DataRow Bounty = null;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("SELECT * FROM `bounties` WHERE `user_id` = @id LIMIT 1");
                                dbClient.AddParameter("id", this.Id);
                                Bounty = dbClient.getRow();
                            }

                            PlusEnvironment.GetGame().GetClientManager().HotelWhisper(LastHitFromUser.GetClient().GetHabbo().Username + " claimed $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " bounty for killing " + this.Username);
                            Webhook.SendWebhook(":moneybag: **" + LastHitFromUser.GetClient().GetHabbo().Username + "** claimed $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " bounty for killing **" + this.Username + "**");

                            LastHitFromUser.GetClient().GetHabbo().Credits += Convert.ToInt32(Bounty["amount"]);
                            LastHitFromUser.GetClient().SendMessage(new CreditBalanceComposer(LastHitFromUser.GetClient().GetHabbo().Credits));
                            LastHitFromUser.GetClient().GetHabbo().RPCache(3);
                            LastHitFromUser.GetClient().SendWhisper("You have received $" + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Bounty["amount"])) + " for killing " + this.Username);

                            this.BountyTimer = 0;
                            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                dbClient.SetQuery("DELETE FROM bounties WHERE user_id = '" + this.Id + "';");
                                dbClient.RunQuery();
                            }
                        }
                    }
                }
                #endregion
                this.GetClient().GetRoleplay().Deaths += 1;
                this.updateDeaths();

                System.Timers.Timer DieTimerUser = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(90));
                DieTimerUser.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(90);
                DieTimerUser.Elapsed += delegate
                {
                    if (this.IsWaitingForParamedic && this.WaitingForParamedicFrom == null && this.Hospital == 0)
                    {
                        this.Hospital = 1;
                        this.updateHospitalEtat(User, 3);
                    }
                    DieTimerUser.Stop();
                };
                DieTimerUser.Start();
            }
        }

        public void UpdateDamageDealt(int Dealt)
        {
            this.GetClient().GetRoleplay().DamageDealt += Dealt;

            Random RandomTimer = new Random();
            int Timer = RandomTimer.Next(1, 5);
            int Time = 0;

            if (Timer == 1)
            {
                Time = 5;
            }
            else if (Timer == 2)
            {
                Time = 15;
            }
            else if (Timer == 3)
            {
                Time = 25;
            }
            else if (Timer == 4)
            {
                Time = 30;
            }

            System.Timers.Timer UpdateTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(Time));
            UpdateTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(Time);
            UpdateTimer.Elapsed += delegate
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE user_rp_stats SET damage_dealt = damage_dealt + " + Dealt + " WHERE `user_id` = @uid LIMIT 1;");
                    dbClient.AddParameter("uid", this.Id);
                    dbClient.RunQuery();
                }
                UpdateTimer.Stop();
            };
            UpdateTimer.Start();
        }

        public void UpdateDamageTaken(int Taken)
        {
            this.GetClient().GetRoleplay().DamageTaken += Taken;

            Random RandomTimer = new Random();
            int Timer = RandomTimer.Next(1, 5);
            int Time = 0;

            if (Timer == 1)
            {
                Time = 5;
            }
            else if (Timer == 2)
            {
                Time = 15;
            }
            else if (Timer == 3)
            {
                Time = 25;
            }
            else if (Timer == 3)
            {
                Time = 30;
            }

            System.Timers.Timer UpdateTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(Time));
            UpdateTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(Time);
            UpdateTimer.Elapsed += delegate
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE user_rp_stats SET damage_taken = damage_taken + " + Taken + " WHERE `user_id` = @uid LIMIT 1;");
                    dbClient.AddParameter("uid", this.Id);
                    dbClient.RunQuery();
                }
                UpdateTimer.Stop();
            };
            UpdateTimer.Start();
        }

        public void RotateEffect()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            int Rot = User.RotBody;
            System.Timers.Timer Timer = new System.Timers.Timer(200);
            Timer.Interval = 200;
            Timer.Elapsed += delegate
            {
                if (Rot == 1)
                {
                    Rot = 2;
                }
                else if (Rot == 2)
                {
                    Rot = 2;
                }
                else if (Rot == 3)
                {
                    Rot = 4;
                }
                else if (Rot == 4)
                {
                    Rot = 5;
                }
                else if (Rot == 5)
                {
                    Rot = 6;
                }
                else if (Rot == 6)
                {
                    Rot = 7;
                }
                else if (Rot == 7)
                {
                    Rot = 1;
                }
                User.SetRot(Rot, false);
                User.UpdateNeeded = true;
            };
            Timer.Start();
        }
        public void StopAction()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (this.usingTrash)
            {
                this.PlayToken = 0;
                this.usingTrash = false;
                User.Say("stops trawling through the bin");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;hide");
            }
            else if (this.isUsingPoliceCar)
            {
                User.Say("hops out of their squad car");
                this.PlayToken = 0;
                this.isUsingPoliceCar = false;
            }
            else if (this.usingMarketplace)
            {
                this.GetClient().GetHabbo().usingMarketplace = false;
                this.GetClient().GetHabbo().usingManageSales = false;
                this.GetClient().GetHabbo().usingCreateSale = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "marketplace;hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "manage-sales;hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "create-sale;hide");
            }
        }

        public void Stun()
        {
            this.StopAction();
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);

            User.Statusses.Clear();
            User.Freezed = true;
            this.Stunned = true;
            User.ApplyEffect(53);

            if (this.Escorting)
            {
                GameClient StunnedBy = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.StunnedBy);
                GameClient Escorting = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.EscortUsername);
                RoomUser EscortUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Escorting.GetHabbo().Id);

                User.Say("feels dizzy and lets go of " + Escorting.GetHabbo().Username);
                EscortUser.UltraFastWalking = false;
                Escorting.GetHabbo().EscortBy = null;
                this.Escorting = false;
                this.EscortUsername = null;
                this.StunnedBy = null;
            }

            System.Timers.Timer RemoveStunTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(4.5));
            RemoveStunTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(4.5);
            RemoveStunTimer.Elapsed += delegate
            {
                User.Freezed = false;
                this.Stunned = false;
                this.resetEffectEvent();
                RemoveStunTimer.Stop();
            };
            RemoveStunTimer.Start();
        }

        public bool CheckWarnings()
        {
            Room Room = this.CurrentRoom;
            if (Room == null)
                return true;

            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);

            if (this.GetClient().GetRoleplay().Passive)
            {
                GetClient().SendWhisper("You cannot perform this action while in passive mode");
                return true;
            }
            else if (this.GetClient().GetRoleplay().GP > 0)
            {
                GetClient().SendWhisper("You cannot perform this action while in god-protection");
                return true;
            }
            else if (this.Stunned)
            {
                GetClient().SendWhisper("You cannot perfrom this action while stunned");
                return true;
            }
            else if (this.Cuffed)
            {
                GetClient().SendWhisper("You cannot perform this action while cuffed");
                return true;
            }
            else if (!Room.Fightable && GetClient().GetHabbo().AggressionToken == 0)
            {
                GetClient().SendWhisper("You cannot perform this action in safezone rooms with no aggression");
                return true;
            }
            else if (this.IsWaitingForParamedic)
            {
                GetClient().SendWhisper("You can not perform this action while dead");
                return true;
            }
            else if (this.Hospital == 1)
            {
                GetClient().SendWhisper("You can not perform this action while unconscious");
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CheckSafezone(string Username)
        {
            Room Room = this.CurrentRoom;
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Username);

            if (TargetUser == null || TargetUser.GetClient().GetHabbo().CurrentRoom != this.CurrentRoom)
            {
                this.GetClient().SendWhisper("Player not found in this room");
                return true;
            }
            else if (TargetUser.GetClient().GetHabbo().Id == this.Id)
            {
                return true;
            }
            else if (TargetUser.OnDuty)
            {
                this.GetClient().SendWhisper("You cannot perform this action to players while they're on duty");
                return true;
            }
            else if (TargetUser.GetClient().GetRoleplay().Passive)
            {
                this.GetClient().SendWhisper("You cannot perform this action to players while they're passive");
                return true;
            }
            else if (TargetUser.GetClient().GetRoleplay().GP >= 1)
            {
                this.GetClient().SendWhisper("You cannot perform this action to players while they're in god protection");
                return true;
            }
            else if (TargetUser.IsAsleep)
            {
                this.GetClient().SendWhisper("You cannot perform this action to players while they are afk");
                return true;
            }
            else if (!Room.Fightable && TargetUser.GetClient().GetHabbo().AggressionToken == 0)
            {
                this.GetClient().SendWhisper("You cannot perform this action to a player with no aggression in a safezone room");
                return true;
            }
            else if (TargetUser.GetClient().GetHabbo().Hospital == 1 || TargetUser.GetClient().GetHabbo().IsWaitingForParamedic)
            {
                this.GetClient().SendWhisper("You cannot perform this action to a dead player");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateAggression(int Precent)
        {

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1, 9999);
            this.AggressionToken = tokenNumber;
            this.GetClient().GetRoleplay().Aggression = Precent;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "aggression;" + this.GetClient().GetRoleplay().Aggression);

            #region Aggression Timer
            System.Timers.Timer AggressionTimer1 = new System.Timers.Timer(500);
            AggressionTimer1.Interval = 500;
            AggressionTimer1.Elapsed += delegate
            {
                if (this.AggressionToken == tokenNumber && !this.isInTurf)
                {
                    if (this.GetClient().GetRoleplay().Aggression <= 0)
                    {
                        this.GetClient().GetRoleplay().Aggression = 0;
                        this.AggressionToken = 0;
                        AggressionTimer1.Stop();
                    }
                    else
                    {
                        this.GetClient().GetRoleplay().Aggression -= 1;
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "aggression;" + this.GetClient().GetRoleplay().Aggression);
                        RoleplayManager.UpdateTargetStats(this.Id);
                        AggressionTimer1.Start();
                    }
                }
            };
            AggressionTimer1.Start();
            #endregion

            RoleplayManager.UpdateTargetStats(this.Id);
        }

        public int CheckChargess()
        {
            int Minutes = 0;

            for (int i = 1; i <= this.assault; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.murder; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.ganghomicide; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.copassault; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.copmurder; i++)
            {
                Minutes += 12;
            }
            for (int i = 1; i <= this.obstruction; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.hacking; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.trespassing; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.robbery; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.illegalarea; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.jailbreak; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.terrorism; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.drugs; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.execution; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.escaping; i++)
            {
                Minutes += 6;
            }
            for (int i = 1; i <= this.nonCompliance; i++)
            {
                Minutes += 3;
            }
            for (int i = 1; i <= this.callAbuse; i++)
            {
                Minutes += 3;
            }

            if (Minutes > 45)
            {
                Minutes = 45;
            }

            return Minutes;
        }

        public void Warp(int X, int Y)
        {
            RoomUser roomUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            Room Room = this.CurrentRoom;
            Room.GetGameMap().UpdateUserMovement(new Point(roomUser.X, roomUser.Y), new Point(X, Y), roomUser);
            roomUser.X = X;
            roomUser.Y = Y;
            roomUser.UpdateNeeded = true;
            roomUser.ClearMovement(true);
        }
        public void SetRot(int rot, bool headonly = false)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (headonly == false)
                User.RotBody = rot;
            User.RotHead = rot;
            User.UpdateNeeded = true;
        }

        public void updateGangXP(int Xp)
        {
            if (this.Gang == 0)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE gang SET xp = xp + @new_xp WHERE id = @gangId");
                dbClient.AddParameter("new_xp", Xp);
                dbClient.AddParameter("id", this.Gang);
                dbClient.RunQuery();
            }
        }
        public void EndPrison()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            this.Timer = 0;
            this.updateTimer();
            this.Prison = 0;
            this.updatePrison();
            User.isFarmingRock = 0;
            this.resetAvatarEvent();
            this.resetEffectEvent();
            //Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(98);
            this.CanUseJailGate = true;
            //this.sendToPrisonChair(User, Room, 2536);
            User.Say("ends their time in prison");
            this.GetClient().SendWhisper("You have been released from jail");
        }

        public void updatePrisonEtat(RoomUser User, int Timer, int Etat)
        {
            if (this.Working == true)
            {
                this.Working = false;
            }

            this.Timer = Timer;
            this.updateTimer();
            this.Prison = Etat;
            this.updatePrison();
            this.sendToPrison(User, 0, Etat);
        }

        public void sendToPrisonChair(RoomUser User, Room Room, int SpriteId)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                List<Item> Beds = new List<Item>();
                List<int> BedsId = new List<int>();

                foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                {
                    if (item.GetBaseItem().SpriteId == SpriteId)
                    {
                        if (!Beds.Contains(item))
                            Beds.Add(item);

                        if (!BedsId.Contains(item.Id))
                            BedsId.Add(item.Id);
                    }
                }

                if (this.CurrentRoomId != Room.Id)
                {
                    Random rnd = new Random();
                    int random = BedsId[rnd.Next(BedsId.Count)];

                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = Room.Id;
                    User.GetClient().GetHabbo().TeleporterId = random;

                    User.GetClient().GetHabbo().CanChangeRoom = true;
                    
                    RoleplayManager.InstantRL(User.GetClient(), User.GetClient().GetHabbo().TeleportingRoomID);
                }
                else
                {
                    Random rnd = new Random();
                    Item random = Beds[rnd.Next(Beds.Count)];
                    Room.GetGameMap().TeleportToItem(User, random);
                    Room.GetRoomUserManager().UpdateUserStatusses();
                }
            }
        }

        public void endHospital(GameClient Session, int Paye)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            this.resetAvatarEvent();
            this.Timer = 0;
            this.updateTimer();
            this.Hospital = 0;
            this.updateHospital();
            
            this.IsWaitingForParamedic = false;
            this.GetClient().GetRoleplay().Health = this.GetClient().GetRoleplay().HealthMax;
            Session.SendWhisper("You have recovered from your injuries, stay safe out there!", 34);
            this.LastHitFrom = null;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
            

        }

        public void TeleportToCourtSeats(RoomUser User, Room Room, int SpriteId = 2647)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                List<Item> Beds = new List<Item>();
                List<int> BedsId = new List<int>();

                foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                {
                    if (item.GetBaseItem().SpriteId == SpriteId)
                    {
                        if (!Beds.Contains(item))
                            Beds.Add(item);

                        if (!BedsId.Contains(item.Id))
                            BedsId.Add(item.Id);
                    }
                }

                Random rnd = new Random();
                Item random = Beds[rnd.Next(Beds.Count)];
                Room.GetGameMap().TeleportToItem(User, random);
                Room.GetRoomUserManager().UpdateUserStatusses();
            }
        }

        public void updateHospitalEtat(RoomUser User, int Timer)
        {
            if (this.Working)
                this.stopWork();

            this.Timer = Timer;
            this.updateTimer();
            this.Hospital = 1;
            this.updateHospital();
            this.sendToHospital(User);
        }

        public void updateSuicide(RoomUser User, int Timer)
        {
            if (this.Working)
                this.stopWork();

            this.Timer = Timer;
            this.updateTimer();
           

        }

        public void sendToHospital(RoomUser User)
        {
            User.GetClient().GetHabbo().Health += User.GetClient().GetHabbo().HealthMax;
            User.GetClient().GetHabbo().updateHealth();
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "my_stats;" + User.GetClient().GetHabbo().Credits + ";" + User.GetClient().GetHabbo().Duckets + ";" + User.GetClient().GetHabbo().EventPoints);

           

            RoleplayManager.InstantRL(User.GetClient(), 63);

        }

        public void sendToPrison(RoomUser User, int AppartAutoriser, int Etat)
        {
            this.updateAvatarEvent("ch-6161865-94.hr-100-100.lg-50858737-94.hd-629-2", "ch-6165820-94.hr-100-0.lg-3116-94-94.ha-1002-70.hd-180-7", "[Arrested]");
            if (AppartAutoriser != 18 && AppartAutoriser != 20)
            {
                Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(99);
                if (Etat == 2)
                {
                    this.sendToPrisonChair(User, Room, 2500);
                }
                else
                {
                    this.sendToPrisonChair(User, Room, 2500);
                }
            }
        }

        public void sendToHospitalBed(RoomUser User, Room Room)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                List<Item> Beds = new List<Item>();
                List<int> BedsId = new List<int>();

                foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                {
                    if (item.GetBaseItem().SpriteId == 3590)
                    {
                        if (!Beds.Contains(item))
                            Beds.Add(item);

                        if (!BedsId.Contains(item.Id))
                            BedsId.Add(item.Id);
                    }
                }

                if (this.CurrentRoomId != Room.Id)
                {
                    Random rnd = new Random();
                    int random = BedsId[rnd.Next(BedsId.Count)];

                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = Room.Id;
                    User.GetClient().GetHabbo().TeleporterId = random;

                    User.GetClient().GetHabbo().CanChangeRoom = true;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "my_stats;" + User.GetClient().GetHabbo().Credits + ";" + User.GetClient().GetHabbo().Duckets + ";" + User.GetClient().GetHabbo().EventPoints);

                    RoleplayManager.InstantRL(User.GetClient(), User.GetClient().GetHabbo().TeleportingRoomID);
                }
                else
                {
                    Random rnd = new Random();
                    Item random = Beds[rnd.Next(Beds.Count)];
                    Room.GetGameMap().TeleportToItem(User, random);
                    Room.GetRoomUserManager().UpdateUserStatusses();
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "my_stats;" + User.GetClient().GetHabbo().Credits + ";" + User.GetClient().GetHabbo().Duckets + ";" + User.GetClient().GetHabbo().EventPoints);

                }
            }
        }

        public void updateTimer()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET timer = @timer WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("timer", this.Timer);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateHospital()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET hospital = @hospital WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("hospital", this.Hospital);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePrison()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET prison = @prison WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("prison", this.Prison);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateGang()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET gang = @gang WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gang", this.Gang);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateGangRank()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET gang_rank = @gang_rank WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("gang_rank", this.GangRank);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateGangHeists()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE gang SET heists = heists + 1 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Gang);
                dbClient.RunQuery();
            }
        }

        public int GetNumberOfArmes()
        {
            int Number = 0;
            if (this.Ak47 == 1)
                Number = Number + 1;

            if (this.Bat == 1)
                Number = Number + 1;

            if (this.Uzi == 1)
                Number = Number + 1;

            if (this.Sabre == 1)
                Number = Number + 1;

            return Number;
        }

        public void updateForfaitTel()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telephoneForfaitTel = @telephoneForfaitTel WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telephoneForfaitTel", this.TelephoneForfait);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateForfaitSms()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telForfaitSms = @telForfaitSms WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telForfaitSms", this.TelephoneForfaitSms);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateForfaitType()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telForfaitType = @telForfaitType WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telForfaitType", this.TelephoneForfaitType);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateForfaitReset()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telForfaitReset = @telForfaitReset WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telForfaitReset", this.TelephoneForfaitReset);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateAudiA8()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET audia8 = @audia8 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("audia8", this.AudiA8);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePorsche911()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET porsche911 = @porsche911 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("porsche911", this.Porsche911);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateFiatPunto()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET fiatpunto = @fiatpunto WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("fiatpunto", this.FiatPunto);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateVolkswagenJetta()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET volkswagenjetta = @volkswagenjetta WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("volkswagenjetta", this.VolkswagenJetta);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateBMWI8()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET bmwi8 = @bmwi8 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("bmwi8", this.BmwI8);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateLook()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET look = @look WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("look", this.Look);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateKill()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET kills = @kills WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("kills", this.Kills);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateTelephone()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telephone = @telephone WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telephone", this.Telephone);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateTelephoneName()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telephone_name = @telephone_name WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telephone_name", this.TelephoneName);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateTelephoneForfaitSms()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET telForfaitSms = @telForfaitSms WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("telForfaitSms", this.TelephoneForfaitSms);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateUzi()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET uzi = @uzi WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("uzi", this.Uzi);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateBat()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET bat = @bat WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("bat", this.Bat);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateSabre()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET sabre = @sabre WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("sabre", this.Sabre);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateAk47()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET ak47 = @ak47 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("ak47", this.Ak47);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateAK47Munitions()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET ak47_munitions = @ak47_munitions WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("ak47_munitions", this.AK47_Munitions);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateUziMunitions()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET uzi_munitions = @uzi_munitions WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("uzi_munitions", this.Uzi_Munitions);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateGangKill()
        {
            if (this.Gang == 0)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE gang SET kills = kills + 1 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Gang);
                dbClient.RunQuery();
            }
        }

        public void updateGangDeaths()
        {
            if (this.Gang == 0)
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE gang SET deaths = deaths + 1 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Gang);
                dbClient.RunQuery();
            }
        }

        public void updateMutuelle()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET mutuelle = @mutuelle WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("mutuelle", this.Mutuelle);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateMutuelleDate()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET mutuelle_expiration = @mutuelle_date WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("mutuelle_date", this.MutuelleDate);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCoca()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET coca = @coca WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("coca", this.Coca);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePain()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET pain = @pain WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("pain", this.Pain);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateDoliprane()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET doliprane = @doliprane WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("doliprane", this.Doliprane);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePierre()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET pierre = @pierre WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("pierre", this.Pierre);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCarte()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET carte = @carte WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("carte", this.Carte);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateFacebook()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET facebook = @facebook WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("facebook", this.Facebook);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePermisArme()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET permis_arme = @permis_arme WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("permis_arme", this.Permis_arme);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateClipper()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET clipper = @clipper WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("clipper", this.Clipper);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateWeed()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET weed = @weed WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("weed", this.Weed);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePhilipMo()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET philipmo = @philipmo WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("philipmo", this.PhilipMo);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateEventDay()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET event_day = @eventday WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("eventday", this.EventDay);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateEventPoints()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET event_points = @eventpoints WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("eventpoints", this.EventPoints);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCasinoJetons()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET casino_jetons = @casinojetons WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("casinojetons", this.Casino_Jetons);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }


        public void updateQuizzPoint()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET quizz_points = @quizz_points WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("quizz_points", this.Quizz_Points);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCocktails()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET cocktails = @cocktails WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("cocktails", this.Cocktails);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateAudiA3()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET audia3 = @audia3 WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("audia3", this.AudiA3);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateEau()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET eau = @eau WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("eau", this.Eau);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCasierWeed()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET casier_weed = @casier_weed WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("casier_weed", this.CasierWeed);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCasierCocktails()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET casier_cocktails = @casier_cocktails WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("casier_cocktails", this.CasierCocktails);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateWhiteHoverboard()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET white_hover = @white_hover WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("white_hover", this.WhiteHoverboard);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCreations()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET creations = @creations WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("creations", this.Creations);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCredits()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET credits = @credits WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("credits", this.Credits);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateSucette()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET sucette = @sucette WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("sucette", this.Sucette);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateFanta()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET fanta = @fanta WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("fanta", this.Fanta);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateBlur()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET blur = @blur WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("blur", this.Blur);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateWorldEventUser()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE world_event_joins SET collected = collected + 1 WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void addCooldown(string Type, int Time)
        {
            if (this.ActiveCooldowns.ContainsKey(Type))
                return;

            this.ActiveCooldowns.TryAdd(Type, Time);

            System.Timers.Timer timer2 = new System.Timers.Timer(Time);
            timer2.Interval = Time;
            timer2.Elapsed += delegate
            {
                this.ActiveCooldowns.TryRemove(Type, out Time);
                timer2.Stop();
            };
            timer2.Start();
        }

        public bool getCooldown(string Type)
        {
            try
            {
                if (this.ActiveCooldowns.ContainsKey(Type))
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

        public void updateAvatarEvent(string LookF, string LookM, string Motto)
        {
            string NewLook = "";
            if (this.Gender == "f")
            {
                NewLook = this.changeLook(this.Look, LookF);
            }
            else
            {
                NewLook = this.changeLook(this.Look, LookM);
            }

            this.Look = NewLook;
            this.Motto = Motto;

            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            this.GetClient().SendMessage(new UserChangeComposer(User, true));
            this.CurrentRoom.SendMessage(new UserChangeComposer(User, false));
            this.GetClient().GetRoleplay().RPCache(2);
        }

        public void footballSpawnChair()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (User == null || this.CurrentRoomId != 56)
                return;

            int SpriteId = 3494;

            Room Room = this.CurrentRoom;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                {
                    if (item.GetBaseItem().SpriteId == SpriteId)
                    {
                        Room.GetGameMap().TeleportToItem(User, item);
                        Room.GetRoomUserManager().UpdateUserStatusses();
                    }
                }
            }

            this.resetAvatarEvent();
        }

        public void footballSpawnInItem(string Team)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (User == null || this.CurrentRoomId != 56)
                return;

            int SpriteId = 0;
            int Direction = 0;

            if (Team == "green")
            {
                SpriteId = 2566;
                Direction = 0;
            }
            else if (Team == "blue")
            {
                SpriteId = 2582;
                Direction = 4;
            }

            Room Room = this.CurrentRoom;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                foreach (Item item in Room.GetRoomItemHandler().GetFloor)
                {
                    if (item.GetBaseItem().SpriteId == SpriteId)
                    {
                        if (Room.GetGameMap().SquareHasUsers(item.GetX, item.GetY))
                            continue;

                        Room.GetGameMap().TeleportToItem(User, item);
                        Room.GetRoomUserManager().UpdateUserStatusses();
                        User.SetRot(Direction, false);
                    }
                }
            }
        }

        public void resetEffectEvent()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (this.UsingParamedic)
            {
                User.ApplyEffect(20);
                User.FastWalking = true;
            }
            else if (this.WaitingForParamedicFrom != null)
            {
                User.ApplyEffect(20);
                User.SuperFastWalking = true;
            }
            else if (this.Duty)
            {
                User.ApplyEffect(102);
            }
            else if (this.GetClient().GetRoleplay().Passive)
            {
                User.ApplyEffect(706);
            }
            else if (this.isUsingSkateboard)
            {
                User.ApplyEffect(71);
                User.FastWalking = this.isUsingSkateboard;
            }
            else if (this.ArmeEquiped != null)
            {
                if (this.ArmeEquiped == "bat")
                {
                    User.ApplyEffect(591);
                }
                else if (this.ArmeEquiped == "sword")
                {
                    User.ApplyEffect(162);
                }
                else if (this.ArmeEquiped == "axe")
                {
                    User.ApplyEffect(707);
                }
                else if (this.ArmeEquiped == "stungun")
                {
                    User.ApplyEffect(182);
                }
                else if (this.ArmeEquiped == "cocktail")
                {
                    this.Effects().ApplyEffect(1005);
                }
            }
            else if (User.isFarmingRock > 0)
            {
                this.Effects().ApplyEffect(594);
            }
            else
            {
                this.Effects().ApplyEffect(0);
            }
        }

        public void sendMsgOffi(string msg)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (User != null)
            {
                this.GetClient().SendMessage(new WhisperComposer(User.VirtualId, msg, 0, 34));
            }
        }

        public void winSalade()
        {
            this.ArmeEquiped = null;
            this.resetEffectEvent();
            if (this.CurrentRoom != null)
            {
                RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
                if (User != null)
                {
                    User.OnChat(User.LastBubble, "* Win the salad [+100 $] *", true);
                }
            }
            this.Credits += 100;
            this.GetClient().SendMessage(new CreditBalanceComposer(this.Credits));
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "my_stats;" + this.Credits + ";" + this.Duckets + ";" + this.EventPoints);
            PlusEnvironment.GetGame().GetClientManager().sendStaffMsg(this.Username + " won the salad.");
        }

        public void resetAvatarEvent()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);

            if (this.Menotted == true)
            {
                this.updateAvatarEvent(this.Look + ".ch-989999938-1193-62", this.Look + ".ch-989999938-1193-62", "Cuffed");
            }
            else if (this.CurrentRoomId == PlusEnvironment.ManVsZombie && User.ManVsZombieTeam == "man")
            {
                this.updateAvatarEvent("fa-1212-63.hd-600-8.lg-710-110.hr-515-33.ch-635-1408.sh-725-110", "ch -3050-1408-1408.hd-180-1359.lg-280-110.hr-828-61.sh-290-110.fa-1212-63", "[Man VS Zombie] Humain");
            }
            else if (this.Working == true)
            {
                this.updateAvatarEvent(this.RankInfo.Look_F, this.RankInfo.Look_H, "[Working] " + this.RankInfo.Name + ", " + this.TravailInfo.Name);
            }
            else
            {
                using (IQueryAdapter motto = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    motto.SetQuery("SELECT `motto` FROM `users` WHERE `id` = @id LIMIT 1");
                    motto.AddParameter("id", this.Id);
                    this.Motto = motto.getString();
                }

                using (IQueryAdapter look = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    look.SetQuery("SELECT `look` FROM `users` WHERE `id` = @id LIMIT 1");
                    look.AddParameter("id", this.Id);
                    this.Look = look.getString();
                }

                if (this.Look.Contains(".."))
                {
                    this.Look = this.Look.Replace("..", ".");

                    using (IQueryAdapter updateLook = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        updateLook.SetQuery("UPDATE users SET look = @look WHERE id = @id LIMIT 1");
                        updateLook.AddParameter("look", this.Look);
                        updateLook.AddParameter("id", this.Id);
                    }
                }

                this.GetClient().SendMessage(new UserChangeComposer(User, true));
                this.CurrentRoom.SendMessage(new UserChangeComposer(User, false));
                this.GetClient().GetRoleplay().RPCache(2);
            }
        }

        public void updateHomeRoom(int Id)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(GetClient().GetHabbo().Id);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET home_room = @room_id WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("room_id", Id);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateBanque()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET banque = @banque WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("banque", this.Banque);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatecoiffure()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET coiffure = @coiffure WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("coiffure", this.Coiffure);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCb()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET cb = @cb WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("cb", this.Cb);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updatePoliceCasier()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET policecasier = @policecasier WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("policecasier", this.PoliceCasier);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateEnergy()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET energy = @energy WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("energy", this.Energy);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(GetClient(), "user-stats;" + this.Id + ";" + this.Username + ";" + this.Look + ";" + this.Passive + ";" + this.Health + ";" + this.HealthMax + ";" + this.Energy);
        }

        public void updateSante()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE users SET health = @health WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("health", this.Health);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(GetClient(), "user-stats;" + this.Id + ";" + this.Username + ";" + this.Look + ";" + this.Passive + ";" + this.Health + ";" + this.HealthMax + ";" + this.Energy);
        }
        public void updateShifts()
        {
            //this.GetClient().GetRoleplay().Shifts += 1;
            //this.GetClient().GetRoleplay().CorpShifts += 1;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET shifts = shifts + 1, corp_shifts = corp_shifts + 1, weekly_shifts = weekly_shifts + 1 WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void ResetShitfs()
        {
            this.GetClient().GetRoleplay().CorpShifts = 0;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET corp_shifts = 0  WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void UpdateArrests()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET arrests = arrests + 1 WHERE `user_id` = @id LIMIT 1;");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void UpdateAssists()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET assists = assists + 1 WHERE `user_id` = @id LIMIT 1;");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void UpdateTasksCompleted()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET tasks_completed = tasks_completed + 1 WHERE `user_id` = @id LIMIT 1;");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }
        public void UpdateSales()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET sales = sales + 1 WHERE `user_id` = @id LIMIT 1;");
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }
        //RP Updates

        public void updateHealth()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET health = @health WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("health", this.GetClient().GetRoleplay().Health);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateHealthMax()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET health_max = @health_max WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("health_max", this.GetClient().GetRoleplay().HealthMax);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }
        public void updateKills()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET kills = @kills WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("kills", this.GetClient().GetRoleplay().Kills);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateDeaths()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET deaths = @deaths WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("deaths", this.GetClient().GetRoleplay().Deaths);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateFarmingLevel()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET farming_level = @farming_level WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("farming_level", this.GetClient().GetRoleplay().FarmingLevel);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }
        public void updateFarmingXP()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE user_rp_stats SET farming_xp = @farming_xp WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("farming_xp", this.GetClient().GetRoleplay().FarmingXP);
                dbClient.AddParameter("id", this.Id);
                dbClient.RunQuery();
            }
        }
        //

        public void UpdateDurability(string Item, int Durability)
        {
            if (this.ArmeEquiped == null)
                return;

            if (this.InventoryEquipSlot1Item == Item)
            {
                this.InventoryEquipSlot1Durability -= Durability;

                if (this.InventoryEquipSlot1Durability <= 0)
                {
                    this.InventoryEquipSlot1Item = null;
                    this.InventoryEquipSlot1Durability = 0;
                    this.ArmeEquiped = null;
                    this.resetEffectEvent();
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;remove;1");
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-durability-update;slot1;" + this.InventoryEquipSlot1Durability);
            }
            else if (this.InventoryEquipSlot2Item == Item)
            {
                this.InventoryEquipSlot2Durability -= Durability;

                if (this.InventoryEquipSlot2Durability <= 0)
                {
                    this.InventoryEquipSlot2Item = null;
                    this.InventoryEquipSlot2Durability = 0;
                    this.ArmeEquiped = null;
                    this.resetEffectEvent();
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;remove;2");
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-durability-update;slot2;" + this.InventoryEquipSlot2Durability);
            }
        }

        public void setFavoriteGroup(int GroupId)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (User == null)
                return;

            this.GetStats().FavouriteGroupId = GroupId;
            Group Group = null;
            PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group);
            this.CurrentRoom.SendMessage(new RefreshFavouriteGroupComposer(this.Id));
            this.CurrentRoom.SendMessage(new HabboGroupBadgesComposer(Group));
            this.CurrentRoom.SendMessage(new UpdateFavouriteGroupComposer(this.Id, Group, User.VirtualId));

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_rp_stats` SET job_id = @jobid, job_rank = '7' WHERE user_id = @uid");
                dbClient.AddParameter("jobid", GroupId);
                dbClient.AddParameter("uid", this.Id);
                dbClient.RunQuery();
            }
        }

        public bool checkCoiff(string NewCoiff)
        {
            string lookSession = this.Look;
            string[] lookSessionReplace = lookSession.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string hr;

            if (lookSession.Contains("hr-"))
            {
                hr = lookSessionReplace[Array.FindIndex(lookSessionReplace, row => row.Contains("hr-"))];
                if (hr == NewCoiff)
                {
                    return true;
                }
            }

            return false;
        }

        public void addTenu(int Code, int Color)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO looks_users (user_id, code, color) VALUES (@user_id, @code, @color);");
                dbClient.AddParameter("user_id", this.Id);
                dbClient.AddParameter("code", Code);
                dbClient.AddParameter("color", Color);
                dbClient.RunQuery();
            }
        }

        public void removeLookType(string Type)
        {
            string lookSession = this.Look;
            string[] lookSessionReplace = lookSession.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string hr;

            if (lookSession.Contains(Type))
            {
                hr = lookSessionReplace[Array.FindIndex(lookSessionReplace, row => row.Contains(Type))];
                lookSession = lookSession.Replace(hr, "");
            }

            if (lookSession.EndsWith("."))
            {
                lookSession = lookSession.Remove(lookSession.Length - 1);
            }

            this.Look = lookSession;
            this.updateLook();
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (User != null)
            {
                this.GetClient().SendMessage(new UserChangeComposer(User, true));
                this.CurrentRoom.SendMessage(new UserChangeComposer(User, false));
            }
        }

        public void changeLookByType(string Type, string NewCoiff)
        {
            string lookSession = this.Look;
            string[] lookSessionReplace = lookSession.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string hr;

            if (lookSession.Contains(Type))
            {
                hr = lookSessionReplace[Array.FindIndex(lookSessionReplace, row => row.Contains(Type))];
                lookSession = lookSession.Replace(hr, Type + NewCoiff);
            }
            else
            {
                lookSession = lookSession + "." + Type + NewCoiff + ".";
            }

            if (lookSession.EndsWith("."))
            {
                lookSession = lookSession.Remove(lookSession.Length - 1);
            }

            if (lookSession.Contains(".."))
            {
                lookSession = lookSession.Replace("..", ".");
            }

            this.Look = lookSession;
            this.updateLook();
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (User != null)
            {
                this.GetClient().SendMessage(new UserChangeComposer(User, true));
                this.CurrentRoom.SendMessage(new UserChangeComposer(User, false));
            }
        }

        public string ChangeLookWithoutHair(string SessionLook, string LookExemple)
        {
            string lookSession = SessionLook;
            string[] lookSessionReplace = lookSession.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string look = LookExemple;
            string[] lookReplace = look.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            this.Hair = look;

            string hr;
            if (lookSession.Contains("hr-"))
            {
                hr = lookSessionReplace[Array.FindIndex(lookSessionReplace, row => row.Contains("hr-"))];
                look = look.Replace(lookReplace[Array.FindIndex(lookReplace, row => row.Contains("hr-"))], hr);
            }
            else
            {
                look = look.Replace(lookReplace[Array.FindIndex(lookReplace, row => row.Contains("hr-"))], "");
            }

            return look;
        }

        public string changeLook(string SessionLook, string LookExemple)
        {
            string lookSession = SessionLook;
            string[] lookSessionReplace = lookSession.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string look = LookExemple;
            string[] lookReplace = look.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            string hr;
            string hd;

            if (lookSession.Contains("hr-"))
            {
                hr = lookSessionReplace[Array.FindIndex(lookSessionReplace, row => row.Contains("hr-"))];
                look = look.Replace(lookReplace[Array.FindIndex(lookReplace, row => row.Contains("hr-"))], hr);
            }
            else
            {
                look = look.Replace(lookReplace[Array.FindIndex(lookReplace, row => row.Contains("hr-"))], "");
            }

            if (lookSession.Contains("hd-"))
            {
                hd = lookSessionReplace[Array.FindIndex(lookSessionReplace, row => row.Contains("hd-"))];
                look = look.Replace(lookReplace[Array.FindIndex(lookReplace, row => row.Contains("hd-"))], hd);
            }
            else
            {
                look = look.Replace(lookReplace[Array.FindIndex(lookReplace, row => row.Contains("hd-"))], "");
            }

            return look;
        }

        public void UpdateInventory(string Slot, int qurability)
        {
            DataRow GetInventory = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `inventory` SET `" + Slot + "_durability` = `" + Slot + "_durability` - " + qurability + " WHERE `user_id` = '" + this.Id + "' LIMIT 1;");
                dbClient.RunQuery();
                dbClient.SetQuery("SELECT * FROM `Inventory` WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Id);
                GetInventory = dbClient.getRow();
            }

            if (Slot == "slot1")
            {
                this.InventorySlot1Durability = Convert.ToInt32(GetInventory[Slot + "_durability"]);

                if (this.InventorySlot1Durability < 0)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE inventory SET " + Slot + " = 'null', slot1_durability = 0 WHERE user_id = '" + this.Id + "' LIMIT 1;");
                    }

                    this.InventorySlot1Durability = 0;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;delete;1;" + this.InventorySlot1 + "; " + Slot + ";" + this.InventorySlot1Durability);
                    this.InventorySlot1 = null;
                    this.ArmeEquiped = null;
                    this.InventoryEquipSlot1Item = null;
                    this.resetEffectEvent();
                }
            }
            else if (Slot == "slot2")
            {
                this.InventorySlot2Durability = Convert.ToInt32(GetInventory[Slot + "_durability"]);
            }
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-durability-update;1;" + Slot + ";" + GetInventory[Slot + "_durability"]);
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-durability-update;0;" + Slot + ";" + GetInventory[Slot + "_durability"]);
        }

        public void InventoryEquip(string item, int damage)
        {
            if (this.InventoryEquipSlot1 == item || this.InventoryEquipSlot2 == item)
            {
                if (this.InventoryEquipSlot1 == item)
                {
                    this.InventoryEquipSlot1 = "";
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;1;remove;" + item + ";" + damage);
                    return;
                }
                else if (this.InventoryEquipSlot2 == item)
                {
                    this.InventoryEquipSlot2 = "";
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;2;remove;" + item + ";" + damage);
                    return;
                }
            }
            else
            {
                if (this.InventoryEquipSlot1 == "")
                {
                    this.InventoryEquipSlot1 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;1;add;" + item + ";" + damage);
                }
                else if (this.InventoryEquipSlot2 == "")
                {
                    this.InventoryEquipSlot2 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-equip;2;add;" + item + ";" + damage);
                }
            }
            return;
        }
        public void Inventory(string item, int quantity, int damage)
        {
            if (this.InventorySlot1 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot1 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot1Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot1Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;1;" + item + ";" + this.InventorySlot1Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot2 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    if (quantity > 0 || damage > 0)
                    {
                        this.InventorySlot2 = item;
                        if (quantity > 5)
                        {
                            this.Inventory(item, quantity - 5, 0);
                            this.InventorySlot2Quantity = 5;
                        }
                        else
                        {
                            this.InventorySlot2Quantity = quantity;
                        }
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;2;" + item + ";" + this.InventorySlot2Quantity + ";" + damage);
                    }
                }
            }
            else if (this.InventorySlot3 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot3 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot3Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot3Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;3;" + item + ";" + this.InventorySlot3Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot4 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot4 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot4Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot4Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;4;" + item + ";" + this.InventorySlot4Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot5 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot5 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot5Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot5Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;5;" + item + ";" + this.InventorySlot5Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot6 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot6 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot6Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot6Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;6;" + item + ";" + this.InventorySlot6Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot7 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot7 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot7Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot7Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;7;" + item + ";" + this.InventorySlot7Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot8 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot8 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot8Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot8Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;8;" + item + ";" + this.InventorySlot8Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot9 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot9 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot9Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot9Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;9;" + item + ";" + this.InventorySlot9Quantity + ";" + damage);
                }
            }
            else if (this.InventorySlot10 == null)
            {
                if (quantity > 0 || damage > 0)
                {
                    this.InventorySlot10 = item;
                    if (quantity > 5)
                    {
                        this.Inventory(item, quantity - 5, 0);
                        this.InventorySlot10Quantity = 5;
                    }
                    else
                    {
                        this.InventorySlot10Quantity = quantity;
                    }
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;10;" + item + ";" + this.InventorySlot10Quantity + ";" + damage);
                }
            }
            else
            {
                this.GetClient().SendWhisper("Your inventory is full!");
                return;
            }
        }
        public void InventoryUpdate(string slot, string plusminus, int quantity, int damage)
        {
            if (slot == "slot1")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot1Quantity = InventorySlot1Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot1Quantity = InventorySlot1Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot1Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot1Quantity + ";" + damage);
            }
            else if (slot == "slot2")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot2Quantity = InventorySlot2Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot2Quantity = InventorySlot2Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot2Quantity + ";" + damage);
            }
            else if (slot == "slot3")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot3Quantity = InventorySlot3Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot3Quantity = InventorySlot3Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot3Quantity + ";" + damage);
            }
            else if (slot == "slot4")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot4Quantity = InventorySlot4Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot4Quantity = InventorySlot4Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot4Quantity + ";" + damage);
            }
            else if (slot == "slot5")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot5Quantity = InventorySlot5Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot5Quantity = InventorySlot5Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot5Quantity + ";" + damage);
            }
            else if (slot == "slot6")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot6Quantity = InventorySlot6Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot6Quantity = InventorySlot6Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot6Quantity + ";" + damage);
            }
            else if (slot == "slot7")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot7Quantity = InventorySlot7Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot7Quantity = InventorySlot7Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot7Quantity + ";" + damage);
            }
            else if (slot == "slot8")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot8Quantity = InventorySlot8Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot8Quantity = InventorySlot8Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot8Quantity + ";" + damage);
            }
            else if (slot == "slot9")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot9Quantity = InventorySlot9Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot9Quantity = InventorySlot9Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot9Quantity + ";" + damage);
            }
            else if (slot == "slot10")
            {
                if (plusminus == "+")
                {
                    this.InventorySlot10Quantity = InventorySlot10Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot10Quantity = InventorySlot10Quantity - quantity;
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;" + slot + ";" + InventorySlot10Quantity + ";" + damage);
            }
            else
            {
                this.GetClient().SendWhisper("Error: No slot id");
                return;
            }
        }
        public void InventoryUpdate2(string item, string plusminus, int quantity, int damage)
        {
            if (this.InventorySlot1 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot1Quantity = InventorySlot1Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot1Quantity = InventorySlot1Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot1;" + InventorySlot1Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot1;" + InventorySlot1Quantity + ";" + damage);
            }
            else if (this.InventorySlot2 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot2Quantity = InventorySlot2Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot2Quantity = InventorySlot2Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot2;" + InventorySlot2Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot2;" + InventorySlot2Quantity + ";" + damage);
            }
            else if (this.InventorySlot3 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot3Quantity = InventorySlot3Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot3Quantity = InventorySlot3Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot3;" + InventorySlot1Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot3;" + InventorySlot1Quantity + ";" + damage);
            }
            else if (this.InventorySlot4 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot4Quantity = InventorySlot4Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot4Quantity = InventorySlot4Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot4;" + InventorySlot4Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot4;" + InventorySlot4Quantity + ";" + damage);
            }
            else if (this.InventorySlot5 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot5Quantity = InventorySlot5Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot5Quantity = InventorySlot5Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot5;" + InventorySlot5Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot5;" + InventorySlot5Quantity + ";" + damage);
            }
            else if (this.InventorySlot6 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot6Quantity = InventorySlot6Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot6Quantity = InventorySlot6Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot6;" + InventorySlot6Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot6;" + InventorySlot6Quantity + ";" + damage);
            }
            else if (this.InventorySlot7 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot7Quantity = InventorySlot7Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot7Quantity = InventorySlot7Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot7;" + InventorySlot7Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot7;" + InventorySlot7Quantity + ";" + damage);
            }
            else if (this.InventorySlot8 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot8Quantity = InventorySlot8Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot8Quantity = InventorySlot8Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot8;" + InventorySlot8Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot8;" + InventorySlot8Quantity + ";" + damage);
            }
            else if (this.InventorySlot9 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot9Quantity = InventorySlot9Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot9Quantity = InventorySlot9Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot9;" + InventorySlot9Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot9;" + InventorySlot9Quantity + ";" + damage);
            }
            else if (this.InventorySlot10 == item)
            {
                if (plusminus == "+")
                {
                    this.InventorySlot10Quantity = InventorySlot10Quantity + quantity;
                }
                else if (plusminus == "-")
                {
                    this.InventorySlot10Quantity = InventorySlot10Quantity - quantity;
                }
                else//damage
                {
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot10;" + InventorySlot10Quantity + ";" + damage);
                }
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-update;slot10;" + InventorySlot10Quantity + ";" + damage);
            }
        }
        public bool CheckDiag(int GetX, int GetY, int roomUserX, int roomUserY)
        {
            if (roomUserX + 1 == GetX && roomUserY - 1 == GetY || roomUserX + 1 == GetX && roomUserY + 1 == GetY || roomUserX - 1 == GetX && roomUserY - 1 == GetY || roomUserX - 1 == GetX && roomUserY + 1 == GetY)
                return true;
            else return false;
        }
        public void GymXP()
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Username);
            if (this.usedCrosstrainer && this.usedTreadMill && this.usedTrampoline)
            {
                this.usedCrosstrainer = false;
                this.usedTreadMill = false;
                this.usedTrampoline = false;
                this.GetClient().GetRoleplay().CombatXP += 25;
                User.Say("completes a circuit of the gym (+25xp)");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "combat-xp;" + this.GetClient().GetRoleplay().CombatXP);

                //Levels & XPs
                int NextLevel;
                int NextCombatXP;
                string NextDamage;
                int NextHealth;
                if (this.GetClient().GetRoleplay().CombatLevel == 0 && this.GetClient().GetRoleplay().CombatXP >= 75 && this.GetClient().GetRoleplay().CombatXP < 150)
                {
                    NextLevel = 2;
                    NextCombatXP = 150;
                    NextDamage = "1-3";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 1;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 1 && this.GetClient().GetRoleplay().CombatXP >= 150 && this.GetClient().GetRoleplay().CombatXP < 225)
                {
                    NextLevel = 3;
                    NextCombatXP = 225;
                    NextDamage = "2-3";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 2;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 2 && this.GetClient().GetRoleplay().CombatXP >= 225 && this.GetClient().GetRoleplay().CombatXP < 325)
                {
                    NextLevel = 4;
                    NextCombatXP = 325;
                    NextDamage = "2-4";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 3;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 3 && this.GetClient().GetRoleplay().CombatXP >= 325 && this.GetClient().GetRoleplay().CombatXP < 425)
                {
                    NextLevel = 5;
                    NextCombatXP = 425;
                    NextDamage = "3-5";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 4;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 4 && this.GetClient().GetRoleplay().CombatXP >= 425 && this.GetClient().GetRoleplay().CombatXP < 525)
                {
                    NextLevel = 6;
                    NextCombatXP = 525;
                    NextDamage = "4-5";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 5;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 5 && this.GetClient().GetRoleplay().CombatXP >= 525 && this.GetClient().GetRoleplay().CombatXP < 650)
                {
                    NextLevel = 7;
                    NextCombatXP = 650;
                    NextDamage = "4-6";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 6;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 6 && this.GetClient().GetRoleplay().CombatXP >= 650 && this.GetClient().GetRoleplay().CombatXP < 775)
                {
                    NextLevel = 8;
                    NextCombatXP = 775;
                    NextDamage = "5-6";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 7;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 7 && this.GetClient().GetRoleplay().CombatXP >= 775 && this.GetClient().GetRoleplay().CombatXP < 900)
                {
                    NextLevel = 9;
                    NextCombatXP = 900;
                    NextDamage = "5-7";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 8;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 8 && this.GetClient().GetRoleplay().CombatXP >= 900 && this.GetClient().GetRoleplay().CombatXP < 1050)
                {
                    NextLevel = 10;
                    NextCombatXP = 1050;
                    NextDamage = "6-7";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 9;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 9 && this.GetClient().GetRoleplay().CombatXP >= 1050 && this.GetClient().GetRoleplay().CombatXP < 1200)
                {
                    NextLevel = 11;
                    NextCombatXP = 1200;
                    NextDamage = "6-8";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 10;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 10 && this.GetClient().GetRoleplay().CombatXP >= 1200 && this.GetClient().GetRoleplay().CombatXP < 1350)
                {
                    NextLevel = 12;
                    NextCombatXP = 1350;
                    NextDamage = "7-8";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 11;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 11 && this.GetClient().GetRoleplay().CombatXP >= 1350 && this.GetClient().GetRoleplay().CombatXP < 1525)
                {
                    NextLevel = 13;
                    NextCombatXP = 1525;
                    NextDamage = "7-9";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 12;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 12 && this.GetClient().GetRoleplay().CombatXP >= 1525 && this.GetClient().GetRoleplay().CombatXP < 1700)
                {
                    NextLevel = 14;
                    NextCombatXP = 1700;
                    NextDamage = "8-9";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 13;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 13 && this.GetClient().GetRoleplay().CombatXP >= 1700 && this.GetClient().GetRoleplay().CombatXP < 1875)
                {
                    NextLevel = 15;
                    NextCombatXP = 1875;
                    NextDamage = "8-10";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 14;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 14 && this.GetClient().GetRoleplay().CombatXP >= 1875 && this.GetClient().GetRoleplay().CombatXP < 2075)
                {
                    NextLevel = 16;
                    NextCombatXP = 2075;
                    NextDamage = "9-10";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 15;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 15 && this.GetClient().GetRoleplay().CombatXP >= 2075 && this.GetClient().GetRoleplay().CombatXP < 2275)
                {
                    NextLevel = 17;
                    NextCombatXP = 2275;
                    NextDamage = "9-11";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 16;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 16 && this.GetClient().GetRoleplay().CombatXP >= 2275 && this.GetClient().GetRoleplay().CombatXP < 2475)
                {
                    NextLevel = 18;
                    NextCombatXP = 2475;
                    NextDamage = "10-11";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 17;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 17 && this.GetClient().GetRoleplay().CombatXP >= 2475 && this.GetClient().GetRoleplay().CombatXP < 2700)
                {
                    NextLevel = 19;
                    NextCombatXP = 2700;
                    NextDamage = "10-12";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 18;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 18 && this.GetClient().GetRoleplay().CombatXP >= 2700 && this.GetClient().GetRoleplay().CombatXP < 2925)
                {
                    NextLevel = 20;
                    NextCombatXP = 2925;
                    NextDamage = "11-12";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 19;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 19 && this.GetClient().GetRoleplay().CombatXP >= 2925 && this.GetClient().GetRoleplay().CombatXP < 3150)
                {
                    NextLevel = 21;
                    NextCombatXP = 3150;
                    NextDamage = "11-13";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 20;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 20 && this.GetClient().GetRoleplay().CombatXP >= 3150 && this.GetClient().GetRoleplay().CombatXP < 3400)
                {
                    NextLevel = 22;
                    NextCombatXP = 3400;
                    NextDamage = "12-13";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 21;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 21 && this.GetClient().GetRoleplay().CombatXP >= 3400 && this.GetClient().GetRoleplay().CombatXP < 3650)
                {
                    NextLevel = 23;
                    NextCombatXP = 3650;
                    NextDamage = "12-14";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 22;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 22 && this.GetClient().GetRoleplay().CombatXP >= 3650 && this.GetClient().GetRoleplay().CombatXP < 3900)
                {
                    NextLevel = 24;
                    NextCombatXP = 3900;
                    NextDamage = "13-14";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 23;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 23 && this.GetClient().GetRoleplay().CombatXP >= 3900)
                {
                    NextLevel = 25;
                    NextCombatXP = 4150;
                    NextDamage = "14-15";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 24;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }
                else if (this.GetClient().GetRoleplay().CombatLevel == 24 && this.GetClient().GetRoleplay().CombatXP == 4150)
                {
                    NextLevel = 25;
                    NextCombatXP = 4150;
                    NextDamage = "14-15";
                    NextHealth = 2;
                    this.GetClient().GetRoleplay().CombatLevel = 25;
                    User.Say("levels up, feels stronger");
                    this.GetClient().GetRoleplay().HealthMax += 2;
                    this.GetClient().GetHabbo().RPCache(1);
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "character;" + this.GetClient().GetRoleplay().CombatXP + ";" + NextLevel + ";" + NextCombatXP + ";" + NextDamage + ";" + NextHealth);
                }

                DataRow GetJob = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                    dbClient.AddParameter("id", this.GetClient().GetRoleplay().JobId);
                    GetJob = dbClient.getRow();
                }

                DataRow GetJobRank = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `groups_rank` WHERE `job_id` = @job_id AND `rank_id` = @rank_id LIMIT 1");
                    dbClient.AddParameter("job_id", GetJob["id"]);
                    dbClient.AddParameter("rank_id", this.GetClient().GetRoleplay().JobRank);
                    GetJobRank = dbClient.getRow();
                }
            }
        }

        public void FarmingXP()
        {
            this.GetClient().GetRoleplay().FarmingXP += 5;
            this.updateFarmingXP();

            int NextFarmingXP;
            int Level;
            if (this.GetClient().GetRoleplay().FarmingLevel == 0 && this.GetClient().GetRoleplay().FarmingXP >= 270 && this.GetClient().GetRoleplay().FarmingXP < 480)
            {
                Level = 1;
                NextFarmingXP = 480;
                this.GetClient().GetRoleplay().FarmingLevel = 1;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 1 && this.GetClient().GetRoleplay().FarmingXP >= 480 && this.GetClient().GetRoleplay().FarmingXP < 690)
            {
                Level = 2;
                NextFarmingXP = 690;
                this.GetClient().GetRoleplay().FarmingLevel = 2;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 2 && this.GetClient().GetRoleplay().FarmingXP >= 690 && this.GetClient().GetRoleplay().FarmingXP < 900)
            {
                Level = 3;
                NextFarmingXP = 900;
                this.GetClient().GetRoleplay().FarmingLevel = 3;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 3 && this.GetClient().GetRoleplay().FarmingXP >= 900 && this.GetClient().GetRoleplay().FarmingXP < 1110)
            {
                Level = 4;
                NextFarmingXP = 1110;
                this.GetClient().GetRoleplay().FarmingLevel = 4;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 4 && this.GetClient().GetRoleplay().FarmingXP >= 1110 && this.GetClient().GetRoleplay().FarmingXP < 1320)
            {
                Level = 5;
                NextFarmingXP = 1320;
                this.GetClient().GetRoleplay().FarmingLevel = 5;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 5 && this.GetClient().GetRoleplay().FarmingXP >= 1320 && this.GetClient().GetRoleplay().FarmingXP < 1530)
            {
                Level = 6;
                NextFarmingXP = 1530;
                this.GetClient().GetRoleplay().FarmingLevel = 6;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 6 && this.GetClient().GetRoleplay().FarmingXP >= 1530 && this.GetClient().GetRoleplay().FarmingXP < 1740)
            {
                Level = 7;
                NextFarmingXP = 1740;
                this.GetClient().GetRoleplay().FarmingLevel = 7;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 7 && this.GetClient().GetRoleplay().FarmingXP >= 1740 && this.GetClient().GetRoleplay().FarmingXP < 1950)
            {
                Level = 8;
                NextFarmingXP = 1950;
                this.GetClient().GetRoleplay().FarmingLevel = 8;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 8 && this.GetClient().GetRoleplay().FarmingXP >= 1950 && this.GetClient().GetRoleplay().FarmingXP < 2160)
            {
                Level = 9;
                NextFarmingXP = 2160;
                this.GetClient().GetRoleplay().FarmingLevel = 9;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            else if (this.GetClient().GetRoleplay().FarmingLevel == 9 && this.GetClient().GetRoleplay().FarmingXP >= 2160)
            {
                Level = 10;
                NextFarmingXP = 2160;
                this.GetClient().GetRoleplay().FarmingLevel = 10;
                this.updateFarmingLevel();
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming;" + Level + ";" + this.GetClient().GetRoleplay().FarmingXP + ";" + NextFarmingXP);
            }
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "farming-xp;" + this.GetClient().GetRoleplay().FarmingXP);
        }

        public void RPCache(int cache)
        {
            if (cache == 1)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "user-stats;" + this.GetClient().GetHabbo().Id + ";" + this.GetClient().GetHabbo().Username + ";" + this.GetClient().GetHabbo().Look + ";" + this.GetClient().GetRoleplay().Passive + ";" + this.GetClient().GetRoleplay().Health + ";" + this.GetClient().GetRoleplay().HealthMax + ";" + this.GetClient().GetRoleplay().Energy);
            }
            else if (cache == 2)
            {

            }
            else if (cache == 3)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "my_stats;" + this.GetClient().GetHabbo().Credits + ";" + this.GetClient().GetHabbo().Duckets + ";" + this.GetClient().GetHabbo().EventPoints);
            }
            else if (cache == 4)
            {
               
            }
            else if (cache == 5)
            {
               
            }
        }

        public void OfferTimer(int Seconds = 15)
        {
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.TransactionTo);
            RoomUser TargetUser = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Username);

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            TargetClient.GetHabbo().OfferToken = tokenNumber;

            System.Timers.Timer OfferExpireTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(Seconds));
            OfferExpireTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(Seconds);
            OfferExpireTimer.Elapsed += delegate
            {
                if (TargetClient != null && TargetClient.GetHabbo().OfferToken == tokenNumber)
                {
                    this.GetClient().SendWhisper("The offer has expired");
                    TargetClient.SendWhisper("The offer has expired");

                    this.TransactionTo = null;
                    this.CorpSell = null;
                    TargetUser.Transaction = null;
                    TargetClient.GetHabbo().TransactionFrom = null;
                    TargetClient.GetHabbo().OfferToken = 0;
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction-hide;");
                }
                OfferExpireTimer.Stop();
            };
            OfferExpireTimer.Start();
        }

        public void AddToInventory(string item, int Durability)
        {
            if (this.InventorySlot1 == null)
            {
                this.InventorySlot1 = item;
                this.InventorySlot1Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot1;" + this.InventorySlot1 + ";0;" + this.InventorySlot1Durability);
            }
            else if (this.InventorySlot2 == null)
            {
                this.InventorySlot2 = item;
                this.InventorySlot2Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot2;" + this.InventorySlot2 + ";0;" + this.InventorySlot2Durability);
            }
            else if (this.InventorySlot3 == null)
            {
                this.InventorySlot3 = item;
                this.InventorySlot3Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot3;" + this.InventorySlot3 + ";0;" + this.InventorySlot3Durability);
            }
            else if (this.InventorySlot4 == null)
            {
                this.InventorySlot4 = item;
                this.InventorySlot4Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot4;" + this.InventorySlot4 + ";0;" + this.InventorySlot4Durability);
            }
            else if (this.InventorySlot5 == null)
            {
                this.InventorySlot5 = item;
                this.InventorySlot6Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot5;" + this.InventorySlot6 + ";0;" + this.InventorySlot6Durability);
            }
            else if (this.InventorySlot6 == null)
            {
                this.InventorySlot6 = item;
                this.InventorySlot6Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot6;" + this.InventorySlot6 + ";0;" + this.InventorySlot6Durability);
            }
            else if (this.InventorySlot7 == null)
            {
                this.InventorySlot7 = item;
                this.InventorySlot7Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot7;" + this.InventorySlot7 + ";0;" + this.InventorySlot7Durability);
            }
            else if (this.InventorySlot8 == null)
            {
                this.InventorySlot8 = item;
                this.InventorySlot8Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot8;" + this.InventorySlot8 + ";0;" + this.InventorySlot8Durability);
            }
            else if (this.InventorySlot9 == null)
            {
                this.InventorySlot9 = item;
                this.InventorySlot9Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot9;" + this.InventorySlot9 + ";0;" + this.InventorySlot9Durability);
            }
            else if (this.InventorySlot10 == null)
            {
                this.InventorySlot10 = item;
                this.InventorySlot10Durability = Durability;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "inventory-add;slot10;" + this.InventorySlot10 + ";0;" + this.InventorySlot10Durability);
            }
            else
            {
                this.GetClient().SendWhisper("Your inventory is full!");
            }
        }

        public void AddToInventory2(string item, int quantity)
        {
            if (this.InventorySlot1 == item && this.InventorySlot1Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot1Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot1Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot1Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot1Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot1Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot1Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot1Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot1Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot1Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot1Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + quantity);
                }
            }
            else if (this.InventorySlot2 == item && this.InventorySlot2Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot2Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot2Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot2Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot2Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot2Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot2Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot2Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot2Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot2Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot2Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + quantity);
                }
            }
            else if (this.InventorySlot3 == item && this.InventorySlot3Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot3Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot3Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot3Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot3Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot3Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot3Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot3Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot3Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot3Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot3Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + quantity);
                }
            }
            else if (this.InventorySlot4 == item && this.InventorySlot4Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot4Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot4Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot4Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot4Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot4Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot4Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot4Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot4Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot4Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot4Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + quantity);
                }
            }
            else if (this.InventorySlot5 == item && this.InventorySlot5Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot5Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot5Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot5Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot5Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot5Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot5Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot5Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot5Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot5Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot5Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + quantity);
                }
            }
            else if (this.InventorySlot6 == item && this.InventorySlot6Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot6Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot6Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot6Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot6Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot6Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot6Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot6Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot6Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot6Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot6Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + quantity);
                }
            }
            else if (this.InventorySlot7 == item && this.InventorySlot7Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot7Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot7Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot7Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot7Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot7Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot7Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot7Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot7Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot7Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot7Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + quantity);
                }
            }
            else if (this.InventorySlot8 == item && this.InventorySlot8Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot8Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot8Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot8Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot8Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot8Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot8Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot8Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot8Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot8Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot8Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + quantity);
                }
            }
            else if (this.InventorySlot9 == item && this.InventorySlot9Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot9Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot9Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot9Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot9Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot9Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot9Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot9Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot9Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot9Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot9Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + quantity);
                }
            }
            else if (this.InventorySlot10 == item && this.InventorySlot10Quantity != 5)
            {
                if (quantity == 5)
                {
                    if (this.InventorySlot10Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot10Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot10Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else if (this.InventorySlot10Quantity == 1)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 4);
                        this.AddToInventory2(item, quantity -= 4);
                    }
                }
                else if (quantity == 4)
                {
                    if (this.InventorySlot10Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot10Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else if (this.InventorySlot10Quantity == 2)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 3);
                        this.AddToInventory2(item, quantity -= 3);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + quantity);
                    }
                }
                else if (quantity == 3)
                {
                    if (this.InventorySlot10Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else if (this.InventorySlot10Quantity == 3)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 2);
                        this.AddToInventory2(item, quantity -= 2);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + quantity);
                    }
                }
                else if (quantity == 2)
                {
                    if (this.InventorySlot10Quantity == 4)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + 1);
                        this.AddToInventory2(item, quantity -= 1);
                    }
                    else
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + quantity);
                    }
                }
                else
                {
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + quantity);
                }
            }
            else
            {
                if (this.InventorySlot1 == null)
                {
                    this.InventorySlot1 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot1," + item);
                }
                else if (this.InventorySlot2 == null)
                {
                    this.InventorySlot2 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot2," + item);
                }
                else if (this.InventorySlot3 == null)
                {
                    this.InventorySlot3 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot3," + item);
                }
                else if (this.InventorySlot4 == null)
                {
                    this.InventorySlot4 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot4," + item);
                }
                else if (this.InventorySlot5 == null)
                {
                    this.InventorySlot5 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot5," + item);
                }
                else if (this.InventorySlot6 == null)
                {
                    this.InventorySlot6 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot6," + item);
                }
                else if (this.InventorySlot7 == null)
                {
                    this.InventorySlot7 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot7," + item);
                }
                else if (this.InventorySlot8 == null)
                {
                    this.InventorySlot8 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot8," + item);
                }
                else if (this.InventorySlot9 == null)
                {
                    this.InventorySlot9 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot9," + item);
                }
                else if (this.InventorySlot10 == null)
                {
                    this.InventorySlot10 = item;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + quantity);
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot10," + item);
                }
                else
                {
                    this.GetClient().SendWhisper("Your inventory is full!");
                }
            }
        }

        public void AddToInventory3(string item, int quantity)
        {
            DataRow Inventory = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `Inventory` WHERE `user_id` = @id LIMIT 1");
                dbClient.AddParameter("id", this.Id);
                Inventory = dbClient.getRow();
            }
            if (Convert.ToString(Inventory["slot1"]) == "null")
            {
                this.InventorySlot1 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot1,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot1," + item);
            }
            else if (Convert.ToString(Inventory["slot2"]) == "null")
            {
                this.InventorySlot2 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot2,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot2," + item);
            }
            else if (Convert.ToString(Inventory["slot3"]) == "null")
            {
                this.InventorySlot3 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot3,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot3," + item);
            }
            else if (Convert.ToString(Inventory["slot4"]) == "null")
            {
                this.InventorySlot4 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot4,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot4," + item);
            }
            else if (Convert.ToString(Inventory["slot5"]) == "null")
            {
                this.InventorySlot5 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot5,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot5," + item);
            }
            else if (Convert.ToString(Inventory["slot6"]) == "null")
            {
                this.InventorySlot6 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot6,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot6," + item);
            }
            else if (Convert.ToString(Inventory["slot7"]) == "null")
            {
                this.InventorySlot7 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot7,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot7," + item);
            }
            else if (Convert.ToString(Inventory["slot8"]) == "null")
            {
                this.InventorySlot8 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot8,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot8," + item);
            }
            else if (Convert.ToString(Inventory["slot9"]) == "null")
            {
                this.InventorySlot9 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot9,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot9," + item);
            }
            else if (Convert.ToString(Inventory["slot10"]) == "null")
            {
                this.InventorySlot10 = item;
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "update_quantity,slot10,+," + quantity);
                PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "inventory", "add,slot10," + item);
            }
            else
            {
                this.GetClient().SendWhisper("Your inventory is full!");
            }
        }

        public bool CheckInventory(string item)
        {
            if (this.InventorySlot1 == item)
            {
                this.FreeInventory = "slot1";
                return true;
            }
            else if (this.InventorySlot2 == item)
            {
                this.FreeInventory = "slot2";
                return true;
            }
            else if (this.InventorySlot3 == item)
            {
                this.FreeInventory = "slot3";
                return true;
            }
            else if (this.InventorySlot4 == item)
            {
                this.FreeInventory = "slot4";
                return true;
            }
            else if (this.InventorySlot5 == item)
            {
                this.FreeInventory = "slot5";
                return true;
            }
            else if (this.InventorySlot6 == item)
            {
                this.FreeInventory = "slot6";
                return true;
            }
            else if (this.InventorySlot7 == item)
            {
                this.FreeInventory = "slot7";
                return true;
            }
            else if (this.InventorySlot8 == item)
            {
                this.FreeInventory = "slot8";
                return true;
            }
            else if (this.InventorySlot9 == item)
            {
                this.FreeInventory = "slot9";
                return true;
            }
            else if (this.InventorySlot10 == item)
            {
                this.FreeInventory = "slot10";
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InventoryFull()
        {
            if (this.InventorySlot1 == null)
            {
                return false;
            }
            else if (this.InventorySlot2 == null)
            {
                return false;
            }
            else if (this.InventorySlot3 == null)
            {
                return false;
            }
            else if (this.InventorySlot4 == null)
            {
                return false;
            }
            else if (this.InventorySlot5 == null)
            {
                return false;
            }
            else if (this.InventorySlot6 == null)
            {
                return false;
            }
            else if (this.InventorySlot7 == null)
            {
                return false;
            }
            else if (this.InventorySlot8 == null)
            {
                return false;
            }
            else if (this.InventorySlot9 == null)
            {
                return false;
            }
            else if (this.InventorySlot10 == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Dispose()
        {
            if (this.InventoryComponent != null)
                this.InventoryComponent.SetIdleState();

            if (this.UsersRooms != null)
                UsersRooms.Clear();

            if (this.InRoom && this.CurrentRoom != null)
                this.CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(this._client, false, false);

            if (Messenger != null)
            {
                this.Messenger.AppearOffline = true;
                this.Messenger.Destroy();
            }

            if (this._fx != null)
                this._fx.Dispose();

            if (this._clothing != null)
                this._clothing.Dispose();

            if (this._permissions != null)
                this._permissions.Dispose();
        }

        public GameClient GetClient()
        {
            if (this._client != null)
                return this._client;

            return PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);
        }

        public HabboMessenger GetMessenger()
        {
            return Messenger;
        }

        public BadgeComponent GetBadgeComponent()
        {
            return BadgeComponent;
        }

        public InventoryComponent GetInventoryComponent()
        {
            return InventoryComponent;
        }

        public SearchesComponent GetNavigatorSearches()
        {
            return this._navigatorSearches;
        }

        public EffectsComponent Effects()
        {
            return this._fx;
        }

        public ClothingComponent GetClothing()
        {
            return this._clothing;
        }

        public int GetQuestProgress(int p)
        {
            int progress = 0;
            quests.TryGetValue(p, out progress);
            return progress;
        }

        public UserAchievement GetAchievementData(string p)
        {
            UserAchievement achievement = null;
            Achievements.TryGetValue(p, out achievement);
            return achievement;
        }

        public void ChangeName(string Username)
        {
            this.LastNameChange = PlusEnvironment.GetUnixTimestamp();
            this.Username = Username;

            this.SaveKey("username", Username);
            this.SaveKey("last_change", this.LastNameChange.ToString());
        }

        public void SaveKey(string Key, string Value)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `users` SET " + Key + " = @value WHERE `id` = '" + this.Id + "' LIMIT 1;");
                dbClient.AddParameter("value", Value);
                dbClient.RunQuery();
            }
        }

        public void PrepareRoom(int Id, string Password)
        {
            if (this.GetClient() == null || this.GetClient().GetHabbo() == null)
                return;

            if (this.GetClient().GetHabbo().IsTeleporting && this.GetClient().GetHabbo().TeleportingRoomID != Id)
            {
                this.GetClient().GetHabbo().IsTeleporting = true;
                this.GetClient().GetHabbo().TeleportingRoomID = Id;
                PrepareRoom(Id, Password);
                Console.WriteLine(this.Username + " PrepareRoom() again");
                return;
            }

            if (this.GetClient().GetHabbo().InRoom)
            {
                Room OldRoom = null;
                if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(this.GetClient().GetHabbo().CurrentRoomId, out OldRoom))
                    return;

                if (OldRoom.GetRoomUserManager() != null)
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(this.GetClient(), false, false);
            }

            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            if (Room == null)
            {
                this.GetClient().SendMessage(new CloseConnectionComposer());
                return;
            }

            if (Room.isCrashed)
            {
                this.GetClient().SendNotification("This room has crashed! :(");
                this.GetClient().SendMessage(new CloseConnectionComposer());
                return;
            }

            this.GetClient().GetHabbo().CurrentRoomId = Room.RoomId;

            #region Non-RP Features
            /* if (Room.GetRoomUserManager().userCount >= Room.UsersMax && !this.GetClient().GetHabbo().GetPermissions().HasRight("room_enter_full") && this.GetClient().GetHabbo().Id != Room.OwnerId)
             {
                 this.GetClient().SendMessage(new CantConnectComposer(1));
                 this.GetClient().SendMessage(new CloseConnectionComposer());
                 return;
             }

             if (!this.GetClient().GetHabbo().GetPermissions().HasRight("room_ban_override") && Room.UserIsBanned(this.GetClient().GetHabbo().Id))
             {
                 if (Room.HasBanExpired(this.GetClient().GetHabbo().Id))
                     Room.RemoveBan(this.GetClient().GetHabbo().Id);
                 else
                 {
                     this.GetClient().GetHabbo().RoomAuthOk = false;
                     this.GetClient().SendMessage(new CantConnectComposer(4));
                     this.GetClient().SendMessage(new CloseConnectionComposer());
                     return;
                 }
             }

             this.GetClient().SendMessage(new OpenConnectionComposer());
             if (!Room.CheckRights(this.GetClient(), true, true) && !this.GetClient().GetHabbo().IsTeleporting && !this.GetClient().GetHabbo().IsHopping)
             {
                 if (Room.Access == RoomAccess.DOORBELL && !this.GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                 {
                     if (Room.UserCount > 0)
                     {
                         this.GetClient().SendMessage(new DoorbellComposer(""));
                         Room.SendMessage(new DoorbellComposer(this.GetClient().GetHabbo().Username), true);
                         return;
                     }
                     else
                     {
                         this.GetClient().SendMessage(new FlatAccessDeniedComposer(""));
                         this.GetClient().SendMessage(new CloseConnectionComposer());
                         return;
                     }
                 }
                 else if (Room.Access == RoomAccess.PASSWORD && !this.GetClient().GetHabbo().GetPermissions().HasRight("room_enter_locked"))
                 {
                     if (Password.ToLower() != Room.Password.ToLower() || String.IsNullOrWhiteSpace(Password))
                     {
                         this.GetClient().SendMessage(new GenericErrorComposer(-100002));
                         this.GetClient().SendMessage(new CloseConnectionComposer());
                         return;
                     }
                 }
             }*/
            #endregion

            if (!EnterRoom(Room))
                this.GetClient().SendMessage(new CloseConnectionComposer());
        }

        public bool EnterRoom(Room Room)
        {
            if (Room == null)
                this.GetClient().SendMessage(new CloseConnectionComposer());

            this.GetClient().SendMessage(new RoomReadyComposer(Room.RoomId, Room.ModelName));
            if (Room.Wallpaper != "0.0")
                this.GetClient().SendMessage(new RoomPropertyComposer("wallpaper", Room.Wallpaper));
            if (Room.Floor != "0.0")
                this.GetClient().SendMessage(new RoomPropertyComposer("floor", Room.Floor));

            this.GetClient().SendMessage(new RoomPropertyComposer("landscape", Room.Landscape));
            this.GetClient().SendMessage(new RoomRatingComposer(Room.Score, !(this.GetClient().GetHabbo().RatedRooms.Contains(Room.RoomId) || Room.OwnerId == this.GetClient().GetHabbo().Id)));


            if (Room.OwnerId != this.Id)
            {
                this.GetClient().GetHabbo().GetStats().RoomVisits += 1;
            }
            return true;
        }

        public void EnterTurf(int TeleRoomId, int LinkedTele, string Action)
        {
            RoomUser User = this.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(this.Id);

            Room TurfInside = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(TeleRoomId);

            if (Action == "enter" && this.GetClient().GetHabbo().CurrentRoom.Capture == this.Gang) //enter
            {
                this.TurfInsideId = TeleRoomId;

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "timed-action;" + "show" + ";" + "Entering " + TurfInside.Name + ";" + 3000 + ";" + 3);
                System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
                timer1.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
                timer1.Elapsed += delegate
                {
                    if (this.TurfInsideId == TeleRoomId)
                    {
                        User.IsWalking = false;
                        User.CanWalk = false;
                        User.GetClient().GetHabbo().IsTeleporting = true;
                        User.GetClient().GetHabbo().TeleportingRoomID = TeleRoomId;
                        User.GetClient().GetHabbo().TeleporterId = LinkedTele;
                        User.GetClient().GetHabbo().CanChangeRoom = true;
                        
                        RoleplayManager.InstantRL(User.GetClient(), TeleRoomId);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "timed-action;" + "hide");
                    }
                    timer1.Stop();
                };
                timer1.Start();
            }
            else if (Action == "leave") //leave
            {
                this.TurfInsideId = TeleRoomId;

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "timed-action;" + "show" + ";" + "Leaving " + this.CurrentRoom.Name + ";" + 3000 + ";" + 3);
                System.Timers.Timer timer1 = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
                timer1.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
                timer1.Elapsed += delegate
                {
                    if (this.TurfInsideId == TeleRoomId)
                    {
                        User.IsWalking = false;
                        User.CanWalk = false;
                        User.GetClient().GetHabbo().IsTeleporting = true;
                        User.GetClient().GetHabbo().TeleportingRoomID = TeleRoomId;
                        User.GetClient().GetHabbo().TeleporterId = LinkedTele;
                        User.GetClient().GetHabbo().CanChangeRoom = true;
                        
                        RoleplayManager.InstantRL(User.GetClient(), TeleRoomId);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(User.GetClient(), "timed-action;" + "hide");
                    }
                    timer1.Stop();
                };
                timer1.Start();
            }
            else
            {
                User.GetClient().SendWhisper("Your gang does not own this turf");
            }
        }
    }
}