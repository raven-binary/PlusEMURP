using System;
using System.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;
using log4net;

namespace Plus.HabboRoleplay.Misc
{
    public static class RoleplayDataManager
    {
        /// <summary>
        /// log4net
        /// </summary>
        private static readonly ILog log = LogManager.GetLogger("Plus.HabboRoleplay.Misc.RoleplayDataManager");

        /// <summary>
        /// Contains all of the data
        /// </summary>
        private static ConcurrentDictionary<string, Dictionary<string, string>> Data;

        /// <summary>
        /// Creates new instance & calls the LoadData method
        /// </summary>
        public static void Init()
        {
            Data = new ConcurrentDictionary<string, Dictionary<string, string>>();
            LoadData();
            LoadWantedListData();
        }

        /// <summary>
        /// Loads all the data into dictionary
        /// </summary>
        public static void LoadData()
        {
            Data.Clear();

            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `user_rp_stats`");
                DataTable AllData = DB.getTable();

                if (AllData == null)
                    return;
                log.Info("Roleplay Stats -> LOADED");

                foreach (DataRow Row in AllData.Rows)
                {
                    InsertData(Row);
                }
            }
        }

        /// <summary>
        /// Loads all the data into dictionary
        /// </summary>
        public static void LoadWantedListData()
        {
            using (var DB = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DB.SetQuery("SELECT * FROM `users_wanted`");
                DataTable AllData = DB.getTable();

                if (AllData == null)
                    return;
                log.Info("Wanted List -> LOADED");
            }
        }

        /// <summary>
        /// Inserts the data into the dictionary
        /// </summary>
        private static void InsertData(DataRow Row)
        {
            try
            {
                string MainKey = Convert.ToString(Row["MainKey"]).ToLower();

                string DataString = Convert.ToString(Row["Data"]);
                DataString = DataString.Replace(" ", "");

                string[] DataArray = DataString.Split('|');
                Dictionary<string, string> KeyValue = new Dictionary<string, string>();

                for (int i = 0; i < DataArray.Length; i++)
                {
                    string SubKey = DataArray[i].Split(':')[0].ToLower();
                    string Value = DataArray[i].Split(':')[1];

                    if (String.IsNullOrEmpty(SubKey) || String.IsNullOrEmpty(Value))
                        continue;

                    KeyValue.Add(SubKey, Value);
                }

                Data.TryAdd(MainKey, KeyValue);
            }
            catch { }
        }

        /// <summary>
        /// Gets data by key
        /// </summary>
        public static string GetData(string MainKey, string SubKey)
        {
            MainKey = MainKey.ToLower();
            SubKey = SubKey.ToLower();

            if (!Data.ContainsKey(MainKey))
                return null;

            if (!Data[MainKey].ContainsKey(SubKey))
                return null;

            return Data[MainKey][SubKey];
        }
    }
}