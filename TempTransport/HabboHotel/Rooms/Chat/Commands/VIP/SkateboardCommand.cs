using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class SkateboardCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 1)
                return true;

            return true;
        }

        public string TypeCommand
        {
            get { return "vip"; }
        }

        public string Parameters
        {
            get { return ""; }
        }

        public string Description
        {
            get { return "Jumps to your skateboard"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().getCooldown("skateboard_command") == true)
            {
                Session.SendWhisper("You must wait before you can use the skateboard command again");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (Session.GetHabbo().IsWaitingForParamedic)
            {
                Session.SendWhisper("You cannot mount your skateboard while dead");
                return;
            }
            else if (Session.GetHabbo().Hospital == 1)
            {
                Session.SendWhisper("You cannot mount your skateboard while unconscious");
                return;
            }
            /*else if (Session.GetHabbo().checkIfWanted())
            {
                Session.SendWhisper("You cannot mount your skateboard while wanted");
                return;
            }*/
            else if (Session.GetRoleplay().Aggression >= 1)
            {
                Session.SendWhisper("You cannot mount your skateboard while having aggression");
                return;
            }
            else if (Session.GetHabbo().Working)
            {
                Session.SendWhisper("You cannot mount your skateboard while on duty");
                return;
            }
            else if (Session.GetHabbo().Stunned)
            {
                Session.SendWhisper("You cannot mount your skateboard while stunned");
                return;
            }

            User.Say("jumps " + (Session.GetHabbo().isUsingSkateboard ? "from" : "on") + " their skateboard");
            Session.GetHabbo().isUsingSkateboard = !Session.GetHabbo().isUsingSkateboard;
            User.FastWalking = Session.GetHabbo().isUsingSkateboard;
            Session.GetHabbo().resetEffectEvent();
            Session.GetHabbo().addCooldown("skateboard_command", 5000);
        }
    }
}