using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni
{
    internal class ThrowDiceEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            Room room = session.GetHabbo().CurrentRoom;

            Item item = room?.GetRoomItemHandler().GetItem(packet.PopInt());
            if (item == null)
                return;

            bool hasRights = room.CheckRights(session, false, true);

            int request = packet.PopInt();

            item.Interactor.OnTrigger(session, item, request, hasRights);
        }
    }
}