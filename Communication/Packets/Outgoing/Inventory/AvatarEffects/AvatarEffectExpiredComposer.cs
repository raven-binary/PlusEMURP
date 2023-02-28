using Plus.HabboHotel.Users.Effects;

namespace Plus.Communication.Packets.Outgoing.Inventory.AvatarEffects
{
    internal class AvatarEffectExpiredComposer : MessageComposer
    {
        public AvatarEffect Effect { get; }

        public AvatarEffectExpiredComposer(AvatarEffect effect)
            : base(ServerPacketHeader.AvatarEffectExpiredMessageComposer)
        {
            Effect = effect;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(Effect.SpriteId);
        }
    }
}