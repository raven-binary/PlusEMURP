using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Rooms.Notifications;


using Plus.HabboHotel.Users;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class PardonCommand : IChatCommand
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
            get { return "Pardons a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {

            if (Params.Length < 2)
            {
                Session.SendWhisper("Please specific a target");
                return;
            }

            if (Session.GetHabbo().getCooldown("pardon_command"))
            {
                Session.SendWhisper("You must wait before you can use pardon command again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

            if (TargetClient == null)
            {
                Session.SendWhisper("Player not found");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (!TargetClient.GetRoleplay().Wan.IsWanted)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " is not wanted");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            Session.GetHabbo().addCooldown("pardon_command", 3000);
            User.Say("pardons " + TargetClient.GetHabbo().Username + " of their crimes");
            TargetClient.SendWhisper("Your crimes has been cleared by " + Session.GetHabbo().Username);
            TargetClient.GetRoleplay().Wan.Remove();
        }
    }
}