namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class AvatarAspectUpdateComposer : MessageComposer
    {
        public string Figure { get; }
        public string Gender { get; }

        public AvatarAspectUpdateComposer(string figure, string gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            Figure = figure;
            Gender = gender;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Figure);
            packet.WriteString(Gender);
        }
    }
}