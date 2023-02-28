namespace Plus.Communication.Packets.Outgoing.Catalog
{
    public class VoucherRedeemErrorComposer : MessageComposer
    {
        public int Type { get; }

        public VoucherRedeemErrorComposer(int type)
            : base(ServerPacketHeader.VoucherRedeemErrorMessageComposer)
        {
            Type = type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(Type.ToString());
        }
    }
}