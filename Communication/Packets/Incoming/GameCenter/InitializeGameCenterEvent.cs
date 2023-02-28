using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    internal class InitializeGameCenterEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
        }
    }
}