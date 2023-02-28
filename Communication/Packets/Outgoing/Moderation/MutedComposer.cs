using System;

namespace Plus.Communication.Packets.Outgoing.Moderation
{
    internal class MutedComposer : MessageComposer
    {
        public double TimeMuted { get; }

        public MutedComposer(double timeMuted)
            : base(ServerPacketHeader.MutedMessageComposer)
        {
            TimeMuted = timeMuted;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Convert.ToInt32(TimeMuted));
        }
    }
}