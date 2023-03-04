using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class MarryCommand : IChatCommand
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
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Marry a player"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Session.GetHabbo().CurrentRoomId != 138)
            {
                Session.SendWhisper("You must be in the Chapel to get married");
                return;
            }
            
            if (!Session.GetHabbo().usingChapelActionPoint)
            {
                Session.SendWhisper("You must stand on the left action point to get married");
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :marry <username>");
                return;
            }

            if (Session.GetRoleplay().MarriedTo != 0)
            {
                Session.SendWhisper("You are currently married to user X, in order to remarry you must divorce");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (TargetClient.GetRoleplay().MarriedTo != 0)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " are already married");
                return;
            }

            if (!TargetClient.GetHabbo().usingChapelActionPoint)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " must be stand on the front action point from you");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User == null || TargetUser == null)
                return;

            if (TargetClient.GetHabbo().MarryingTo == Session.GetHabbo().Username)
            {
                Session.GetHabbo().MarryingTo = TargetClient.GetHabbo().Username;
                PlusEnvironment.GetGame().GetClientManager().HotelWhisper(TargetClient.GetHabbo().Username + " gets down on one knee and proposes to " + Session.GetHabbo().Username);
            }
            else
            {
                Session.GetHabbo().IsWaitingToMarry = true;
                Session.GetHabbo().MarryingTo = TargetClient.GetHabbo().Username;
                TargetClient.SendWhisper(Session.GetHabbo().Username + " has just offered to marry you, type :marry " + Session.GetHabbo().Username + " to marry them");
            }
        }
    }
}
