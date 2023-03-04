using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class LockpickCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "combat"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return ""; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (!Session.GetHabbo().CheckInventory("lockpick"))
            {
                Session.SendWhisper("You need a lockpick to perform this action");
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
            {
                Session.SendWhisper("You cannot perform this action on yourself");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetRoleplay().TargetId);

            if (!TargetClient.GetHabbo().Cuffed)
            {
                Session.SendWhisper("You cannot perform this action to a player where isn't cuffed");
                return;
            }

            if (Math.Abs(TargetUser.X - User.X) > 1 || Math.Abs(TargetUser.Y - User.Y) > 1)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " are too far away");
                return;
            }

            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;

            User.Say("attempts to lockpick " + TargetClient.GetHabbo().Username + "'s cuffs");
            User.ApplyEffect(4);
            Session.GetHabbo().LockpickingTo = TargetClient.GetHabbo().Username;
            TargetClient.GetHabbo().LockpickingFrom = Session.GetHabbo().Username;

            System.Timers.Timer Timer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(20));
            Timer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(20);
            Timer.Elapsed += delegate
            {
                if (Session.GetHabbo().LockpickingTo == TargetClient.GetHabbo().Username && TargetClient.GetHabbo().LockpickingFrom == Session.GetHabbo().Username && Session.GetHabbo().PlayToken == tokenNumber)
                {
                    Session.GetHabbo().LockpickingTo = null;
                    TargetClient.GetHabbo().LockpickingFrom = null;
                    PlusEnvironment.GetGame().GetWebEventManager().ExecuteWebEvent(Session, "inventory", "update_quantity," + Session.GetHabbo().FreeInventory + ",-," + 1);
                    Session.GetHabbo().PlayToken = 0;
                    Session.GetHabbo().resetEffectEvent();

                    Random rand = new Random();
                    int Lockpick = rand.Next(1, 101);

                    if (Lockpick <= 40)
                    {
                        User.Say("successfully lockpicks " + TargetClient.GetHabbo().Username + " from their cuffs");
                        if (TargetClient.GetHabbo().EscortBy != null)
                        {
                            GameClient EscortBy = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(TargetClient.GetHabbo().EscortBy);

                            TargetClient.GetHabbo().Escort = false;
                            TargetClient.GetHabbo().EscortBy = null;
                            EscortBy.GetHabbo().Escorting = false;
                            EscortBy.GetHabbo().EscortUsername = null;
                        }

                        TargetUser.UltraFastWalking = false;
                        TargetClient.GetHabbo().Cuffed = false;
                        TargetClient.GetHabbo().resetAvatarEvent();
                    }
                    else if (Lockpick > 40)
                    {
                        User.Say("fails to lockpick " + TargetClient.GetHabbo().Username + " cuffs");
                    }

                }
                Timer.Stop();
            };
            Timer.Start();
        }
    }
}
