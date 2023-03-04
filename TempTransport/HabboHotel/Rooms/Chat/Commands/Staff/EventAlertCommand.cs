using Plus.Communication.Packets.Outgoing.Rooms.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Rooms;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class EventAlertCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8)
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
            get { return "Sends a event alert"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            Room = Session.GetHabbo().CurrentRoom;
            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session != null)
            {
                if (Room != null)
                {
                    if (Params.Length == 1)
                    {
                        Session.SendWhisper("Type the name of the event");
                        return;
                    }
                    else
                    {
                        string Message = CommandManager.MergeParams(Params, 1);

                        PlusEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer("An event is starting!",
                        "<b><font size='17px' color='#E74C3C'>Hey, this is your chance!<br></font></b>\n" +
                        "The event <b> " + Message + "</b> Message <b>" + Session.GetHabbo().Username + "</b> is currently taking place at\n\n" +
                        "" + PlusEnvironment.Hotelname + " Events help ensure that you never get bored in the city. You can win coins, points or rares at " + PlusEnvironment.Hotelname + " What are you waiting for?",
                        "event_notification", "Join the event now!", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                    }
                }
            }
        }
    }
}