using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class QuestionCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().JobId == 18 && Session.GetHabbo().RankId == 2)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<question>"; }
        }

        public string Description
        {
            get { return "Start a quiz question"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (PlusEnvironment.Quizz == 0)
            {
                Session.SendWhisper("No quiz will be organized.");
                return;
            }

            if (PlusEnvironment.QuizzReponse == null)
            {
                Session.SendWhisper("You cannot start a question because you have not defined an answer.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            if (Message == null || Message == "")
            {
                Session.SendWhisper("Please enter a question.");
                return;
            }

            if(Message.Contains(";"))
            {
                Session.SendWhisper("Your question must not contain the character ';'");
                return;
            }

            byte[] encodeToUtf8 = Encoding.Default.GetBytes(Message);
            Message = Encoding.UTF8.GetString(encodeToUtf8);

            foreach (RoomUser UserInRoom in Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetUserList().ToList())
            {
                if (UserInRoom == null || UserInRoom.IsBot || UserInRoom.GetClient() == null || UserInRoom.GetClient().GetHabbo() == null)
                    continue;

                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(UserInRoom.GetClient(), "quizz;newQuestion;" + Message);
            }

        }
    }
}