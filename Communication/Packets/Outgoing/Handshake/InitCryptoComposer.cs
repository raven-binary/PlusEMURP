namespace Plus.Communication.Packets.Outgoing.Handshake
{
    public class InitCryptoComposer : MessageComposer
    {
        public string Prime { get; }
        public string Generator { get; }

        public InitCryptoComposer(string prime, string generator)
            : base(ServerPacketHeader.InitCryptoMessageComposer)
        {
            Prime = prime;
            Generator = generator;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Prime);
            packet.WriteString(Generator);
        }
    }
}