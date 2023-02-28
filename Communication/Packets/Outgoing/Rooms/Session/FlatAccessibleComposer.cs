namespace Plus.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleComposer : MessageComposer
    {
        public string Username { get; }

        public FlatAccessibleComposer(string username)
            : base(ServerPacketHeader.FlatAccessibleMessageComposer)
        {
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Username);
        }
    }
}