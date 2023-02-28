using Plus.HabboHotel.Achievements;

namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementUnlockedComposer : MessageComposer
    {
        public Achievement Achievement { get; }
        public int Level { get; }
        public int PointReward { get; }
        public int PixelReward { get; }

        public AchievementUnlockedComposer(Achievement achievement, int level, int pointReward, int pixelReward)
            : base(ServerPacketHeader.AchievementUnlockedMessageComposer)
        {
            Achievement = achievement;
            Level = level;
            PointReward = pointReward;
            PixelReward = pixelReward;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Achievement.Id); // Achievement ID
            packet.WriteInteger(Level); // Achieved level
            packet.WriteInteger(144); // Unknown. Random useless number.
            packet.WriteString(Achievement.GroupName + Level); // Achieved name
            packet.WriteInteger(PointReward); // Point reward
            packet.WriteInteger(PixelReward); // Pixel reward
            packet.WriteInteger(0); // Unknown.
            packet.WriteInteger(10); // Unknown.
            packet.WriteInteger(21); // Unknown. (Extra reward?)
            packet.WriteString(Level > 1 ? Achievement.GroupName + (Level - 1) : string.Empty);
            packet.WriteString(Achievement.Category);
            packet.WriteBoolean(true);
        }
    }
}