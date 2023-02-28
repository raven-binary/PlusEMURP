namespace Plus.HabboHotel.Users
{
    public class HabboStats
    {
        public int RoomVisits { get; set; }
        public double OnlineTime { get; set; }
        public int Respect { get; set; }
        public int RespectGiven { get; set; }
        public int GiftsGiven { get; set; }
        public int GiftsReceived { get; set; }
        public int DailyRespectPoints { get; set; }
        public int DailyPetRespectPoints { get; set; }
        public int AchievementPoints { get; set; }
        public int QuestId { get; set; }
        public int QuestProgress { get; set; }
        public int FavouriteGroupId { get; set; }
        public string RespectsTimestamp { get; set; }
        public int ForumPosts { get; set; }
        public int CombatLevel { get; set; }

        public HabboStats(int roomVisits, double onlineTime, int respect, int respectGiven, int giftsGiven, int giftsReceived, int dailyRespectPoints, int dailyPetRespectPoints, int achievementPoints, int questId, int questProgress, int groupId, string respectsTimestamp, int forumPosts, int combatLevel)
        {
            RoomVisits = roomVisits;
            OnlineTime = onlineTime;
            Respect = respect;
            RespectGiven = respectGiven;
            GiftsGiven = giftsGiven;
            GiftsReceived = giftsReceived;
            DailyRespectPoints = dailyRespectPoints;
            DailyPetRespectPoints = dailyPetRespectPoints;
            AchievementPoints = achievementPoints;
            QuestId = questId;
            QuestProgress = questProgress;
            FavouriteGroupId = groupId;
            RespectsTimestamp = respectsTimestamp;
            ForumPosts = forumPosts;
            CombatLevel = combatLevel;
        }
    }
}