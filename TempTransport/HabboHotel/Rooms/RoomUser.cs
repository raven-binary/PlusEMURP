using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Plus.Communication.Packets.Outgoing;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.Core;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Pathfinding;
using Plus.HabboHotel.Rooms.AI;
using Plus.HabboHotel.Rooms.AI.Speech;
using Plus.HabboHotel.Rooms.Games;
using System.Text.RegularExpressions;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.Rooms.Games.Freeze;
using Plus.HabboHotel.Rooms.Games.Teams;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.HabboHotel.Rooms.Chat.Commands;
using System.Collections.Concurrent;
using Plus.Utilities;

namespace Plus.HabboHotel.Rooms
{
    public class RoomUser
    {
        public bool AllowOverride;
        public BotAI BotAI;
        public RoomBot BotData;
        public bool CanWalk;
        public int CarryItemID; //byte
        public int CarryTimer; //byte
        public int ChatSpamCount = 0;
        public int ChatSpamTicks = 16;
        public ItemEffectType CurrentItemEffect;
        public int DanceId;
        public bool FastWalking = false;
        public bool SuperFastWalking = false;
        public bool UltraFastWalking = false;
        public int FreezeCounter;
        public int FreezeLives;
        public bool Freezed;
        public bool Frozen;
        public bool Tased;
        public string userTased;
        public string userFocused = null;
        public bool Disconnect;
        public int GateId;
        public bool makeAction;
        public bool ConnectedMetier;
        public string Transaction;
        public int AppartItem = 0;
        public int Item_On;
        public DateTime lastAction;
        public DateTime lastActionWebEvent;
        public bool WebSocketUse;
        public bool usingATM;
        public bool RefillinAtm;
        public bool wantSoin = false;
        public bool canUseCB = false;
        public bool isDemarreCar;
        public bool isMakingCard;
        public string isMakingCardCode = null;
        public int GoalX; //byte
        public int GoalY; //byte
        public int HabboId;
        public int HorseID = 0;
        public int IdleTime; //byte
        public bool InteractingGate;
        public int InternalRoomID;
        public bool canUseFoutain = false;
        public bool isBruling = false;
        public int isBrulingItem = 0;
        public bool IsAsleep;
        public bool IsWalking;
        public int LastBubble = 0;
        public double LastInteraction;
        public Item LastItem = null;
        public int LockedTilesCount;
        public string AnalyseUser;
        public string DuelUser = null;
        public string DuelToken = null;
        public bool makeCreation = false;
        public int creationTimer;
        public int farmingTimer;
        public string CreationName;
        public bool Immunised = false;
        public string ImmunisedToken;
        public bool HaveTicket;
        public bool connectedToSlot = false;
        public bool isSlot = false;
        public string boissonToken = null;
        public string boissonPrepared = null;
        public bool participateRoulette = false;
        public int numberRoulette = 0;
        public int miseRoulette = 0;
        public string ManVsZombieTeam = null;
        public bool usingCasier = false;
        public bool usingFish = false;
        public bool usingSellfish = false;
        public bool usingSellWeapon = false;
        public bool usingPhoneStore = false;
        public bool usingPhoneCreditStore = false;
        public bool usingCasinoChips = false;
        public bool TaxiOpen = false;
        public bool usingClothShop = false;
        public bool usingTaxi = false;
        public bool usingTaxiDrive = false;
        public bool usingShop = false;
        public bool usingFarm = false;
        public bool usingJobcenter = false;
        public bool usingTrash = false;

        public bool NowLogged = false;
        internal string LastMessage = "";
        public bool sellingStock = false;
        public bool usingBank = false;
        public bool usingBankDeposit = false;
        public bool usingBankWithdraw = false;
        public bool OnDuty = false;
        public bool mainPropre = false;
        public bool isDisconnecting = false;
        public string fouillerUser = null;
        public bool isRobCash = false;
        public bool usingInfoTerminal = false;
        public bool usingClothingStore = false;


        #region Roleplay Releated
        public int RockId = 0;
        public int RockTimer = 0;
        #endregion


        // EVENT
        public bool haveMunition = false;
        public int creationGift = 0;
        public int creationGiftTotal = 0;

        public List<Vector2D> Path = new List<Vector2D>();
        public bool PathRecalcNeeded = false;
        public int PathStep = 1;
        public Pet PetData;

        public int PrevTime;
        public bool RidingHorse = false;
        public int RoomId;
        public int RotBody; //byte
        public int RotHead; //byte

        public bool SetStep;
        public int SetX; //byte
        public int SetY; //byte
        public double SetZ;
        public double SignTime;
        public byte SqState;
        public Dictionary<string, string> Statusses;
        public int TeleDelay; //byte
        public bool TeleportEnabled;
        public bool UpdateNeeded;
        public int VirtualId;
        public string Purchase = "";
        public int isFarmingRock = 0;

        public int X; //byte
        public int Y; //byte
        public double Z;

        public FreezePowerUp banzaiPowerUp;
        public bool isLying = false;
        public bool isSitting = false;
        private GameClient mClient;
        private Room mRoom;
        public bool moonwalkEnabled = false;
        public bool shieldActive;
        public int shieldCounter;
        public TEAM Team;
        public bool FreezeInteracting;
        public int UserId;
        public bool IsJumping;

        public bool isRolling = false;
        public int rollerDelay = 0;

        public int LLPartner = 0;
        public double TimeInRoom = 0;
        public int Capture = 0;
        public int CaptureProgress = 0;
        public bool cheveuxPropre = false;
        public string usernameCoiff = null;
        public int userChangeRank;
        public bool policeFouille = false;

        internal int jailgate;
        internal int jailgate_timer;

