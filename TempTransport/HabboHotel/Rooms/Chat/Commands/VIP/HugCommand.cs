using System;
using Plus.HabboHotel.GameClients;
using System.Drawing;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class HugCommand : IChatCommand
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
            get { return "Hugs a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (PlusEnvironment.getCooldown("hug" + Session.GetHabbo().Id))
            {
                Session.SendWhisper("You must wait before you can hug a player again");
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
                Session.SendWhisper("You cannot hug players while they are afk");
                return;
            }

            if (TargetClient.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot hug players while they are cuffed");
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

            if (Session.GetHabbo().LastHug == TargetClient.GetHabbo().Username)
            {
                User.Say("hugs " + TargetClient.GetHabbo().Username + " also back", 16);
                User.ApplyEffect(168);

                System.Timers.Timer UndoHugTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
                UndoHugTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
                UndoHugTimer.Elapsed += delegate
                {
                    Session.GetHabbo().resetEffectEvent();
                    UndoHugTimer.Stop();
                };
                UndoHugTimer.Start();

                Session.GetHabbo().LastHug = null;
                PlusEnvironment.addCooldown("hug" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(5)));
                return;
            }

            User.Say("hugs " + TargetClient.GetHabbo().Username + "", 16);
            User.ApplyEffect(168);
            TargetClient.GetHabbo().LastHug = Session.GetHabbo().Username;
            PlusEnvironment.addCooldown("hug" + Session.GetHabbo().Id, Convert.ToInt32(PlusEnvironment.ConvertSecondsToMilliseconds(5)));

            System.Timers.Timer LastHugTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(10));
            LastHugTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(10);
            LastHugTimer.Elapsed += delegate
            {
                TargetClient.GetHabbo().LastHug = null;
                LastHugTimer.Stop();
            };
            LastHugTimer.Start();

            System.Timers.Timer UndoTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(3));
            UndoTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(3);
            UndoTimer.Elapsed += delegate
            {
                Session.GetHabbo().resetEffectEvent();
                UndoTimer.Stop();
            };
            UndoTimer.Start();
        }
    }
}