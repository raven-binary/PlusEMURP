using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    internal class LayCommand : IChatCommand
    {
        public string PermissionRequired => "command_lay";

        public string Parameters => "";

        public string Description => "Allows you to lay down in the room, without needing a bed.";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabbo(session.GetHabbo().Id);
            if (user == null)
                return;

            if (!room.GetGameMap().ValidTile(user.X + 2, user.Y + 2) && !room.GetGameMap().ValidTile(user.X + 1, user.Y + 1))
            {
                session.SendWhisper("Oops, cannot lay down here - try elsewhere!");
                return;
            }

            if (user.HasStatus("sit") || user.IsSitting || user.RidingHorse || user.IsWalking)
                return;

            if (session.GetHabbo().Effects().CurrentEffect > 0)
                session.GetHabbo().Effects().ApplyEffect(0);

            if (!user.HasStatus("lay"))
            {
                if (user.RotBody % 2 == 0)
                {
                    try
                    {
                        user.Statusses.Add("lay", "1.0 null");
                        user.Z -= 0.35;
                        user.IsLying = true;
                        user.UpdateNeeded = true;
                    }
                    catch
                    {
                    }
                }
                else
                {
                    user.RotBody--;
                    user.Statusses.Add("lay", "1.0 null");
                    user.Z -= 0.35;
                    user.IsLying = true;
                    user.UpdateNeeded = true;
                }
            }
            else
            {
                user.Z += 0.35;
                user.Statusses.Remove("lay");
                user.Statusses.Remove("1.0");
                user.IsLying = false;
                user.UpdateNeeded = true;
            }
        }
    }
}