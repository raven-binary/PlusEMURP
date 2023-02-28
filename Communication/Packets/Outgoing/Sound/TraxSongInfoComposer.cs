namespace Plus.Communication.Packets.Outgoing.Sound
{
    internal class TraxSongInfoComposer : MessageComposer
    {
        public TraxSongInfoComposer()
            : base(ServerPacketHeader.TraxSongInfoMessageComposer)
        {
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0); //Count
        }
    }
}