using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User.Fun
{
    internal class MoonwalkCommand : IChatCommand
    {
        public string PermissionRequired => "command_moonwalk";

        public string Parameters => "";

        public string Description => "Wear the shoes of Michael Jackson.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            user.MoonwalkEnabled = !user.MoonwalkEnabled;

            if (user.MoonwalkEnabled)
                session.SendWhisper("Moonwalk enabled!");
            else
                session.SendWhisper("Moonwalk disabled!");
        }
    }
}