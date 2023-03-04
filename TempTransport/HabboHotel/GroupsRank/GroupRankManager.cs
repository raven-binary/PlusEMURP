using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;
using Plus.Communication.Packets.Incoming;
using System.Collections.Concurrent;

using Plus.Database.Interfaces;
using log4net;

namespace Plus.HabboHotel.GroupsRank
{
    public class GroupRankManager
    {
        private ConcurrentDictionary<string, GroupRank> _groupsRank;

        public GroupRankManager()
        {
            this.Init();
        }

        public bool TryGetRank(int CorpID, int RankID, out GroupRank GroupRank)
        {
            GroupRank = null;
            string Name = Convert.ToString(CorpID) + "/" + Convert.ToString(RankID);

            if (this._groupsRank.ContainsKey(Name))
                return this._groupsRank.TryGetValue(Name, out GroupRank);

            DataRow Row = null;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups_rank` WHERE `job_id` = @job AND `rank_id` = @id LIMIT 1");
                dbClient.AddParameter("job", CorpID);
                dbClient.AddParameter("id", RankID);
                Row = dbClient.getRow();

                if (Row != null)
                {
                    GroupRank = new GroupRank(Convert.ToInt32(Row["job_id"]), Convert.ToInt32(Row["rank_id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["look_h"]), Convert.ToString(Row["look_f"]), Convert.ToInt32(Row["pay"]), Convert.ToInt32(Row["work_everywhere"]), Convert.ToInt32(Row["rank"]));
                    this._groupsRank.TryAdd(Name, GroupRank);
                    return true;
                }
            }
            return false;
        }

        public void Init()
        {
            _groupsRank = new ConcurrentDictionary<string, GroupRank>();
        }
    }
}