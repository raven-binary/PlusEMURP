using Plus.Communication.Packets.Outgoing.GameCenter;
using Plus.HabboHotel.GameClients;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    internal class GetPlayableGamesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int gameId = packet.PopInt();

            session.SendPacket(new GameAccountStatusComposer(gameId));
            session.SendPacket(new PlayableGamesComposer(gameId));
            session.SendPacket(new GameAchievementListComposer(session, PlusEnvironment.GetGame().GetAchievementManager().GetGameAchievements(gameId), gameId));
        }
    }
}