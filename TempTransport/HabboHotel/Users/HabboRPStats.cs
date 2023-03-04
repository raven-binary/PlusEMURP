using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Users
{
    public class RPStats
    {
        public bool PetPlaced;
        public string Bio { get; set; }
        public int Health { get; set; }
        public int HealthMax { get; set; }
        public int Energy { get; set; }
        public int Aggression { get; set; }
        public int RoomX { get; set; }
        public int RoomY { get; set; }
        public int Rotation { get; set; }
        public int Sitting { get; set; }
        public int Damage { get; set; }
        public int CombatLevel { get; set; }
        public int CombatXP { get; set; }
        public int FarmingLevel { get; set; }
        public int FarmingXP { get; set; }
        public int GP { get; set; }
        public int Passive { get; set; }
        public int GymMembership { get; set; }
        public int Shifts { get; set; }
        public int CorpShifts { get; set; }
        public int WeeklyShifts { get; set; }
        public int hasPet { get; set; }
        public int JobId { get; set; }
        public int JobRank { get; set; }
        public int HairCutCount { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int LiveFeedEnabled { get; set; }
        public string CustomKOMessage { get; set; }
        public bool Dead { get; set; }
        public DateTime DepositDate { get; set; }
        public int MarriedTo { get; set; }
        public int Wanted { get; set; }
        public int Arrests { get; set; }
        public int Assists { get; set; }
        public int DamageDealt { get; set; }
        public int DamageTaken { get; set; }
        public int Sales { get; set; }
        public int TasksCompleted { get; set; }
        public RPStats(string Bio, int Health, int HealthMax, int Energy, int Aggression, int RoomX, int RoomY, int Rotation, int Sitting, int Damage, int CombatLevel, int CombatXP, int FarmingLevel, int FarmingXP, int GP, int Passive, int GymMembership, int Shifts, int CorpShifts, int WeeklyShifts, int hasPet, int JobId, int JobRank, int HairCutCount, int Kills, int Deaths, int LiveFeedEnabled, string CustomKOMessage, bool Dead, DateTime DepositDate, int MarriedTo, int Wanted, int Arrests, int Assists, int DamageDealt, int DamageTaken, int Sales, int SalesCompleted)
        {
            this.Bio = Bio;
            this.Health = Health;
            this.HealthMax = HealthMax;
            this.Energy = Energy;
            this.Aggression = Aggression;
            this.RoomX = RoomX;
            this.RoomY = RoomY;
            this.Rotation = Rotation;
            this.Sitting = Sitting;
            this.Damage = Damage;
            this.CombatLevel = CombatLevel;
            this.CombatXP = CombatXP;
            this.FarmingLevel = FarmingLevel;
            this.FarmingXP = FarmingXP;
            this.GP = GP;
            this.Passive = Passive;
            this.GymMembership = GymMembership;
            this.Shifts = Shifts;
            this.CorpShifts = CorpShifts;
            this.WeeklyShifts = WeeklyShifts;
            this.hasPet = hasPet;
            this.JobId = JobId;
            this.JobRank = JobRank;
            this.HairCutCount = HairCutCount;
            this.Kills = Kills;
            this.Deaths = Deaths;
            this.LiveFeedEnabled = LiveFeedEnabled;
            this.CustomKOMessage = CustomKOMessage;
            this.Dead = Dead;
            this.DepositDate = DepositDate;
            this.MarriedTo = MarriedTo;
            this.Wanted = Wanted;
            this.Arrests = Arrests;
            this.Assists = Assists;
            this.DamageDealt = DamageDealt;
            this.DamageTaken = DamageTaken;
            this.Sales = Sales;
            this.TasksCompleted = TasksCompleted;
        }
    }
}
