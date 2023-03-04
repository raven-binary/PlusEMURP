using System;
using Plus;
using System.Data;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;
using log4net;
using Plus.HabboRoleplay.RoleplayUsers;

namespace Plus.HabboRoleplay.RoleplayUsers
{
    public class UserDataHandler
    {
        /// <summary>
        /// Log mechanism
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("UserDataHandler");

        /// <summary>
        /// The users session
        /// </summary>
        GameClient Client;

        /// <summary>
        /// The users roleplay instance
        /// </summary>
        RoleplayUser RoleplayUser;

        /// <summary>
        /// Constructs the class
        /// </summary>
        public UserDataHandler(GameClient Client, RoleplayUser RoleplayUser)
        {
            this.Client = Client;
            this.RoleplayUser = RoleplayUser;
        }

        /// <summary>
        /// Saves all rp data for the user to the db
        /// </summary>
        /// <returns></returns>
        public bool SaveData()
        {
            if (Client == null)
                return false;

            if (Client.GetHabbo() == null)
                return false;

            if (Client.GetRoleplay() == null)
                return false;

            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery(GetQueryString());
                AddParameters(DB);

                DB.RunQuery();
            }
            return true;
        }

        /// <summary>
        /// Saves all wanted data for the user to the db
        /// </summary>
        /// <returns></returns>
        public bool SaveWantedData()
        {
            if (Client == null || RoleplayUser == null)
                return false;

            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                try
                {
                    DB.SetQuery(GetWantedQueryString());
                    AddWantedParameters(DB);

                    DB.RunQuery();
                }
                catch (Exception Ex)
                { log.Info("ERROR WHILE TRYING TO SAVE WANTED USER DATA! EXCEPTION: " + Ex.Message); }
            }
            return true;
        }


        /// <summary>
        /// Gets the query string to update the user details
        /// </summary>
        private string GetQueryString()
        {
            string Query = @"UPDATE user_rp_stats SET 
                            bio = @bio,
                            health = @health,
                            health_max = @healthMax,
                            energy = @energy,
                            aggression = @aggression,
                            passive = @passive,
                            timer = @timer,
                            hospital = @hospital,
                            prison = @prison,
                            god_protection = @godProtection,
                            gym_membership = @gymMembership,
                            roomX = @roomX,
                            roomY = @roomY,
                            job_id = @jobId,
                            job_rank = @jobRank,
                            gang_id = @gang,
                            gang_rank = @gangRank,
                            combat_level = @combatLevel,
                            combat_xp = @combatXP,
                            farming_level = @farmingLevel,
                            farming_xp = @farmingXP,
                            kills = @kills,
                            deaths = @deaths,
                            arrests = @arrests,
                            assists = @assists,
                            damage_dealt = @damageDealt,
                            damage_taken = @damageTaken,
                            gang_kills = @gangKills,
                            explosive_kills = @explosiveKills,
                            paramedic_revives = @paramedicRevives,
                            criminals_arrested = @criminalsArrested,
                            sales = @sales,
                            net_worth = @netWorth,
                            money_earned = @moneyEarned,
                            money_won = @moneyWon,
                            money_lost = @moneyLost,
                            tickets_given = @ticketsGiven,
                            shifts = @shifts,
                            corp_shifts = @corpShifts,
                            weekly_shifts = @weeklyShifts,
                            corp_tasks = @corpTasks,
                            drugs_taken = @drugsTaken,
                            arena_wins = @arenawins,
                            card_games_won = @cardGamesWon,
                            card_games_lost = @cardGamesLost,
                            lottery_jackpots = @lotteryJackpots,
                            dead = @dead,
                            married_to = @marriedTo,
                            desposit_rent = @despositRent,
                            custom_KO_msg = @customKOMsg,
                            custom_death_msg = @customDeathMsg,
                            custom_arrest_msg = @customArrestMsg,
                            livefeed = @livefeed
                            
                            WHERE user_id = @userid";
            return Query;
        }

        /// <summary>
        /// Gets the group query string to update the wanted data
        /// </summary>
        private string GetWantedQueryString()
        {
            string Query = @"UPDATE users_wanted SET 
                            date = @date,
                            added_date = @addedDate,
                            wanted = @wanted,
                            passed = @passed,
                            assault = @assault,
                            murder = @murder,
                            copassault = @copAssault,
                            copmurder = @copMurder,
                            ganghomicide = @ganghomicide,
                            obstruction = @obstruction,
                            hacking = @hacking,
                            trespassing = @trespassing,
                            robbery = @robbery,
                            illegalarea = @illegalarea,
                            jailbreak = @jailbreak,
                            terrorism = @terrorism,
                            drugs = @drugs,
                            execution = @execution,
                            escaping = @escaping,
                            nonCompliance = @nonCompliance,
                            callAbuse = @callAbuse
        
                            WHERE user_id = @userid";
            return Query;
        }

        /// <summary>
        /// Adds the parameters to the mysql command
        /// </summary>
        private void AddParameters(IQueryAdapter DB)
        {
            // User ID
            DB.AddParameter("userid", Client.GetHabbo().Id);

            DB.AddParameter("bio", RoleplayUser.Bio);
            DB.AddParameter("health", RoleplayUser.Health);
            DB.AddParameter("healthMax", RoleplayUser.HealthMax);
            DB.AddParameter("energy", RoleplayUser.Energy);
            DB.AddParameter("aggression", RoleplayUser.Aggression);
            DB.AddParameter("passive", PlusEnvironment.BoolToEnum(RoleplayUser.Passive));
            DB.AddParameter("timer", RoleplayUser.Timer);
            DB.AddParameter("hospital", PlusEnvironment.BoolToEnum(RoleplayUser.Hospital));
            DB.AddParameter("prison", PlusEnvironment.BoolToEnum(RoleplayUser.Prison));
            DB.AddParameter("godProtection", RoleplayUser.GP);
            DB.AddParameter("gymMembership", RoleplayUser.GymMembership);
            DB.AddParameter("roomX", RoleplayUser.RoomX);
            DB.AddParameter("roomY", RoleplayUser.RoomY);
            DB.AddParameter("jobId", RoleplayUser.JobId);
            DB.AddParameter("jobRank", RoleplayUser.JobRank);
            DB.AddParameter("gang", RoleplayUser.Gang);
            DB.AddParameter("gangRank", RoleplayUser.GangRank);
            DB.AddParameter("combatLevel", RoleplayUser.CombatLevel);
            DB.AddParameter("combatXP", RoleplayUser.CombatXP);
            DB.AddParameter("farmingLevel", RoleplayUser.FarmingLevel);
            DB.AddParameter("farmingXP", RoleplayUser.CombatXP);
            DB.AddParameter("kills", RoleplayUser.Kills);
            DB.AddParameter("deaths", RoleplayUser.Deaths);
            DB.AddParameter("arrests", RoleplayUser.Arrests);
            DB.AddParameter("assists", RoleplayUser.Assists);
            DB.AddParameter("damageDealt", RoleplayUser.DamageDealt);
            DB.AddParameter("damageTaken", RoleplayUser.DamageTaken);
            DB.AddParameter("gangKills", RoleplayUser.GangKills);
            DB.AddParameter("explosiveKills", RoleplayUser.ExplosiveKills);
            DB.AddParameter("paramedicRevives", RoleplayUser.ParamedicRevives);
            DB.AddParameter("criminalsArrested", RoleplayUser.CriminalsArrested);
            DB.AddParameter("sales", RoleplayUser.Sales);
            DB.AddParameter("netWorth", RoleplayUser.NetWorth);
            DB.AddParameter("moneyEarned", RoleplayUser.MoneyEarned);
            DB.AddParameter("moneyWon", RoleplayUser.MoneyWon);
            DB.AddParameter("moneyLost", RoleplayUser.MoneyLost);
            DB.AddParameter("ticketsGiven", RoleplayUser.TicketsGiven);
            DB.AddParameter("shifts", RoleplayUser.Shifts);
            DB.AddParameter("corpShifts", RoleplayUser.CorpShifts);
            DB.AddParameter("weeklyShifts", RoleplayUser.WeeklyShifts);
            DB.AddParameter("corpTasks", RoleplayUser.CorpTasks);
            DB.AddParameter("drugsTaken", RoleplayUser.DrugsTaken);
            DB.AddParameter("arenawins", RoleplayUser.ArenaWins);
            DB.AddParameter("cardGamesWon", RoleplayUser.CardGamesWon);
            DB.AddParameter("cardGamesLost", RoleplayUser.CardGamesLost);
            DB.AddParameter("lotteryJackpots", RoleplayUser.LotteryJackpots);
            DB.AddParameter("dead", PlusEnvironment.BoolToEnum(RoleplayUser.Dead));
            DB.AddParameter("marriedTo", RoleplayUser.MarriedTo);
            DB.AddParameter("despositRent", RoleplayUser.DepositRent);
            DB.AddParameter("customKOMsg", RoleplayUser.CustomKOMessage);
            DB.AddParameter("customDeathMsg", RoleplayUser.CustomDeathMessage);
            DB.AddParameter("customArrestMsg", RoleplayUser.CustomArrestMessage);
            DB.AddParameter("livefeed", PlusEnvironment.BoolToEnum(RoleplayUser.Livefeed));
        }

        // <summary>
        /// Adds the wanted parameters to the mysql command
        /// </summary>
        private void AddWantedParameters(IQueryAdapter DB)
        {
            DB.AddParameter("userid", Client.GetHabbo().Id);
            DB.AddParameter("date", Client.GetRoleplay().Wan.Date);
            DB.AddParameter("addedDate", Client.GetRoleplay().Wan.AddedTime);
            DB.AddParameter("wanted", PlusEnvironment.BoolToEnum(RoleplayUser.Wan.IsWanted));
            DB.AddParameter("passed", PlusEnvironment.BoolToEnum(RoleplayUser.Wan.Passed));
            DB.AddParameter("assault", Client.GetRoleplay().Wan.Assault);
            DB.AddParameter("murder", Client.GetRoleplay().Wan.Murder);
            DB.AddParameter("copassault", Client.GetRoleplay().Wan.Copassault);
            DB.AddParameter("copMurder", Client.GetRoleplay().Wan.Copmurder);
            DB.AddParameter("ganghomicide", Client.GetRoleplay().Wan.Ganghomicide);
            DB.AddParameter("obstruction", Client.GetRoleplay().Wan.Obstruction);
            DB.AddParameter("hacking", Client.GetRoleplay().Wan.Hacking);
            DB.AddParameter("trespassing", Client.GetRoleplay().Wan.Trespassing);
            DB.AddParameter("robbery", Client.GetRoleplay().Wan.Robbery);
            DB.AddParameter("illegalarea", Client.GetRoleplay().Wan.Illegalarea);
            DB.AddParameter("jailbreak", Client.GetRoleplay().Wan.Jailbreak);
            DB.AddParameter("terrorism", Client.GetRoleplay().Wan.Terrorism);
            DB.AddParameter("drugs", Client.GetRoleplay().Wan.Drugs);
            DB.AddParameter("execution", Client.GetRoleplay().Wan.Execution);
            DB.AddParameter("escaping", Client.GetRoleplay().Wan.Escaping);
            DB.AddParameter("nonCompliance", Client.GetRoleplay().Wan.NonCompliance);
            DB.AddParameter("callAbuse", Client.GetRoleplay().Wan.CallAbuse);
        }
    }
}
