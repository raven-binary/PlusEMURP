using Plus.HabboHotel.Users.Messenger;

namespace Plus.Communication.Packets.Outgoing.Messenger
{
    internal class InstantMessageErrorComposer : MessageComposer
    {
        public MessengerMessageErrors Error { get; }
        public int Target { get; }

        public InstantMessageErrorComposer(MessengerMessageErrors error, int target)
            : base(ServerPacketHeader.InstantMessageErrorMessageComposer)
        {
            Error = error;
            Target = target;
        }

        public override void Compose(ServerPacket packet)
        {
            packet.WriteInteger(MessengerMessageErrorsUtility.GetMessageErrorPacketNum(Error));
            packet.WriteInteger(Target);
            packet.WriteString("");
        }
    }
}