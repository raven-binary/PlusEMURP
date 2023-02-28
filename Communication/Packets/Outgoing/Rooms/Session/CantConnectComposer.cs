namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    internal class CantConnectComposer : MessageComposer
    {
        public int Error { get; }

        public CantConnectComposer(int error)
            : base(ServerPacketHeader.CantConnectMessageComposer)
        {
            Error = error;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error);
        }
    }
}