using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    internal class MiddleFingerCommand : IChatCommand
    {
        public string PermissionRequired => "command_alert_user";

        public string Parameters => "%username%";

        public string Description => "Raising official dissatisfacty complaint";

        public void Execute(GameClient session, Room room, string[] @params)
        {
            if (@params.Length == 1)
            {
                session.SendWhisper("Please enter the f***k*r's name.!");
                return;
            }

            GameClient targetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(@params[1]);
            if (targetClient == null)
            {
                session.SendWhisper("Cant' find that f**ker.");
                return;
            }

            if (targetClient.GetHabbo() == null)
            {
                session.SendWhisper("Cant' find that f**ker.");
                return;
            }

            /*if (targetClient.GetHabbo().Username == session.GetHabbo().Username)
            {
                session.SendWhisper("Get a life.");
                return;
            }*/

            //string message = CommandManager.MergeParams(@params, 2);

            targetClient.SendNotification(session.GetHabbo().Username + " raised their middle finger towards you in dissactifactory.!");
            session.SendWhisper("Dissactisfaction via Middle Finger raised successfully to " + targetClient.GetHabbo().Username);
        }
    }
}