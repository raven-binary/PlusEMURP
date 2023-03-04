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
    class GiveCreditsCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank > 4 && Session.GetHabbo().isLoggedIn)
                return true;

            return false;
        }

        public string TypeCommand
        {
            get { return "staff"; }
        }

        public string Parameters
        {
            get { return "<username> <amount>"; }
        }

        public string Description
        {
            get { return "Give money to a citizen"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax :givecredits <username> <amount>", 1);
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null)
            {
                Session.SendWhisper(Username + " could not be found.", 1);
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            int Amount;
            string Montant = Params[2];
            if (!int.TryParse(Montant, out Amount) || Convert.ToInt32(Params[2]) <= 0 || Montant.StartsWith("0"))
            {
                Session.SendWhisper("The amount is invalid.", 1);
                return;
            }

            if (Session.GetHabbo() != null)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                TargetClient.GetHabbo().Credits = TargetClient.GetHabbo().Credits + Convert.ToInt32(Montant);
                TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "my_stats;" + TargetClient.GetHabbo().Credits + ";" + TargetClient.GetHabbo().Duckets + ";" + TargetClient.GetHabbo().EventPoints);
                User.GetClient().Shout("*Gives " + Montant + " $ to " + TargetClient.GetHabbo().Username + "*");
                Room.AddChatlog(Session.GetHabbo().Id, "*Gives " + Montant + " $ to " + TargetClient.GetHabbo().Username + "*");
                if (Convert.ToInt32(Montant) >= 2500)
                {
                    PlusEnvironment.GetGame().GetClientManager().sendStaffMsg(Session.GetHabbo().Username + " gave " + Convert.ToInt32(Montant) + " $ to " + TargetClient.GetHabbo().Username + " delivered.");
                }
            }
        }
    }
}