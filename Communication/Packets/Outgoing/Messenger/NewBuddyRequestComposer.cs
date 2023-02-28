using Plus.HabboHotel.Cache.Type;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class NewBuddyRequestComposer : MessageComposer
    {
        public UserCache UserCache { get; }

        public NewBuddyRequestComposer(UserCache habbo)
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {
            UserCache = habbo;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(UserCache.Id);
            packet.WriteString(UserCache.Username);
            packet.WriteString(UserCache.Look);
        }
    }
}