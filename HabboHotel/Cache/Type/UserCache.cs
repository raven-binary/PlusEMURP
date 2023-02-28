﻿using System;

namespace Plus.HabboHotel.Cache.Type
{
    public class UserCache
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Motto { get; set; }
        public string Look { get; set; }
        public DateTime AddedTime { get; set; }

        public UserCache(int id, string username, string motto, string look)
        {
            Id = id;
            Username = username;
            Motto = motto;
            Look = look;
            AddedTime = DateTime.Now;
        }

        public bool IsExpired()
        {
            TimeSpan cacheTime = DateTime.Now - AddedTime;
            return cacheTime.TotalMinutes >= 30;
        }
    }
}