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
    class BalanceCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().JobId == 3 && Session.GetHabbo().Working == true)
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
            get { return "Checks balance on a player"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Invalid syntax :balance <username>");
                return;
            }

            if (Session.GetHabbo().getCooldown("balance_command"))
            {
                Session.SendWhisper("You must wait before you can use the balance command again");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient.GetHabbo() == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper("The target user are too far away");
                return;
            }

            Session.GetHabbo().addCooldown("balance_command", 2000);
            User.Say("offers to check " + TargetClient.GetHabbo().Username + "'s balance");
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;
            TargetUser.Transaction = "bank:balance";
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to check your balance");
            Session.GetHabbo().OfferTimer();
        }
    }
}