﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

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
        public bool ForumEnabled { get; set; }
        public GroupType Type { get; set; }
        public bool HasForum;
        private readonly List<int> _members;
        private readonly List<int> _requests;
        private readonly List<int> _administrators;

        private RoomData _room;

        public Group(int id, string name, string description, string badge, int roomId, int owner, int time, int type, int colour1, int colour2, int adminOnlyDeco, bool hasForum)
        {
            Id = id;
            Name = name;
            Description = description;
            RoomId = roomId;
            Badge = badge;
            CreateTime = time;
            CreatorId = owner;
            Colour1 = colour1 == 0 ? 1 : colour1;
            Colour2 = colour2 == 0 ? 1 : colour2;
            HasForum = hasForum;
            Type = (GroupType) type;

            AdminOnlyDeco = adminOnlyDeco;
            ForumEnabled = ForumEnabled;

            _members = new List<int>();
            _requests = new List<int>();
            _administrators = new List<int>();

            InitMembers();
        }

        public void InitMembers()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `user_id`, `rank` FROM `group_memberships` WHERE `group_id` = @id");
                dbClient.AddParameter("id", Id);
                DataTable members = dbClient.GetTable();

                if (members != null)
                {
                    foreach (DataRow row in members.Rows)
                    {
                        int userId = Convert.ToInt32(row["user_id"]);
                        bool isAdmin = Convert.ToInt32(row["rank"]) != 0;

                        if (isAdmin)
                        {
                            if (!_administrators.Contains(userId))
                                _administrators.Add(userId);
                        }
                        else
                        {
                            if (!_members.Contains(userId))
                                _members.Add(userId);
                        }
                    }
                }

                dbClient.SetQuery("SELECT `user_id` FROM `group_requests` WHERE `group_id` = @id");
                dbClient.AddParameter("id", Id);
                DataTable requests = dbClient.GetTable();

                if (requests != null)
                {
                    foreach (DataRow row in requests.Rows)
                    {
                        int userId = Convert.ToInt32(row["user_id"]);

                        if (_members.Contains(userId) || _administrators.Contains(userId))
                        {
                            dbClient.RunQuery("DELETE FROM `group_requests` WHERE `group_id` = '" + Id + "' AND `user_id` = '" + userId + "'");
                        }
                        else if (!_requests.Contains(userId))
                        {
                            _requests.Add(userId);
                        }
                    }
                }
            }
        }

        public List<int> GetMembers => _members.ToList();

        public List<int> GetRequests => _requests.ToList();

        public List<int> GetAdministrators => _administrators.ToList();

        public List<int> GetAllMembers
        {
            get
            {
                List<int> members = new(_administrators.ToList());
                members.AddRange(_members.ToList());

                return members;
            }
        }

        public int MemberCount => _members.Count + _administrators.Count;

        public int RequestCount => _requests.Count;

        public bool IsMember(int id)
        {
            return _members.Contains(id) || _administrators.Contains(id);
        }

        public bool IsAdmin(int id)
        {
            return _administrators.Contains(id);
        }

        public bool HasRequest(int id)
        {
            return _requests.Contains(id);
        }

        public void MakeAdmin(int id)
        {
            if (_members.Contains(id))
                _members.Remove(id);

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET `rank` = '1' WHERE `user_id` = @uid AND `group_id` = @gid LIMIT 1");
                dbClient.AddParameter("gid", Id);
                dbClient.AddParameter("uid", id);
                dbClient.RunQuery();
            }

            if (!_administrators.Contains(id))
                _administrators.Add(id);
        }

        public void TakeAdmin(int userId)
        {
            if (!_administrators.Contains(userId))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE group_memberships SET `rank` = '0' WHERE user_id = @uid AND group_id = @gid");
                dbClient.AddParameter("gid", Id);
                dbClient.AddParameter("uid", userId);
                dbClient.RunQuery();
            }

            _administrators.Remove(userId);
            _members.Add(userId);
        }

        public void AddMember(int id)
        {
            if (IsMember(id) || Type == GroupType.Locked && _requests.Contains(id))
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (IsAdmin(id))
                {
                    dbClient.SetQuery("UPDATE `group_memberships` SET `rank` = '0' WHERE user_id = @uid AND group_id = @gid");
                    _administrators.Remove(id);
                    _members.Add(id);
                }
                else if (Type == GroupType.Locked)
                {
                    dbClient.SetQuery("INSERT INTO `group_requests` (user_id, group_id) VALUES (@uid, @gid)");
                    _requests.Add(id);
                }
                else
                {
                    dbClient.SetQuery("INSERT INTO `group_memberships` (user_id, group_id) VALUES (@uid, @gid)");
                    _members.Add(id);
                }

                dbClient.AddParameter("gid", Id);
                dbClient.AddParameter("uid", id);
                dbClient.RunQuery();
            }
        }

        public void DeleteMember(int id)
        {
            if (IsMember(id))
            {
                if (_members.Contains(id))
                    _members.Remove(id);
            }
            else if (IsAdmin(id))
            {
                if (_administrators.Contains(id))
                    _administrators.Remove(id);
            }
            else
                return;

            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM group_memberships WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", Id);
                dbClient.AddParameter("uid", id);
                dbClient.RunQuery();
            }
        }

        public RoomRightLevels getGroupRightLevel(Habbo habbo)
        {
            if (this.Id > 0 && this.IsMember(habbo.Id) || this.IsAdmin(habbo.Id))
            {
                if (this.IsAdmin(habbo.Id))
                    return RoomRightLevels.GUILD_ADMIN;

                if (this.AdminOnlyDeco == 0)
                    return RoomRightLevels.GUILD_RIGHTS;
            }

            return RoomRightLevels.NONE;
        }

        public void HandleRequest(int id, bool accepted)
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                if (accepted)
                {
                    dbClient.SetQuery("INSERT INTO group_memberships (user_id, group_id) VALUES (@uid, @gid)");
                    dbClient.AddParameter("gid", Id);
                    dbClient.AddParameter("uid", id);
                    dbClient.RunQuery();

                    _members.Add(id);
                }

                dbClient.SetQuery("DELETE FROM group_requests WHERE user_id=@uid AND group_id=@gid LIMIT 1");
                dbClient.AddParameter("gid", Id);
                dbClient.AddParameter("uid", id);
                dbClient.RunQuery();
            }

            if (_requests.Contains(id))
                _requests.Remove(id);
        }

        public RoomData GetRoom()
        {
            if (_room == null)
            {
                if (!RoomFactory.TryGetData(RoomId, out RoomData data))
                    return null;

                _room = data;
                return data;
            }

            return _room;
        }


        public void ClearRequests()
        {
            _requests.Clear();
        }

        public void Dispose()
        {
            _requests.Clear();
            _members.Clear();
            _administrators.Clear();
        }
    }
}