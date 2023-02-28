using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator.Fun
{
    internal class CoordsCommand : IChatCommand
    {
        public string PermissionRequired => "command_coords";

        public string Parameters => "";

        public string Description => "Used to get your current position within the room.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            RoomUser thisUser = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (thisUser == null)
                return;

            session.SendNotification("X: " + thisUser.X + "\n - Y: " + thisUser.Y + "\n - Z: " + thisUser.Z + "\n - Rot: " + thisUser.RotBody + ", sqState: " + room.GetGameMap().GameMap[thisUser.X, thisUser.Y] + "\n\n - RoomID: " + session.GetHabbo().CurrentRoomId);
        }
    }
}