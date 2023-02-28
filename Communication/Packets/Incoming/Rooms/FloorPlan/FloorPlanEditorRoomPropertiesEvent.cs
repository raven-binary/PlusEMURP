using System.Collections.Generic;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Items;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Rooms.FloorPlan
{
    internal class FloorPlanEditorRoomPropertiesEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;

            DynamicRoomModel model = room?.GetGameMap().Model;
            if (model == null)
                return;

            ICollection<Item> floorItems = room.GetRoomItemHandler().GetFloor;

            session.SendPacket(new FloorPlanFloorMapComposer(floorItems));
            session.SendPacket(new FloorPlanSendDoorComposer(model.DoorX, model.DoorY, model.DoorOrientation));
            session.SendPacket(new RoomVisualizationSettingsComposer(room.WallThickness, room.FloorThickness, PlusEnvironment.EnumToBool(room.HideWall.ToString())));
        }
    }
}