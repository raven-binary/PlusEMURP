namespace Plus.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedComposer : MessageComposer
    {
        public string Username { get; }

        public FlatAccessDeniedComposer(string username)
            : base(ServerPacketHeader.FlatAccessDeniedMessageComposer)
        {
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Username);
        }
    }
}