namespace Plus.Communication.Packets.Outgoing.Misc
{
    internal class LatencyTestComposer : MessageComposer
    {
        public int TestResponse { get; }

        public LatencyTestComposer(int testResponse)
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {
            TestResponse = testResponse;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(TestResponse);
        }
    }
}