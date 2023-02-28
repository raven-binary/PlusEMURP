namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    internal class AvatarEffectComposer : MessageComposer
    {
        public int PlayerId { get; }
        public int EffectId { get; }

        public AvatarEffectComposer(int playerId, int effectId)
            : base(ServerPacketHeader.AvatarEffectMessageComposer)
        {
            PlayerId = playerId;
            EffectId = effectId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(PlayerId);
            packet.WriteInteger(EffectId);
            packet.WriteInteger(0);
        }
    }
}