using Plus.HabboHotel.Groups;

namespace Plus.Communication.Packets.Outgoing.Groups
{
    internal class UpdateFavouriteGroupComposer : MessageComposer
    {
        public Group Group { get; }
        public int VirtualId { get; }

        public UpdateFavouriteGroupComposer(Group group, int virtualId)
            : base(ServerPacketHeader.UpdateFavouriteGroupMessageComposer)
        {
            Group = group;
            VirtualId = virtualId;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId); //Sends 0 on .COM
            packet.WriteInteger(Group != null ? Group.Id : 0);
            packet.WriteInteger(3);
            packet.WriteString(Group != null ? Group.Name : string.Empty);
        }
    }
}