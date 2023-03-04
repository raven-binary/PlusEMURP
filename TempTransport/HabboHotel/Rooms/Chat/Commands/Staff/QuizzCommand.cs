using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class QuizzCommand : IChatCommand
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
            get { return ""; }
        }

        public string Description
        {
            get { return "Starts a quiz in the current room"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (PlusEnvironment.Quizz != 0)
            {
                Session.SendWhisper("A quiz has already started");
                return;
            }

            PlusEnvironment.Quizz = Session.GetHabbo().CurrentRoomId;
            foreach (RoomUser UserInRoom in Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                    continue;

                //PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "quizz;refreshClassement;" + UserInRoom.GetClient().GetHabbo().Quizz_Points);
            }

            //PlusEnvironment.GetGame().GetClientManager().HotelAlert("A Quiz Event is staring in " + Session.GetHabbo().CurrentRoom.Name);
        }
    }
}