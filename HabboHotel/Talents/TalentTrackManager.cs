﻿using System;
using System.Collections.Generic;
using System.Data;
using log4net;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Talents
{
    public class TalentTrackManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TalentTrackManager));

        private readonly Dictionary<int, TalentTrackLevel> _citizenshipLevels;

        public TalentTrackManager()
        {
            _citizenshipLevels = new Dictionary<int, TalentTrackLevel>();
        }

        public void Init()
        {
            DataTable data = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `type`,`level`,`data_actions`,`data_gifts` FROM `talents`");
                data = dbClient.GetTable();
            }

            if (data != null)
            {
                foreach (DataRow row in data.Rows)
                {
                    _citizenshipLevels.Add(Convert.ToInt32(row["level"]), new TalentTrackLevel(Convert.ToString(row["type"]), Convert.ToInt32(row["level"]), Convert.ToString(row["data_actions"]), Convert.ToString(row["data_gifts"])));
                }
            }

            Log.Info("Loaded " + _citizenshipLevels.Count + " talent track levels");
        }

        public ICollection<TalentTrackLevel> GetLevels()
        {
            return _citizenshipLevels.Values;
        }
    }
}