namespace Plus.Communication.Packets.Outgoing.LandingView
{
    internal class ConcurrentUsersGoalProgressComposer : MessageComposer
    {
        public int UsersNow { get; }

        public ConcurrentUsersGoalProgressComposer(int usersNow)
            : base(ServerPacketHeader.ConcurrentUsersGoalProgressMessageComposer)
        {
            UsersNow = usersNow;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(0); //0/1 = Not done, 2 = Done & can claim, 3 = claimed.
            packet.WriteInteger(UsersNow);
            packet.WriteInteger(1000);
        }
    }
}