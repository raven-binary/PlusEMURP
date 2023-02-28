namespace Plus.Communication.Packets.Outgoing.Catalog
{
    internal class CheckGnomeNameComposer : MessageComposer
    {
        public string PetName { get; }
        public int ErrorId { get; }

        public CheckGnomeNameComposer(string petName, int errorId)
            : base(ServerPacketHeader.CheckGnomeNameMessageComposer)
        {
            PetName = petName;
            ErrorId = errorId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0);
            packet.WriteInteger(ErrorId);
            packet.WriteString(PetName);
        }
    }
}