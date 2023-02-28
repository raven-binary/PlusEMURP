namespace Plus.Communication.Packets.Outgoing.Navigator
{
    internal class RoomRatingComposer : MessageComposer
    {
        public int Score { get; }
        public bool CanVote { get; }

        public RoomRatingComposer(int score, bool canVote)
            : base(ServerPacketHeader.RoomRatingMessageComposer)
        {
            Score = score;
            CanVote = canVote;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Score);
            packet.WriteBoolean(CanVote);
        }
    }
}