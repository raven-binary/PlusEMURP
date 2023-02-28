using System.Collections.Generic;
using System.Linq;
using Plus.Communication.Packets.Outgoing.Catalog;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;
using Plus.Utilities;

namespace Plus.Communication.Packets.Incoming.Catalog
{
    internal class GetPromotableRoomsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            List<RoomData> rooms = RoomFactory.GetRoomsDataByOwnerSortByName(session.GetHabbo().Id);

            rooms = rooms.Where(x => x.Promotion == null || x.Promotion.TimestampExpires < UnixTimestamp.GetNow()).ToList();

            session.SendPacket(new PromotableRoomsComposer(rooms));
        }
    }
}