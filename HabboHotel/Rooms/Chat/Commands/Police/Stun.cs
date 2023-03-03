using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using System.Collections.Generic;
using System.Linq;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Police
{
    internal class Stun : IChatCommand
    {
        public string PermissionRequired => "command_freeze";

        public string Parameters => "%username%";

        public string Description => "Stun a suspect.!";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (@params.Length == 1)
            {
                session.SendWhisper("Who to stun?");
                return;
            }

            GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(@params[1]);
            if (targetClient == null)
            {
                session.SendWhisper("An error occoured whilst finding that suspect, maybe they're not online.");
                return;
            }

            RoomUser targetUser = session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(@params[1]);
            if (targetUser != null && targetClient != null)
		room.SendPacket(new ChatComposer(session.GetHabbo().Username, "Fires a stungun at " + targetClient.GetHabbo().Username, 0, targetUser.LastBubble));
		targetUser.ApplyEffect(enableId);
		targetUser.Frozen = true;
		targetClient.SendWhisper(session.GetHabbo().Username + " has stunned you.!");

            session.SendWhisper("Successfully stunned " + targetClient.GetHabbo().Username + "!");
        }
    }
}
// add
