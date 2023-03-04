using System;
using System.Linq;
using System.Text;
using System.Data;

using Plus.Database.Interfaces;
using Plus.Communication.Packets.Outgoing.Notifications;
using Plus.HabboHotel.GameClients;
using Plus.Communication.Packets.Outgoing.Rooms.Engine;
using Plus.Communication.Packets.Outgoing.Inventory.Purse;

namespace Plus.HabboHotel.Rooms.Chat.Commands.User
{
    class GiveMoneyCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            return true;
        }

        public string TypeCommand
        {
            get { return "user"; }
        }

        public string Parameters
        {
            get { return "<username> <amount>"; }
        }

        public string Description
        {
            get { return "Gives a player credits"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax :givemoney <username> <amount>");
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
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            if (User.isTradingItems)
            {
                return;
            }

            if (Math.Abs(User.Y - TargetUser.Y) > 1 || Math.Abs(User.X - TargetUser.X) > 1)
            {
                Session.SendWhisper("You must next to the target");
                return;
            }

            if (Session.GetHabbo().getCooldown("give_command") == true)
            {
                Session.SendWhisper("Cooldown");
                return;
            }

            if (Session.GetHabbo().Stunned)
                return;

            int Amount;
            string Montant = Params[2];
            if (!int.TryParse(Montant, out Amount) || Convert.ToInt32(Params[2]) <= 0 || Montant.StartsWith("0"))
            {
                Session.SendWhisper("The amount is invalid");
                return;
            }

            if (Convert.ToInt32(Montant) > Session.GetHabbo().Credits)
            {
                Session.SendWhisper("You do not have that much money");
                return;
            }

            if (Session.GetHabbo() != null)
            {
                Session.GetHabbo().addCooldown("give_command", 5000);
                Session.GetHabbo().Credits = Session.GetHabbo().Credits - Convert.ToInt32(Montant);
                Session.SendMessage(new CreditBalanceComposer(Session.GetHabbo().Credits));
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                TargetClient.GetHabbo().Credits = TargetClient.GetHabbo().Credits + Convert.ToInt32(Montant);
                TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "my_stats;" + TargetClient.GetHabbo().Credits + ";" + TargetClient.GetHabbo().Duckets + ";" + TargetClient.GetHabbo().EventPoints);
                User.Say("gives " + TargetClient.GetHabbo().Username + " " + PlusEnvironment.ConvertToPrice(Convert.ToInt32(Montant)) + " dollars", 14);
            }
        }
    }
}