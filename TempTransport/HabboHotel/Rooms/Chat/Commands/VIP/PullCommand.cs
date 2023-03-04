using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.HabboHotel.Rooms;
using Plus.HabboHotel.Pathfinding;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboHotel.Items;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class PullCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 1)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "vip"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Pull a player to you"; }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a targe");
                return;
            }

            if (PlusEnvironment.getCooldown("pull" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before you can pull again");
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

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (User == null)
                return;

            if (Session.GetHabbo().usingArena)
            {
                Session.SendWhisper("You cannot perform this action while fighting in the arena");
                return;
            }

            if (Session.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action while cuffed");
                return;
            }

            if (Session.GetRoleplay().Passive && !TargetClient.GetRoleplay().Passive)
            {
                Session.SendWhisper("You cannot pull this player while being on passive mode");
                return;
            }

            if (TargetClient.GetHabbo().Hospital == 1 || TargetClient.GetHabbo().IsWaitingForParamedic || TargetClient.GetRoleplay().Dead)
            {
                Session.SendWhisper("You cannot perform this action to a dead player");
                return;
            }

            if (TargetClient.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot pull players while they are cuffed");
                return;
            }

            if (TargetUser.IsAsleep)
            {
                Session.SendWhisper("You cannot pull players while they are afk");
                return;
            }

            string PushDirection = "down";
            if (TargetClient.GetHabbo().CurrentRoomId == Session.GetHabbo().CurrentRoomId && (Math.Abs(User.X - TargetUser.X) < 3 && Math.Abs(User.Y - TargetUser.Y) < 3))
            {
                if (User.RotBody == 0)
                    PushDirection = "up";
                if (User.RotBody == 2)
                    PushDirection = "right";
                if (User.RotBody == 4)
                    PushDirection = "down";
                if (User.RotBody == 6)
                    PushDirection = "left";

                if (PushDirection == "up")
                    TargetUser.MoveTo(User.X, User.Y - 1);

                if (PushDirection == "right")
                    TargetUser.MoveTo(User.X + 1, User.Y);

                if (PushDirection == "down")
                    TargetUser.MoveTo(User.X, User.Y + 1);

                if (PushDirection == "left")
                    TargetUser.MoveTo(User.X - 1, User.Y);

                User.Say("pulls " + TargetUser.GetClient().GetHabbo().Username + " to them");
                PlusEnvironment.addCooldown("pull" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertMinutesToMilliseconds(2)));
                return;
            }
            else
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " are too far away");
            }
        }
    }
}
