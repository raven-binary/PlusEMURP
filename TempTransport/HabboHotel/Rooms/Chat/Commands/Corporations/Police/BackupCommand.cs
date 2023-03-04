using System;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class BackupCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Requests a backup"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            if (User == null)
                return;

            if (PlusEnvironment.getCooldown("backup" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before request a backup again");
                return;
            }

            PlusEnvironment.addCooldown("backup" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertMinutesToMilliseconds(15)));
            PlusEnvironment.GetGame().GetClientManager().PoliceCall(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().Look, Session.GetHabbo().CurrentRoom.Name, Session.GetHabbo().CurrentRoomId, "is requesting immediate backup", 20);
            PlusEnvironment.GetGame().GetClientManager().PoliceRadio(Session.GetHabbo().Username + " is requesting backup at [" + Session.GetHabbo().CurrentRoomId + "] " + Session.GetHabbo().CurrentRoom.Name, 34);
        }
    }
}