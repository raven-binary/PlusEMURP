using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Users;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Groups
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AdminOnlyDeco { get; set; }
        public string Badge { get; set; }
        public int CreateTime { get; set; }
        public int CreatorId { get; set; }
        public string Description { get; set; }
        public int RoomId { get; set; }
        public int Colour1 { get; set; }
        public int Colour2 { get; set; }
        public int ChiffreAffaire { get; set; }
        public int PayedByEtat { get; set; }
        public int Usine { get; set; }
        public int isOpen { get; set; }
        public bool ForumEnabled { get; set; }
        public GroupType GroupType { get; set; }
        public int Cash { get; set; }

        public int Safe = 0;

        private List<int> _members;
        private List<int> _requests;
        private List<int> _administrators;
        
        public Group(int Id, string Name, string Description, string Badge, int RoomId, int Owner, int Time, int Type, int Colour1, int Colour2, int AdminOnlyDeco, int ChiffreAffaire, int PayedByEtat, int Usine, int Cash, int Safe)
        {
            this.Id = Id;
            this.Name = Name;
            this.Description = Description;
            this.RoomId = RoomId;
            this.Badge = Badge;
            this.CreateTime = Time;
            this.CreatorId = Owner;
            this.Colour1 = (Colour1 == 0) ? 1 : Colour1;
            this.Colour2 = (Colour2 == 0) ? 1 : Colour2;

            switch (Type)
            {
                case 0:
                    this.GroupType = GroupType.OPEN;
                    break;
                case 1:
                    this.GroupType = GroupType.LOCKED;
                    break;
                case 2:
                    this.GroupType = GroupType.PRIVATE;
                    break;
            }

            this.AdminOnlyDeco = 1;
            this.ChiffreAffaire = ChiffreAffaire;
            this.PayedByEtat = PayedByEtat;
            this.Usine = Usine;
            this.ForumEnabled = ForumEnabled;

            this._members = new List<int>();
            this._requests = new List<int>();
            this._administrators = new List<int>();

            this.Cash = Cash;
            this.Safe = Safe;

            InitMembers();
        }

        public void InitMembers()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable GetMembers = null;
                dbClient.SetQuery("SELECT `user_id`, `rank_id`  FROM `group_memberships` WHERE `group_id` = @id");
                dbClient.AddParameter("id", this.Id);
                GetMembers = dbClient.getTable();

                if (GetMembers != null)
                {
                    foreach (DataRow Row in GetMembers.Rows)
                    {
                        bool IsAdmin = false;
                        DataTable getRankMember = null;
                        dbClient.SetQuery("SELECT `rank` FROM `groups_rank` WHERE `job_id` = @id AND `rank_id` = @rid LIMIT 1");
                        dbClient.AddParameter("id", this.Id);
                        dbClient.AddParameter("rid", Convert.ToInt32(Row["rank_id"]));
                        getRankMember = dbClient.getTable();
                        foreach (DataRow Row2 in getRankMember.Rows)
                        {
                            if (Convert.ToInt32(Row2["rank"]) == 2)
                            {
                                IsAdmin = true;
                            }
                        }

                        int UserId = Convert.ToInt32(Row["user_id"]);

                        if (IsAdmin)
                        {
                            if (!this._administrators.Contains(UserId))
                                this._administrators.Add(UserId);
                        }
                        else
                        {
                            if (!this._members.Contains(UserId))
                                this._members.Add(UserId);
                        }
                    }
                }

                DataTable GetRequests = null;
                dbClient.SetQuery("SELECT `user_id` FROM `group_requests` WHERE `group_id` = @id");
                dbClient.AddParameter("id", this.Id);
                GetRequests = dbClient.getTable();

                if (GetRequests != null)
                {
                    foreach (DataRow Row in GetRequests.Rows)
                    {
                        int UserId = Convert.ToInt32(Row["user_id"]);
                        
                        if (this._members.Contains(UserId) || this._administrators.Contains(UserId))
                        {
                            dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + this.Id + "' AND `user_id` = '" + UserId + "'");
                        }
                        else if (!this._requests.Contains(UserId))
                        {
                            this._requests.Add(UserId);
                        }
                    }
                }
            }
        }

        public List<int> GetMembers
        {
            get { return this._members.ToList(); }
        }

        public List<int> GetRequests
        {
            get { return this._requests.ToList(); }
        }

        public List<int> GetAdministrators
        {
            get { return this._administrators.ToList(); }
        }

        public List<int> GetAllMembers
        {
            get
            {
                List<int> Members = new List<int>(this._administrators.ToList());
                Members.AddRange(this._members.ToList());

                return Members;
            }
        }

        public int MemberCount
        {
            get { return this._members.Count + this._administrators.Count; }
        }

        public int RequestCount
        {
            get { return this._requests.Count; }
        }

        public bool IsMember(int Id)
        {
            return this._members.Contains(Id) || this._administrators.Contains(Id);
        }

        public bool IsAdmin(int Id)
        {
            return this._administrators.Contains(Id);
        }

        public bool HasRequest(int Id)
        {
            return this._requests.Contains(Id);
        }

        public void MakeAdmin(int Id)
        {
            if (this._members.Contains(Id))
                this._members.Remove(Id);

            if (!this._administrators.Contains(Id))
                this._administrators.Add(Id);
        }

        public void removeAdmin(int Id)
        {
            if (!this._members.Contains(Id))
                this._members.Add(Id);

            if (this._administrators.Contains(Id))
                this._administrators.Remove(Id);
        }

        public void TakeAdmin(int UserId)
        {
            if (!this._administrators.Contains(UserId))
                return;

            this._administrators.Remove(UserId);
            this._members.Add(UserId);
        }

        public void AddMemberList(int Id)
        {
            this._members.Add(Id);
        }

        public void updateChiffre()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET stock = @stock WHERE id = @uid");
                dbClient.AddParameter("stock", this.ChiffreAffaire);
                dbClient.AddParameter("uid", this.Id);
                dbClient.RunQuery();
            }
        }

        public void updateCash()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET cash = @cash WHERE id = @uid");
                dbClient.AddParameter("cash", this.Cash);
                dbClient.AddParameter("uid", this.Id);
                dbClient.RunQuery();
            }
        }

        public void PromoteRank(int Id)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET rank_id = rank_id - 1  WHERE user_id = @uid");
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void DemoteTier(int UserId, int Number)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET tier = tier - " + Number + "  WHERE user_id = @uid");
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }
        }

        public void PromoteTier(int UserId, int Number)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET tier = tier + " + Number + "  WHERE user_id = @uid");
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }
        }

        public void ChangeTier(int UserId, int Number)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET tier = " + Number + "  WHERE user_id = @uid");
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }
        }

        public void updateRank(int Id)
        {
            if (!this.IsMember(Id))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET rank_id = rank_id + 1 WHERE user_id = @uid");
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void updateRankid(int UserId, int Number)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET rank_id = " + Number + "  WHERE user_id = @uid");
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }
        }

        public void AddMemberByForce(int Id)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Group Group = null;
                PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(Id, out Group);
                Group._members.Remove(Id);
                this._members.Add(Id);
                dbClient.SetQuery("UPDATE `group_memberships` SET group_id = @gid, rank_id = '7' WHERE user_id = @uid");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();

                dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid LIMIT 1");
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void QuitJob (int UserId)
        {
            Group Group = null;
            PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(1, out Group);

            this._members.Add(Id);
            Group._members.Remove(Id);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `group_memberships` SET group_id = '8', rank_id = '1', tier = '0' WHERE user_id = @uid");
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }
            Group.ChangeTier(UserId, 0);
        }

        public void AddMember(int Id)
        {
            if (this.IsMember(Id) || this.GroupType == GroupType.LOCKED && this._requests.Contains(Id))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (this.GroupType == GroupType.LOCKED)
                {
                    dbClient.SetQuery("INSERT INTO `group_requests` (user_id, group_id) VALUES (@uid, @gid)");
                    this._requests.Add(Id);
                }
                else
                {
                    Group Group = null;
                    PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(1, out Group);
                    Group._members.Remove(Id);
                    dbClient.SetQuery("UPDATE `group_memberships` SET group_id = @gid, rank_id = '7' WHERE user_id = @uid");
                    this._members.Add(Id);
                }

                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }
        }

        public void sendToChomage(int UserId)
        {
            if (this._members.Contains(UserId))
            {
                    this._members.Remove(UserId);
            }
            else if (this._administrators.Contains(UserId))
            {
                    this._administrators.Remove(UserId);
            }
            else
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET group_id = 1, rank_id = 1 WHERE user_id=@uid LIMIT 1");
                dbClient.AddParameter("uid", UserId);
                dbClient.RunQuery();
            }
        }

        public void HandleRequest(int Id, bool Accepted)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (Accepted)
                {
                    dbClient.SetQuery("UPDATE group_memberships SET group_id = @gid, rank_id = 1 WHERE user_id = @uid");
                    dbClient.AddParameter("gid", this.Id);
                    dbClient.AddParameter("uid", Id);
                    dbClient.RunQuery();

                    Group Group = null;
                    PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(1, out Group);
                    Group._members.Remove(Id);
                    this._members.Add(Id);
                }

                dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", this.Id);
                dbClient.AddParameter("uid", Id);
                dbClient.RunQuery();
            }

            if (this._requests.Contains(Id))
                this._requests.Remove(Id);
        }

        public void ClearRequests()
        {
            this._requests.Clear();
        }

        public void Dispose()
        {
            this._requests.Clear();
            this._members.Clear();
            this._administrators.Clear();
        }
    }
}
