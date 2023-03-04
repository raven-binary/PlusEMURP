using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SuperPullCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 4 && Session.GetHabbo().isLoggedIn)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<id>"; }
        }

        public string Description
        {
            get { return "Pulls a player to you, with no limits"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please enter a username");
                return;
            }

            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (TargetClient == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Username);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Username);
            if (TargetUser == null)
            {
                Session.SendWhisper("An error occoured whilst finding that user, maybe they're not online or in this room");
                return;
            }

            if (TargetClient.GetHabbo().Username == Session.GetHabbo().Username)
            {
                Session.SendWhisper("You cannot super push yourself!");
                return;
            }

            if (TargetUser.RotBody % 2 != 0)
                TargetUser.RotBody--;
            if (TargetUser.RotBody == 0)
                TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
            else if (TargetUser.RotBody == 2)
                TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
            else if (TargetUser.RotBody == 4)
                TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
            else if (TargetUser.RotBody == 6)
                TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);

            User.Say("super pulls " + TargetClient.GetHabbo().Username + " towards them*");
        }
    }
}
