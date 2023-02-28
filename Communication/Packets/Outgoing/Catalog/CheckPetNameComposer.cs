namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class CheckPetNameComposer : MessageComposer
    {
        public int Error { get; }
        public string ExtraData { get; }

        public CheckPetNameComposer(int error, string extraData)
            : base(ServerPacketHeader.CheckPetNameMessageComposer)
        {
            Error = error;
            ExtraData = extraData;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Error); //0 = nothing, 1 = too long, 2 = too short, 3 = invalid characters
            packet.WriteString(ExtraData);
        }
    }
}