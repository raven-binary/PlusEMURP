using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Help
{
    internal class GetSanctionStatusEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            //Session.SendMessage(new SanctionStatusComposer());
        }
    }
}