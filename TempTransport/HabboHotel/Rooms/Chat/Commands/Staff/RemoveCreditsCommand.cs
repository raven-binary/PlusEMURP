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
    class RemoveCredtisCommand : IChatCommand
    {
        public bool getPermission(GameClient Session)
        {
            if (Session.GetHabbo().Rank == 8 || Session.GetHabbo().JobId == 4 && Session.GetHabbo().Working == true)
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
            get { return "Remove money from a user"; }
        }

        public void Execute(GameClient Session, Room Room, string[] Params)
        {
            if (Params.Length < 3)
            {
                Session.SendWhisper("Invalid syntax :removecredits <username> <amount>");
                return;
            }

            string Username = Params[1];
            GameClient TargetClient = PlusEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);
            if (TargetClient == null || TargetClient.GetHabbo().CurrentRoom != Session.GetHabbo().CurrentRoom)
            {
                Session.SendWhisper(Username + " could not be found in this apartment.");
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);
            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(TargetClient.GetHabbo().Id);

            int Amount;
            string Montant = Params[2];
            if (!int.TryParse(Montant, out Amount) || Convert.ToInt32(Params[2]) <= 0 || Montant.StartsWith("0"))
            {
                Session.SendWhisper("The amount is invalid.");
                return;
            }

            if (Session.GetHabbo() != null)
            {
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(Session, "my_stats;" + Session.GetHabbo().Credits + ";" + Session.GetHabbo().Duckets + ";" + Session.GetHabbo().EventPoints);
                TargetClient.GetHabbo().Credits = TargetClient.GetHabbo().Credits - Convert.ToInt32(Montant);
                TargetClient.SendMessage(new CreditBalanceComposer(TargetClient.GetHabbo().Credits));
                PlusEnvironment.GetGame().GetWebEventManager().SendDataDirect(TargetClient, "my_stats;" + TargetClient.GetHabbo().Credits + ";" + TargetClient.GetHabbo().Duckets + ";" + TargetClient.GetHabbo().EventPoints);
                User.GetClient().Shout("*Removes  " + Montant + " $ from " + TargetClient.GetHabbo().Username + "*");
                if (Convert.ToInt32(Montant) >= 2500)
                {
                    PlusEnvironment.GetGame().GetClientManager().sendStaffMsg(Session.GetHabbo().Username + " removed " + Convert.ToInt32(Montant) + " $ from " + TargetClient.GetHabbo().Username + " away.");
                }
            }
        }
    }
}