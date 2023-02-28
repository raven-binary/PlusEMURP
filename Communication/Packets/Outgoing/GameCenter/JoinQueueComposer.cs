namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    internal class JoinQueueComposer : MessageComposer
    {
        public int GameId { get; }

        public JoinQueueComposer(int gameId)
            : base(ServerPacketHeader.JoinQueueMessageComposer)
        {
            GameId = gameId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
        }
    }
}