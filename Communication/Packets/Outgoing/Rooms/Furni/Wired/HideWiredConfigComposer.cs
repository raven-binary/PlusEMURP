namespace Plus.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class HideWiredConfigComposer : MessageComposer
    {
        public HideWiredConfigComposer()
            : base(ServerPacketHeader.HideWiredConfigMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
        }
    }
}