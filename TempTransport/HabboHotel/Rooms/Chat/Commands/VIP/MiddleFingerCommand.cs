using System;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class MiddleFingerCommand : IChatCommand
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
            get { return ""; }
        }

        public string Description
        {
            get { return "Shows your middle finger"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Session.GetHabbo().Hospital > 0)
            {
                Session.SendWhisper("You can not perform this action while unconscious");
                return;
            }

            if (Session.GetHabbo().IsWaitingForParamedic)
            {
                Session.SendWhisper("You can not perform this action while dead");
                return;
            }

            if (Session.GetHabbo().getCooldown("middle_finger"))
            {
                Session.SendWhisper("You must wait before you can show your middle finger again");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            User.Say("shows their middle finger");
            User.ApplyEffect(401);
            Session.GetHabbo().addCooldown("middle_finger", 3000);

            System.Timers.Timer MDTimer = new System.Timers.Timer(1900);
            MDTimer.Interval = 1900;
            MDTimer.Elapsed += delegate
            {
                Session.GetHabbo().resetEffectEvent();
                MDTimer.Stop();
            };
            MDTimer.Start();
        }
    }
}