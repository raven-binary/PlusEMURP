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
    class TransferCommand : IChatCommand
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
            get { return "<from> <to> <amount>"; }
        }

        public string Description
        {
            get { return "Transfers money from the target player to other player bank account"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 4)
            {
                Session.SendWhisper("Invalid syntax :transfer <from> <to> <amount>");
                return;
            }

            if (Session.GetHabbo().getCooldown("transfer_command"))
            {
                Session.SendWhisper("You must wait before you can use the transfer command again");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            string From = Params[1];
            string To = Params[2];
            int Amount = Convert.ToInt32(Params[3]);
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(From);

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Username);

            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper("Player not found in this room");
                return;
            }

            if (Math.Abs(User.X - TargetUser.X) > 2 || Math.Abs(User.Y - TargetUser.Y) > 2)
            {
                Session.SendWhisper("he target user are too far away");
                return;
            }

            int num;
            if (!Int32.TryParse(Params[3], out num) || Params[2].StartsWith("0"))
            {
                Session.SendWhisper("The amount is invalid");
                return;
            }

            int Fee = Amount * 2 / 100;
            if (Amount > TargetClient.GetHabbo().Banque)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " has not $" + PlusEnvironment.ConvertToPrice(Amount) + " in their bank account");
                return;
            }
            
            if (Amount + Fee > TargetClient.GetHabbo().Banque)
            {
                Session.SendWhisper(TargetClient.GetHabbo().Username + " doesn't have enough money to cover the fees for this transaction");
                return;
            }

            if (Fee < 1)
            {
                Session.SendWhisper("The minimum amount of deposit it $50");
                return;
            }

            GameClient Player2 = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(To);
            if (Player2 == null)
            {
                DataRow FindTo = null;
                using (IQueryAdapter dbClient = PlusEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("SELECT * FROM `users` WHERE `username` = @username LIMIT 1;");
                    dbClient.AddParameter("username", To);
                    FindTo = dbClient.getRow();
                }

                if (FindTo == null)
                {
                    Session.SendWhisper(To +  " could not be found");
                    return;
                }

                Session.GetHabbo().addCooldown("transfer_command", 2000);
                User.Say("offers to transfer $" + PlusEnvironment.ConvertToPrice(Amount) + " to " + FindTo["username"] + " for " + TargetClient.GetHabbo().Username);
                TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
                Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;
                TargetUser.Transaction = "bank:transfer:" + FindTo["username"] + ":" + Amount + ":" + Fee;
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to transfer <b>$" + PlusEnvironment.ConvertToPrice(Amount) + "</b> from your bank account to <b>" + FindTo["username"] + "</b> bank account");
                Session.GetHabbo().OfferTimer();
            }

            Session.GetHabbo().addCooldown("transfer_command", 2000);
            User.Say("offers to transfer $" + PlusEnvironment.ConvertToPrice(Amount) + " to " + Player2.GetHabbo().Username + " for " + TargetClient.GetHabbo().Username);
            TargetClient.GetHabbo().TransactionFrom = Session.GetHabbo().Username;
            Session.GetHabbo().TransactionTo = TargetClient.GetHabbo().Username;
            TargetUser.Transaction = "bank:transfer:" + To + ":" + Amount + ":" + Fee;
            PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "transaction;<b>" + Session.GetHabbo().Username + "</b> wants to transfer <b>$" + PlusEnvironment.ConvertToPrice(Amount) + "</b> from your bank account to <b>" + Player2.GetHabbo().Username + "</b> bank account");
            Session.GetHabbo().OfferTimer();
        }
    }
}