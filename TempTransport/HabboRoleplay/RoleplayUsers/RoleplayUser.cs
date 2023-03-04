using System;
using System.Data;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fleck;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.Rooms;
using Plus.HabboRoleplay.Misc;
using Plus.HabboRoleplay.Timer;
using Plus.HabboRoleplay.Inventory;
using Plus.HabboRoleplay.Bin;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Session;
using Plus.Communication.Packets.Outgoing.Rooms.Avatar;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboRoleplay.RoleplayUsers
{
    public class RoleplayUser
    {
        //Client Info
        GameClient Client;

        public RoleplayTimer RPTimer;
        public Wanted Wan;
        public Inv Inventory;
        public Bins Bin;

        private string _username;
        private string _bio;
        private int _health;
        private int _health_max;
        private int _energy;
        private int _aggression;
        private bool _passive;
        private int _timer;
        private bool _hospital;
        private bool _prison;
        private int _god_protection;
        private int _gym_membership;
        private int _roomX;
        private int _roomY;
        private int _job_id;
        private int _job_rank;
        private int _gang_id;
        private int _gang_rank;
        private int _combat_level;
        private int _combat_xp;
        private int _farming_level;
        private int _farming_xp;
        private int _kills;
        private int _deaths;
        private int _arrests;
        private int _assists;
        private int _damage_dealt;
        private int _damage_taken;
        private int _gang_kills;
        private int _explosive_kills;
        private int _paramedic_revives;
        private int _criminals_arrested;
        private int _sales;
        private int _net_worth;
        private int _money_earned;
        private int _money_won;
        private int _money_lost;
        private int _tickets_given;
        private int _shifts;
        private int _corp_shifts;
        private int _weekly_shifts;
        private int _corp_tasks;
        private int _drugs_taken;
        private int _arena_wins;
        private int _card_games_won;
        private int _card_games_lost;
        private int _lottery_jackpots;
        private int _married_to;
        private bool _dead;
        private DateTime _deposit_rent;
        private string _custom_KO_msg;
        private string _custom_death_msg;
        private string _custom_arrest_msg;
        private bool _livefeed;
        #region Inventory
        //equip area
        private string _inventory_equip_slot_1;
        private string _inventory_equip_slot_2;
        private string _inventory_equip_slot_1_item;
        private string _inventory_equip_slot_2_item;
        private int _inventory_equip_slot_1_durability;
        private int _inventory_equip_slot_2_durability;
        //slots area
        private string _inventory_slot_1;
        private string _inventory_slot_2;
        private string _inventory_slot_3;
        private string _inventory_slot_4;
        private string _inventory_slot_5;
        private string _inventory_slot_6;
        private string _inventory_slot_7;
        private string _inventory_slot_8;
        private string _inventory_slot_9;
        private string _inventory_slot_10;
        private string _inventory_slot_1_type;
        private string _inventory_slot_2_type;
        private string _inventory_slot_3_type;
        private string _inventory_slot_4_type;
        private string _inventory_slot_5_type;
        private string _inventory_slot_6_type;
        private string _inventory_slot_7_type;
        private string _inventory_slot_8_type;
        private string _inventory_slot_9_type;
        private string _inventory_slot_10_type;
        private int _inventory_slot_1_quantity;
        private int _inventory_slot_2_quantity;
        private int _inventory_slot_3_quantity;
        private int _inventory_slot_4_quantity;
        private int _inventory_slot_5_quantity;
        private int _inventory_slot_6_quantity;
        private int _inventory_slot_7_quantity;
        private int _inventory_slot_8_quantity;
        private int _inventory_slot_9_quantity;
        private int _inventory_slot_10_quantity;
        private int _inventory_slot_1_durability;
        private int _inventory_slot_2_durability;
        private int _inventory_slot_3_durability;
        private int _inventory_slot_4_durability;
        private int _inventory_slot_5_durability;
        private int _inventory_slot_6_durability;
        private int _inventory_slot_7_durability;
        private int _inventory_slot_8_durability;
        private int _inventory_slot_9_durability;
        private int _inventory_slot_10_durability;
        #endregion

        // Handles the users data
        public UserDataHandler UserDataHandler;

        #region Unsaved Values

        //play token
        public int PlayToken = 0;

        //staff
        public bool Duty = false;

        public int TargetId = 0;
        public int TargetLockId = 0;
        public int LockBot = 0;

        //court
        public bool Trial = false;
        public bool IsInCourt = false;
        public bool CalledToCourt = false;
        public bool JoinedCourt = false;
        public bool IsTeleportingToCourt = false;
        public bool CourtVoted = false;

        public bool Stunned = false;
        public string StunType = null;
        public bool Cuffed = false;

        public int PrisonTimer = 0;

        //Medkit, Snack etc
        public bool UsingMedkit = false;
        public int MedkitCooldown = 0;
        public bool UsingSnack = false;
        public bool UsingHeroinDrug = false;

        #region Escort Values
        //police
        public bool Escorting = false;
        public string EscortUsername = null;
        //player
        public bool Escort = false;
        public string EscortBy = null;

        public int TicketTimer = 0;
        public string TicketFrom = null;
        #endregion

        //using values
        public bool usingTrash = false;

        //taxi values
        public bool usingTaxiPole = false;
        public bool CallingTaxi = false;
        public int CallingTaxiTimer = 0;
        public bool usingTaxiRide = false;
        public int TaxiRideTimer = 0;
        public int TaxiRideId = 0;

        //action timer values
        public int ActionTimerToken = 0;
        public int ActionTimerSeconds = 0;
        public int ActionTimerMil = 0;
        #endregion

        public GameClient GetClient()
        {
            return this.Client;
        }

        public Habbo Habbo
        {
            get
            {
                return this.GetClient().GetHabbo();
            }
        }

        public RoomUser roomUser
        {
            get
            {
                return Room.GetRoomUserManager().GetRoomUserByHabbo(Habbo.Id);
            }
        }

        public Room Room
        {
            get
            {
                if (Client.GetHabbo().CurrentRoomId <= 0)
                    return null;

                Room _room = null;
                if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(Habbo.CurrentRoomId, out _room))
                    return _room;

                return null;
            }
        }

        public void SendWeb(string Data)
        {
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(GetClient(), Data);
        }

        public void SendExecuteWeb(string Event, string Data)
        {
            PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(GetClient(), Event, Data);
        }
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        public string Bio
        {
            get { return _bio; }
            set { _bio = value; }
        }
        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        public int HealthMax
        {
            get { return _health_max; }
            set { _health_max = value; }
        }

        public int Energy
        {
            get { return _energy; }
            set { _energy = value; }
        }

        public int Aggression
        {
            get { return _aggression; }
            set { _aggression = value; }
        }

        public bool Passive
        {
            get { return _passive; }
            set { _passive = value; }
        }

        public int Timer
        {
            get { return _timer; }
            set { _timer = value; }
        }

        public bool Hospital
        {
            get { return _hospital; }
            set { _hospital = value; }
        }

        public bool Prison
        {
            get { return _prison; }
            set { _prison = value; }
        }

        public int GP
        {
            get { return _god_protection; }
            set { _god_protection = value; }
        }

        public int GymMembership
        {
            get { return _gym_membership; }
            set { _gym_membership = value; }
        }

        public int RoomX
        {
            get { return _roomX; }
            set { _roomX = value; }
        }

        public int RoomY
        {
            get { return _roomY; }
            set { _roomY = value; }
        }

        public int JobId
        {
            get { return _job_id; }
            set { _job_id = value; }
        }

        public int JobRank
        {
            get { return _job_rank; }
            set { _job_rank = value; }
        }

        public int Gang
        {
            get { return _gang_id; }
            set { _gang_id = value; }
        }

        public int GangRank
        {
            get { return _gang_rank; }
            set { _gang_rank = value; }
        }

        public int CombatLevel
        {
            get { return _combat_level; }
            set { _combat_level = value; }
        }

        public int CombatXP
        {
            get { return _combat_xp; }
            set { _combat_xp = value; }
        }

        public int FarmingLevel
        {
            get { return _farming_level; }
            set { _farming_level = value; }
        }

        public int FarmingXP
        {
            get { return _farming_xp; }
            set { _farming_xp = value; }
        }

        public int Kills
        {
            get { return _kills; }
            set { _kills = value; }
        }

        public int Deaths
        {
            get { return _deaths; }
            set { _deaths = value; }
        }

        public int Arrests
        {
            get { return _arrests; }
            set { _arrests = value; }
        }

        public int Assists
        {
            get { return _assists; }
            set { _assists = value; }
        }

        public int DamageDealt
        {
            get { return _damage_dealt; }
            set { _damage_dealt = value; }
        }

        public int DamageTaken
        {
            get { return _damage_taken; }
            set { _damage_taken = value; }
        }

        public int GangKills
        {
            get { return _gang_kills; }
            set { _gang_kills = value; }
        }

        public int ExplosiveKills
        {
            get { return _explosive_kills; }
            set { _explosive_kills = value; }
        }

        public int ParamedicRevives
        {
            get { return _paramedic_revives; }
            set { _paramedic_revives = value; }
        }

        public int CriminalsArrested
        {
            get { return _criminals_arrested; }
            set { _criminals_arrested = value; }
        }

        public int Sales
        {
            get { return _sales; }
            set { _sales = value; }
        }

        public int NetWorth
        {
            get { return _net_worth; }
            set { _net_worth = value; }
        }

        public int MoneyEarned
        {
            get { return _money_earned; }
            set { _money_earned = value; }
        }

        public int MoneyWon
        {
            get { return _money_won; }
            set { _money_won = value; }
        }

        public int MoneyLost
        {
            get { return _money_lost; }
            set { _money_lost = value; }
        }

        public int TicketsGiven
        {
            get { return _tickets_given; }
            set { _tickets_given = value; }
        }

        public int Shifts
        {
            get { return _shifts; }
            set { _shifts = value; }
        }

        public int CorpShifts
        {
            get { return _corp_shifts; }
            set { _corp_shifts = value; }
        }

        public int WeeklyShifts
        {
            get { return _weekly_shifts; }
            set { _weekly_shifts = value; }
        }

        public int CorpTasks
        {
            get { return _corp_tasks; }
            set { _corp_tasks = value; }
        }

        public int DrugsTaken
        {
            get { return _drugs_taken; }
            set { _drugs_taken = value; }
        }

        public int ArenaWins
        {
            get { return _arena_wins; }
            set { _arena_wins = value; }
        }

        public int CardGamesWon
        {
            get { return _card_games_won; }
            set { _card_games_won = value; }
        }

        public int CardGamesLost
        {
            get { return _card_games_lost; }
            set { _card_games_lost = value; }
        }

        public int LotteryJackpots
        {
            get { return _lottery_jackpots; }
            set { _lottery_jackpots = value; }
        }

        public bool Dead
        {
            get { return _dead; }
            set { _dead = value; }
        }

        public int MarriedTo
        {
            get { return _married_to; }
            set { _married_to = value; }
        }

        public DateTime DepositRent
        {
            get { return _deposit_rent; }
            set { _deposit_rent = value; }
        }

        public string CustomKOMessage
        {
            get { return _custom_KO_msg; }
            set { _custom_KO_msg = value; }
        }

        public string CustomDeathMessage
        {
            get { return _custom_death_msg; }
            set { _custom_death_msg = value; }
        }

        public string CustomArrestMessage
        {
            get { return _custom_arrest_msg; }
            set { _custom_arrest_msg = value; }
        }

        public bool Livefeed
        {
            get { return _livefeed; }
            set { _livefeed = value; }
        }

        public RoleplayTimer GetTimer()
        {
            return RPTimer;
        }

        public Wanted Wanted()
        {
            return Wan;
        }

        #region Inventory
        public string InventoryEquipSlot1
        {
            get { return _inventory_equip_slot_1; }
            set { _inventory_equip_slot_1 = value; }
        }
        public string InventoryEquipSlot2
        {
            get { return _inventory_equip_slot_2; }
            set { _inventory_equip_slot_2 = value; }
        }
        public string InventoryEquipSlot1Item
        {
            get { return _inventory_equip_slot_1_item; }
            set { _inventory_equip_slot_1_item = value; }
        }
        public string InventoryEquipSlot2Item
        {
            get { return _inventory_equip_slot_2_item; }
            set { _inventory_equip_slot_2_item = value; }
        }
        public int InventoryEquipSlot1Durability
        {
            get { return _inventory_equip_slot_1_durability; }
            set { _inventory_equip_slot_1_durability = value; }
        }
        public int InventoryEquipSlot2Durability
        {
            get { return _inventory_equip_slot_2_durability; }
            set { _inventory_equip_slot_2_durability = value; }
        }
        public string InventorySlot1
        {
            get { return _inventory_slot_1; }
            set { _inventory_slot_1 = value; }
        }
        public string InventorySlot2
        {
            get { return _inventory_slot_2; }
            set { _inventory_slot_2 = value; }
        }
        public string InventorySlot3
        {
            get { return _inventory_slot_3; }
            set { _inventory_slot_3 = value; }
        }
        public string InventorySlot4
        {
            get { return _inventory_slot_4; }
            set { _inventory_slot_4 = value; }
        }
        public string InventorySlot5
        {
            get { return _inventory_slot_5; }
            set { _inventory_slot_5 = value; }
        }
        public string InventorySlot6
        {
            get { return _inventory_slot_6; }
            set { _inventory_slot_6 = value; }
        }
        public string InventorySlot7
        {
            get { return _inventory_slot_7; }
            set { _inventory_slot_7 = value; }
        }
        public string InventorySlot8
        {
            get { return _inventory_slot_8; }
            set { _inventory_slot_8 = value; }
        }
        public string InventorySlot9
        {
            get { return _inventory_slot_9; }
            set { _inventory_slot_9 = value; }
        }
        public string InventorySlot10
        {
            get { return _inventory_slot_10; }
            set { _inventory_slot_10 = value; }
        }
        public string InventorySlot1Type
        {
            get { return _inventory_slot_1_type; }
            set { _inventory_slot_1_type = value; }
        }
        public string InventorySlot2Type
        {
            get { return _inventory_slot_2_type; }
            set { _inventory_slot_2_type = value; }
        }
        public string InventorySlot3Type
        {
            get { return _inventory_slot_3_type; }
            set { _inventory_slot_3_type = value; }
        }
        public string InventorySlot4Type
        {
            get { return _inventory_slot_4_type; }
            set { _inventory_slot_4_type = value; }
        }
        public string InventorySlot5Type
        {
            get { return _inventory_slot_5_type; }
            set { _inventory_slot_5_type = value; }
        }
        public string InventorySlot6Type
        {
            get { return _inventory_slot_6_type; }
            set { _inventory_slot_6_type = value; }
        }
        public string InventorySlot7Type
        {
            get { return _inventory_slot_7_type; }
            set { _inventory_slot_7_type = value; }
        }
        public string InventorySlot8Type
        {
            get { return _inventory_slot_8_type; }
            set { _inventory_slot_8_type = value; }
        }
        public string InventorySlot9Type
        {
            get { return _inventory_slot_9_type; }
            set { _inventory_slot_9_type = value; }
        }
        public string InventorySlot10Type
        {
            get { return _inventory_slot_10_type; }
            set { _inventory_slot_10_type = value; }
        }
        public int InventorySlot1Quantity
        {
            get { return _inventory_slot_1_quantity; }
            set { _inventory_slot_1_quantity = value; }
        }
        public int InventorySlot2Quantity
        {
            get { return _inventory_slot_2_quantity; }
            set { _inventory_slot_2_quantity = value; }
        }
        public int InventorySlot3Quantity
        {
            get { return _inventory_slot_3_quantity; }
            set { _inventory_slot_3_quantity = value; }
        }
        public int InventorySlot4Quantity
        {
            get { return _inventory_slot_4_quantity; }
            set { _inventory_slot_4_quantity = value; }
        }
        public int InventorySlot5Quantity
        {
            get { return _inventory_slot_5_quantity; }
            set { _inventory_slot_5_quantity = value; }
        }
        public int InventorySlot6Quantity
        {
            get { return _inventory_slot_6_quantity; }
            set { _inventory_slot_6_quantity = value; }
        }
        public int InventorySlot7Quantity
        {
            get { return _inventory_slot_7_quantity; }
            set { _inventory_slot_7_quantity = value; }
        }
        public int InventorySlot8Quantity
        {
            get { return _inventory_slot_8_quantity; }
            set { _inventory_slot_8_quantity = value; }
        }
        public int InventorySlot9Quantity
        {
            get { return _inventory_slot_9_quantity; }
            set { _inventory_slot_9_quantity = value; }
        }
        public int InventorySlot10Quantity
        {
            get { return _inventory_slot_10_quantity; }
            set { _inventory_slot_10_quantity = value; }
        }
        public int InventorySlot1Durability
        {
            get { return _inventory_slot_1_durability; }
            set { _inventory_slot_1_durability = value; }
        }
        public int InventorySlot2Durability
        {
            get { return _inventory_slot_2_durability; }
            set { _inventory_slot_2_durability = value; }
        }
        public int InventorySlot3Durability
        {
            get { return _inventory_slot_3_durability; }
            set { _inventory_slot_3_durability = value; }
        }
        public int InventorySlot4Durability
        {
            get { return _inventory_slot_4_durability; }
            set { _inventory_slot_4_durability = value; }
        }
        public int InventorySlot5Durability
        {
            get { return _inventory_slot_5_durability; }
            set { _inventory_slot_5_durability = value; }
        }
        public int InventorySlot6Durability
        {
            get { return _inventory_slot_6_durability; }
            set { _inventory_slot_6_durability = value; }
        }
        public int InventorySlot7Durability
        {
            get { return _inventory_slot_7_durability; }
            set { _inventory_slot_7_durability = value; }
        }
        public int InventorySlot8Durability
        {
            get { return _inventory_slot_8_durability; }
            set { _inventory_slot_8_durability = value; }
        }
        public int InventorySlot9Durability
        {
            get { return _inventory_slot_9_durability; }
            set { _inventory_slot_9_durability = value; }
        }
        public int InventorySlot10Durability
        {
            get { return _inventory_slot_10_durability; }
            set { _inventory_slot_10_durability = value; }
        }
        #endregion

        public RoleplayUser(GameClient Client, DataRow RPStat, DataRow Inventory, DataRow WantedList)
        {
            this.Client = Client;

            this.Username = Habbo.Username;
            this._bio = Convert.ToString(RPStat["bio"]);

            this._health = Convert.ToInt32(RPStat["health"]);
            this._health_max = Convert.ToInt32(RPStat["health_max"]);
            this._energy = Convert.ToInt32(RPStat["energy"]);
            this._aggression = Convert.ToInt32(RPStat["aggression"]);
            this._passive = PlusEnvironment.EnumToBool(RPStat["passive"].ToString());
            this._timer = Convert.ToInt32(RPStat["timer"]);
            this._hospital = PlusEnvironment.EnumToBool(RPStat["hospital"].ToString());
            this._prison = PlusEnvironment.EnumToBool(RPStat["prison"].ToString());
            this._god_protection = Convert.ToInt32(RPStat["god_protection"]);
            this._roomX = Convert.ToInt32(RPStat["roomX"]);
            this._roomY = Convert.ToInt32(RPStat["roomY"]);
            this._job_id = Convert.ToInt32(RPStat["job_id"]);
            this._job_rank = Convert.ToInt32(RPStat["job_rank"]);
            this._gang_id = Convert.ToInt32(RPStat["gang_id"]);
            this._gang_rank = Convert.ToInt32(RPStat["gang_rank"]);
            this._combat_level = Convert.ToInt32(RPStat["combat_level"]);
            this._combat_xp = Convert.ToInt32(RPStat["combat_xp"]);
            this._farming_level = Convert.ToInt32(RPStat["farming_level"]);
            this._farming_xp = Convert.ToInt32(RPStat["farming_xp"]);
            this._kills = Convert.ToInt32(RPStat["kills"]);
            this._deaths = Convert.ToInt32(RPStat["deaths"]);
            this._arrests = Convert.ToInt32(RPStat["arrests"]);
            this._assists = Convert.ToInt32(RPStat["assists"]);
            this._damage_dealt = Convert.ToInt32(RPStat["damage_dealt"]);
            this._damage_taken = Convert.ToInt32(RPStat["damage_taken"]);
            this._gang_kills = Convert.ToInt32(RPStat["gang_kills"]);
            this._explosive_kills = Convert.ToInt32(RPStat["explosive_kills"]);
            this._paramedic_revives = Convert.ToInt32(RPStat["paramedic_revives"]);
            this._criminals_arrested = Convert.ToInt32(RPStat["criminals_arrested"]);
            this._sales = Convert.ToInt32(RPStat["sales"]);
            this._net_worth = Convert.ToInt32(RPStat["net_worth"]);
            this._money_earned = Convert.ToInt32(RPStat["money_earned"]);
            this._money_won = Convert.ToInt32(RPStat["money_won"]);
            this._money_lost = Convert.ToInt32(RPStat["money_lost"]);
            this._tickets_given = Convert.ToInt32(RPStat["tickets_given"]);
            this._shifts = Convert.ToInt32(RPStat["shifts"]);
            this._corp_shifts = Convert.ToInt32(RPStat["corp_shifts"]);
            this._weekly_shifts = Convert.ToInt32(RPStat["weekly_shifts"]);
            this._corp_tasks = Convert.ToInt32(RPStat["corp_tasks"]);
            this._arena_wins = Convert.ToInt32(RPStat["arena_wins"]);
            this._card_games_won = Convert.ToInt32(RPStat["card_games_won"]);
            this._card_games_lost = Convert.ToInt32(RPStat["card_games_lost"]);
            this._lottery_jackpots = Convert.ToInt32(RPStat["lottery_jackpots"]);
            this._dead = PlusEnvironment.EnumToBool(RPStat["dead"].ToString());
            this._married_to = Convert.ToInt32(RPStat["married_to"]);
            this._deposit_rent = Convert.ToDateTime(RPStat["desposit_rent"]);
            this._custom_KO_msg = Convert.ToString(RPStat["custom_KO_msg"]);
            this._custom_death_msg = Convert.ToString(RPStat["custom_death_msg"]);
            this._custom_arrest_msg = Convert.ToString(RPStat["custom_arrest_msg"]);
            this._livefeed = PlusEnvironment.EnumToBool(RPStat["livefeed"].ToString());

            this.Wan = new Wanted(this, WantedList);
            this.Inventory = new Inv(this, Inventory);
            this.Bin = new Bins(this);

            // Handles the users data
            this.UserDataHandler = new UserDataHandler(Client, this);
        }

        public void Login()
        {
            RPCache(1);
            RPCache(2);
            RPCache(3);
            RPTimer = new RoleplayTimer(this);
            Wan.Load();
            SendExecuteWeb("macro", "load");
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Client, "day-night;" + PlusStaticGameSettings.CurrentMood + ";" + PlusStaticGameSettings.DayNightTimer);
            SendWeb("connected");
            SendExecuteWeb("login", "ad");
        }

        public void OnDisconnect()
        {
            if (this != null)
            {
                if (UserDataHandler != null)
                {
                    UserDataHandler.SaveData();
                    UserDataHandler.SaveWantedData();
                    UserDataHandler = null;
                }
            }
        }

        public void UpdateValue(string Key, string Value)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `user_rp_stats` SET " + Key + " = @value WHERE `user_id` = '" + Habbo.Id + "' LIMIT 1;");
                dbClient.AddParameter("value", Value);
                dbClient.RunQuery();
            }
        }

        public void RPCache(int Cache)
        {
            if (Cache == 1)
            {
                if (!Habbo.isDisconnecting)
                    SendWeb("user-stats;" + Habbo.Username + ";" + Health + ";" + HealthMax + ";" + Energy + ";" + PlusEnvironment.BoolToEnum(Passive) + ";" + Aggression);

                RoleplayManager.UpdateTargetStats(Habbo.Id);
            }
            else if (Cache == 2)
            {
                if (!Habbo.isDisconnecting)
                    SendWeb("look;" + Habbo.Look);

                RoleplayManager.UpdateTargetStats(Habbo.Id);
            }
            else if (Cache == 3)
            {
                SendWeb("credits;" + Habbo.Credits);
            }
            else if (Cache == 4)
            {
                RoomUser User = Habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(Habbo.Username);
                if (User == null || User.SetX == 0 && User.SetY == 0)
                    return;

                RoomX = User.SetX;
                RoomY = User.SetY;
            }
        }

        public void ResetEffect()
        {
            if (usingTaxiRide)
            {
                roomUser.ApplyEffect(19);
            }
            else if (Duty)
            {
                roomUser.ApplyEffect(102);
            }
            else if (UsingHeroinDrug)
            {
                roomUser.ApplyEffect(12);
            }
            else if (Inventory.WeaponEquipped != null)
            {
                if (Inventory.WeaponEquipped == "stungun")
                {
                    if (Inventory.EquipHasDurability(Inventory.WeaponEquipped, 14))
                        roomUser.ApplyEffect(182);
                    else
                        roomUser.ApplyEffect(0);
                }
                else if (Inventory.WeaponEquipped == "bat")
                {
                    if (Inventory.EquipHasDurability(Inventory.WeaponEquipped, 1))
                        roomUser.ApplyEffect(591);
                    else
                        roomUser.ApplyEffect(0);
                }
                else if (Inventory.WeaponEquipped == "sword")
                {
                    if (Inventory.EquipHasDurability(Inventory.WeaponEquipped, 1))
                        roomUser.ApplyEffect(162);
                    else
                        roomUser.ApplyEffect(0);
                }
                else if (Inventory.WeaponEquipped == "axe")
                {
                    if (Inventory.EquipHasDurability(Inventory.WeaponEquipped, 1))
                        roomUser.ApplyEffect(707);
                    else
                        roomUser.ApplyEffect(0);
                }
            }
            else if (UsingSnack)
            {
                roomUser.CarryItem(51);
            }
            else
                roomUser.ApplyEffect(0);
        }

        public void ResetAvatar()
        {
            using (IQueryAdapter motto = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                motto.SetQuery("SELECT `motto` FROM `users` WHERE `id` = @id LIMIT 1");
                motto.AddParameter("id", Habbo.Id);
                Habbo.Motto = motto.getString();
            }

            using (IQueryAdapter look = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                look.SetQuery("SELECT `look` FROM `users` WHERE `id` = @id LIMIT 1");
                look.AddParameter("id", Habbo.Id);
                Habbo.Look = look.getString();
            }

            if (Habbo.Look.Contains(".."))
            {
                Habbo.Look = Habbo.Look.Replace("..", ".");

                using (IQueryAdapter updateLook = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    updateLook.SetQuery("UPDATE users SET look = @look WHERE id = @id LIMIT 1");
                    updateLook.AddParameter("look", Habbo.Look);
                    updateLook.AddParameter("id", Habbo.Id);
                }
            }

            GetClient().SendMessage(new UserChangeComposer(roomUser, true));
            Habbo.CurrentRoom.SendMessage(new UserChangeComposer(roomUser, false));
            RPCache(2);
        }

        public void CreateAggression(int Precent = 100)
        {
            Aggression = Precent;
            RPCache(1);
            RoleplayManager.UpdateTargetStats(Habbo.Id);
        }

        public void SendToHospital(int Minutes)
        {

        }

        public void SendToPrison(int Minutes)
        {
            Arrests += 1;
            Timer = Minutes;
            Prison = true;
            Wan.Remove();
            SendToPrisonChair();
            UpdateValue("timer", Convert.ToString(Minutes));
            UpdateValue("prison", "1");
        }

        public void EndPrison()
        {
            Timer = 0;
            PrisonTimer = 0;
            Prison = false;
            UpdateValue("timer", "0");
            UpdateValue("prison", "0");
            ResetAvatar();
            RPCache(2);
            GetClient().SendWhisper("You have been released from jail");
            roomUser.Say("ends their time in prison");
        }

        public void SendToPrisonChair()
        {
            Habbo.updateAvatarEvent("ch-6161865-94.hr-100-100.lg-50858737-94.hd-629-2", "ch-6165820-94.hr-100-0.lg-3116-94-94.ha-1002-70.hd-180-7", "[Arrested]");

            Room Room = PlusEnvironment.GetGame().GetRoomManager().LoadRoom(99);
            List<Item> Beds = new List<Item>();
            List<int> BedsId = new List<int>();

            foreach (Item item in Room.GetRoomItemHandler().GetFloor)
            {
                if (item.GetBaseItem().SpriteId == 2500)
                {
                    if (!Beds.Contains(item))
                        Beds.Add(item);

                    if (!BedsId.Contains(item.Id))
                        BedsId.Add(item.Id);
                }
            }

            if (Habbo.CurrentRoomId != Room.Id)
            {
                Random rnd = new Random();
                int random = BedsId[rnd.Next(BedsId.Count)];

                GetClient().GetHabbo().IsTeleporting = true;
                GetClient().GetHabbo().TeleportingRoomID = Room.Id;
                GetClient().GetHabbo().TeleporterId = random;

                GetClient().GetHabbo().CanChangeRoom = true;
                GetClient().GetHabbo().PrepareRoom(GetClient().GetHabbo().TeleportingRoomID, "");
                SendToPrisonChair();
            }
            else
            {
                Random rnd = new Random();
                Item random = Beds[rnd.Next(Beds.Count)];
                Room.GetGameMap().TeleportToItem(roomUser, random);
                Room.GetRoomUserManager().UpdateUserStatusses();
            }
        }

        public void SendToCourt()
        {

        }

        public void StartEscorting(GameClient TargetClient, RoomUser TargetUser)
        {
            if (TargetClient == null || TargetClient.GetHabbo() == null || TargetClient.GetRoleplay() == null)
            {
                Client.SendWhisper("An error occurred while escorting this player");
                return;
            }

            if (TargetUser.Freezed)
                TargetUser.Freezed = false;

            TargetClient.GetRoleplay().Escort = true;
            TargetClient.GetRoleplay().EscortBy = Habbo.Username;
            TargetClient.GetHabbo().resetEffectEvent();
            TargetClient.GetHabbo().updateAvatarEvent(TargetClient.GetHabbo().Look + ".ch-989999938-1193-62", TargetClient.GetHabbo().Look + ".ch-989999938-1193-62", "Cuffed");

            Escorting = true;
            EscortUsername = TargetClient.GetHabbo().Username;
            EscortMovement(roomUser.SetX, roomUser.SetY, roomUser.RotBody);
        }

        public void EscortMovement(int nextX, int nextY, int newRot)
        {
            RoomUser EscortingUser = Room.GetRoomUserManager().GetRoomUserByHabbo(EscortUsername);
            if (EscortingUser == null)
                return;

            if (!EscortingUser.UltraFastWalking)
                EscortingUser.UltraFastWalking = true;

            if (newRot == 0)
                EscortingUser.MoveTo(nextX, nextY - 1);
            else if (newRot == 1)
                EscortingUser.MoveTo(nextX + 1, nextY - 1);
            else if (newRot == 2)
                EscortingUser.MoveTo(nextX + 1, nextY);
            else if (newRot == 3)
                EscortingUser.MoveTo(nextX + 1, nextY + 1);
            else if (newRot == 4)
                EscortingUser.MoveTo(nextX, nextY + 1);
            else if (newRot == 5)
                EscortingUser.MoveTo(nextX - 1, nextY + 1);
            else if (newRot == 6)
                EscortingUser.MoveTo(nextX - 1, nextY);
            else if (newRot == 7)
                EscortingUser.MoveTo(nextX - 1, nextY - 1);
            if (EscortingUser.RotBody != newRot)
                EscortingUser.GetClient().GetRoleplay().SetRot(newRot);
        }

        public void EndEscorting(bool Cuffed)
        {
            if (!Escorting || EscortUsername == null)
                return;

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(EscortUsername);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            TargetUser.UltraFastWalking = false;
            if (!Cuffed)
            {
                TargetClient.GetHabbo().resetAvatarEvent();
            }
            TargetClient.GetRoleplay().Cuffed = Cuffed;
            TargetClient.GetRoleplay().Escort = false;
            TargetClient.GetRoleplay().EscortBy = null;

            Escorting = false;
            EscortUsername = null;
            Habbo.isUncuffing = false;
        }

        public void StopAction()
        {
            if (Bin.Using)
            {
                Bin.End();
            }
            else if (usingTaxiPole && !CallingTaxi)
            {
                usingTaxiPole = false;
                SendWeb("taxi;hide");
            }
            else if (usingTaxiPole && CallingTaxi)
            {
                CallingTaxi = false;
                usingTaxiPole = false;
                CallingTaxiTimer = 0;
                roomUser.Say("cancels their taxi", 5);
                SendWeb("taxi;hide");
                SendWeb("action-timer;hide");
                ActionTimerSeconds = 0;
                ActionTimerMil = 0;
                ResetEffect();
            }
        }

        public bool CheckWarnings()
        {
            if (Passive)
            {
                GetClient().SendWhisper("You cannot perform this action while in passive mode");
                return true;
            }
            else if (GP > 0)
            {
                GetClient().SendWhisper("You cannot perform this action while in god protection");
                return true;
            }
            else return false;
        }

        public void ActionTimer(string title, int seconds)
        {

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);

            ActionTimerToken = tokenNumber;
            ActionTimerSeconds = seconds;
            ActionTimerMil = 10;
            SendWeb("action-timer;show;" + title + ";" + ActionTimerSeconds + ";" + ActionTimerMil);

            System.Timers.Timer taxi = new System.Timers.Timer(50);
            taxi.Interval = 50;
            taxi.Elapsed += delegate
            {
                if (ActionTimerToken == tokenNumber)
                {
                    ActionTimerMil--;
                    SendWeb("action-timer-seconds;" + ActionTimerSeconds + ";" + ActionTimerMil);
                    if (ActionTimerMil == 0)
                    {
                        ActionTimerSeconds--;
                        SendWeb("action-timer-seconds;" + ActionTimerSeconds + ";" + ActionTimerMil);
                        if (ActionTimerSeconds == -1)
                        {
                            ActionTimerSeconds = 0;
                            ActionTimerMil = 0;
                            SendWeb("action-timer;hide");
                            ActionTimerFinished(); //checks for action since the timer is finished
                            taxi.Stop();
                        }
                        else
                            ActionTimerMil = 10;
                    }
                }
                else
                    taxi.Stop();
            };
            taxi.Start();
        }

        public void ActionTimerFinished()
        {
            if (CallingTaxi)
            {
                CallingTaxi = false;
                usingTaxiPole = false;
                usingTaxiRide = true;

                roomUser.CanWalk = false;
                roomUser.SuperFastWalking = true;
                ResetEffect();
                Taxi.Route(GetClient());
            }
        }

        public void Stun(string type)
        {
            int Token = PlusEnvironment.GetRandomNumber(99, 9999);
            PlayToken = Token;

            StopAction();

            Stunned = true;
            StunType = type;
            roomUser.Statusses.Clear();
            roomUser.Freezed = true;
            roomUser.ApplyEffect(53);

            if (Escorting)
            {
                roomUser.Say("feels dizzy and lets go of " + EscortUsername);
                EndEscorting(true);
            }

            System.Timers.Timer StunTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(4.5));
            StunTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(4.5);
            StunTimer.Elapsed += delegate
            {
                if (PlayToken == Token)
                {
                    PlayToken = 0;
                    Stunned = false;
                    StunType = null;
                    roomUser.Freezed = false;
                    ResetEffect();
                }
                StunTimer.Stop();
            };
            StunTimer.Start();
        }

        public void SetRot(int rot, bool headonly = false)
        {
            if (headonly == false)
                roomUser.RotBody = rot;
            roomUser.RotHead = rot;
            roomUser.UpdateNeeded = true;
        }
    }
}
