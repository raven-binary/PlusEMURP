using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.Communication.Packets.Incoming.Moderation
{
    internal class ModeratorActionEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().GetPermissions().HasRight("mod_caution"))
                return;

            if (!session.GetHabbo().InRoom)
                return;

            Room currentRoom = session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;

            int alertMode = packet.PopInt();
            string alertMessage = packet.PopString();
            bool isCaution = alertMode != 3;

            alertMessage = isCaution ? "Caution from Moderator:\n\n" + alertMessage : "Message from Moderator:\n\n" + alertMessage;
            session.GetHabbo().CurrentRoom.SendPacket(new BroadcastMessageAlertComposer(alertMessage));
        }
    }
}