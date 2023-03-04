using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class WithdrawCommand : IChatCommand
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
            get { return "<username> <amount>"; }
        }

        public string Description
        {
            get { return "Withdraw funds from a player's bank account"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length == 2)
            {
                Session.SendWhisper("Invalid syntax :withdraw <username> <amount>");
                return;
            }

            if (Session.GetHabbo().getCooldown("withdraw_command"))
            {
                Session.SendWhisper("You must wait before you can use the withdraw command again");
                return;
            }

            string Username = Params[1];
            int Amount = Convert.ToInt32(Params[2]);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient.GetHabbo() == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this room");
                return;
            }

            if (TargetClient.GetHabbo().Id == Session.GetHabbo().Id)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);
            if (Math.Abs(User.Y - TargetUser.Y) > 2 || Math.Abs(User.X - TargetUser.X) > 2)
            {
                Session.SendWhisper("The target user are too far away");
                return;
            }

            int num;
            if (!Int32.TryParse(Params[2], out num) || Params[2].StartsWith("0"))
            {
                Session.SendWhisper("The amount is invalid");
                return;
            }

            if (Amount > TargetClient.GetHabbo().Banque)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + PlusEnvironment.ConvertToPrice(Amount) + " in their bank account");
                return;
            }

            Session.GetHabbo().addCooldown("withdraw_command", 2000);
            User.Say("offers to withdraw $" + PlusEnvironment.ConvertToPrice(Amount) + " for " + TargetClient.GetHabbo().Username);
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;
            TargetUser.Transaction = "bank:withdraw:" + Amount;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to withdraw <b>$" + PlusEnvironment.ConvertToPrice(Amount) + "</b> from your bank account");
            Session.GetHabbo().OfferTimer();
        }
    }
}