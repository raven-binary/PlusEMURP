using DotNetty.Buffers;

namespace Plus.Communication.Packets.Outgoing
{
    public abstract class MessageComposer
    {
        protected short Id { get; }

        protected MessageComposer(short id)
        {
            Id = id;
        }

        public ServerPacket WriteMessage(IByteBuffer buf)
        {
            ServerPacket packet = new(Id, buf);
            try
            {
                Compose(packet);
            }
            finally
            {
                Dispose();
            }

            return packet;
        }

        public abstract void Compose(ServerPacket packet);

        public void Dispose()
        {
        }
    }
}