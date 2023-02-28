namespace Plus.Communication.Packets.Outgoing.Inventory.Achievements
{
    internal class AchievementScoreComposer : MessageComposer
    {
        public int AchievementScore { get; }

        public AchievementScoreComposer(int achScore)
            : base(ServerPacketHeader.AchievementScoreMessageComposer)
        {
            AchievementScore = achScore;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(AchievementScore);
        }
    }
}