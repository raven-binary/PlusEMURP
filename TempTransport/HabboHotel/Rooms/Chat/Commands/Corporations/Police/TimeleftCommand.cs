using System;
using System.Linq;
using System.Text;
using System.Data;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class TimeleftCommand : IChatCommand
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
            get { return "<timeleft> <username>"; }
        }

        public string Description
        {
            get { return "Shows the charges of a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (!TargetClient.GetRoleplay().Prison)
            {
                Session.SendWhisper("This player is not arrested");
                return;
            }

            Room.SendMessage(new ChatComposer(Session.GetRoleplay().roomUser.VirtualId, TargetClient.GetHabbo().Username + ", you have " + TargetClient.GetRoleplay().Timer + " minutes left", 0, Session.GetRoleplay().roomUser.LastBubble));
        }
    }
}