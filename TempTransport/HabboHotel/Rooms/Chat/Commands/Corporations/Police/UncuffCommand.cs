using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class UnCuffCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 1 && Session.GetHabbo().Working)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "job"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Uncuffs a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Please specific a target");
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

            if (!TargetClient.GetRoleplay().Escort || !TargetClient.GetRoleplay().Cuffed)
            {
                Session.SendWhisper("This player is not cuffed");
                return;
            }

            if (TargetClient.GetRoleplay().Escort && TargetClient.GetRoleplay().EscortBy != Session.GetRoleplay().Username)
            {
                Session.SendWhisper("You can't uncuff someone where isn't escorting by you");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            User.Say("starts to remove the cuffs from " + TargetClient.GetHabbo().Username + "'s wrists");
            Session.GetHabbo().isUncuffing = true;
            Random TokenRand = new Random();
            int tokenNumber = TokenRand.Next(1600, 2894354);
            Session.GetHabbo().PlayToken = tokenNumber;

            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "show" + ";" + "Uncuff..." + ";" + PlusEnvironment.ConvertSecondsToMilliseconds(1.6) + ";" + 1.6);
            System.Timers.Timer UncuffTimer = new System.Timers.Timer(PlusEnvironment.ConvertSecondsToMilliseconds(1.6));
            UncuffTimer.Interval = PlusEnvironment.ConvertSecondsToMilliseconds(1.6);
            UncuffTimer.Elapsed += delegate
            {
                if (Session.GetHabbo().isUncuffing && Session.GetHabbo().PlayToken == tokenNumber)
                {
                    if (TargetClient.GetRoleplay().Escort)
                    {
                        PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "timed-action;" + "hide");
                        Session.GetRoleplay().Inventory.Add("handcuffs", "coploadout", 1, 0, true, false);
                        Session.GetRoleplay().EndEscorting(false);
                    }
                }
                UncuffTimer.Stop();
            };
            UncuffTimer.Start();
        }
    }
}