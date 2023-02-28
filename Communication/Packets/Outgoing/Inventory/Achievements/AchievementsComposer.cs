using System.Collections.Generic;
using Plus.HabboHotel.Achievements;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementsComposer : MessageComposer
    {
        public List<Achievement> Achievements { get; }
        public Habbo Habbo { get; }

        public AchievementsComposer(Habbo habbo, List<Achievement> achievements)
            : base(ServerPacketHeader.AchievementsMessageComposer)
        {
            Achievements = achievements;
            Habbo = habbo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Achievements.Count);
            foreach (Achievement achievement in Achievements)
            {
                UserAchievement userData = Habbo.GetAchievementData(achievement.GroupName);
                int targetLevel = (userData != null ? userData.Level + 1 : 1);
                int totalLevels = achievement.Levels.Count;

                targetLevel = (targetLevel > totalLevels ? totalLevels : targetLevel);

                AchievementLevel targetLevelData = achievement.Levels[targetLevel];
                packet.WriteInteger(achievement.Id); // Unknown (ID?)
                packet.WriteInteger(targetLevel); // Target level
                packet.WriteString(achievement.GroupName + targetLevel); // Target name/desc/badge

                packet.WriteInteger(1);
                packet.WriteInteger(targetLevelData.Requirement); // Progress req/target          
                packet.WriteInteger(targetLevelData.RewardPixels);

                packet.WriteInteger(0); // Type of reward
                packet.WriteInteger(userData != null ? userData.Progress : 0); // Current progress

                packet.WriteBoolean(userData != null ? (userData.Level >= totalLevels) : false); // Set 100% completed(??)
                packet.WriteString(achievement.Category); // Category
                packet.WriteString(string.Empty);
                packet.WriteInteger(totalLevels); // Total amount of levels 
                packet.WriteInteger(0);
            }

            packet.WriteString("");
        }
    }
}