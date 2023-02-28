using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class FriendNotificationComposer : MessageComposer
    {
        public int UserId { get; }
        public MessengerEventTypes Type { get; }
        public string Data { get; }

        public FriendNotificationComposer(int userId, MessengerEventTypes type, string data)
            : base(ServerPacketHeader.FriendNotificationMessageComposer)
        {
            UserId = userId;
            Type = type;
            Data = data;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteString(UserId.ToString());
            packet.WriteInteger(MessengerEventTypesUtility.GetEventTypePacketNum(Type));
            packet.WriteString(Data);
        }
    }
}