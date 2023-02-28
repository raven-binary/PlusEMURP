﻿using System;
using System.Collections.Generic;
using System.Data;
using Plus.Database.Interfaces;
using Plus.HabboHotel.Groups;
using Plus.Utilities;

namespace Plus.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string OwnerName { get; set; }
        public int OwnerId { get; set; }
        public string Password { get; set; }
        public int Score { get; set; }
        public RoomAccess Access { get; set; }
        public string Type { get; set; }
        public int UsersMax { get; set; }
        public int UsersNow { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
        public string Floor { get; set; }
        public string Landscape { get; set; }
        public int AllowPets { get; set; }
        public int AllowPetsEating { get; set; }
        public int RoomBlockingEnabled { get; set; }
        public int HideWall { get; set; }
        public int WallThickness { get; set; }
        public int FloorThickness { get; set; }
        public string Wallpaper { get; set; }
        public int WhoCanMute { get; set; }
        public int WhoCanBan { get; set; }
        public int WhoCanKick { get; set; }
        public int ChatMode { get; set; }
        public int ChatSize { get; set; }
        public int ChatSpeed { get; set; }
        public int ExtraFlood { get; set; }
        public int ChatDistance { get; set; }
        public int TradeSettings { get; set; }
        public bool PushEnabled { get; set; }
        public bool PullEnabled { get; set; }
        public bool SuperPushEnabled { get; set; }
        public bool SuperPullEnabled { get; set; }
        public bool EnablesEnabled { get; set; }
        public bool RespectNotificationsEnabled { get; set; }
        public bool PetMorphsAllowed { get; set; }
        public int SalePrice { get; set; }
        public bool ReverseRollers { get; set; }
        public bool LayEnabled { get; set; }
        public bool CombatEnabled { get; set; }

        public List<string> Tags;

        private Group _group;


        public RoomModel Model { get; set; }

        public RoomData(int id, string caption, string modelName, string ownerName, int ownerId, string password, int score, string type, string access, int usersNow, int usersMax, int category, string description,
            string tags, string floor, string landscape, int allowPets, int allowPetsEating, int roomBlockingEnabled, int hideWall, int wallThickness, int floorThickness, string wallpaper, int muteSettings,
            int banSettings, int kickSettings, int chatMode, int chatSize, int chatSpeed, int extraFlood, int chatDistance, int tradeSettings, bool pushEnabled, bool pullEnabled, bool superPushEnabled,
            bool superPullEnabled, bool enablesEnabled, bool respectedNotificationsEnabled, bool petMorphsAllowed, int groupId, int salePrice, bool layEnabled, bool combatEnabled, RoomModel model)
        {
            Id = id;
            Name = caption;
            ModelName = modelName;
            OwnerName = ownerName;
            OwnerId = ownerId;
            Password = password;
            Score = score;
            Type = type;
            Access = RoomAccessUtility.ToRoomAccess(access);
            UsersNow = usersNow;
            UsersMax = usersMax;
            Category = category;
            Description = description;

            Tags = new List<string>();
            foreach (string tag in tags.Split(','))
            {
                Tags.Add(tag);
            }

            Floor = floor;
            Landscape = landscape;
            AllowPets = allowPets;
            AllowPetsEating = allowPetsEating;
            RoomBlockingEnabled = roomBlockingEnabled;
            HideWall = hideWall;
            WallThickness = wallThickness;
            FloorThickness = floorThickness;
            Wallpaper = wallpaper;
            WhoCanMute = muteSettings;
            WhoCanBan = banSettings;
            WhoCanKick = kickSettings;
            ChatMode = chatMode;
            ChatSize = chatSize;
            ChatSpeed = chatSpeed;
            ExtraFlood = extraFlood;
            ChatDistance = chatDistance;
            TradeSettings = tradeSettings;
            PushEnabled = pushEnabled;
            PullEnabled = pullEnabled;
            SuperPushEnabled = superPushEnabled;
            SuperPullEnabled = superPullEnabled;
            EnablesEnabled = enablesEnabled;
            RespectNotificationsEnabled = respectedNotificationsEnabled;
            PetMorphsAllowed = petMorphsAllowed;
            SalePrice = salePrice;
            ReverseRollers = false;
            LayEnabled = layEnabled;
            CombatEnabled = combatEnabled;

            if (groupId > 0)
                PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out _group);

            LoadPromotions();

            Model = model;
        }

        public RoomPromotion Promotion { get; set; }

        public Group Group
        {
            get => _group;
            set => _group = value;
        }

        public RoomData(RoomData data)
        {
            Id = data.Id;
            Name = data.Name;
            ModelName = data.ModelName;
            OwnerName = data.OwnerName;
            OwnerId = data.OwnerId;
            Password = data.Password;
            Score = data.Score;
            Type = data.Type;
            Access = data.Access;
            UsersNow = data.UsersNow;
            UsersMax = data.UsersMax;
            Category = data.Category;
            Description = data.Description;
            Tags = data.Tags;
            Floor = data.Floor;
            Landscape = data.Landscape;
            AllowPets = data.AllowPets;
            AllowPetsEating = data.AllowPetsEating;
            RoomBlockingEnabled = data.RoomBlockingEnabled;
            HideWall = data.HideWall;
            WallThickness = data.WallThickness;
            FloorThickness = data.FloorThickness;
            Wallpaper = data.Wallpaper;
            WhoCanMute = data.WhoCanMute;
            WhoCanBan = data.WhoCanBan;
            WhoCanKick = data.WhoCanKick;
            ChatMode = data.ChatMode;
            ChatSize = data.ChatSize;
            ChatSpeed = data.ChatSpeed;
            ExtraFlood = data.ExtraFlood;
            ChatDistance = data.ChatDistance;
            TradeSettings = data.TradeSettings;
            PushEnabled = data.PushEnabled;
            PullEnabled = data.PullEnabled;
            SuperPushEnabled = data.SuperPushEnabled;
            SuperPullEnabled = data.SuperPullEnabled;
            RespectNotificationsEnabled = data.RespectNotificationsEnabled;
            PetMorphsAllowed = data.PetMorphsAllowed;
            Group = data.Group;
            SalePrice = data.SalePrice;
            EnablesEnabled = data.EnablesEnabled;
            ReverseRollers = data.ReverseRollers;
            LayEnabled = data.LayEnabled;
            CombatEnabled = data.CombatEnabled;

            Model = data.Model;
        }

        public void LoadPromotions()
        {
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_promotions` WHERE `room_id` = " + Id + " LIMIT 1;");
                DataRow getPromotion = dbClient.GetRow();

                if (getPromotion != null)
                {
                    if (Convert.ToDouble(getPromotion["timestamp_expire"]) > UnixTimestamp.GetNow())
                        Promotion = new RoomPromotion(Convert.ToString(getPromotion["title"]), Convert.ToString(getPromotion["description"]), Convert.ToDouble(getPromotion["timestamp_start"]), Convert.ToDouble(getPromotion["timestamp_expire"]), Convert.ToInt32(getPromotion["category_id"]));
                }
            }
        }

        public bool HasActivePromotion => Promotion != null;

        public void EndPromotion()
        {
            if (!HasActivePromotion)
                return;

            Promotion = null;
        }
    }
}