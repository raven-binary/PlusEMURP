using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Handshake
{
    public class GetClientVersionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            string build = packet.PopString();

            if (PlusEnvironment.ClientRevision != build)
                PlusEnvironment.ClientRevision = build;
        }
    }
}