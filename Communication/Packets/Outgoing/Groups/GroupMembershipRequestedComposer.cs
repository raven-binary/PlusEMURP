using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    internal class GroupMembershipRequestedComposer : MessageComposer
    {
        public int GroupId { get; }
        public Habbo Habbo { get; }
        public int Type { get; }

        public GroupMembershipRequestedComposer(int groupId, Habbo habbo, int type)
            : base(ServerPacketHeader.GroupMembershipRequestedMessageComposer)
        {
            GroupId = groupId;
            Habbo = habbo;
            Type = type;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(GroupId); //GroupId
            packet.WriteInteger(Type); //Type?
            {
                packet.WriteInteger(Habbo.Id); //UserId
                packet.WriteString(Habbo.Username);
                packet.WriteString(Habbo.Look);
                packet.WriteString(string.Empty);
            }
        }
    }
}