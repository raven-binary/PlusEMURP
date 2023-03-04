using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Chat;
using Plus.HabboRoleplay.Misc;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class RestoreStatsCommand : IChatCommand
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
            get { return "Restores someone stats to full"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Type a username");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper(Username + " couldn't be found in this room");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            User.Say("restores " + TargetClient.GetHabbo().Username + "'s stats to full");
            TargetClient.SendWhisper("Your stats are restored to full by " + Session.GetHabbo().Username);
            TargetClient.GetRoleplay().Health = TargetClient.GetRoleplay().HealthMax;
            TargetClient.GetRoleplay().Energy = 100;
            TargetClient.GetHabbo().RPCache(1);
            TargetUser.LastBubble = TargetClient.GetHabbo().CustomBubbleId;
            PlusEnvironment.GetGame().GetClientManager().StaffWhisper(Session.GetHabbo().Username + " has restored " + TargetClient.GetHabbo().Username + "'s stats to full");
            RoleplayManager.UpdateTargetStats(TargetClient.GetHabbo().Id);
        }
    }
}