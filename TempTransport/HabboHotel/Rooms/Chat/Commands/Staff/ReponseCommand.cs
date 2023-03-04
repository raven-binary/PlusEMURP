using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class ReponseCommand : IChatCommand
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
            get { return "<reponse>"; }
        }

        public string Description
        {
            get { return "Define the answer to a quiz question"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if(PlusEnvironment.Quizz == 0)
            {
                Session.SendWhisper("No quiz is started.");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            if (Message == null || Message == "")
            {
                Session.SendWhisper("Please enter an answer.");
                return;
            }

            PlusEnvironment.QuizzReponse = Message.ToLower();
            Session.SendWhisper("Answer defined as \"" + Message + "\".");
        }
    }
}