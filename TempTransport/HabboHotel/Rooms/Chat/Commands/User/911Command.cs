using System;
using Plus.HabboHotel.GameClients;
using Plus.Database.Interfaces;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class CallPoliceCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return "<message>"; }
        }

        public string Description
        {
            get { return "Calls the police for help"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().Hospital == 1 || Session.GetRoleplay().Dead)
                return;

            if (PlusEnvironment.getCooldown("911" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before trying to call 911 again");
                return;
            }

            if (Session.GetHabbo().callAbuse > 0)
            {
                Session.SendWhisper("You cannot call 911");
                return;
            }

            if (!PlusEnvironment.GetGame().GetClientManager().PoliceOnDuty(Session.GetHabbo().Id))
            {
                Session.SendWhisper("There is no on-duty cops");
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specify a message");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);

            User.Say("calls the police for help");
            PlusEnvironment.addCooldown("911" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(60)));
            PlusEnvironment.GetGame().GetClientManager().PoliceCall(Session.GetHabbo().Id ,Session.GetHabbo().Username, Session.GetHabbo().Look, Session.GetHabbo().CurrentRoom.Name, Session.GetHabbo().CurrentRoomId, Message, 10); 
        }
    }
}
