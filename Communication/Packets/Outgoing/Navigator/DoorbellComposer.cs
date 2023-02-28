namespace Plus.Communication.Packets.Outgoing.Navigator
{
    internal class DoorbellComposer : MessageComposer
    {
        public string Username { get; }

        public DoorbellComposer(string username)
            : base(ServerPacketHeader.DoorbellMessageComposer)
        {
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Username);
        }
    }
}