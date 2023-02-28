namespace Plus.Communication.Packets.Outgoing.Rooms.Action
{
    internal class IgnoreStatusComposer : MessageComposer
    {
        public int Status { get; }
        public string Username { get; }

        public IgnoreStatusComposer(int status, string username)
            : base(ServerPacketHeader.IgnoreStatusMessageComposer)
        {
            Status = status;
            Username = username;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Status);
            packet.WriteString(Username);
        }
    }
}