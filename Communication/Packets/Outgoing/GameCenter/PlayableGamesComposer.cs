namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    internal class PlayableGamesComposer : MessageComposer
    {
        public int GameId { get; }

        public PlayableGamesComposer(int gameId)
            : base(ServerPacketHeader.PlayableGamesMessageComposer)
        {
            GameId = gameId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(0);
        }
    }
}