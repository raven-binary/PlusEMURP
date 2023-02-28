using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    internal class GroupFurniSettingsComposer : MessageComposer
    {
        public Group Group { get; }
        public int ItemId { get; }
        public int UserId { get; }

        public GroupFurniSettingsComposer(Group group, int itemId, int userId)
            : base(ServerPacketHeader.GroupFurniSettingsMessageComposer)
        {
            Group = group;
            ItemId = itemId;
            UserId = userId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(ItemId); //Item Id
            packet.WriteInteger(Group.Id); //Group Id?
            packet.WriteString(Group.Name);
            packet.WriteInteger(Group.RoomId); //RoomId
            packet.WriteBoolean(Group.IsMember(UserId)); //Member?
            packet.WriteBoolean(Group.ForumEnabled); //Has a forum
        }
    }
}