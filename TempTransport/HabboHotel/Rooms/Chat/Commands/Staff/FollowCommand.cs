using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Utilities;
using Plus.HabboHotel.Users;
using Plus.HabboHotel.GameClients;
using Plus.HabboHotel.Moderation;
using Plus.Database.Interfaces;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class FollowCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 5 && Session.GetHabbo().isLoggedIn)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "follow a user."; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :follow <username>");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("The user could not be found.");
                return;
            }

            if (TargetClient.GetHabbo().CurrentRoom == Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is already in the room!");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("You can't follow yourself.");
                return;
            }

            /*if (TargetClient.GetHabbo().Rank > 7)
            {
                Session.SendWhisper("Du kannst keine Stafss folgen.");
                return;
            }*/

            if (!TargetClient.GetHabbo().InRoom)
            {
                Session.SendWhisper("This user is not in a room!");
                return;
            }

            Session.GetHabbo().CanChangeRoom = true;
            RoleplayManager.InstantRL(Session, TargetClient.GetHabbo().CurrentRoom.RoomId);
        }
    }
}
