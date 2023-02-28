using System.Text;
using DotNetty.Buffers;

namespace Plus.Communication.Packets.Incoming
{
    public class ClientPacket
    {
        private readonly IByteBuffer _buffer;
        public short Id { get; }

        public ClientPacket(short id, IByteBuffer buf)
        {
            Id = id;
            _buffer = buf;
        }

        public string PopString()
        {
            int length = _buffer.ReadShort();
            IByteBuffer data = _buffer.ReadBytes(length);
            return Encoding.UTF8.GetString(data.Array);
        }

        public int PopInt()
        {
            return _buffer.ReadInt();
        }

        public bool PopBoolean()
        {
            return _buffer.ReadByte() == 1;
        }

        public int RemainingLength()
        {
            return _buffer.ReadableBytes;
        }
    }
}