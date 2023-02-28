using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.GameCenter;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Games;

namespace Plus.Communication.Packets.Incoming.GameCenter
{
    internal class GetGameListingEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            ICollection<GameData> games = PlusEnvironment.GetGame().GetGameDataManager().GameData;

            session.SendPacket(new GameListComposer(games));
        }
    }
}