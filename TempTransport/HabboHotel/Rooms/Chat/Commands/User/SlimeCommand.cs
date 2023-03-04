using System;
using Plus.HabboHotel.GameClients;
using System.Drawing;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SlimeCommand : IChatCommand
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
            get { return "Throws slime to a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (PlusEnvironment.getCooldown("slime" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before you can slime a player again");
                return;
            }

            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :slime <username>");
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

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (TargetUser.IsAsleep)
            {
                Session.SendWhisper("You cannot slime players while they are afk");
                return;
            }

            if (TargetClient.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot slime players while they are cuffed");
                return;
            }

            if (TargetClient.GetHabbo().Hospital == 1 || TargetClient.GetHabbo().IsWaitingForParamedic || TargetClient.GetRoleplay().Dead)
            {
                Session.SendWhisper("You cannot perform this action to a dead player");
                return;
            }

            if (Math.Abs(TargetUser.X - User.X) > 1 || Math.Abs(TargetUser.Y - User.Y) > 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " are too far away");
                return;
            }

            User.Say("throws a handful of slime to " + TargetClient.GetHabbo().Username, 6);
            TargetUser.ApplyEffect(169);
            PlusEnvironment.addCooldown("slime" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertMinutesToMilliseconds(2)));

            System.Timers.Timer ResetEffectTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
            ResetEffectTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
            ResetEffectTimer.Elapsed += delegate
            {
                TargetClient.GetHabbo().resetEffectEvent();
                ResetEffectTimer.Stop();
            };
            ResetEffectTimer.Start();
        }
    }
}