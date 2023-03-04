using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SendhomeCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().RankId == 1 | Session.GetHabbo().RankId == 2 | Session.GetHabbo().RankId == 3 && Session.GetHabbo().Working)
                return true;

            return false;
        }
        public string TypeCommand
        {
            get { return "manager"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Sends a player home"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Invalid syntax :sendhome <username> <time>");
                return;
            }

            string Username = Params[1];
            string Minutes = Params[2];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            if (TargetClient.GetHabbo().JobId != Session.GetHabbo().JobId)
                return;

            if (Session.GetHabbo().RankId >= TargetClient.GetHabbo().RankId)
            {
                Session.SendWhisper("You don’t have the permissions to sendhome a " + TargetClient.GetHabbo().RankInfo.Name);
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (Convert.ToInt32(Minutes) > 60)
            {
                Session.SendWhisper("The maximum amount of minutes to sendhome is 60");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Convert.ToInt32(Minutes) == 0)
            {
                if (PlusEnvironment.usersSuspendus.ContainsKey(TargetClient.GetHabbo().Username))
                {
                    PlusEnvironment.usersSuspendus.Remove(TargetClient.GetHabbo().Username);
                    User.Say("removes " + TargetClient.GetHabbo().Username + "'s sendhome");
                    TargetClient.SendWhisper("Your sendhome has been removed");
                    return;
                }
            }

            PlusEnvironment.SendUserHome(TargetClient.GetHabbo().Username, Convert.ToInt32(Minutes));
            if (TargetClient.GetHabbo().Working == true)
            {
                TargetClient.GetHabbo().Working = false;
                TargetClient.GetHabbo().resetAvatarEvent();
            }
            User.Say("sends home " + TargetClient.GetHabbo().Username + " for " + Minutes + " minutes");
            TargetClient.SendWhisper("You have been sent home for " + Minutes + " minutes");
        }
    }
}