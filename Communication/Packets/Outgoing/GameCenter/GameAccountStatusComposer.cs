namespace Plus.Communication.Packets.Outgoing.GameCenter
{
    internal class GameAccountStatusComposer : MessageComposer
    {
        public int GameId { get; }

        public GameAccountStatusComposer(int gameId)
            : base(ServerPacketHeader.GameAccountStatusMessageComposer)
        {
            GameId = gameId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GameId);
            packet.WriteInteger(-1); // Games Left
            packet.WriteInteger(0); //Was 16?
        }
    }
}