        // ECHANGE
        public bool isTradingItems = false;
        public string isTradingUsername = null;
        public bool isTradingConfirm = false;
        public Dictionary<string, string> isTradingListItems = new Dictionary<string, string>();

        public RoomUser(int HabboId, int RoomId, int VirtualId, Room room)
        {
            this.Freezed = false;
            this.HabboId = HabboId;
            this.RoomId = RoomId;
            this.VirtualId = VirtualId;
            this.IdleTime = 0;

            this.X = 0;
            this.Y = 0;
            this.Z = 0;
            this.PrevTime = 0;
            this.RotHead = 0;
            this.RotBody = 0;
            this.UpdateNeeded = true;
            this.Statusses = new Dictionary<string, string>();

            this.TeleDelay = -1;
            this.mRoom = room;

            this.AllowOverride = false;
            this.CanWalk = true;


            this.SqState = 3;

            this.InternalRoomID = 0;
            this.CurrentItemEffect = ItemEffectType.NONE;

            this.FreezeLives = 0;
            this.InteractingGate = false;
            this.GateId = 0;
            this.LastInteraction = 0;
            this.LockedTilesCount = 0;

            this.IsJumping = false;
            this.TimeInRoom = 0;
        }


        public Point Coordinate
        {
            get { return new Point(X, Y); }
        }

        public bool IsPet
        {
            get { return (IsBot && BotData.IsPet); }
        }

        public int CurrentEffect
        {
            get { return GetClient().GetHabbo().Effects().CurrentEffect; }
        }


        public bool IsDancing
        {
            get
            {
                if (DanceId >= 1)
                {
                    return true;
                }

                return false;
            }
        }

        public bool NeedsAutokick
        {
            get
            {
                if (IsBot)
                    return false;

                if (GetClient() == null || GetClient().GetHabbo() == null)
                    return true;

                if (GetClient().GetHabbo().GetPermissions().HasRight("mod_tool") || GetRoom().OwnerId == HabboId)
                    return false;

                if (GetRoom().Id == 1649919)
                    return false;

                if (IdleTime >= 7200)
                    return true;

                return false;
            }
        }

        public bool IsTrading
        {
            get
            {
                if (IsBot)
                    return false;

                if (Statusses.ContainsKey("trd"))
                    return true;

                return false;
            }
        }

        public bool IsBot
        {
            get
            {
                if (BotData != null)
                    return true;

                return false;
            }
        }

        public string GetUsername()
        {
            if (IsBot)
                return string.Empty;

            if (GetClient() != null)
            {
                if (GetClient().GetHabbo() != null)
                {
                    return GetClient().GetHabbo().Username;
                }
                else
                    return PlusEnvironment.GetUsernameById(HabboId);

            }
            else
                return PlusEnvironment.GetUsernameById(HabboId);
        }

        public void UnIdle()
        {
            if (!IsBot)
            {
                if (GetClient() != null && GetClient().GetHabbo() != null)
                    GetClient().GetHabbo().TimeAFK = 0;
            }

            IdleTime = 0;

            if (IsAsleep)
            {
                IsAsleep = false;
                GetRoom().SendMessage(new SleepComposer(this, false));
                this.GetClient().GetHabbo().resetEffectEvent();
            }
        }

        public void Dispose()
        {
            Statusses.Clear();
            mRoom = null;
            mClient = null;
        }

        public void Chat(string Message, bool Shout, int colour = 0)
        {
            if (GetRoom() == null)
                return;

            if (!IsBot)
                return;


            if (IsPet)
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetUserList().ToList())
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                        return;

