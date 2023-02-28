using System.Collections.Concurrent;
using System.Collections.Generic;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Users.Badges;
using Plus.HabboHotel.Users.Messenger;
using Plus.HabboHotel.Users.Relationships;

namespace Plus.HabboHotel.Users.UserData
{
    public class UserData
    {
        public int UserId { get; }
        public Habbo User;

        public Dictionary<int, Relationship> Relations;
        public ConcurrentDictionary<string, UserAchievement> Achievements;
        public List<Badge> Badges;
        public List<int> FavoritedRooms;
        public Dictionary<int, MessengerRequest> Requests;
        public Dictionary<int, MessengerBuddy> Friends;
        public Dictionary<int, int> Quests;

        public UserData(int userId, ConcurrentDictionary<string, UserAchievement> achievements, List<int> favoritedRooms,
            List<Badge> badges, Dictionary<int, MessengerBuddy> friends, Dictionary<int, MessengerRequest> requests, Dictionary<int, int> quests, Habbo user,
            Dictionary<int, Relationship> relations)
        {
            UserId = userId;
            Achievements = achievements;
            FavoritedRooms = favoritedRooms;
            Badges = badges;
            Friends = friends;
            Requests = requests;
            Quests = quests;
            User = user;
            Relations = relations;
        }
    }
}