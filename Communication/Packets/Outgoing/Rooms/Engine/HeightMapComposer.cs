using System;

namespace Plus.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class HeightMapComposer : MessageComposer
    {
        public string Map { get; }

        public HeightMapComposer(string map)
            : base(ServerPacketHeader.HeightMapMessageComposer)
        {
            Map = map.Replace("\n", "");
        }

        public override void Compose(ServerPacket packet)
        {
            string[] split = Map.Split('\r');
            packet.WriteInteger(split[0].Length);
            packet.WriteInteger((split.Length - 1) * split[0].Length);
            int x = 0;
            int y = 0;
            for (y = 0; y < split.Length - 1; y++)
            {
                for (x = 0; x < split[0].Length; x++)
                {
                    char pos;

                    try
                    {
                        pos = split[y][x];
                    }
                    catch
                    {
                        pos = 'x';
                    }

                    if (pos == 'x')
                        packet.WriteShort(-1);
                    else
                    {
                        int height = 0;
                        if (int.TryParse(pos.ToString(), out height))
                        {
                            height = height * 256;
                        }
                        else
                        {
                            height = ((Convert.ToInt32(pos) - 87) * 256);
                        }

                        packet.WriteShort(height);
                    }
                }
            }
        }
    }
}