                    if (!User.GetClient().GetHabbo().AllowPetSpeech)
                        User.GetClient().SendMessage(new ChatComposer(VirtualId, Message, 0, 0));
                }
            }
            else
            {
                foreach (RoomUser User in GetRoom().GetRoomUserManager().GetUserList().ToList())
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
                        return;

                    if (!User.GetClient().GetHabbo().AllowBotSpeech)
                        User.GetClient().SendMessage(new ChatComposer(VirtualId, Message, 0, (colour == 0 ? 2 : colour)));
                }
            }
        }

        public void HandleSpamTicks()
        {
            if (ChatSpamTicks >= 0)
            {
                ChatSpamTicks--;

                if (ChatSpamTicks == -1)
                {
                    ChatSpamCount = 0;
                }
            }
        }

        public bool IncrementAndCheckFlood(out int MuteTime)
        {
            MuteTime = 0;

            ChatSpamCount++;
            if (ChatSpamTicks == -1)
                ChatSpamTicks = 8;
            else if (ChatSpamCount >= 6)
            {
                if (GetClient().GetHabbo().GetPermissions().HasRight("events_staff"))
                    MuteTime = 3;
                else if (GetClient().GetHabbo().GetPermissions().HasRight("gold_vip"))
                    MuteTime = 7;
                else if (GetClient().GetHabbo().GetPermissions().HasRight("silver_vip"))
                    MuteTime = 10;
                else
                    MuteTime = 20;

                GetClient().GetHabbo().FloodTime = PlusEnvironment.GetUnixTimestamp() + MuteTime;

                ChatSpamCount = 0;
                return true;
            }
            return false;
        }

        public void OnChat(int Colour, string Message, bool Shout)
        {
            if (GetClient() == null || GetClient().GetHabbo() == null || mRoom == null)
                return;

            if (mRoom.GetWired().TriggerEvent(Items.Wired.WiredBoxType.TriggerUserSays, GetClient().GetHabbo(), Message))
                return;

            GetClient().GetHabbo().HasSpoken = true;

            if (mRoom.WordFilterList.Count > 0 && !GetClient().GetHabbo().GetPermissions().HasRight("word_filter_override"))
            {
                Message = mRoom.GetFilter().CheckMessage(Message);
            }
            string Name = "";
            if (Message.Contains("@x"))
            {
                var Target = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(GetClient().GetRoleplay().TargetId);
                if (Target != null)
                {
                    Name = Target.GetHabbo().Username;
                    Message = Regex.Replace(Message, "@x", "@" + Name + "").Trim();
                    ServerPacket packet = new ChatComposer(VirtualId, Message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message), 4);
                    if (Shout)
                        packet = new ShoutComposer(VirtualId, Message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message), 4);
                    Target.SendMessage(packet);
                }
            }
            ServerPacket Packet = null;
            if (Shout)
                Packet = new ShoutComposer(VirtualId, Message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message), Colour);
            else
                Packet = new ChatComposer(VirtualId, Message, PlusEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(Message), Colour);

            ColorAndPrefix();

            foreach (RoomUser User in mRoom.GetRoomUserManager().GetRoomUsers().ToList())
                {
                if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().MutedUsers.Contains(mClient.GetHabbo().Id))
                    continue;

                if (mRoom.chatDistance > 0 && Gamemap.TileDistance(this.X, this.Y, User.X, User.Y) > mRoom.chatDistance)
                    continue;

                User.GetClient().SendMessage(Packet);
            }
            NoColor();

            #region Pets/Bots responces
            if (Shout)
            {
                foreach (RoomUser User in mRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (!User.IsBot)
                        continue;

                    if (User.IsBot)
                        User.BotAI.OnUserShout(this, Message);
                }
            }
            else
            {
                foreach (RoomUser User in mRoom.GetRoomUserManager().GetUserList().ToList())
                {
                    if (!User.IsBot)
                        continue;

                    if (User.IsBot)
                        User.BotAI.OnUserSay(this, Message);
                }
            }
            #endregion

        }
     
        public void Say(string Message, int Color = 0, bool Shout = false)
        {
            PlusEnvironment.GetGame().GetChatManager().GetLogs().StoreChatlog(new Plus.HabboHotel.Rooms.Chat.Logs.ChatlogEntry(GetClient().GetHabbo().Id, GetClient().GetHabbo().CurrentRoomId, "<Action> " + Message, UnixTimestamp.GetNow(), GetClient().GetHabbo(), GetClient().GetHabbo().CurrentRoom));

            GetClient().GetHabbo().CurrentRoom.AddChatlog(GetClient().GetHabbo().Id, "<Action> " + Message);

            if (Color == 0)
            {
                Color = this.LastBubble;
            }

            if (IsAsleep)
                UnIdle();

            GetClient().GetRoleplay().Username = "*" + GetUsername();
            Message = Message + "*";
            GetRoom().SendMessage(new UserNameChangeComposer(GetRoom().Id, VirtualId, GetClient().GetRoleplay().Username));
            OnChat(Color, Message, Shout);
        }

        public void ColorAndPrefix()
        {
            if (IsBot || IsPet || GetClient().GetHabbo().NameColor == "#000000")
                return;

            string Username;
            if (GetClient().GetHabbo().PrefixName != "null")
            {
                if (GetClient().GetRoleplay().Username.Contains("*"))
                {
                    GetClient().GetRoleplay().Username = "*" + GetClient().GetHabbo().PrefixName + " <font color='" + GetClient().GetHabbo().NameColor + "'>" + GetUsername() + "</font>";
                }
                else
                {
                    GetClient().GetRoleplay().Username = GetClient().GetHabbo().PrefixName + " <font color='" + GetClient().GetHabbo().NameColor + "'>" + GetUsername() + "</font>";
                }
            }
            else
            {
                Username = "<font color='" + GetClient().GetHabbo().NameColor + "'>" + GetUsername() + "</font>";

            }
            GetRoom().SendMessage(new UserNameChangeComposer(GetRoom().Id, VirtualId, GetClient().GetRoleplay().Username));
        }
        public void NoColor()
        {
            if (IsBot || IsPet)
                return;

            GetClient().GetRoleplay().Username = GetUsername();
            GetRoom().SendMessage(new UserNameChangeComposer(GetRoom().Id, VirtualId, GetUsername()));
        }

        public static string RainbowText(string Name)
        {
            StringBuilder NewName = new StringBuilder();

            string[] Colours = { "FF0000", "FFA500", "FFFF00", "008000", "0000FF", "800080" };

            int Count = 0;
            int Count2 = 0;
            while (Count < Name.Length)
            {
                NewName.Append("<font color='#" + Colours[Count2] + "'>" + Name[Count] + "</font>");

                Count++;
                Count2++;

                if (Count2 >= 6)
                    Count2 = 0;
            }

            return NewName.ToString();
        }

        public void SendMeCommandPacket()
        {
            if (IsBot || GetClient() == null || GetClient().GetHabbo() == null)
                return;

            string Username = "*" + GetClient().GetHabbo().Username;

            if (GetRoom() != null)
                GetRoom().SendMessage(new UserNameChangeComposer(RoomId, VirtualId, Username));
        }

        public void SendNamePacket()
        {
            if (IsBot || GetClient() == null || GetClient().GetHabbo() == null)
                return;

            string Username = GetClient().GetHabbo().Username;

            if (GetRoom() != null)
                GetRoom().SendMessage(new UserNameChangeComposer(RoomId, VirtualId, Username));
        }

        public void ClearMovement(bool Update)
        {
            IsWalking = false;
            Statusses.Remove("mv");
            GoalX = 0;
            GoalY = 0;
            SetStep = false;
            SetX = 0;
            SetY = 0;
            SetZ = 0;

            if (Update)
            {
                UpdateNeeded = true;
            }
        }

        public void setDuelWinner()
        {
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.DuelUser);
            if (TargetClient != null)
            {
                Room TargetRoom = TargetClient.GetHabbo().CurrentRoom;
                RoomUser TargetUser = TargetRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
                foreach (Item item in GetRoom().GetRoomItemHandler().GetFloor)
                {
                    if (item.GetBaseItem().SpriteId == 2536)
                    {
                        TargetUser.DuelUser = null;
                        TargetUser.DuelToken = null;
                        this.DuelUser = null;
                        this.DuelToken = null;
                        TargetClient.GetHabbo().resetAvatarEvent();
                        TargetClient.GetHabbo().resetEffectEvent();
                        TargetClient.GetHabbo().usingArena = false;
                        GetRoom().GetGameMap().TeleportToItem(TargetUser, item);
                        GetRoom().GetRoomUserManager().UpdateUserStatusses();
                        PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + TargetClient.GetHabbo().Username + "</span> beat <span class=\"red\">" + this.GetClient().GetHabbo().Username + "</span> in the arena");
                        PlusEnvironment.ArenaUsing = false;
                    }
                }
            }
        }
        public void MoveToIfCanWalk(Point c)
        {
            if (this.CanWalk == false || !this.IsBot && this.isTradingItems == true)
                return;

            MoveTo(c.X, c.Y);
        }

        public void MoveTo(Point c)
        {
            MoveTo(c.X, c.Y);
        }

        public void MoveTo(int pX, int pY, bool pOverride)
        {
            if (TeleportEnabled)
            {
                UnIdle();
                GetRoom().SendMessage(GetRoom().GetRoomItemHandler().UpdateUserOnRoller(this, new Point(pX, pY), 0, GetRoom().GetGameMap().SqAbsoluteHeight(GoalX, GoalY)));
                if (Statusses.ContainsKey("sit"))
                    Z -= 0.35;
                UpdateNeeded = true;
                return;
            }

            if ((GetRoom().GetGameMap().SquareHasUsers(pX, pY) && !pOverride) || Frozen)
                return;

            UnIdle();

            GoalX = pX;
            GoalY = pY;
            PathRecalcNeeded = true;
            FreezeInteracting = false;

            if (!this.IsBot)
                this.GetClient().GetRoleplay().StopAction();

            if (!this.IsBot && this.usingCasier)
            {
                this.usingCasier = false;
                this.GetClient().Shout("*Locks his locker with his padlock*");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "casier;hide");
            }


            if (!this.IsBot && this.usingTaxi)
            {
                this.usingTaxi = false;
                this.Say("Cancels their taxi");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "taxi;close");
                this.ApplyEffect(0);
            }

            if (!this.IsBot && this.usingFish)
            {
                this.usingFish = false;
                this.GetClient().Shout("*Stops fishing*");
                this.ApplyEffect(0);
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "fish_carrot_inventory;close;");
            }

            if (!this.IsBot && this.usingFarm)
            {
                this.usingFarm = false;
                this.Say("stops to harvesting carrots");
                this.ApplyEffect(0);
                this.GetClient().GetHabbo().FarmToken = 0;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingCrosstrainer)
            {
                this.GetClient().GetHabbo().usingCrosstrainer = false;
                this.GetClient().SendWhisper("You stop working out on the Cross Trainer");
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().PlayToken = 0;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingTreadMill)
            {
                this.GetClient().GetHabbo().usingTreadMill = false;
                this.GetClient().SendWhisper("You stop working out on the Treadmill");
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().PlayToken = 0;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingTrampoline)
            {
                this.GetClient().GetHabbo().usingTrampoline = false;
                this.GetClient().SendWhisper("You stop working out on the Trampoline");
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().PlayToken = 0;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingPassive)
            {
                this.GetClient().GetHabbo().usingPassive = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().isUncuffing)
            {
                this.GetClient().GetHabbo().isUncuffing = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().LockpickingTo != null)
            {
                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.GetClient().GetHabbo().LockpickingTo);
                if (TargetClient != null)
                {
                    TargetClient.GetHabbo().LockpickingFrom = null;
                }

                this.Say("stops lockpicking " + this.GetClient().GetHabbo().LockpickingTo + "'s cuffs");
                this.GetClient().GetHabbo().LockpickingTo = null;
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().PlayToken = 0;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().Escorting)
            {
                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.GetClient().GetHabbo().EscortUsername);
                if (TargetClient != null && TargetClient.GetHabbo().LockpickingFrom != null)
                {
                    GameClient LockpickingFrom = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(TargetClient.GetHabbo().LockpickingFrom);
                    RoomUser UserLockpickingFrom = LockpickingFrom.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().LockpickingFrom);
                    if (LockpickingFrom != null && UserLockpickingFrom != null && LockpickingFrom.GetHabbo().LockpickingTo == TargetClient.GetHabbo().Username)
                    {
                        UserLockpickingFrom.Say("stops lockpicking " + LockpickingFrom.GetHabbo().LockpickingTo + "'s cuffs");
                        TargetClient.GetHabbo().LockpickingFrom = null;
                        LockpickingFrom.GetHabbo().LockpickingTo = null;
                        LockpickingFrom.GetHabbo().resetEffectEvent();
                        LockpickingFrom.GetHabbo().PlayToken = 0;
                    }
                }
            }

            if (!this.IsBot && this.GetClient().GetHabbo().isUsingPoliceCar)
            {
                this.Say("hops out of their squad car");
                this.GetClient().GetHabbo().isUsingPoliceCar = false;
                this.GetClient().GetHabbo().PlayToken = 0;
                this.GetClient().GetHabbo().resetEffectEvent();
            }

            if (!this.IsBot && this.GetClient().GetHabbo().isTradingWith != null)
            {
                this.Say("cancels the trade");

                GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.GetClient().GetHabbo().isTradingWith);
                if (TargetClient != null)
                {
                    this.GetClient().GetHabbo().isTradingWith = null;
                    this.GetClient().GetHabbo().TradeConfirmed = false;
                    this.GetClient().GetHabbo().TradeMoney = 0;
                    this.GetClient().GetHabbo().TradeSlot1 = null;
                    this.GetClient().GetHabbo().TradeSlot1Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot1Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot2 = null;
                    this.GetClient().GetHabbo().TradeSlot2Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot2Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot3 = null;
                    this.GetClient().GetHabbo().TradeSlot3Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot3Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot4 = null;
                    this.GetClient().GetHabbo().TradeSlot4Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot4Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot5 = null;
                    this.GetClient().GetHabbo().TradeSlot5Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot5Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot6 = null;
                    this.GetClient().GetHabbo().TradeSlot6Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot6Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot7 = null;
                    this.GetClient().GetHabbo().TradeSlot7Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot7Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot8 = null;
                    this.GetClient().GetHabbo().TradeSlot8Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot8Durability = 0;

                    TargetClient.GetHabbo().isTradingWith = null;
                    TargetClient.GetHabbo().TradeConfirmed = false;
                    TargetClient.GetHabbo().TradeMoney = 0;
                    TargetClient.GetHabbo().TradeSlot1 = null;
                    TargetClient.GetHabbo().TradeSlot1Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot1Durability = 0;
                    TargetClient.GetHabbo().TradeSlot2 = null;
                    TargetClient.GetHabbo().TradeSlot2Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot2Durability = 0;
                    TargetClient.GetHabbo().TradeSlot3 = null;
                    TargetClient.GetHabbo().TradeSlot3Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot3Durability = 0;
                    TargetClient.GetHabbo().TradeSlot4 = null;
                    TargetClient.GetHabbo().TradeSlot4Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot4Durability = 0;
                    TargetClient.GetHabbo().TradeSlot5 = null;
                    TargetClient.GetHabbo().TradeSlot5Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot5Durability = 0;
                    TargetClient.GetHabbo().TradeSlot6 = null;
                    TargetClient.GetHabbo().TradeSlot6Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot6Durability = 0;
                    TargetClient.GetHabbo().TradeSlot7 = null;
                    TargetClient.GetHabbo().TradeSlot7Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot7Durability = 0;
                    TargetClient.GetHabbo().TradeSlot8 = null;
                    TargetClient.GetHabbo().TradeSlot8Quantity = 0;
                    TargetClient.GetHabbo().TradeSlot8Durability = 0;

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "trade-accept;end");
                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;end");

                    for (int i = 1; i <= 10; i++)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "trade", "take-item,slot" + i);
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(TargetClient, "trade", "take-item,slot" + i);

                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "trade-accept;slots;" + i);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "trade-accept;slots;" + i);
                    }
                }
                else
                {
                    this.GetClient().GetHabbo().isTradingWith = null;
                    this.GetClient().GetHabbo().TradeConfirmed = false;
                    this.GetClient().GetHabbo().TradeMoney = 0;
                    this.GetClient().GetHabbo().TradeSlot1 = null;
                    this.GetClient().GetHabbo().TradeSlot1Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot1Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot2 = null;
                    this.GetClient().GetHabbo().TradeSlot2Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot2Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot3 = null;
                    this.GetClient().GetHabbo().TradeSlot3Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot3Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot4 = null;
                    this.GetClient().GetHabbo().TradeSlot4Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot4Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot5 = null;
                    this.GetClient().GetHabbo().TradeSlot5Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot5Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot6 = null;
                    this.GetClient().GetHabbo().TradeSlot6Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot6Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot7 = null;
                    this.GetClient().GetHabbo().TradeSlot7Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot7Durability = 0;
                    this.GetClient().GetHabbo().TradeSlot8 = null;
                    this.GetClient().GetHabbo().TradeSlot8Quantity = 0;
                    this.GetClient().GetHabbo().TradeSlot8Durability = 0;

                    PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "trade-accept;end");
                    for (int i = 1; i <= 10; i++)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(this.GetClient(), "trade", "take-item,slot" + i);
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "trade-accept;slots;" + i);
                    }
                }
            }

            if (!this.IsBot && this.GetClient().GetHabbo().TurfInsideId > 0)
            {
                this.GetClient().GetHabbo().TurfInsideId = 0;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingSuicide)
            {
                this.GetClient().GetHabbo().usingSuicide = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingSafeRob)
            {
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().usingSafeRob = false;
                this.GetClient().GetHabbo().PlayToken = 0;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingTrash)
            {
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().usingTrash = false;
                this.GetClient().GetHabbo().PlayToken = 0;
                this.Say("stops trawling through the bin");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.usingJobcenter)
            {
                this.usingJobcenter = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "jobcenter;hide");
            }

            if (!this.IsBot && this.usingClothingStore)
            {
                this.usingClothingStore = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "Clothing;hide");
            }

            /*if (!this.IsBot && this.Transaction != null && !this.Transaction.StartsWith("paramedic"))
            {
                this.Say("cancels the offer");
                this.Transaction = null;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "transaction-hide;");
            }*/

            if (!this.IsBot && this.GetClient().GetHabbo().IsUsingAppleEnergy)
            {
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().IsUsingAppleEnergy = false;
                this.GetClient().GetHabbo().PlayToken = 0;
                this.Say("stops eating the apple");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().UsingTask != "")
            {
                if (this.GetClient().GetHabbo().UsingTask == "blood")
                {
                    this.Say("stops cleaning the blood");
                }
                else if (this.GetClient().GetHabbo().UsingTask == "rubbish")
                {
                    this.Say("stops cleaning the rubbish");
                }
                else if (this.GetClient().GetHabbo().UsingTask == "coffespill")
                {
                    this.Say("stops cleaning the coffee spill");
                }
                else if (this.GetClient().GetHabbo().UsingTask == "cofferepair")
                {
                    this.Say("stops repairing the coffee machine");
                }
                this.GetClient().GetHabbo().UsingTask = "";
                this.GetClient().GetHabbo().PlayToken = 0;
                this.GetClient().GetHabbo().resetEffectEvent();
            }

            if (!this.IsBot && this.GetClient().GetHabbo().UsingFarm != "")
            {
                if (this.GetClient().GetHabbo().UsingFarm == "coffee beans")
                {
                    this.Say("stops farming the coffee plant");
                }
                else if (this.GetClient().GetHabbo().UsingFarm == "mending weed")
                {
                    this.Say("stops farming the weed plant");
                }
                else if (this.GetClient().GetHabbo().UsingFarm == "wool")
                {
                    this.Say("stops farming the wool plant");
                }
                else if (this.GetClient().GetHabbo().UsingFarm == "rock")
                {
                    this.Say("stops smashing the rock");
                }
                this.GetClient().GetHabbo().UsingFarm = "";
                this.GetClient().GetHabbo().PlayToken = 0;
                this.GetClient().GetHabbo().resetEffectEvent();
            }

            #region Rolepoay Releated
            if (!this.IsBot && this.RockId > 0)
            {
                Say("stops smashing the rock");
                RockId = 0;
                RockTimer = 0;
                GetClient().GetRoleplay().ResetEffect();
            }
            #endregion


            if (!this.IsBot && this.Transaction != null)
            {
                GameClient TransactionFrom = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.GetClient().GetHabbo().TransactionFrom);

                this.GetClient().SendWhisper("You have canceled the offer");
                TransactionFrom.SendWhisper(this.GetUsername() + " canceled the offer");
                TransactionFrom.GetHabbo().TransactionTo = null;

                this.Transaction = null;
                this.GetClient().GetHabbo().TransactionFrom = null;
                this.GetClient().GetHabbo().OfferToken = 0;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "transaction-hide;");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().TransactionTo != null)
            {
                GameClient TransactionTo = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(this.GetClient().GetHabbo().TransactionTo);

                Room Room = this.GetClient().GetHabbo().CurrentRoom;
                RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(this.GetClient().GetHabbo().TransactionTo);

                this.GetClient().SendWhisper("You have canceled the offer");
                TransactionTo.SendWhisper(this.GetUsername() + " canceled the offer");

                this.GetClient().GetHabbo().TransactionTo = null;

                User2.Transaction = null;
                User2.GetClient().GetHabbo().TransactionFrom = null;
                User2.GetClient().GetHabbo().OfferToken = 0;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TransactionTo, "transaction-hide;");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().UsingManagerMail)
            {
                this.GetClient().GetHabbo().UsingManagerMail = false;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().UsingArmouryMerchandise)
            {
                this.GetClient().GetHabbo().UsingArmouryMerchandise = false;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingMenuSell)
            {
                this.GetClient().GetHabbo().usingMenuSell = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "button-menu;hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingSellingStock)
            {
                this.GetClient().GetHabbo().usingSellingStock = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "selling-stock;hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingMarketplace)
            {
                this.GetClient().GetHabbo().usingMarketplace = false;
                this.GetClient().GetHabbo().usingManageSales = false;
                this.GetClient().GetHabbo().usingCreateSale = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "marketplace;hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "manage-sales;hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "create-sale;hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().UsingJukebox)
            {
                this.GetClient().GetHabbo().UsingJukebox = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "jukebox-add;hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingRobTill)
            {
                this.Freezed = false;
                this.GetClient().GetHabbo().resetEffectEvent();
                this.GetClient().GetHabbo().UsingItem = 0;
                this.GetClient().GetHabbo().usingRobTill = false;
                this.GetClient().GetHabbo().PlayToken = 0;
                this.Say("stops taking money from the till");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
            }

            if (!this.IsBot && this.usingATM)
            {
                if (this.GetClient().GetHabbo().ClaimedATM != 0)
                {
                    using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `atms` SET `claimed` = 0 WHERE `atm_id` = '" + this.GetClient().GetHabbo().ClaimedATM + "' LIMIT 1;");
                        dbClient.RunQuery();
                    }
                    this.GetClient().GetHabbo().ClaimedATM = 0;
                }

                if (this.RefillinAtm)
                {
                    this.Say("stops to refilling the ATM");
                    this.RefillinAtm = false;
                    this.GetClient().GetHabbo().PlayToken = 0;
                    this.GetClient().GetHabbo().resetEffectEvent();
                }

                this.usingATM = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "ATM;disconnect;");

            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingChapelActionPoint)
            {
                this.GetClient().GetHabbo().usingChapelActionPoint = false;

                if (this.GetClient().GetHabbo().MarryingTo != null)
                {
                    Room Room = this.GetClient().GetHabbo().CurrentRoom;
                    RoomUser User2 = Room.GetRoomUserManager().GetRoomUserByHabbo(this.GetClient().GetHabbo().MarryingTo);

                    this.GetClient().SendWhisper("Looks like you didn't want to get married after all");
                    User2.GetClient().SendWhisper("Looks like " + this.GetClient().GetHabbo().Username + " didn't want to get married after all");
                    User2.GetClient().GetHabbo().MarryingTo = null;
                    User2.GetClient().GetHabbo().IsWaitingToMarry = false;
                    User2.GetClient().GetHabbo().IsWaitingToMarryReply = false;
                    User2.GetClient().GetHabbo().IsWaitingToMarryReplyDone = false;
                }
                this.GetClient().GetHabbo().MarryingTo = null;
                this.GetClient().GetHabbo().IsWaitingToMarry = false;
                this.GetClient().GetHabbo().IsWaitingToMarryReply = false;
                this.GetClient().GetHabbo().IsWaitingToMarryReplyDone = false;

            }

            /*if (!this.IsBot && this.GetClient().GetHabbo().CurrentRoomId == 16 && this.X == 21 && this.Y == 11)
            {
                this.sellingStock = true;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "sell-stock;show;Carrot;carrot");
            }
            else if (!this.IsBot && this.sellingStock)
            {
                this.sellingStock = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "sell-stock;hide");
            }*/

            if (!this.IsBot && this.GetClient().GetHabbo().usingArrestActionPoint)
            {
                this.GetClient().GetHabbo().usingArrestActionPoint = false;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingGymMembershipPurchase)
            {
                this.GetClient().GetHabbo().usingGymMembershipPurchase = false;
            }

            if (!this.IsBot && this.GetClient().GetHabbo().UsingBounties)
            {
                this.GetClient().GetHabbo().UsingBounties = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "bounty;hide");
            }

            if (!this.IsBot && this.GetClient().GetHabbo().usingDepositBox)
            {
                this.GetClient().GetHabbo().usingDepositBox = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "deposit-box;hide");
            }

            if (!this.IsBot && this.usingSellfish)
            {
                this.usingSellfish = false;
                this.GetClient().Shout("*Close the laptop*");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "sellfish;close");
            }

            if (!this.IsBot && this.usingSellWeapon)
            {
                this.usingSellWeapon = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "sellweapon;close");
            }

            if (!this.IsBot && this.usingPhoneStore)
            {
                this.usingPhoneStore = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "phonestore;close");
            }

            if (!this.IsBot && this.usingPhoneCreditStore)
            {
                this.usingPhoneCreditStore = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "phonecreditstore;close");
            }

            if (!this.IsBot && this.usingCasinoChips)
            {
                this.usingCasinoChips = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "casinochips;close");
            }

            if (!this.IsBot && this.canUseFoutain)
            {
                this.canUseFoutain = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "foutain;hide");
            }

            if (!this.IsBot && this.TaxiOpen)
            {
                this.usingTaxi = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "timed-action;" + "hide");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "taxi;close");
            }

            if (!this.IsBot && this.usingClothShop)
            {
                this.usingClothShop = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "Clothing;close");
            }


            if (!this.IsBot && this.usingShop)
            {
                this.usingShop = false;
                this.GetClient().Shout("*Close the PC*");
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "shop;close");
            }

            if (!this.IsBot && this.usingJobcenter)
            {
                this.usingJobcenter = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "jobcenter;close");
            }

            if (!this.IsBot && this.connectedToSlot == true)
            {
                this.connectedToSlot = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "slot_machine;hide");
            }

            if (!this.IsBot && this.isRobCash == true)
            {
                this.isRobCash = false;
                this.GetClient().Shout("*Stop robbing the register*");
                this.ApplyEffect(0);
            }

            if (!this.IsBot && this.usingInfoTerminal == true)
            {
                this.usingInfoTerminal = false;
            }

            if (!this.IsBot && this.usingClothingStore == true)
            {
                this.usingClothingStore = false;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(this.GetClient(), "Clothing;close;");
            }

            if (this.Item_On == 3451)
            {
                this.Item_On = 0;
                this.GetClient().GetHabbo().resetAvatarEvent();
                this.GetClient().GetHabbo().Hair = null;
            }
        }

        public void MoveTo(int pX, int pY)
        {
            MoveTo(pX, pY, false);
        }

        public void UnlockWalking()
        {
            AllowOverride = false;
            CanWalk = true;
        }
        public void SetPos(int pX, int pY, double pZ)
        {
            if (HabboId > 0)
            { 
                if (!GetClient().GetHabbo().LoadedPos)
                {
                    X = GetClient().GetRoleplay().RoomX;
                    Y = GetClient().GetRoleplay().RoomY;
                    Z = pZ;

                    GetClient().GetHabbo().LoadedPos = true;
                    this.SetPos(GetClient().GetRoleplay().RoomX, GetClient().GetRoleplay().RoomY, pZ);
                    this.UpdateNeeded = true;

                    #region Waiting For Paramedic
                    if (this.GetClient().GetRoleplay().Dead)
                    {
                        this.GetClient().GetHabbo().IsWaitingForParamedic = true;

                        if (this.RotBody % 2 == 0)
                        {
                            if (this != null)
                            {
                                try
                                {
                                    this.Statusses.Add("lay", "1.0 null");
                                    this.Z -= 0.35;
                                    this.isLying = true;
                                    this.UpdateNeeded = true;
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            this.RotBody--;
                            this.Statusses.Add("lay", "1.0 null");
                            this.Z -= 0.35;
                            this.isLying = true;
                            this.UpdateNeeded = true;
                        }

                        System.Timers.Timer DieTimerUser = new System.Timers.Timer(PlusEnvironment.ConvertMinutesToMilliseconds(1));
                        DieTimerUser.Interval = PlusEnvironment.ConvertMinutesToMilliseconds(1);
                        DieTimerUser.Elapsed += delegate
                        {
                            if (this.GetClient().GetRoleplay().Dead)
                            {
                                this.GetClient().GetHabbo().IsWaitingForParamedic = false;
                                this.GetClient().GetRoleplay().Dead = false;
                                this.GetClient().GetHabbo().RPCache(5);
                                this.GetClient().GetHabbo().Hospital = 1;
                                this.GetClient().GetHabbo().updateHospitalEtat(this, 3);
                            }
                            DieTimerUser.Stop();
                        };
                        DieTimerUser.Start();
                    }
                    #endregion
                }
                else
                {
                    X = pX;
                    Y = pY;
                    Z = pZ;

                    SetX = pX;
                    SetY = pY;
                    SetZ = pZ;
                }
            }
            else
            {
                X = pX;
                Y = pY;
                Z = pZ;

                SetX = pX;
                SetY = pY;
                SetZ = pZ;
            }
        }
        public void CarryItem(int Item)
        {
            CarryItemID = Item;

            if (Item > 0)
                CarryTimer = 240;
            else
                CarryTimer = 0;

            GetRoom().SendMessage(new CarryObjectComposer(VirtualId, Item));
        }
        public void SetRot(int Rotation, bool HeadOnly)
        {
            if (Statusses.ContainsKey("lay") || IsWalking)
            {
                return;
            }

            int diff = RotBody - Rotation;

            RotHead = RotBody;

            if (Statusses.ContainsKey("sit") || HeadOnly)
            {
                if (RotBody == 2 || RotBody == 4)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
                else if (RotBody == 0 || RotBody == 6)
                {
                    if (diff > 0)
                    {
                        RotHead = RotBody - 1;
                    }
                    else if (diff < 0)
                    {
                        RotHead = RotBody + 1;
                    }
                }
            }
            else if (diff <= -2 || diff >= 2)
            {
                RotHead = Rotation;
                RotBody = Rotation;
            }
            else
            {
                RotHead = Rotation;
            }
            UpdateNeeded = true;
        }

        public void SetStatus(string Key, string Value)
        {
            if (Statusses.ContainsKey(Key))
            {
                Statusses[Key] = Value;
            }
            else
            {
                AddStatus(Key, Value);
            }
        }

        public void AddStatus(string Key, string Value)
        {
            Statusses[Key] = Value;
        }

        public void RemoveStatus(string Key)
        {
            if (Statusses.ContainsKey(Key))
            {
                Statusses.Remove(Key);
            }
        }

        public void ApplyEffect(int effectID)
        {
            if (IsBot)
            {
                this.mRoom.SendMessage(new AvatarEffectComposer(VirtualId, effectID));
                return;
            }

            if (IsBot || GetClient() == null || GetClient().GetHabbo() == null || GetClient().GetHabbo().Effects() == null)
                return;

            GetClient().GetHabbo().Effects().ApplyEffect(effectID);
        }

        public Point SquareInFront
        {
            get
            {
                var Sq = new Point(this.X, this.Y);

                if (RotBody == 0)
                {
                    Sq.Y--;
                }
                else if (RotBody == 2)
                {
                    Sq.X++;
                }
                else if (RotBody == 4)
                {
                    Sq.Y++;
                }
                else if (RotBody == 6)
                {
                    Sq.X--;
                }

                return Sq;
            }
        }

        public Point SquareBehind
        {
            get
            {
                var Sq = new Point(this.X, this.Y);

                if (RotBody == 0)
                {
                    Sq.Y++;
                }
                else if (RotBody == 2)
                {
                    Sq.X--;
                }
                else if (RotBody == 4)
                {
                    Sq.Y--;
                }
                else if (RotBody == 6)
                {
                    Sq.X++;
                }
                else
                {
                    Sq.X--;
                }

                return Sq;
            }
        }

        public Point SquareLeft
        {
            get
            {
                var Sq = new Point(this.X, this.Y);

                if (RotBody == 0)
                {
                    Sq.X++;
                }
                else if (RotBody == 2)
                {
                    Sq.Y--;
                }
                else if (RotBody == 4)
                {
                    Sq.X--;
                }
                else if (RotBody == 6)
                {
                    Sq.Y++;
                }

                return Sq;
            }
        }

        public Point SquareRight
        {
            get
            {
                var Sq = new Point(this.X, this.Y);

                if (RotBody == 0)
                {
                    Sq.X--;
                }
                else if (RotBody == 2)
                {
                    Sq.Y++;
                }
                else if (RotBody == 4)
                {
                    Sq.X++;
                }
                else if (RotBody == 6)
                {
                    Sq.Y--;
                }
                return Sq;
            }
        }


        public GameClient GetClient()
        {
            if (IsBot)
            {
                return null;
            }
            if (mClient == null)
                mClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUserID(HabboId);
            return mClient;
        }

        public Room GetRoom()
        {
            if (mRoom == null)
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(RoomId, out mRoom))
                    return mRoom;

            return mRoom;
        }
    }

    public enum ItemEffectType
    {
        NONE,
        SWIM,
        SwimLow,
        SwimHalloween,
        Iceskates,
        Normalskates,
        PublicPool,
        //Skateboard?
    }

    public static class ByteToItemEffectEnum
    {
        public static ItemEffectType Parse(byte pByte)
        {
            switch (pByte)
            {
                case 0:
                    return ItemEffectType.NONE;
                case 1:
                    return ItemEffectType.SWIM;
                case 2:
                    return ItemEffectType.Normalskates;
                case 3:
                    return ItemEffectType.Iceskates;
                case 4:
                    return ItemEffectType.SwimLow;
                case 5:
                    return ItemEffectType.SwimHalloween;
                case 6:
                    return ItemEffectType.PublicPool;
                //case 7:
                //return ItemEffectType.Custom;
                default:
                    return ItemEffectType.NONE;
            }
        }
    }

    //0 = none
    //1 = pool
    //2 = normal skates
    //3 = ice skates
}