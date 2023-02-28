using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Inventory.AvatarEffects
{
    internal class AvatarEffectSelectedEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            int effectId = packet.PopInt();
            if (effectId < 0)
                effectId = 0;

            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;

            RoomUser user = room?.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            if (effectId != 0 && session.GetHabbo().Effects().HasEffect(effectId, true))
                user.ApplyEffect(effectId);
        }
    }
}