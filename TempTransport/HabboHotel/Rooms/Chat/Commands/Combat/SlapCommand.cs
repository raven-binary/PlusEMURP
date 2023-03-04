using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Plus.Communication.Packets.Outgoing.Moderation;
using Plus.HabboHotel.GameClients;

namespace Plus.HabboHotel.Rooms.Chat.Commands.Moderator
{
    class SlapCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "arme"; }
        }

        public string Parameters
        {
            get { return "<username>"; }
        }

        public string Description
        {
            get { return "Slap a player"; }
        }

        public void Execute(GameClients.GameClient Session, Rooms.Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :slap <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("spray_command") == true)
            {
                Session.SendWhisper("Wait before you can slap someone again");
                return;
            }

            if (Session.GetHabbo().Hospital == 1)
                return;

            string Username = Params[1];
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Params[1] + " not found in this room");
                return;
            }

            if (TargetClient == null)
            {
                Session.SendWhisper(Params[1] + " could not be found");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
            {
                return;
            }

            if (Room.RoomData.GroupId > 0)
            {
                Session.SendWhisper("You cannot slap in safezones rooms");
                return;
            }

            if (TargetUser.IsAsleep)
            {
                Session.SendWhisper("You cannot slap players while they are afk");
                return;
            }

            if (TargetClient.GetRoleplay().Passive)
            {
                Session.SendWhisper("You cannot slap players while they are in passive mode");
                return;
            }

            if (TargetUser.OnDuty == true)
            {
                return;
            }

            if (User.Immunised == true || TargetUser.Immunised == true)
            {
                return;
            }

            if (Math.Abs(User.Y - TargetUser.Y) > 1 || Math.Abs(User.X - TargetUser.X) > 1 || User.X != TargetUser.X && User.Y != TargetUser.Y)
            {
                User.Say("slaps at " + TargetClient.GetHabbo().Username + ", but misses");
                Session.GetRoleplay().Energy -= 2;
                Session.GetHabbo().RPCache(1);
                return;
            }

            User.Say("slaps at " + TargetClient.GetHabbo().Username + ", dealing 1 damage");
            Session.GetRoleplay().Energy -= 2;
            Session.GetHabbo().RPCache(1);

            TargetClient.GetRoleplay().Health -= 1;
            TargetClient.GetHabbo().RPCache(1);

            if (TargetClient.GetRoleplay().Health == 0)
            {
                TargetClient.GetHabbo().Hospital = 1;
                PlusEnvironment.GetGame().GetClientManager().LiveFeed("<span class=\"blue\">" + Session.GetHabbo().Username + "</span> knocked out <span class=\"red\">" + TargetClient.GetHabbo().Username + "</span>");
            }

        }
    }
}
