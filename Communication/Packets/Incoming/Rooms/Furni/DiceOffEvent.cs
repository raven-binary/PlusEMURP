using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    internal class DiceOffEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;

            Item item = room?.GetRoomItemHandler().GetItem(packet.PopInt());
            if (item == null)
                return;

            bool hasRights = room.CheckRights(session);

            item.Interactor.OnTrigger(session, item, -1, hasRights);
        }
    }
}