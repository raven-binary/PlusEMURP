using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    internal class GetRoomSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int roomId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetRoomManager().TryLoadRoom(roomId, out Room room))
                return;

            if (!room.CheckRights(session, true))
                return;

            session.SendPacket(new RoomSettingsDataComposer(room));
        }
    }
}