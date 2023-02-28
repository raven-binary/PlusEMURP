using Plus.Communication.Packets.Outgoing.LandingView;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.Quests
{
    internal class GetDailyQuestEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int usersOnline = PlusEnvironment.GetGame().GetClientManager().Count;

            session.SendPacket(new ConcurrentUsersGoalProgressComposer(usersOnline));
        }
    }
}