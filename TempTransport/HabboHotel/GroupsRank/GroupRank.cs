using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.GroupsRank
{
    public class GroupRank
    {
        public int RankId { get; set; }
        public int JobId { get; set; }
        public string Name { get; set; }
        public string Look_H { get; set; }
        public string Look_F { get; set; }
        public int Pay { get; set; }
        public int WorkEverywhere { get; set; }
        public int Rank { get; set; }

        public GroupRank(int RankId, int JobId, string Name, string Look_H, string Look_F, int Pay, int WorkEverywhere, int Rank)
        {
            this.RankId = RankId;
            this.JobId = JobId;
            this.Name = Name;
            this.Look_H = Look_H;
            this.Look_F = Look_F;
            this.Pay = Pay;
            this.WorkEverywhere = WorkEverywhere;
            this.Rank = Rank;
        }
    }
}
