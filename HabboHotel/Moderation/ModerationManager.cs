﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using log4net;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Moderation
{
    public sealed class ModerationManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ModerationManager));

        private int _ticketCount = 1;
        private readonly List<string> _userPresets = new();
        private readonly List<string> _roomPresets = new();
        private readonly Dictionary<string, ModerationBan> _bans = new();
        private readonly Dictionary<int, string> _userActionPresetCategories = new();
        private readonly Dictionary<int, List<ModerationPresetActionMessages>> _userActionPresetMessages = new();
        private readonly ConcurrentDictionary<int, ModerationTicket> _modTickets = new();

        private readonly Dictionary<int, string> _moderationCfhTopics = new();
        private readonly Dictionary<int, List<ModerationPresetActions>> _moderationCfhTopicActions = new();

        public void Init()
        {
            if (_userPresets.Count > 0)
                _userPresets.Clear();
            if (_moderationCfhTopics.Count > 0)
                _moderationCfhTopics.Clear();
            if (_moderationCfhTopicActions.Count > 0)
                _moderationCfhTopicActions.Clear();
            if (_bans.Count > 0)
                _bans.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `moderation_presets`;");
                DataTable presetsTable = dbClient.GetTable();

                if (presetsTable != null)
                {
                    foreach (DataRow row in presetsTable.Rows)
                    {
                        string type = Convert.ToString(row["type"])?.ToLower();
                        switch (type)
                        {
                            case "user":
                                _userPresets.Add(Convert.ToString(row["message"]));
                                break;

                            case "room":
                                _roomPresets.Add(Convert.ToString(row["message"]));
                                break;
                        }
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `moderation_topics`;");
                DataTable moderationTopics = dbClient.GetTable();

                if (moderationTopics != null)
                {
                    foreach (DataRow row in moderationTopics.Rows)
                    {
                        if (!_moderationCfhTopics.ContainsKey(Convert.ToInt32(row["id"])))
                            _moderationCfhTopics.Add(Convert.ToInt32(row["id"]), Convert.ToString(row["caption"]));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `moderation_topic_actions`;");
                DataTable moderationTopicsActions = dbClient.GetTable();

                if (moderationTopicsActions != null)
                {
                    foreach (DataRow row in moderationTopicsActions.Rows)
                    {
                        int parentId = Convert.ToInt32(row["parent_id"]);

                        if (!_moderationCfhTopicActions.ContainsKey(parentId))
                        {
                            _moderationCfhTopicActions.Add(parentId, new List<ModerationPresetActions>());
                        }

                        _moderationCfhTopicActions[parentId].Add(new ModerationPresetActions(Convert.ToInt32(row["id"]), Convert.ToInt32(row["parent_id"]), Convert.ToString(row["type"]), Convert.ToString(row["caption"]), Convert.ToString(row["message_text"]),
                            Convert.ToInt32(row["mute_time"]), Convert.ToInt32(row["ban_time"]), Convert.ToInt32(row["ip_time"]), Convert.ToInt32(row["trade_lock_time"]), Convert.ToString(row["default_sanction"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `moderation_preset_action_categories`;");
                DataTable presetsActionCats = dbClient.GetTable();

                if (presetsActionCats != null)
                {
                    foreach (DataRow row in presetsActionCats.Rows)
                    {
                        _userActionPresetCategories.Add(Convert.ToInt32(row["id"]), Convert.ToString(row["caption"]));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `moderation_preset_action_messages`;");
                DataTable presetsActionMessages = dbClient.GetTable();

                if (presetsActionMessages != null)
                {
                    foreach (DataRow row in presetsActionMessages.Rows)
                    {
                        int parentId = Convert.ToInt32(row["parent_id"]);

                        if (!_userActionPresetMessages.ContainsKey(parentId))
                        {
                            _userActionPresetMessages.Add(parentId, new List<ModerationPresetActionMessages>());
                        }

                        _userActionPresetMessages[parentId].Add(new ModerationPresetActionMessages(Convert.ToInt32(row["id"]), Convert.ToInt32(row["parent_id"]), Convert.ToString(row["caption"]), Convert.ToString(row["message_text"]),
                            Convert.ToInt32(row["mute_hours"]), Convert.ToInt32(row["ban_hours"]), Convert.ToInt32(row["ip_ban_hours"]), Convert.ToInt32(row["trade_lock_days"]), Convert.ToString(row["notice"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `bantype`,`value`,`reason`,`expire` FROM `bans` WHERE `bantype` = 'machine' OR `bantype` = 'user'");
                DataTable getBans = dbClient.GetTable();

                if (getBans != null)
                {
                    foreach (DataRow row in getBans.Rows)
                    {
                        string value = Convert.ToString(row["value"]);
                        string reason = Convert.ToString(row["reason"]);
                        double expires = (double) row["expire"];
                        string type = Convert.ToString(row["bantype"]);

                        ModerationBan ban = new(BanTypeUtility.GetModerationBanType(type), value, reason, expires);
                        if (expires > PlusEnvironment.GetUnixTimestamp())
                        {
                            if (value != null && !_bans.ContainsKey(value))
                                _bans.Add(value, ban);
                        }
                        else
                        {
                            dbClient.SetQuery("DELETE FROM `bans` WHERE `bantype` = @banType AND `value` = @key LIMIT 1");
                            dbClient.AddParameter("banType", BanTypeUtility.FromModerationBanType(ban.Type));
                            dbClient.AddParameter("key", value);
                            dbClient.RunQuery();
                        }
                    }
                }
            }

            Log.Info("Loaded " + (_userPresets.Count + _roomPresets.Count) + " moderation presets.");
            Log.Info("Loaded " + _userActionPresetCategories.Count + " moderation categories.");
            Log.Info("Loaded " + _userActionPresetMessages.Count + " moderation action preset messages.");
            Log.Info("Cached " + _bans.Count + " username and machine bans.");
        }

        public void ReCacheBans()
        {
            if (_bans.Count > 0)
                _bans.Clear();

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `bantype`,`value`,`reason`,`expire` FROM `bans` WHERE `bantype` = 'machine' OR `bantype` = 'user'");
                DataTable getBans = dbClient.GetTable();

                if (getBans != null)
                {
                    foreach (DataRow row in getBans.Rows)
                    {
                        string value = Convert.ToString(row["value"]);
                        string reason = Convert.ToString(row["reason"]);
                        double expires = (double) row["expire"];
                        string type = Convert.ToString(row["bantype"]);

                        ModerationBan ban = new(BanTypeUtility.GetModerationBanType(type), value, reason, expires);
                        if (expires > PlusEnvironment.GetUnixTimestamp())
                        {
                            if (value != null && !_bans.ContainsKey(value))
                                _bans.Add(value, ban);
                        }
                        else
                        {
                            dbClient.SetQuery("DELETE FROM `bans` WHERE `bantype` = @banType AND `value` = @key LIMIT 1");
                            dbClient.AddParameter("banType", BanTypeUtility.FromModerationBanType(ban.Type));
                            dbClient.AddParameter("key", value);
                            dbClient.RunQuery();
                        }
                    }
                }
            }

            Log.Info("Cached " + _bans.Count + " username and machine bans.");
        }

        public void BanUser(string mod, ModerationBanType type, string banValue, string reason, double expireTimestamp)
        {
            string banType = type == ModerationBanType.Ip ? "ip" : type == ModerationBanType.Machine ? "machine" : "user";
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `bans` (`bantype`, `value`, `reason`, `expire`, `added_by`,`added_date`) VALUES (@banType, @banValue, @reason, @expireTimestamp, @mod, @timestamp);");
                dbClient.AddParameter("banType", banType);
                dbClient.AddParameter("banValue", banValue);
                dbClient.AddParameter("reason", reason);
                dbClient.AddParameter("expireTimestamp", expireTimestamp);
                dbClient.AddParameter("mod", mod);
                dbClient.AddParameter("timestamp", PlusEnvironment.GetUnixTimestamp());
                dbClient.RunQuery();
            }

            if (type == ModerationBanType.Machine || type == ModerationBanType.Username)
            {
                if (!_bans.ContainsKey(banValue))
                    _bans.Add(banValue, new ModerationBan(type, banValue, reason, expireTimestamp));
            }
        }

        public ICollection<string> UserMessagePresets => _userPresets;

        public ICollection<string> RoomMessagePresets => _roomPresets;

        public ICollection<ModerationTicket> GetTickets => _modTickets.Values;

        public Dictionary<string, List<ModerationPresetActions>> UserActionPresets
        {
            get
            {
                Dictionary<string, List<ModerationPresetActions>> result = new();
                foreach (KeyValuePair<int, string> category in _moderationCfhTopics.ToList())
                {
                    result.Add(category.Value, new List<ModerationPresetActions>());

                    if (_moderationCfhTopicActions.ContainsKey(category.Key))
                    {
                        foreach (ModerationPresetActions data in _moderationCfhTopicActions[category.Key])
                        {
                            result[category.Value].Add(data);
                        }
                    }
                }

                return result;
            }
        }

        public bool TryAddTicket(ModerationTicket ticket)
        {
            ticket.Id = _ticketCount++;
            return _modTickets.TryAdd(ticket.Id, ticket);
        }

        public bool TryGetTicket(int ticketId, out ModerationTicket ticket)
        {
            return _modTickets.TryGetValue(ticketId, out ticket);
        }

        public bool UserHasTickets(int userId)
        {
            return _modTickets.Any(x => x.Value.Sender.Id == userId && x.Value.Answered == false);
        }

        public ModerationTicket GetTicketBySenderId(int userId)
        {
            return _modTickets.FirstOrDefault(x => x.Value.Sender.Id == userId).Value;
        }

        /// <summary>
        /// Runs a quick check to see if a ban record is cached in the server.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ban"></param>
        /// <returns></returns>
        public bool IsBanned(string key, out ModerationBan ban)
        {
            if (_bans.TryGetValue(key, out ban))
            {
                if (!ban.Expired)
                    return true;

                //This ban has expired, let us quickly remove it here.
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `bans` WHERE `bantype` = @banType AND `value` = @key LIMIT 1");
                    dbClient.AddParameter("banType", BanTypeUtility.FromModerationBanType(ban.Type));
                    dbClient.AddParameter("key", key);
                    dbClient.RunQuery();
                }

                //And finally, let us remove the ban record from the cache.
                if (_bans.ContainsKey(key))
                    _bans.Remove(key);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Run a quick database check to see if this ban exists in the database.
        /// </summary>
        /// <param name="machineId">The value of the ban.</param>
        /// <returns></returns>
        public bool MachineBanCheck(string machineId)
        {
            if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(machineId, out ModerationBan _))
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `bans` WHERE `bantype` = 'machine' AND `value` = @value LIMIT 1");
                    dbClient.AddParameter("value", machineId);
                    DataRow banRow = dbClient.GetRow();

                    //If there is no more ban record, then we can simply remove it from our cache!
                    if (banRow == null)
                    {
                        PlusEnvironment.GetGame().GetModerationManager().RemoveBan(machineId);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Run a quick database check to see if this ban exists in the database.
        /// </summary>
        /// <param name="username">The value of the ban.</param>
        /// <returns></returns>
        public bool UsernameBanCheck(string username)
        {
            if (PlusEnvironment.GetGame().GetModerationManager().IsBanned(username, out ModerationBan _))
            {
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `bans` WHERE `bantype` = 'user' AND `value` = @value LIMIT 1");
                    dbClient.AddParameter("value", username);
                    DataRow banRow = dbClient.GetRow();

                    //If there is no more ban record, then we can simply remove it from our cache!
                    if (banRow == null)
                    {
                        PlusEnvironment.GetGame().GetModerationManager().RemoveBan(username);
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Remove a ban from the cache based on a given value.
        /// </summary>
        /// <param name="value"></param>
        public void RemoveBan(string value)
        {
            _bans.Remove(value);
        }
    }
}