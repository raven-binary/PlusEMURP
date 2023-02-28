namespace Plus.Communication.Packets.Outgoing.Catalog
{
    internal class PresentDeliverErrorMessageComposer : MessageComposer
    {
        public bool CreditError { get; }
        public bool DucketError { get; }

        public PresentDeliverErrorMessageComposer(bool creditError, bool ducketError)
            : base(ServerPacketHeader.PresentDeliverErrorMessageComposer)
        {
            CreditError = creditError;
            DucketError = ducketError;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteBoolean(CreditError); //Do we have enough credits?
            packet.WriteBoolean(DucketError); //Do we have enough duckets?
        }
    }
}