using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.Furni.Moodlight
{
    internal class MoodlightUpdateEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!PlusEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(session, true) || room.MoodlightData == null)
                return;

            Item item = room.GetRoomItemHandler().GetItem(room.MoodlightData.ItemId);
            if (item == null || item.GetBaseItem().InteractionType != InteractionType.Moodlight)
                return;

            int preset = packet.PopInt();
            int backgroundMode = packet.PopInt();
            string colorCode = packet.PopString();
            int intensity = packet.PopInt();

            bool backgroundOnly = backgroundMode >= 2;

            room.MoodlightData.Enabled = true;
            room.MoodlightData.CurrentPreset = preset;
            room.MoodlightData.UpdatePreset(preset, colorCode, intensity, backgroundOnly);

            item.ExtraData = room.MoodlightData.GenerateExtraData();
            item.UpdateState();
        }
    }
}