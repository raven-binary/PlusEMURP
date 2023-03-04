using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class GiveBadgeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().Rank == 7)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<message>"; }
        }

        public string Description
        {
            get { return "Send a hotel alert"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length != 3)
            {
                Session.SendWhisper("Invalid syntax :givebadge <username> <code>");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], true, TargetClient);
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                        TargetClient.SendWhisper("You just received a badge!");

                    else
                        Session.SendWhisper("You have successfully submitted the " + Params[2] + " " + Params[1] + " badge!");

                }
                else
                    Session.SendWhisper(Params[1] + " already has this badge!");
                return;
            }
            else
            {
                Session.SendWhisper(Params[1] + " could not be found.");
                return;
            }
        }
    }
}
