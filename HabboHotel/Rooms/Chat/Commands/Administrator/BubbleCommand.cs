using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms.Chat.Styles;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Administrator
{
    internal class BubbleCommand : IChatCommand
    {
        public string PermissionRequired => "command_bubble";

        public string Parameters => "%id%";

        public string Description => "Use a custom bubble to chat with.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            if (@params.Length == 1)
            {
                session.SendWhisper("Oops, you forgot to enter a bubble ID!");
                return;
            }

            if (!int.TryParse(@params[1], out int bubble))
            {
                session.SendWhisper("Please enter a valid number.");
                return;
            }

            if (!PlusEnvironment.GetGame().GetChatManager().GetChatStyles().TryGetStyle(bubble, out ChatStyle style) || (style.RequiredRight.Length > 0 && !session.GetHabbo().GetPermissions().HasRight(style.RequiredRight)))
            {
                session.SendWhisper("Oops, you cannot use this bubble due to a rank requirement, sorry!");
                return;
            }

            user.LastBubble = bubble;
            session.GetHabbo().CustomBubbleId = bubble;
            session.SendWhisper("Bubble set to: " + bubble);
        }
    }
}