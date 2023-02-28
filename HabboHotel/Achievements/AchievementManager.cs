﻿using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Plus.Communication.Packets.Outgoing.Inventory.Achievements;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.Database.Interfaces;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Users.Messenger;

namespace Plus.HabboHotel.Achievements
{
    public class AchievementManager
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AchievementManager));

        public Dictionary<string, Achievement> Achievements;

        public AchievementManager()
        {
            Achievements = new Dictionary<string, Achievement>();
        }

        public void Init()
        {
            AchievementLevelFactory.GetAchievementLevels(out Achievements);
        }

        public bool ProgressAchievement(GameClient session, string group, int progress, bool fromBeginning = false)
        {
            if (!Achievements.ContainsKey(group) || session == null)
                return false;

            Achievement data = Achievements[group];
            if (data == null)
            {
                return false;
            }

            UserAchievement userData = session.GetHabbo().GetAchievementData(group);
            if (userData == null)
            {
                userData = new UserAchievement(group, 0, 0);
                session.GetHabbo().Achievements.TryAdd(group, userData);
            }

            int totalLevels = data.Levels.Count;

            if (userData.Level == totalLevels)
                return false; // done, no more.

            int targetLevel = userData.Level + 1;

            if (targetLevel > totalLevels)
                targetLevel = totalLevels;

            AchievementLevel level = data.Levels[targetLevel];
            int newProgress;
            if (fromBeginning)
                newProgress = progress;
            else
                newProgress = userData.Progress + progress;

            int newLevel = userData.Level;
            int newTarget = newLevel + 1;

            if (newTarget > totalLevels)
                newTarget = totalLevels;

            if (newProgress >= level.Requirement)
            {
                newLevel++;
                newTarget++;
                newProgress = 0;

                if (targetLevel == 1)
                    session.GetHabbo().GetBadgeComponent().GiveBadge(group + targetLevel, true, session);
                else
                {
                    session.GetHabbo().GetBadgeComponent().RemoveBadge(Convert.ToString(group + (targetLevel - 1)));
                    session.GetHabbo().GetBadgeComponent().GiveBadge(group + targetLevel, true, session);
                }

                if (newTarget > totalLevels)
                {
                    newTarget = totalLevels;
                }

                session.SendPacket(new AchievementUnlockedComposer(data, targetLevel, level.RewardPoints, level.RewardPixels));
                session.GetHabbo().GetMessenger().BroadcastAchievement(session.GetHabbo().Id, MessengerEventTypes.AchievementUnlocked, group + targetLevel);

                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES (@userId, @group, @newLevel, @newProgress)");
                    dbClient.AddParameter("userId", session.GetHabbo().Id);
                    dbClient.AddParameter("group", group);
                    dbClient.AddParameter("newLevel", newLevel);
                    dbClient.AddParameter("newProgress", newProgress);
                    dbClient.RunQuery();
                }

                userData.Level = newLevel;
                userData.Progress = newProgress;

                session.GetHabbo().Duckets += level.RewardPixels;
                session.GetHabbo().GetStats().AchievementPoints += level.RewardPoints;
                session.SendPacket(new HabboActivityPointNotificationComposer(session.GetHabbo().Duckets, level.RewardPixels));
                session.SendPacket(new AchievementScoreComposer(session.GetHabbo().GetStats().AchievementPoints));

                AchievementLevel newLevelData = data.Levels[newTarget];
                session.SendPacket(new AchievementProgressedComposer(data, newTarget, newLevelData, totalLevels, session.GetHabbo().GetAchievementData(group)));

                return true;
            }

            userData.Level = newLevel;
            userData.Progress = newProgress;
            using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO `user_achievements` VALUES (@userId, @group, @newLevel, @newProgress)");
                dbClient.AddParameter("userId", session.GetHabbo().Id);
                dbClient.AddParameter("group", group);
                dbClient.AddParameter("newLevel", newLevel);
                dbClient.AddParameter("newProgress", newProgress);
                dbClient.RunQuery();
            }

            session.SendPacket(new AchievementProgressedComposer(data, targetLevel, level, totalLevels, session.GetHabbo().GetAchievementData(group)));
            return false;
        }

        public ICollection<Achievement> GetGameAchievements(int gameId)
        {
            List<Achievement> achievements = new();

            foreach (Achievement achievement in Achievements.Values.ToList())
            {
                if (achievement.Category == "games" && achievement.GameId == gameId)
                    achievements.Add(achievement);
            }

            return achievements;
        }
    }
}