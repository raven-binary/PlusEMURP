using Plus.Communication.Packets.Outgoing.Groups;
using Plus.Communication.Packets.Outgoing.Rooms.Permissions;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Groups;
using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Users;

namespace Plus.Communication.Packets.Incoming.Groups
{
    internal class TakeAdminRightsEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int groupId = packet.PopInt();
            int userId = packet.PopInt();

            if (!PlusEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out Group group))
                return;

            if (session.GetHabbo().Id != group.CreatorId || !group.IsMember(userId))
                return;

            Habbo habbo = PlusEnvironment.GetHabboById(userId);
            if (habbo == null)
            {
                session.SendNotification("Oops, an error occurred whilst finding this user.");
                return;
            }

            group.TakeAdmin(userId);

            if (PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(group.RoomId, out Room room))
            {
                RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(userId);
                if (user != null)
                {
                    RoomRightLevels level = group.getGroupRightLevel(habbo);

                    user.SetStatus("flatctrl", ((int)level).ToString());

                    user.UpdateNeeded = true;

                    if (user.GetClient() != null)
                        user.GetClient().SendPacket(new YouAreControllerComposer(level));
                }
            }

            session.SendPacket(new GroupMemberUpdatedComposer(groupId, habbo, 2));
        }
    }
}