namespace Plus.Communication.Packets.Outgoing.Handshake
{
    internal class SetUniqueIdComposer : MessageComposer
    {
        public string UniqueId { get; }

        public SetUniqueIdComposer(string id)
            : base(ServerPacketHeader.SetUniqueIdMessageComposer)
        {
            UniqueId = id;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(UniqueId);
        }
    }
}