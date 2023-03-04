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
    class PushCommand : IChatCommand
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
            get { return "Push a player"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a targe");
                return;
            }

            if (PlusEnvironment.getCooldown("push" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before you can push again");
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

            if (Session.GetRoleplay().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action while cuffed");
                return;
            }

            if (Session.GetRoleplay().Passive && !TargetClient.GetRoleplay().Passive)
            {
                Session.SendWhisper("You cannot push this player while being on passive mode");
                return;
            }

            if (TargetClient.GetRoleplay().Hospital || TargetClient.GetHabbo().IsWaitingForParamedic || TargetClient.GetRoleplay().Dead)
            {
                Session.SendWhisper("You cannot perform this action to a dead player");
                return;
            }

            if (TargetClient.GetRoleplay().Cuffed)
            {
                Session.SendWhisper("You cannot push players while they are cuffed");
                return;
            }

            if (TargetUser.IsAsleep)
            {
                Session.SendWhisper("You cannot push players while they are afk");
                return;
            }

            if (!((Math.Abs(TargetUser.X - User.X) >= 2) || (Math.Abs(TargetUser.Y - User.Y) >= 2)))
            {
                if (User.RotBody == 4)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                if (User.RotBody == 0)
                {
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (User.RotBody == 6)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                }

                if (User.RotBody == 2)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                }

                if (User.RotBody == 3)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                if (User.RotBody == 1)
                {
                    TargetUser.MoveTo(TargetUser.X + 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (User.RotBody == 7)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y - 1);
                }

                if (User.RotBody == 5)
                {
                    TargetUser.MoveTo(TargetUser.X - 1, TargetUser.Y);
                    TargetUser.MoveTo(TargetUser.X, TargetUser.Y + 1);
                }

                User.Say("pushes " + TargetUser.GetClient().GetHabbo().Username);
                PlusEnvironment.addCooldown("push" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertMinutesToMilliseconds(2)));
            }
            else
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " are too far away");
            }
        }
    }
}
