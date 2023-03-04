using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;
using Plus.HabboHotel.Groups;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class DepositCommand : IChatCommand
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
            get { return "Deposit funds into a player's bank account"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 2)
            {
                Session.SendWhisper("Invalid syntax :deposit <username> <amount>");
                return;
            }

            if (Session.GetHabbo().getCooldown("deposit_command"))
            {
                Session.SendWhisper("You must wait before you can use the deposit command again");
                return;
            }

            string Username = Params[1];
            int Amount = Convert.ToInt32(Params[2]);
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

            int Fee = Amount * 2 / 100;
            if (Amount > TargetClient.GetHabbo().Credits)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + PlusEnvironment.ConvertToPrice(Amount));
                return;
            }

            if (Amount + Fee > TargetClient.GetHabbo().Credits)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " doesn't have enough money to cover the fees for this transaction");
                return;
            }

            if (Fee < 1)
            {
                Session.SendWhisper("The minimum amount of deposit it $50");
                return;
            }

            Session.GetHabbo().addCooldown("deposit_command", 2000);
            User.Say("offers to deposit $" + PlusEnvironment.ConvertToPrice(Amount) + " for " + TargetClient.GetHabbo().Username);
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;
            TargetUser.Transaction = "bank:deposit:" + Amount + ":" + Fee;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to deposit <b>$" + PlusEnvironment.ConvertToPrice(Amount) + "</b> to your bank account");
            Session.GetHabbo().OfferTimer();
        }
    }
}