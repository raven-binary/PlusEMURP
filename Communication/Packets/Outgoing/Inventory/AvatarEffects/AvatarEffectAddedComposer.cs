namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectAddedComposer : MessageComposer
    {
        public int SpriteId { get; }
        public int Duration { get; }

        public AvatarEffectAddedComposer(int spriteId, int duration)
            : base(ServerPacketHeader.AvatarEffectAddedMessageComposer)
        {
            SpriteId = spriteId;
            Duration = duration;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(SpriteId);
            packet.WriteInteger(0); //Types
            packet.WriteInteger(Duration);
            packet.WriteBoolean(false); //Permanent
        }
    }
}