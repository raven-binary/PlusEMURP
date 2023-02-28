using Plus.Communication.Packets.Outgoing.Rooms.Settings;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Settings
{
    internal class GetRoomBannedUsersEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room instance = session.GetHabbo().CurrentRoom;
            if (instance == null || !instance.CheckRights(session, true))
                return;

            if (instance.GetBans().BannedUsers().Count > 0)
                session.SendPacket(new GetRoomBannedUsersComposer(instance.Id, instance.GetBans().BannedUsers()));
        }
    }
}