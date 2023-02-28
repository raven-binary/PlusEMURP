namespace Plus.Communication.Packets.Outgoing.Rooms.Avatar
{
    public class ActionComposer : MessageComposer
    {
        public int VirtualId { get; }
        public int Action { get; }

        public ActionComposer(int virtualId, int action)
            : base(ServerPacketHeader.ActionMessageComposer)
        {
            VirtualId = virtualId;
            Action = action;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(VirtualId);
            packet.WriteInteger(Action);
        }
    }
}